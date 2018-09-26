﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_3D_Fog_Smoke
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/3D/Fog_Smoke")]
[ExecuteInEditMode]
public class CameraFilterPack_3D_Fog_Smoke : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.0f, 100f)]
  public float _FixDistance = 1f;
  [Range(-0.99f, 0.99f)]
  public float _Distance = 0.5f;
  [Range(0.0f, 0.5f)]
  public float _Size = 0.1f;
  [Range(0.0f, 10f)]
  public float DistortionLevel = 1.2f;
  [Range(0.1f, 10f)]
  public float DistortionSize = 1.4f;
  [Range(-2f, 4f)]
  public float LightIntensity = 0.08f;
  [Range(-5f, 5f)]
  public float AutoAnimatedNearSpeed = 0.5f;
  public Shader SCShader;
  public bool _Visualize;
  private Material SCMaterial;
  public bool AutoAnimatedNear;
  private Texture2D Texture2;
  public static Color ChangeColorRGB;

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
    this.Texture2 = Resources.Load("CameraFilterPack_3D_Myst1") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/3D_Myst");
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
      if (this.AutoAnimatedNear)
      {
        this._Distance += Time.deltaTime * this.AutoAnimatedNearSpeed;
        if ((double) this._Distance > 1.0)
          this._Distance = -1f;
        if ((double) this._Distance < -1.0)
          this._Distance = 1f;
        this.material.SetFloat("_Near", this._Distance);
      }
      else
        this.material.SetFloat("_Near", this._Distance);
      this.material.SetFloat("_Far", this._Size);
      this.material.SetFloat("_Visualize", !this._Visualize ? 0.0f : 1f);
      this.material.SetFloat("_FixDistance", this._FixDistance);
      this.material.SetFloat("_DistortionLevel", this.DistortionLevel * 28f);
      this.material.SetFloat("_DistortionSize", this.DistortionSize * 16f);
      this.material.SetFloat("_LightIntensity", this.LightIntensity * 64f);
      this.material.SetTexture("_MainTex2", (Texture) this.Texture2);
      this.material.SetFloat("_FarCamera", 1000f / this.GetComponent<Camera>().farClipPlane);
      this.material.SetVector("_ScreenResolution", new Vector4((float) sourceTexture.width, (float) sourceTexture.height, 0.0f, 0.0f));
      this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
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
