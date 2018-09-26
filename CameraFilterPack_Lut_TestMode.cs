﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Lut_TestMode
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Lut/TestMode")]
[ExecuteInEditMode]
public class CameraFilterPack_Lut_TestMode : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.0f, 1f)]
  public float Blend = 1f;
  [Range(0.0f, 3f)]
  public float OriginalIntensity = 1f;
  [Range(0.0f, 1f)]
  public float TestMode = 0.5f;
  public Shader SCShader;
  private Material SCMaterial;
  public Texture2D LutTexture;
  private Texture3D converted3DLut;
  [Range(-1f, 1f)]
  public float ResultIntensity;
  [Range(-1f, 1f)]
  public float FinalIntensity;
  private string MemoPath;

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
    this.SCShader = Shader.Find("CameraFilterPack/Lut_TestMode");
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
  }

  public bool ValidDimensions(Texture2D tex2d)
  {
    return (bool) ((Object) tex2d) && tex2d.height == Mathf.FloorToInt(Mathf.Sqrt((float) tex2d.width));
  }

  public void Convert(Texture2D temp2DTex)
  {
    if ((bool) ((Object) temp2DTex))
    {
      int num1 = temp2DTex.width * temp2DTex.height;
      int height = temp2DTex.height;
      if (!this.ValidDimensions(temp2DTex))
      {
        Debug.LogWarning((object) ("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT."));
      }
      else
      {
        Color[] pixels = temp2DTex.GetPixels();
        Color[] colors = new Color[pixels.Length];
        for (int index1 = 0; index1 < height; ++index1)
        {
          for (int index2 = 0; index2 < height; ++index2)
          {
            for (int index3 = 0; index3 < height; ++index3)
            {
              int num2 = height - index2 - 1;
              colors[index1 + index2 * height + index3 * height * height] = pixels[index3 * height + index1 + num2 * height * height];
            }
          }
        }
        if ((bool) ((Object) this.converted3DLut))
          Object.DestroyImmediate((Object) this.converted3DLut);
        this.converted3DLut = new Texture3D(height, height, height, TextureFormat.ARGB32, false);
        this.converted3DLut.SetPixels(colors);
        this.converted3DLut.Apply();
      }
    }
    else
      this.SetIdentityLut();
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if ((Object) this.SCShader != (Object) null || !SystemInfo.supports3DTextures)
    {
      this.TimeX += Time.deltaTime;
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      if ((Object) this.converted3DLut == (Object) null)
        this.Convert(this.LutTexture);
      this.converted3DLut.wrapMode = TextureWrapMode.Clamp;
      this.material.SetTexture("_LutTex", (Texture) this.converted3DLut);
      this.material.SetFloat("_Blend", this.Blend);
      this.material.SetFloat("_Intensity", this.OriginalIntensity);
      this.material.SetFloat("_Extra", this.ResultIntensity);
      this.material.SetFloat("_Extra2", this.FinalIntensity);
      this.material.SetFloat("_Extra3", this.TestMode);
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
