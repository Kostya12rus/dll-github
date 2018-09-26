// Decompiled with JetBrains decompiler
// Type: VolumetricLightRenderer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (Camera))]
public class VolumetricLightRenderer : MonoBehaviour
{
  private static Mesh _pointLightMesh;
  private static Mesh _spotLightMesh;
  private static Material _lightMaterial;
  private Camera _camera;
  private CommandBuffer _preLightPass;
  private Matrix4x4 _viewProj;
  private Material _blitAddMaterial;
  private Material _bilateralBlurMaterial;
  private RenderTexture _volumeLightTexture;
  private RenderTexture _halfVolumeLightTexture;
  private RenderTexture _quarterVolumeLightTexture;
  private static Texture _defaultSpotCookie;
  private RenderTexture _halfDepthBuffer;
  private RenderTexture _quarterDepthBuffer;
  private VolumetricLightRenderer.VolumtericResolution _currentResolution;
  private Texture2D _ditheringTexture;
  private Texture3D _noiseTexture;
  public VolumetricLightRenderer.VolumtericResolution Resolution;
  public Texture DefaultSpotCookie;

  public static event Action<VolumetricLightRenderer, Matrix4x4> PreRenderEvent;

  public CommandBuffer GlobalCommandBuffer
  {
    get
    {
      return this._preLightPass;
    }
  }

  public static Material GetLightMaterial()
  {
    return VolumetricLightRenderer._lightMaterial;
  }

  public static Mesh GetPointLightMesh()
  {
    return VolumetricLightRenderer._pointLightMesh;
  }

  public static Mesh GetSpotLightMesh()
  {
    return VolumetricLightRenderer._spotLightMesh;
  }

