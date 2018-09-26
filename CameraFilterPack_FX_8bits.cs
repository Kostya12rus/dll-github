﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_FX_8bits
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Pixel/8bits")]
public class CameraFilterPack_FX_8bits : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(80f, 640f)]
  public int ResolutionX = 160;
  [Range(60f, 480f)]
  public int ResolutionY = 240;
  public Shader SCShader;
  private Material SCMaterial;
  [Range(-1f, 1f)]
  public float Brightness;

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
    this.SCShader = Shader.Find("CameraFilterPack/FX_8bits");
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
      if ((double) this.Brightness == 0.0)
        this.Brightness = 1f / 1000f;
      this.material.SetFloat("_Distortion", this.Brightness);
      RenderTexture temporary = RenderTexture.GetTemporary(this.ResolutionX, this.ResolutionY, 0);
      Graphics.Blit((Texture) sourceTexture, temporary, this.material);
      temporary.filterMode = FilterMode.Point;
      Graphics.Blit((Texture) temporary, destTexture);
      RenderTexture.ReleaseTemporary(temporary);
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
