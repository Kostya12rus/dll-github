﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_EyesVision_1
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Vision/Eyes 1")]
public class CameraFilterPack_EyesVision_1 : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(1f, 32f)]
  public float _EyeWave = 15f;
  [Range(0.0f, 10f)]
  public float _EyeSpeed = 1f;
  [Range(0.0f, 8f)]
  public float _EyeMove = 2f;
  [Range(0.0f, 1f)]
  public float _EyeBlink = 1f;
  public Shader SCShader;
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
    this.Texture2 = Resources.Load("CameraFilterPack_eyes_vision_1") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/EyesVision_1");
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
      this.material.SetFloat("_Value", this._EyeWave);
      this.material.SetFloat("_Value2", this._EyeSpeed);
      this.material.SetFloat("_Value3", this._EyeMove);
      this.material.SetFloat("_Value4", this._EyeBlink);
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
