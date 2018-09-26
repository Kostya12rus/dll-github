﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Distortion_Half_Sphere
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Distortion/Half_Sphere")]
[ExecuteInEditMode]
public class CameraFilterPack_Distortion_Half_Sphere : MonoBehaviour
{
  private float TimeX = 1f;
  public float SphereSize = 2.5f;
  [Range(1f, 10f)]
  public float Strength = 5f;
  public Shader SCShader;
  [Range(1f, 6f)]
  private Material SCMaterial;
  [Range(-1f, 1f)]
  public float SpherePositionX;
  [Range(-1f, 1f)]
  public float SpherePositionY;

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
    this.SCShader = Shader.Find("CameraFilterPack/Distortion_Half_Sphere");
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
      this.material.SetFloat("_SphereSize", this.SphereSize);
      this.material.SetFloat("_SpherePositionX", this.SpherePositionX);
      this.material.SetFloat("_SpherePositionY", this.SpherePositionY);
      this.material.SetFloat("_Strength", this.Strength);
      this.material.SetVector("_ScreenResolution", (Vector4) new Vector2((float) Screen.width, (float) Screen.height));
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
