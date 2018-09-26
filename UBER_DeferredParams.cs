// Decompiled with JetBrains decompiler
// Type: UBER_DeferredParams
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("UBER/Deferred Params")]
[RequireComponent(typeof (Camera))]
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class UBER_DeferredParams : MonoBehaviour
{
  [ColorUsage(false)]
  [Header("Translucency setup 1")]
  public Color TranslucencyColor1;
  [Tooltip("You can control strength per light using its color alpha (first enable in UBER config file)")]
  public float Strength1;
  [Range(0.0f, 1f)]
  public float PointLightsDirectionality1;
  [Range(0.0f, 0.5f)]
  public float Constant1;
  [Range(0.0f, 0.3f)]
  public float Scattering1;
  [Range(0.0f, 100f)]
  public float SpotExponent1;
  [Range(0.0f, 20f)]
  public float SuppressShadows1;
  [Range(0.0f, 1f)]
  public float NdotLReduction1;
  [ColorUsage(false)]
  [Header("Translucency setup 2")]
  [Space]
  public Color TranslucencyColor2;
  [Tooltip("You can control strength per light using its color alpha (first enable in UBER config file)")]
  public float Strength2;
  [Range(0.0f, 1f)]
  public float PointLightsDirectionality2;
  [Range(0.0f, 0.5f)]
  public float Constant2;
  [Range(0.0f, 0.3f)]
  public float Scattering2;
  [Range(0.0f, 100f)]
  public float SpotExponent2;
  [Range(0.0f, 20f)]
  public float SuppressShadows2;
  [Range(0.0f, 1f)]
  public float NdotLReduction2;
  [ColorUsage(false)]
  [Header("Translucency setup 3")]
  [Space]
  public Color TranslucencyColor3;
  [Tooltip("You can control strength per light using its color alpha (first enable in UBER config file)")]
  public float Strength3;
  [Range(0.0f, 1f)]
  public float PointLightsDirectionality3;
  [Range(0.0f, 0.5f)]
  public float Constant3;
  [Range(0.0f, 0.3f)]
  public float Scattering3;
  [Range(0.0f, 100f)]
  public float SpotExponent3;
  [Range(0.0f, 20f)]
  public float SuppressShadows3;
  [Range(0.0f, 1f)]
  public float NdotLReduction3;
  [ColorUsage(false)]
  [Header("Translucency setup 4")]
  [Space]
  public Color TranslucencyColor4;
  [Tooltip("You can control strength per light using its color alpha (first enable in UBER config file)")]
  public float Strength4;
  [Range(0.0f, 1f)]
  public float PointLightsDirectionality4;
  [Range(0.0f, 0.5f)]
  public float Constant4;
  [Range(0.0f, 0.3f)]
  public float Scattering4;
  [Range(0.0f, 100f)]
  public float SpotExponent4;
  [Range(0.0f, 20f)]
  public float SuppressShadows4;
  [Range(0.0f, 1f)]
  public float NdotLReduction4;
  private Camera mycam;
  private CommandBuffer combufPreLight;
  private CommandBuffer combufPostLight;
  private Material CopyPropsMat;
  private bool UBERPresenceChecked;
  private bool UBERPresent;
  [HideInInspector]
  public Texture2D TranslucencyPropsTex;
  private HashSet<Camera> sceneCamsWithBuffer;

  public UBER_DeferredParams()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.SetupTranslucencyValues();
  }

  public void OnValidate()
  {
    this.SetupTranslucencyValues();
  }

  public void SetupTranslucencyValues()
  {
    if (Object.op_Equality((Object) this.TranslucencyPropsTex, (Object) null))
    {
      this.TranslucencyPropsTex = new Texture2D(4, 3, (TextureFormat) 20, false, true);
      ((Texture) this.TranslucencyPropsTex).set_anisoLevel(0);
      ((Texture) this.TranslucencyPropsTex).set_filterMode((FilterMode) 0);
      ((Texture) this.TranslucencyPropsTex).set_wrapMode((TextureWrapMode) 1);
      ((Object) this.TranslucencyPropsTex).set_hideFlags((HideFlags) 61);
    }
    Shader.SetGlobalTexture("_UBERTranslucencySetup", (Texture) this.TranslucencyPropsTex);
    byte[] rawTexdata = new byte[192];
    this.EncodeRGBAFloatTo16Bytes((float) this.TranslucencyColor1.r, (float) this.TranslucencyColor1.g, (float) this.TranslucencyColor1.b, this.Strength1, rawTexdata, 0, 0);
    this.EncodeRGBAFloatTo16Bytes(this.PointLightsDirectionality1, this.Constant1, this.Scattering1, this.SpotExponent1, rawTexdata, 0, 1);
    this.EncodeRGBAFloatTo16Bytes(this.SuppressShadows1, this.NdotLReduction1, 1f, 1f, rawTexdata, 0, 2);
    this.EncodeRGBAFloatTo16Bytes((float) this.TranslucencyColor2.r, (float) this.TranslucencyColor2.g, (float) this.TranslucencyColor2.b, this.Strength2, rawTexdata, 1, 0);
    this.EncodeRGBAFloatTo16Bytes(this.PointLightsDirectionality2, this.Constant2, this.Scattering2, this.SpotExponent2, rawTexdata, 1, 1);
    this.EncodeRGBAFloatTo16Bytes(this.SuppressShadows2, this.NdotLReduction2, 1f, 1f, rawTexdata, 1, 2);
    this.EncodeRGBAFloatTo16Bytes((float) this.TranslucencyColor3.r, (float) this.TranslucencyColor3.g, (float) this.TranslucencyColor3.b, this.Strength3, rawTexdata, 2, 0);
    this.EncodeRGBAFloatTo16Bytes(this.PointLightsDirectionality3, this.Constant3, this.Scattering3, this.SpotExponent3, rawTexdata, 2, 1);
    this.EncodeRGBAFloatTo16Bytes(this.SuppressShadows3, this.NdotLReduction3, 1f, 1f, rawTexdata, 2, 2);
    this.EncodeRGBAFloatTo16Bytes((float) this.TranslucencyColor4.r, (float) this.TranslucencyColor4.g, (float) this.TranslucencyColor4.b, this.Strength4, rawTexdata, 3, 0);
    this.EncodeRGBAFloatTo16Bytes(this.PointLightsDirectionality4, this.Constant4, this.Scattering4, this.SpotExponent4, rawTexdata, 3, 1);
    this.EncodeRGBAFloatTo16Bytes(this.SuppressShadows4, this.NdotLReduction4, 1f, 1f, rawTexdata, 3, 2);
    this.TranslucencyPropsTex.LoadRawTextureData(rawTexdata);
    this.TranslucencyPropsTex.Apply();
  }

  private void EncodeRGBAFloatTo16Bytes(float r, float g, float b, float a, byte[] rawTexdata, int idx_u, int idx_v)
  {
    int num1 = idx_v * 4 * 16 + idx_u * 16;
    UBER_RGBA_ByteArray uberRgbaByteArray = new UBER_RGBA_ByteArray();
    uberRgbaByteArray.R = r;
    uberRgbaByteArray.G = g;
    uberRgbaByteArray.B = b;
    uberRgbaByteArray.A = a;
    byte[] numArray1 = rawTexdata;
    int index1 = num1;
    int num2 = index1 + 1;
    int byte0 = (int) uberRgbaByteArray.Byte0;
    numArray1[index1] = (byte) byte0;
    byte[] numArray2 = rawTexdata;
    int index2 = num2;
    int num3 = index2 + 1;
    int byte1 = (int) uberRgbaByteArray.Byte1;
    numArray2[index2] = (byte) byte1;
    byte[] numArray3 = rawTexdata;
    int index3 = num3;
    int num4 = index3 + 1;
    int byte2 = (int) uberRgbaByteArray.Byte2;
    numArray3[index3] = (byte) byte2;
    byte[] numArray4 = rawTexdata;
    int index4 = num4;
    int num5 = index4 + 1;
    int byte3 = (int) uberRgbaByteArray.Byte3;
    numArray4[index4] = (byte) byte3;
    byte[] numArray5 = rawTexdata;
    int index5 = num5;
    int num6 = index5 + 1;
    int byte4 = (int) uberRgbaByteArray.Byte4;
    numArray5[index5] = (byte) byte4;
    byte[] numArray6 = rawTexdata;
    int index6 = num6;
    int num7 = index6 + 1;
    int byte5 = (int) uberRgbaByteArray.Byte5;
    numArray6[index6] = (byte) byte5;
    byte[] numArray7 = rawTexdata;
    int index7 = num7;
    int num8 = index7 + 1;
    int byte6 = (int) uberRgbaByteArray.Byte6;
    numArray7[index7] = (byte) byte6;
    byte[] numArray8 = rawTexdata;
    int index8 = num8;
    int num9 = index8 + 1;
    int byte7 = (int) uberRgbaByteArray.Byte7;
    numArray8[index8] = (byte) byte7;
    byte[] numArray9 = rawTexdata;
    int index9 = num9;
    int num10 = index9 + 1;
    int byte8 = (int) uberRgbaByteArray.Byte8;
    numArray9[index9] = (byte) byte8;
    byte[] numArray10 = rawTexdata;
    int index10 = num10;
    int num11 = index10 + 1;
    int byte9 = (int) uberRgbaByteArray.Byte9;
    numArray10[index10] = (byte) byte9;
    byte[] numArray11 = rawTexdata;
    int index11 = num11;
    int num12 = index11 + 1;
    int byte10 = (int) uberRgbaByteArray.Byte10;
    numArray11[index11] = (byte) byte10;
    byte[] numArray12 = rawTexdata;
    int index12 = num12;
    int num13 = index12 + 1;
    int byte11 = (int) uberRgbaByteArray.Byte11;
    numArray12[index12] = (byte) byte11;
    byte[] numArray13 = rawTexdata;
    int index13 = num13;
    int num14 = index13 + 1;
    int byte12 = (int) uberRgbaByteArray.Byte12;
    numArray13[index13] = (byte) byte12;
    byte[] numArray14 = rawTexdata;
    int index14 = num14;
    int num15 = index14 + 1;
    int byte13 = (int) uberRgbaByteArray.Byte13;
    numArray14[index14] = (byte) byte13;
    byte[] numArray15 = rawTexdata;
    int index15 = num15;
    int num16 = index15 + 1;
    int byte14 = (int) uberRgbaByteArray.Byte14;
    numArray15[index15] = (byte) byte14;
    byte[] numArray16 = rawTexdata;
    int index16 = num16;
    int num17 = index16 + 1;
    int byte15 = (int) uberRgbaByteArray.Byte15;
    numArray16[index16] = (byte) byte15;
  }

  public void OnEnable()
  {
    this.SetupTranslucencyValues();
    if (this.NotifyDecals())
      return;
    if (Object.op_Equality((Object) this.mycam, (Object) null))
    {
      this.mycam = (Camera) ((Component) this).GetComponent<Camera>();
      if (Object.op_Equality((Object) this.mycam, (Object) null))
        return;
    }
    this.Initialize();
    // ISSUE: method pointer
    Camera.onPreRender = (__Null) Delegate.Combine((Delegate) Camera.onPreRender, (Delegate) new Camera.CameraCallback((object) this, __methodptr(SetupCam)));
  }

  public void OnDisable()
  {
    this.NotifyDecals();
    this.Cleanup();
  }

  public void OnDestroy()
  {
    this.NotifyDecals();
    this.Cleanup();
  }

  private bool NotifyDecals()
  {
    Type type = Type.GetType("UBERDecalSystem.DecalManager");
    if (type != null)
    {
      if (Object.op_Inequality(Object.FindObjectOfType(type), (Object) null) && Object.FindObjectOfType(type) is MonoBehaviour && ((Behaviour) (Object.FindObjectOfType(type) as MonoBehaviour)).get_enabled())
      {
        (Object.FindObjectOfType(type) as MonoBehaviour).Invoke("OnDisable", 0.0f);
        (Object.FindObjectOfType(type) as MonoBehaviour).Invoke("OnEnable", 0.0f);
        return true;
      }
    }
    return false;
  }

  private void Cleanup()
  {
    if (Object.op_Implicit((Object) this.TranslucencyPropsTex))
    {
      Object.DestroyImmediate((Object) this.TranslucencyPropsTex);
      this.TranslucencyPropsTex = (Texture2D) null;
    }
    if (this.combufPreLight != null)
    {
      if (Object.op_Implicit((Object) this.mycam))
      {
        this.mycam.RemoveCommandBuffer((CameraEvent) 21, this.combufPreLight);
        this.mycam.RemoveCommandBuffer((CameraEvent) 7, this.combufPostLight);
      }
      using (HashSet<Camera>.Enumerator enumerator = this.sceneCamsWithBuffer.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Camera current = enumerator.Current;
          if (Object.op_Implicit((Object) current))
          {
            current.RemoveCommandBuffer((CameraEvent) 21, this.combufPreLight);
            current.RemoveCommandBuffer((CameraEvent) 7, this.combufPostLight);
          }
        }
      }
    }
    this.sceneCamsWithBuffer.Clear();
    // ISSUE: method pointer
    Camera.onPreRender = (__Null) Delegate.Remove((Delegate) Camera.onPreRender, (Delegate) new Camera.CameraCallback((object) this, __methodptr(SetupCam)));
  }

  private void SetupCam(Camera cam)
  {
    bool isSceneCam = false;
    if (!Object.op_Equality((Object) cam, (Object) this.mycam) && !isSceneCam)
      return;
    this.RefreshComBufs(cam, isSceneCam);
  }

  public void RefreshComBufs(Camera cam, bool isSceneCam)
  {
    if (!Object.op_Implicit((Object) cam) || this.combufPreLight == null || this.combufPostLight == null)
      return;
    CommandBuffer[] commandBuffers = cam.GetCommandBuffers((CameraEvent) 21);
    bool flag = false;
    foreach (CommandBuffer commandBuffer in commandBuffers)
    {
      if (commandBuffer.get_name() == this.combufPreLight.get_name())
      {
        flag = true;
        break;
      }
    }
    if (flag)
      return;
    cam.AddCommandBuffer((CameraEvent) 21, this.combufPreLight);
    cam.AddCommandBuffer((CameraEvent) 7, this.combufPostLight);
    if (!isSceneCam)
      return;
    this.sceneCamsWithBuffer.Add(cam);
  }

  public void Initialize()
  {
    if (this.combufPreLight != null)
      return;
    int id = Shader.PropertyToID("_UBERPropsBuffer");
    if (Object.op_Equality((Object) this.CopyPropsMat, (Object) null))
    {
      if (Object.op_Inequality((Object) this.CopyPropsMat, (Object) null))
        Object.DestroyImmediate((Object) this.CopyPropsMat);
      this.CopyPropsMat = new Material(Shader.Find("Hidden/UBER_CopyPropsTexture"));
      ((Object) this.CopyPropsMat).set_hideFlags((HideFlags) 52);
    }
    this.combufPreLight = new CommandBuffer();
    this.combufPreLight.set_name("UBERPropsPrelight");
    this.combufPreLight.GetTemporaryRT(id, -1, -1, 0, (FilterMode) 0, (RenderTextureFormat) 15);
    this.combufPreLight.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), RenderTargetIdentifier.op_Implicit(id), this.CopyPropsMat);
    this.combufPostLight = new CommandBuffer();
    this.combufPostLight.set_name("UBERPropsPostlight");
    this.combufPostLight.ReleaseTemporaryRT(id);
  }
}
