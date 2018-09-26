﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_FX_DarkMatter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/FX/DarkMatter")]
public class CameraFilterPack_FX_DarkMatter : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(-10f, 10f)]
  public float Speed = 0.8f;
  [Range(0.0f, 1f)]
  public float Intensity = 1f;
  [Range(-1f, 2f)]
  public float PosX = 0.5f;
  [Range(-1f, 2f)]
  public float PosY = 0.5f;
  [Range(-2f, 2f)]
  public float Zoom = 0.33f;
  [Range(0.0f, 5f)]
  public float DarkIntensity = 2f;
  public Shader SCShader;
  private Material SCMaterial;

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
    this.SCShader = Shader.Find("CameraFilterPack/FX_DarkMatter");
    if (SystemInfo.supportsImageEffects)
      return;
    this.enabled = false;
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if ((Object) this.SCShader != (Object) null)
    {
      this.TimeX += Time.deltaTime;
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.Speed);
      this.material.SetFloat("_Value2", this.Intensity);
      this.material.SetFloat("_Value3", this.PosX);
      this.material.SetFloat("_Value4", this.PosY);
      this.material.SetFloat("_Value5", this.Zoom);
      this.material.SetFloat("_Value6", this.DarkIntensity);
      this.material.SetVector("_ScreenResolution", new Vector4((float) sourceTexture.width, (float) sourceTexture.height, 0.0f, 0.0f));
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
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
