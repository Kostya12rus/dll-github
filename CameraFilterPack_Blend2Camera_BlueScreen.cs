﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Blend2Camera_BlueScreen
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Chroma Key/BlueScreen")]
[ExecuteInEditMode]
public class CameraFilterPack_Blend2Camera_BlueScreen : MonoBehaviour
{
  private string ShaderName = "CameraFilterPack/Blend2Camera_BlueScreen";
  private float TimeX = 1f;
  [Range(0.0f, 1f)]
  public float BlendFX = 1f;
  public Shader SCShader;
  public Camera Camera2;
  private Material SCMaterial;
  [Range(-0.2f, 0.2f)]
  public float Adjust;
  [Range(-0.2f, 0.2f)]
  public float Precision;
  [Range(-0.2f, 0.2f)]
  public float Luminosity;
  [Range(-0.3f, 0.3f)]
  public float Change_Red;
  [Range(-0.3f, 0.3f)]
  public float Change_Green;
  [Range(-0.3f, 0.3f)]
  public float Change_Blue;
  private RenderTexture Camera2tex;
  private Vector2 ScreenSize;

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

  private void OnValidate()
  {
    this.ScreenSize.x = (float) Screen.width;
    this.ScreenSize.y = (float) Screen.height;
  }

  private void Start()
  {
    if ((Object) this.Camera2 != (Object) null)
    {
      this.Camera2tex = new RenderTexture((int) this.ScreenSize.x, (int) this.ScreenSize.y, 24);
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
      this.material.SetFloat("_Value2", this.Adjust);
      this.material.SetFloat("_Value3", this.Precision);
      this.material.SetFloat("_Value4", this.Luminosity);
      this.material.SetFloat("_Value5", this.Change_Red);
      this.material.SetFloat("_Value6", this.Change_Green);
      this.material.SetFloat("_Value7", this.Change_Blue);
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void Update()
  {
    this.ScreenSize.x = (float) Screen.width;
    this.ScreenSize.y = (float) Screen.height;
    if (Application.isPlaying)
      ;
  }

  private void OnEnable()
  {
    this.Start();
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
