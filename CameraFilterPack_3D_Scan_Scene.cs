﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_3D_Scan_Scene
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/3D/Scan_Scene")]
[ExecuteInEditMode]
public class CameraFilterPack_3D_Scan_Scene : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.0f, 100f)]
  public float _FixDistance = 1f;
  [Range(0.0f, 0.99f)]
  public float _Distance = 1f;
  [Range(0.0f, 0.1f)]
  public float _Size = 0.01f;
  [Range(-5f, 5f)]
  public float AutoAnimatedNearSpeed = 1f;
  public Color ScanColor = new Color(2f, 0.0f, 0.0f, 1f);
  [Range(0.0f, 1f)]
  public float Fade = 1f;
  public Shader SCShader;
  public bool _Visualize;
  private Material SCMaterial;
  public bool AutoAnimatedNear;
  public static Color ChangeColorRGB;
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
    this.SCShader = Shader.Find("CameraFilterPack/3D_Scan_Scene");
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
      this.material.SetFloat("_DepthLevel", this.Fade);
      if (this.AutoAnimatedNear)
      {
        this._Distance += Time.deltaTime * this.AutoAnimatedNearSpeed;
        if ((double) this._Distance > 1.0)
          this._Distance = 0.0f;
        if ((double) this._Distance < 0.0)
          this._Distance = 1f;
        this.material.SetFloat("_Near", this._Distance);
      }
      else
        this.material.SetFloat("_Near", this._Distance);
      this.material.SetFloat("_Far", this._Size);
      this.material.SetColor("_ColorRGB", this.ScanColor);
      this.material.SetFloat("_FixDistance", this._FixDistance);
      this.material.SetFloat("_Visualize", !this._Visualize ? 0.0f : 1f);
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
