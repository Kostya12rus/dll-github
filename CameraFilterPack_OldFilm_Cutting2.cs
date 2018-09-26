﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_OldFilm_Cutting2
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Old Film/Cutting 2")]
[ExecuteInEditMode]
public class CameraFilterPack_OldFilm_Cutting2 : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.0f, 10f)]
  public float Speed = 5f;
  [Range(0.0f, 2f)]
  public float Luminosity = 1f;
  [Range(0.0f, 1f)]
  public float Vignette = 1f;
  public Shader SCShader;
  [Range(0.0f, 1f)]
  public float Negative;
  private Material SCMaterial;
  private Texture2D Texture2;

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
    this.Texture2 = Resources.Load("CameraFilterPack_OldFilm2") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/OldFilm_Cutting2");
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
      this.material.SetFloat("_Value", 2f - this.Luminosity);
      this.material.SetFloat("_Value2", 1f - this.Vignette);
      this.material.SetFloat("_Value3", this.Negative);
      this.material.SetFloat("_Speed", this.Speed);
      this.material.SetTexture("_MainTex2", (Texture) this.Texture2);
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
