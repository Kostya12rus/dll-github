﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Blend2Camera_VividLight
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Blend 2 Camera/VividLight")]
[ExecuteInEditMode]
public class CameraFilterPack_Blend2Camera_VividLight : MonoBehaviour
{
  private string ShaderName = "CameraFilterPack/Blend2Camera_VividLight";
  private float TimeX = 1f;
  [Range(0.0f, 1f)]
  public float BlendFX = 0.5f;
  public Shader SCShader;
  public Camera Camera2;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float SwitchCameraToCamera2;
  private RenderTexture Camera2tex;

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
    if ((Object) this.Camera2 != (Object) null)
    {
      this.Camera2tex = new RenderTexture(Screen.width, Screen.height, 24);
      this.Camera2.targetTexture = this.Camera2tex;
    }
    this.SCShader = Shader.Find(this.ShaderName);
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
      if ((Object) this.Camera2 != (Object) null)
        this.material.SetTexture("_MainTex2", (Texture) this.Camera2tex);
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.BlendFX);
      this.material.SetFloat("_Value2", this.SwitchCameraToCamera2);
      this.material.SetVector("_ScreenResolution", new Vector4((float) sourceTexture.width, (float) sourceTexture.height, 0.0f, 0.0f));
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void OnValidate()
  {
    if (!((Object) this.Camera2 != (Object) null))
      return;
    this.Camera2tex = new RenderTexture(Screen.width, Screen.height, 24);
    this.Camera2.targetTexture = this.Camera2tex;
  }

  private void Update()
  {
  }

  private void OnEnable()
  {
    if (!((Object) this.Camera2 != (Object) null))
      return;
    this.Camera2tex = new RenderTexture(Screen.width, Screen.height, 24);
    this.Camera2.targetTexture = this.Camera2tex;
  }

  private void OnDisable()
  {
    if ((Object) this.Camera2 != (Object) null)
      this.Camera2.targetTexture = (RenderTexture) null;
    if (!(bool) ((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
