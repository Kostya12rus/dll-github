﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_TV_BrokenGlass
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/TV/Broken Glass")]
[ExecuteInEditMode]
public class CameraFilterPack_TV_BrokenGlass : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.0f, 128f)]
  public float Broken_Big = 1f;
  [Range(0.0f, 0.004f)]
  public float LightReflect = 1f / 500f;
  public Shader SCShader;
  [Range(0.0f, 128f)]
  public float Broken_Small;
  [Range(0.0f, 128f)]
  public float Broken_Medium;
  [Range(0.0f, 128f)]
  public float Broken_High;
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
    this.Texture2 = Resources.Load("CameraFilterPack_TV_BrokenGlass1") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/TV_BrokenGlass");
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
      this.material.SetFloat("_Value", this.LightReflect);
      this.material.SetFloat("_Value2", this.Broken_Small);
      this.material.SetFloat("_Value3", this.Broken_Medium);
      this.material.SetFloat("_Value4", this.Broken_High);
      this.material.SetFloat("_Value5", this.Broken_Big);
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