  public RenderTexture GetVolumeLightBuffer()
  {
    if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
      return this._quarterVolumeLightTexture;
    if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half)
      return this._halfVolumeLightTexture;
    return this._volumeLightTexture;
  }

  public RenderTexture GetVolumeLightDepthBuffer()
  {
    if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
      return this._quarterDepthBuffer;
    if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half)
      return this._halfDepthBuffer;
    return (RenderTexture) null;
  }

  public static Texture GetDefaultSpotCookie()
  {
    return VolumetricLightRenderer._defaultSpotCookie;
  }

  private void Awake()
  {
    this._camera = this.GetComponent<Camera>();
    if (this._camera.actualRenderingPath == RenderingPath.Forward)
      this._camera.depthTextureMode = DepthTextureMode.Depth;
    this._currentResolution = this.Resolution;
    Shader shader1 = Shader.Find("Hidden/BlitAdd");
    if ((Object) shader1 == (Object) null)
      throw new Exception("Critical Error: \"Hidden/BlitAdd\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
    this._blitAddMaterial = new Material(shader1);
    Shader shader2 = Shader.Find("Hidden/BilateralBlur");
    if ((Object) shader2 == (Object) null)
      throw new Exception("Critical Error: \"Hidden/BilateralBlur\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
    this._bilateralBlurMaterial = new Material(shader2);
    this._preLightPass = new CommandBuffer();
    this._preLightPass.name = "PreLight";
    this.ChangeResolution();
    if ((Object) VolumetricLightRenderer._pointLightMesh == (Object) null)
    {
      GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      VolumetricLightRenderer._pointLightMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
      Object.Destroy((Object) primitive);
    }
    if ((Object) VolumetricLightRenderer._spotLightMesh == (Object) null)
      VolumetricLightRenderer._spotLightMesh = this.CreateSpotLightMesh();
    if ((Object) VolumetricLightRenderer._lightMaterial == (Object) null)
    {
      Shader shader3 = Shader.Find("Sandbox/VolumetricLight");
      if ((Object) shader3 == (Object) null)
        throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
      VolumetricLightRenderer._lightMaterial = new Material(shader3);
    }
    if ((Object) VolumetricLightRenderer._defaultSpotCookie == (Object) null)
      VolumetricLightRenderer._defaultSpotCookie = this.DefaultSpotCookie;
    this.LoadNoise3dTexture();
    this.GenerateDitherTexture();
  }

  private void OnEnable()
  {
    if (this._camera.actualRenderingPath == RenderingPath.Forward)
      this._camera.AddCommandBuffer(CameraEvent.AfterDepthTexture, this._preLightPass);
    else
      this._camera.AddCommandBuffer(CameraEvent.BeforeLighting, this._preLightPass);
  }

  private void OnDisable()
  {
    if (this._camera.actualRenderingPath == RenderingPath.Forward)
      this._camera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, this._preLightPass);
    else
      this._camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, this._preLightPass);
  }

  private void ChangeResolution()
  {
    int pixelWidth = this._camera.pixelWidth;
    int pixelHeight = this._camera.pixelHeight;
    if ((Object) this._volumeLightTexture != (Object) null)
      Object.Destroy((Object) this._volumeLightTexture);
    this._volumeLightTexture = new RenderTexture(pixelWidth, pixelHeight, 0, RenderTextureFormat.ARGBHalf);
    this._volumeLightTexture.name = "VolumeLightBuffer";
    this._volumeLightTexture.filterMode = FilterMode.Bilinear;
    if ((Object) this._halfDepthBuffer != (Object) null)
      Object.Destroy((Object) this._halfDepthBuffer);
    if ((Object) this._halfVolumeLightTexture != (Object) null)
      Object.Destroy((Object) this._halfVolumeLightTexture);
    if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half || this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
    {
      this._halfVolumeLightTexture = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, RenderTextureFormat.ARGBHalf);
      this._halfVolumeLightTexture.name = "VolumeLightBufferHalf";
      this._halfVolumeLightTexture.filterMode = FilterMode.Bilinear;
      this._halfDepthBuffer = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, RenderTextureFormat.RFloat);
      this._halfDepthBuffer.name = "VolumeLightHalfDepth";
      this._halfDepthBuffer.Create();
      this._halfDepthBuffer.filterMode = FilterMode.Point;
    }
    if ((Object) this._quarterVolumeLightTexture != (Object) null)
      Object.Destroy((Object) this._quarterVolumeLightTexture);
    if ((Object) this._quarterDepthBuffer != (Object) null)
      Object.Destroy((Object) this._quarterDepthBuffer);
    if (this.Resolution != VolumetricLightRenderer.VolumtericResolution.Quarter)
      return;
    this._quarterVolumeLightTexture = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, RenderTextureFormat.ARGBHalf);
    this._quarterVolumeLightTexture.name = "VolumeLightBufferQuarter";
    this._quarterVolumeLightTexture.filterMode = FilterMode.Bilinear;
    this._quarterDepthBuffer = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, RenderTextureFormat.RFloat);
    this._quarterDepthBuffer.name = "VolumeLightQuarterDepth";
    this._quarterDepthBuffer.Create();
    this._quarterDepthBuffer.filterMode = FilterMode.Point;
  }

  public void OnPreRender()
  {
    this._viewProj = GL.GetGPUProjectionMatrix(Matrix4x4.Perspective(this._camera.fieldOfView, this._camera.aspect, 0.01f, this._camera.farClipPlane), true) * this._camera.worldToCameraMatrix;
    this._preLightPass.Clear();
    bool flag = SystemInfo.graphicsShaderLevel > 40;
    if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
    {
      Texture source = (Texture) null;
      this._preLightPass.Blit(source, (RenderTargetIdentifier) ((Texture) this._halfDepthBuffer), this._bilateralBlurMaterial, !flag ? 10 : 4);
      this._preLightPass.Blit(source, (RenderTargetIdentifier) ((Texture) this._quarterDepthBuffer), this._bilateralBlurMaterial, !flag ? 11 : 6);
      this._preLightPass.SetRenderTarget((RenderTargetIdentifier) ((Texture) this._quarterVolumeLightTexture));
    }
    else if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half)
    {
      this._preLightPass.Blit((Texture) null, (RenderTargetIdentifier) ((Texture) this._halfDepthBuffer), this._bilateralBlurMaterial, !flag ? 10 : 4);
      this._preLightPass.SetRenderTarget((RenderTargetIdentifier) ((Texture) this._halfVolumeLightTexture));
    }
    else
      this._preLightPass.SetRenderTarget((RenderTargetIdentifier) ((Texture) this._volumeLightTexture));
    this._preLightPass.ClearRenderTarget(false, true, new Color(0.0f, 0.0f, 0.0f, 1f));
    this.UpdateMaterialParameters();
    // ISSUE: reference to a compiler-generated field
    if (VolumetricLightRenderer.PreRenderEvent == null)
      return;
    // ISSUE: reference to a compiler-generated field
    VolumetricLightRenderer.PreRenderEvent(this, this._viewProj);
  }

  [ImageEffectOpaque]
  public void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
    {
      RenderTexture temporary = RenderTexture.GetTemporary(this._quarterDepthBuffer.width, this._quarterDepthBuffer.height, 0, RenderTextureFormat.ARGBHalf);
      temporary.filterMode = FilterMode.Bilinear;
      Graphics.Blit((Texture) this._quarterVolumeLightTexture, temporary, this._bilateralBlurMaterial, 8);
      Graphics.Blit((Texture) temporary, this._quarterVolumeLightTexture, this._bilateralBlurMaterial, 9);
      Graphics.Blit((Texture) this._quarterVolumeLightTexture, this._volumeLightTexture, this._bilateralBlurMaterial, 7);
      RenderTexture.ReleaseTemporary(temporary);
    }
    else if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half)
    {
      RenderTexture temporary = RenderTexture.GetTemporary(this._halfVolumeLightTexture.width, this._halfVolumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
      temporary.filterMode = FilterMode.Bilinear;
      Graphics.Blit((Texture) this._halfVolumeLightTexture, temporary, this._bilateralBlurMaterial, 2);
      Graphics.Blit((Texture) temporary, this._halfVolumeLightTexture, this._bilateralBlurMaterial, 3);
      Graphics.Blit((Texture) this._halfVolumeLightTexture, this._volumeLightTexture, this._bilateralBlurMaterial, 5);
      RenderTexture.ReleaseTemporary(temporary);
    }
    else
    {
      RenderTexture temporary = RenderTexture.GetTemporary(this._volumeLightTexture.width, this._volumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
      temporary.filterMode = FilterMode.Bilinear;
      Graphics.Blit((Texture) this._volumeLightTexture, temporary, this._bilateralBlurMaterial, 0);
      Graphics.Blit((Texture) temporary, this._volumeLightTexture, this._bilateralBlurMaterial, 1);
      RenderTexture.ReleaseTemporary(temporary);
    }
    this._blitAddMaterial.SetTexture("_Source", (Texture) source);
    Graphics.Blit((Texture) this._volumeLightTexture, destination, this._blitAddMaterial, 0);
  }

  private void UpdateMaterialParameters()
  {
    this._bilateralBlurMaterial.SetTexture("_HalfResDepthBuffer", (Texture) this._halfDepthBuffer);
    this._bilateralBlurMaterial.SetTexture("_HalfResColor", (Texture) this._halfVolumeLightTexture);
    this._bilateralBlurMaterial.SetTexture("_QuarterResDepthBuffer", (Texture) this._quarterDepthBuffer);
    this._bilateralBlurMaterial.SetTexture("_QuarterResColor", (Texture) this._quarterVolumeLightTexture);
    Shader.SetGlobalTexture("_DitherTexture", (Texture) this._ditheringTexture);
    Shader.SetGlobalTexture("_NoiseTexture", (Texture) this._noiseTexture);
  }

  private void Update()
  {
    if (this._currentResolution != this.Resolution)
    {
      this._currentResolution = this.Resolution;
      this.ChangeResolution();
    }
    if (this._volumeLightTexture.width == this._camera.pixelWidth && this._volumeLightTexture.height == this._camera.pixelHeight)
      return;
    this.ChangeResolution();
  }

  private void LoadNoise3dTexture()
  {
    TextAsset textAsset = Resources.Load("NoiseVolume") as TextAsset;
    byte[] bytes = textAsset.bytes;
    uint uint32_1 = BitConverter.ToUInt32(textAsset.bytes, 12);
    uint uint32_2 = BitConverter.ToUInt32(textAsset.bytes, 16);
    uint uint32_3 = BitConverter.ToUInt32(textAsset.bytes, 20);
    uint uint32_4 = BitConverter.ToUInt32(textAsset.bytes, 24);
    uint uint32_5 = BitConverter.ToUInt32(textAsset.bytes, 80);
    uint num1 = BitConverter.ToUInt32(textAsset.bytes, 88);
    if (num1 == 0U)
      num1 = uint32_3 / uint32_2 * 8U;
    this._noiseTexture = new Texture3D((int) uint32_2, (int) uint32_1, (int) uint32_4, TextureFormat.RGBA32, false);
    this._noiseTexture.name = "3D Noise";
    Color[] colors = new Color[(IntPtr) (uint32_2 * uint32_1 * uint32_4)];
    uint num2 = 128;
    if (textAsset.bytes[84] == (byte) 68 && textAsset.bytes[85] == (byte) 88 && (textAsset.bytes[86] == (byte) 49 && textAsset.bytes[87] == (byte) 48) && ((int) uint32_5 & 4) != 0)
    {
      uint uint32_6 = BitConverter.ToUInt32(textAsset.bytes, (int) num2);
      if (uint32_6 >= 60U && uint32_6 <= 65U)
        num1 = 8U;
      else if (uint32_6 >= 48U && uint32_6 <= 52U)
        num1 = 16U;
      else if (uint32_6 >= 27U && uint32_6 <= 32U)
        num1 = 32U;
      num2 += 20U;
    }
    uint num3 = num1 / 8U;
    uint num4 = (uint) ((int) uint32_2 * (int) num1 + 7) / 8U;
    for (int index1 = 0; (long) index1 < (long) uint32_4; ++index1)
    {
      for (int index2 = 0; (long) index2 < (long) uint32_1; ++index2)
      {
        for (int index3 = 0; (long) index3 < (long) uint32_2; ++index3)
        {
          float num5 = (float) bytes[(long) num2 + (long) index3 * (long) num3] / (float) byte.MaxValue;
          colors[(long) index3 + (long) index2 * (long) uint32_2 + (long) index1 * (long) uint32_2 * (long) uint32_1] = new Color(num5, num5, num5, num5);
        }
        num2 += num4;
      }
    }
    this._noiseTexture.SetPixels(colors);
    this._noiseTexture.Apply();
  }

  private void GenerateDitherTexture()
  {
    if ((Object) this._ditheringTexture != (Object) null)
      return;
    int num1 = 8;
    this._ditheringTexture = new Texture2D(num1, num1, TextureFormat.Alpha8, false, true);
    this._ditheringTexture.filterMode = FilterMode.Point;
    Color32[] colors = new Color32[num1 * num1];
    int num2 = 0;
    byte num3 = 3;
    Color32[] color32Array1 = colors;
    int index1 = num2;
    int num4 = index1 + 1;
    color32Array1[index1] = new Color32(num3, num3, num3, num3);
    byte num5 = 192;
    Color32[] color32Array2 = colors;
    int index2 = num4;
    int num6 = index2 + 1;
    color32Array2[index2] = new Color32(num5, num5, num5, num5);
    byte num7 = 51;
    Color32[] color32Array3 = colors;
    int index3 = num6;
    int num8 = index3 + 1;
    color32Array3[index3] = new Color32(num7, num7, num7, num7);
    byte num9 = 239;
    Color32[] color32Array4 = colors;
    int index4 = num8;
    int num10 = index4 + 1;
    color32Array4[index4] = new Color32(num9, num9, num9, num9);
    byte num11 = 15;
    Color32[] color32Array5 = colors;
    int index5 = num10;
    int num12 = index5 + 1;
    color32Array5[index5] = new Color32(num11, num11, num11, num11);
    byte num13 = 204;
    Color32[] color32Array6 = colors;
    int index6 = num12;
    int num14 = index6 + 1;
    color32Array6[index6] = new Color32(num13, num13, num13, num13);
    byte num15 = 62;
    Color32[] color32Array7 = colors;
    int index7 = num14;
    int num16 = index7 + 1;
    color32Array7[index7] = new Color32(num15, num15, num15, num15);
    byte num17 = 251;
    Color32[] color32Array8 = colors;
    int index8 = num16;
    int num18 = index8 + 1;
    color32Array8[index8] = new Color32(num17, num17, num17, num17);
    byte num19 = 129;
    Color32[] color32Array9 = colors;
    int index9 = num18;
    int num20 = index9 + 1;
    color32Array9[index9] = new Color32(num19, num19, num19, num19);
    byte num21 = 66;
    Color32[] color32Array10 = colors;
    int index10 = num20;
    int num22 = index10 + 1;
    color32Array10[index10] = new Color32(num21, num21, num21, num21);
    byte num23 = 176;
    Color32[] color32Array11 = colors;
    int index11 = num22;
    int num24 = index11 + 1;
    color32Array11[index11] = new Color32(num23, num23, num23, num23);
    byte num25 = 113;
    Color32[] color32Array12 = colors;
    int index12 = num24;
    int num26 = index12 + 1;
    color32Array12[index12] = new Color32(num25, num25, num25, num25);
    byte num27 = 141;
    Color32[] color32Array13 = colors;
    int index13 = num26;
    int num28 = index13 + 1;
    color32Array13[index13] = new Color32(num27, num27, num27, num27);
    byte num29 = 78;
    Color32[] color32Array14 = colors;
    int index14 = num28;
    int num30 = index14 + 1;
    color32Array14[index14] = new Color32(num29, num29, num29, num29);
    byte num31 = 188;
    Color32[] color32Array15 = colors;
    int index15 = num30;
    int num32 = index15 + 1;
    color32Array15[index15] = new Color32(num31, num31, num31, num31);
    byte num33 = 125;
    Color32[] color32Array16 = colors;
    int index16 = num32;
    int num34 = index16 + 1;
    color32Array16[index16] = new Color32(num33, num33, num33, num33);
    byte num35 = 35;
    Color32[] color32Array17 = colors;
    int index17 = num34;
    int num36 = index17 + 1;
    color32Array17[index17] = new Color32(num35, num35, num35, num35);
    byte num37 = 223;
    Color32[] color32Array18 = colors;
    int index18 = num36;
    int num38 = index18 + 1;
    color32Array18[index18] = new Color32(num37, num37, num37, num37);
    byte num39 = 19;
    Color32[] color32Array19 = colors;
    int index19 = num38;
    int num40 = index19 + 1;
    color32Array19[index19] = new Color32(num39, num39, num39, num39);
    byte num41 = 207;
    Color32[] color32Array20 = colors;
    int index20 = num40;
    int num42 = index20 + 1;
    color32Array20[index20] = new Color32(num41, num41, num41, num41);
    byte num43 = 47;
    Color32[] color32Array21 = colors;
    int index21 = num42;
    int num44 = index21 + 1;
    color32Array21[index21] = new Color32(num43, num43, num43, num43);
    byte num45 = 235;
    Color32[] color32Array22 = colors;
    int index22 = num44;
    int num46 = index22 + 1;
    color32Array22[index22] = new Color32(num45, num45, num45, num45);
    byte num47 = 31;
    Color32[] color32Array23 = colors;
    int index23 = num46;
    int num48 = index23 + 1;
    color32Array23[index23] = new Color32(num47, num47, num47, num47);
    byte num49 = 219;
    Color32[] color32Array24 = colors;
    int index24 = num48;
    int num50 = index24 + 1;
    color32Array24[index24] = new Color32(num49, num49, num49, num49);
    byte num51 = 160;
    Color32[] color32Array25 = colors;
    int index25 = num50;
    int num52 = index25 + 1;
    color32Array25[index25] = new Color32(num51, num51, num51, num51);
    byte num53 = 98;
    Color32[] color32Array26 = colors;
    int index26 = num52;
    int num54 = index26 + 1;
    color32Array26[index26] = new Color32(num53, num53, num53, num53);
    byte num55 = 145;
    Color32[] color32Array27 = colors;
    int index27 = num54;
    int num56 = index27 + 1;
    color32Array27[index27] = new Color32(num55, num55, num55, num55);
    byte num57 = 82;
    Color32[] color32Array28 = colors;
    int index28 = num56;
    int num58 = index28 + 1;
    color32Array28[index28] = new Color32(num57, num57, num57, num57);
    byte num59 = 172;
    Color32[] color32Array29 = colors;
    int index29 = num58;
    int num60 = index29 + 1;
    color32Array29[index29] = new Color32(num59, num59, num59, num59);
    byte num61 = 109;
    Color32[] color32Array30 = colors;
    int index30 = num60;
    int num62 = index30 + 1;
    color32Array30[index30] = new Color32(num61, num61, num61, num61);
    byte num63 = 156;
    Color32[] color32Array31 = colors;
    int index31 = num62;
    int num64 = index31 + 1;
    color32Array31[index31] = new Color32(num63, num63, num63, num63);
    byte num65 = 94;
    Color32[] color32Array32 = colors;
    int index32 = num64;
    int num66 = index32 + 1;
    color32Array32[index32] = new Color32(num65, num65, num65, num65);
    byte num67 = 11;
    Color32[] color32Array33 = colors;
    int index33 = num66;
    int num68 = index33 + 1;
    color32Array33[index33] = new Color32(num67, num67, num67, num67);
    byte num69 = 200;
    Color32[] color32Array34 = colors;
    int index34 = num68;
    int num70 = index34 + 1;
    color32Array34[index34] = new Color32(num69, num69, num69, num69);
    byte num71 = 58;
    Color32[] color32Array35 = colors;
    int index35 = num70;
    int num72 = index35 + 1;
    color32Array35[index35] = new Color32(num71, num71, num71, num71);
    byte num73 = 247;
    Color32[] color32Array36 = colors;
    int index36 = num72;
    int num74 = index36 + 1;
    color32Array36[index36] = new Color32(num73, num73, num73, num73);
    byte num75 = 7;
    Color32[] color32Array37 = colors;
    int index37 = num74;
    int num76 = index37 + 1;
    color32Array37[index37] = new Color32(num75, num75, num75, num75);
    byte num77 = 196;
    Color32[] color32Array38 = colors;
    int index38 = num76;
    int num78 = index38 + 1;
    color32Array38[index38] = new Color32(num77, num77, num77, num77);
    byte num79 = 54;
    Color32[] color32Array39 = colors;
    int index39 = num78;
    int num80 = index39 + 1;
    color32Array39[index39] = new Color32(num79, num79, num79, num79);
    byte num81 = 243;
    Color32[] color32Array40 = colors;
    int index40 = num80;
    int num82 = index40 + 1;
    color32Array40[index40] = new Color32(num81, num81, num81, num81);
    byte num83 = 137;
    Color32[] color32Array41 = colors;
    int index41 = num82;
    int num84 = index41 + 1;
    color32Array41[index41] = new Color32(num83, num83, num83, num83);
    byte num85 = 74;
    Color32[] color32Array42 = colors;
    int index42 = num84;
    int num86 = index42 + 1;
    color32Array42[index42] = new Color32(num85, num85, num85, num85);
    byte num87 = 184;
    Color32[] color32Array43 = colors;
    int index43 = num86;
    int num88 = index43 + 1;
    color32Array43[index43] = new Color32(num87, num87, num87, num87);
    byte num89 = 121;
    Color32[] color32Array44 = colors;
    int index44 = num88;
    int num90 = index44 + 1;
    color32Array44[index44] = new Color32(num89, num89, num89, num89);
    byte num91 = 133;
    Color32[] color32Array45 = colors;
    int index45 = num90;
    int num92 = index45 + 1;
    color32Array45[index45] = new Color32(num91, num91, num91, num91);
    byte num93 = 70;
    Color32[] color32Array46 = colors;
    int index46 = num92;
    int num94 = index46 + 1;
    color32Array46[index46] = new Color32(num93, num93, num93, num93);
    byte num95 = 180;
    Color32[] color32Array47 = colors;
    int index47 = num94;
    int num96 = index47 + 1;
    color32Array47[index47] = new Color32(num95, num95, num95, num95);
    byte num97 = 117;
    Color32[] color32Array48 = colors;
    int index48 = num96;
    int num98 = index48 + 1;
    color32Array48[index48] = new Color32(num97, num97, num97, num97);
    byte num99 = 43;
    Color32[] color32Array49 = colors;
    int index49 = num98;
    int num100 = index49 + 1;
    color32Array49[index49] = new Color32(num99, num99, num99, num99);
    byte num101 = 231;
    Color32[] color32Array50 = colors;
    int index50 = num100;
    int num102 = index50 + 1;
    color32Array50[index50] = new Color32(num101, num101, num101, num101);
    byte num103 = 27;
    Color32[] color32Array51 = colors;
    int index51 = num102;
    int num104 = index51 + 1;
    color32Array51[index51] = new Color32(num103, num103, num103, num103);
    byte num105 = 215;
    Color32[] color32Array52 = colors;
    int index52 = num104;
    int num106 = index52 + 1;
    color32Array52[index52] = new Color32(num105, num105, num105, num105);
    byte num107 = 39;
    Color32[] color32Array53 = colors;
    int index53 = num106;
    int num108 = index53 + 1;
    color32Array53[index53] = new Color32(num107, num107, num107, num107);
    byte num109 = 227;
    Color32[] color32Array54 = colors;
    int index54 = num108;
    int num110 = index54 + 1;
    color32Array54[index54] = new Color32(num109, num109, num109, num109);
    byte num111 = 23;
    Color32[] color32Array55 = colors;
    int index55 = num110;
    int num112 = index55 + 1;
    color32Array55[index55] = new Color32(num111, num111, num111, num111);
    byte num113 = 211;
    Color32[] color32Array56 = colors;
    int index56 = num112;
    int num114 = index56 + 1;
    color32Array56[index56] = new Color32(num113, num113, num113, num113);
    byte num115 = 168;
    Color32[] color32Array57 = colors;
    int index57 = num114;
    int num116 = index57 + 1;
    color32Array57[index57] = new Color32(num115, num115, num115, num115);
    byte num117 = 105;
    Color32[] color32Array58 = colors;
    int index58 = num116;
    int num118 = index58 + 1;
    color32Array58[index58] = new Color32(num117, num117, num117, num117);
    byte num119 = 153;
    Color32[] color32Array59 = colors;
    int index59 = num118;
    int num120 = index59 + 1;
    color32Array59[index59] = new Color32(num119, num119, num119, num119);
    byte num121 = 90;
    Color32[] color32Array60 = colors;
    int index60 = num120;
    int num122 = index60 + 1;
    color32Array60[index60] = new Color32(num121, num121, num121, num121);
    byte num123 = 164;
    Color32[] color32Array61 = colors;
    int index61 = num122;
    int num124 = index61 + 1;
    color32Array61[index61] = new Color32(num123, num123, num123, num123);
    byte num125 = 102;
    Color32[] color32Array62 = colors;
    int index62 = num124;
    int num126 = index62 + 1;
    color32Array62[index62] = new Color32(num125, num125, num125, num125);
    byte num127 = 149;
    Color32[] color32Array63 = colors;
    int index63 = num126;
    int num128 = index63 + 1;
    color32Array63[index63] = new Color32(num127, num127, num127, num127);
    byte num129 = 86;
    Color32[] color32Array64 = colors;
    int index64 = num128;
    int num130 = index64 + 1;
    color32Array64[index64] = new Color32(num129, num129, num129, num129);
    this._ditheringTexture.SetPixels32(colors);
    this._ditheringTexture.Apply();
  }

  private Mesh CreateSpotLightMesh()
  {
    Mesh mesh = new Mesh();
    Vector3[] vector3Array = new Vector3[50];
    Color32[] color32Array = new Color32[50];
    vector3Array[0] = new Vector3(0.0f, 0.0f, 0.0f);
    vector3Array[1] = new Vector3(0.0f, 0.0f, 1f);
    float f = 0.0f;
    float num1 = 0.3926991f;
    float z = 0.9f;
    for (int index = 0; index < 16; ++index)
    {
      vector3Array[index + 2] = new Vector3(-Mathf.Cos(f) * z, Mathf.Sin(f) * z, z);
      color32Array[index + 2] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      vector3Array[index + 2 + 16] = new Vector3(-Mathf.Cos(f), Mathf.Sin(f), 1f);
      color32Array[index + 2 + 16] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte) 0);
      vector3Array[index + 2 + 32] = new Vector3(-Mathf.Cos(f) * z, Mathf.Sin(f) * z, 1f);
      color32Array[index + 2 + 32] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      f += num1;
    }
    mesh.vertices = vector3Array;
    mesh.colors32 = color32Array;
    int[] numArray1 = new int[288];
    int num2 = 0;
    for (int index1 = 2; index1 < 17; ++index1)
    {
      int[] numArray2 = numArray1;
      int index2 = num2;
      int num3 = index2 + 1;
      int num4 = 0;
      numArray2[index2] = num4;
      int[] numArray3 = numArray1;
      int index3 = num3;
      int num5 = index3 + 1;
      int num6 = index1;
      numArray3[index3] = num6;
      int[] numArray4 = numArray1;
      int index4 = num5;
      num2 = index4 + 1;
      int num7 = index1 + 1;
      numArray4[index4] = num7;
    }
    int[] numArray5 = numArray1;
    int index5 = num2;
    int num8 = index5 + 1;
    int num9 = 0;
    numArray5[index5] = num9;
    int[] numArray6 = numArray1;
    int index6 = num8;
    int num10 = index6 + 1;
    int num11 = 17;
    numArray6[index6] = num11;
    int[] numArray7 = numArray1;
    int index7 = num10;
    int num12 = index7 + 1;
    int num13 = 2;
    numArray7[index7] = num13;
    for (int index1 = 2; index1 < 17; ++index1)
    {
      int[] numArray2 = numArray1;
      int index2 = num12;
      int num3 = index2 + 1;
      int num4 = index1;
      numArray2[index2] = num4;
      int[] numArray3 = numArray1;
      int index3 = num3;
      int num5 = index3 + 1;
      int num6 = index1 + 16;
      numArray3[index3] = num6;
      int[] numArray4 = numArray1;
      int index4 = num5;
      int num7 = index4 + 1;
      int num14 = index1 + 1;
      numArray4[index4] = num14;
      int[] numArray8 = numArray1;
      int index8 = num7;
      int num15 = index8 + 1;
      int num16 = index1 + 1;
      numArray8[index8] = num16;
      int[] numArray9 = numArray1;
      int index9 = num15;
      int num17 = index9 + 1;
      int num18 = index1 + 16;
      numArray9[index9] = num18;
      int[] numArray10 = numArray1;
      int index10 = num17;
      num12 = index10 + 1;
      int num19 = index1 + 16 + 1;
      numArray10[index10] = num19;
    }
    int[] numArray11 = numArray1;
    int index11 = num12;
    int num20 = index11 + 1;
    int num21 = 2;
    numArray11[index11] = num21;
    int[] numArray12 = numArray1;
    int index12 = num20;
    int num22 = index12 + 1;
    int num23 = 17;
    numArray12[index12] = num23;
    int[] numArray13 = numArray1;
    int index13 = num22;
    int num24 = index13 + 1;
    int num25 = 18;
    numArray13[index13] = num25;
    int[] numArray14 = numArray1;
    int index14 = num24;
    int num26 = index14 + 1;
    int num27 = 18;
    numArray14[index14] = num27;
    int[] numArray15 = numArray1;
    int index15 = num26;
    int num28 = index15 + 1;
    int num29 = 17;
    numArray15[index15] = num29;
    int[] numArray16 = numArray1;
    int index16 = num28;
    int num30 = index16 + 1;
    int num31 = 33;
    numArray16[index16] = num31;
    for (int index1 = 18; index1 < 33; ++index1)
    {
      int[] numArray2 = numArray1;
      int index2 = num30;
      int num3 = index2 + 1;
      int num4 = index1;
      numArray2[index2] = num4;
      int[] numArray3 = numArray1;
      int index3 = num3;
      int num5 = index3 + 1;
      int num6 = index1 + 16;
      numArray3[index3] = num6;
      int[] numArray4 = numArray1;
      int index4 = num5;
      int num7 = index4 + 1;
      int num14 = index1 + 1;
      numArray4[index4] = num14;
      int[] numArray8 = numArray1;
      int index8 = num7;
      int num15 = index8 + 1;
      int num16 = index1 + 1;
      numArray8[index8] = num16;
      int[] numArray9 = numArray1;
      int index9 = num15;
      int num17 = index9 + 1;
      int num18 = index1 + 16;
      numArray9[index9] = num18;
      int[] numArray10 = numArray1;
      int index10 = num17;
      num30 = index10 + 1;
      int num19 = index1 + 16 + 1;
      numArray10[index10] = num19;
    }
    int[] numArray17 = numArray1;
    int index17 = num30;
    int num32 = index17 + 1;
    int num33 = 18;
    numArray17[index17] = num33;
    int[] numArray18 = numArray1;
    int index18 = num32;
    int num34 = index18 + 1;
    int num35 = 33;
    numArray18[index18] = num35;
    int[] numArray19 = numArray1;
    int index19 = num34;
    int num36 = index19 + 1;
    int num37 = 34;
    numArray19[index19] = num37;
    int[] numArray20 = numArray1;
    int index20 = num36;
    int num38 = index20 + 1;
    int num39 = 34;
    numArray20[index20] = num39;
    int[] numArray21 = numArray1;
    int index21 = num38;
    int num40 = index21 + 1;
    int num41 = 33;
    numArray21[index21] = num41;
    int[] numArray22 = numArray1;
    int index22 = num40;
    int num42 = index22 + 1;
    int num43 = 49;
    numArray22[index22] = num43;
    for (int index1 = 34; index1 < 49; ++index1)
    {
      int[] numArray2 = numArray1;
      int index2 = num42;
      int num3 = index2 + 1;
      int num4 = 1;
      numArray2[index2] = num4;
      int[] numArray3 = numArray1;
      int index3 = num3;
      int num5 = index3 + 1;
      int num6 = index1 + 1;
      numArray3[index3] = num6;
      int[] numArray4 = numArray1;
      int index4 = num5;
      num42 = index4 + 1;
      int num7 = index1;
      numArray4[index4] = num7;
    }
    int[] numArray23 = numArray1;
    int index23 = num42;
    int num44 = index23 + 1;
    int num45 = 1;
    numArray23[index23] = num45;
    int[] numArray24 = numArray1;
    int index24 = num44;
    int num46 = index24 + 1;
    int num47 = 34;
    numArray24[index24] = num47;
    int[] numArray25 = numArray1;
    int index25 = num46;
    int num48 = index25 + 1;
    int num49 = 49;
    numArray25[index25] = num49;
    mesh.triangles = numArray1;
    mesh.RecalculateBounds();
    return mesh;
  }

  public enum VolumtericResolution
  {
    Full,
    Half,
    Quarter,
  }
}
