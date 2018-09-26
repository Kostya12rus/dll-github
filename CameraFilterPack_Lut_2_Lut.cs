﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Lut_2_Lut
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Lut/Lut 2 Lut")]
[ExecuteInEditMode]
public class CameraFilterPack_Lut_2_Lut : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.0f, 1f)]
  public float Blend = 1f;
  [Range(0.0f, 1f)]
  public float Fade = 1f;
  public Shader SCShader;
  private Vector4 ScreenResolution;
  private Material SCMaterial;
  public Texture2D LutTexture;
  public Texture2D LutTexture2;
  private Texture3D converted3DLut;
  private Texture3D converted3DLut2;
  private string MemoPath;
  private string MemoPath2;

  private Material material
  {
    get
    {
      if ((Object) this.SCMaterial == (Object) null)
      {
        this.SCMaterial = new Material(this.SCShader);
        this.SCMaterial.hideFlags = HideFlags.HideAndDontSave;
      }
      return this.SCMaterial;
    }
  }

  private void Start()
  {
    this.SCShader = Shader.Find("CameraFilterPack/Lut_2_lut");
    if (SystemInfo.supportsImageEffects)
      return;
    this.enabled = false;
  }

  public void SetIdentityLut()
  {
    int num1 = 16;
    Color[] colors = new Color[num1 * num1 * num1];
    float num2 = (float) (1.0 / (1.0 * (double) num1 - 1.0));
    for (int index1 = 0; index1 < num1; ++index1)
    {
      for (int index2 = 0; index2 < num1; ++index2)
      {
        for (int index3 = 0; index3 < num1; ++index3)
          colors[index1 + index2 * num1 + index3 * num1 * num1] = new Color((float) index1 * 1f * num2, (float) index2 * 1f * num2, (float) index3 * 1f * num2, 1f);
      }
    }
    if ((bool) ((Object) this.converted3DLut))
      Object.DestroyImmediate((Object) this.converted3DLut);
    this.converted3DLut = new Texture3D(num1, num1, num1, TextureFormat.ARGB32, false);
    this.converted3DLut.SetPixels(colors);
    this.converted3DLut.Apply();
    if ((bool) ((Object) this.converted3DLut2))
      Object.DestroyImmediate((Object) this.converted3DLut2);
    this.converted3DLut2 = new Texture3D(num1, num1, num1, TextureFormat.ARGB32, false);
    this.converted3DLut2.SetPixels(colors);
    this.converted3DLut2.Apply();
  }

  public bool ValidDimensions(Texture2D tex2d)
  {
    return (bool) ((Object) tex2d) && tex2d.height == Mathf.FloorToInt(Mathf.Sqrt((float) tex2d.width));
  }

  public Texture3D Convert(Texture2D temp2DTex, Texture3D cv3D)
  {
    int num1 = 4096;
    if ((bool) ((Object) temp2DTex))
    {
      int num2 = temp2DTex.width * temp2DTex.height;
      num1 = temp2DTex.height;
      if (!this.ValidDimensions(temp2DTex))
      {
        Debug.LogWarning((object) ("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT."));
        return cv3D;
      }
    }
    Color[] pixels = temp2DTex.GetPixels();
    Color[] colors = new Color[pixels.Length];
    for (int index1 = 0; index1 < num1; ++index1)
    {
      for (int index2 = 0; index2 < num1; ++index2)
      {
        for (int index3 = 0; index3 < num1; ++index3)
        {
          int num2 = num1 - index2 - 1;
          colors[index1 + index2 * num1 + index3 * num1 * num1] = pixels[index3 * num1 + index1 + num2 * num1 * num1];
        }
      }
    }
    if ((bool) ((Object) cv3D))
      Object.DestroyImmediate((Object) cv3D);
    cv3D = new Texture3D(num1, num1, num1, TextureFormat.ARGB32, false);
    cv3D.SetPixels(colors);
    cv3D.Apply();
    return cv3D;
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if ((Object) this.SCShader != (Object) null || !SystemInfo.supports3DTextures)
    {
      this.TimeX += Time.deltaTime;
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      if ((Object) this.converted3DLut == (Object) null)
      {
        if (!(bool) ((Object) this.LutTexture))
          this.SetIdentityLut();
        if ((bool) ((Object) this.LutTexture))
          this.converted3DLut = this.Convert(this.LutTexture, this.converted3DLut);
      }
      if ((Object) this.converted3DLut2 == (Object) null)
      {
        if (!(bool) ((Object) this.LutTexture2))
          this.SetIdentityLut();
        if ((bool) ((Object) this.LutTexture2))
          this.converted3DLut2 = this.Convert(this.LutTexture2, this.converted3DLut2);
      }
      if ((bool) ((Object) this.LutTexture))
        this.converted3DLut.wrapMode = TextureWrapMode.Clamp;
      if ((bool) ((Object) this.LutTexture2))
        this.converted3DLut2.wrapMode = TextureWrapMode.Clamp;
      this.material.SetFloat("_Blend", this.Blend);
      this.material.SetFloat("_Fade", this.Fade);
      this.material.SetTexture("_LutTex", (Texture) this.converted3DLut);
      this.material.SetTexture("_LutTex2", (Texture) this.converted3DLut2);
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material, QualitySettings.activeColorSpace != ColorSpace.Linear ? 0 : 1);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void OnValidate()
  {
  }

  private void Update()
  {
  }

  private void OnDisable()
  {
    if (!(bool) ((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
