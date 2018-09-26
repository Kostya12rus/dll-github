﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Blur_Tilt_Shift
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Blur/Tilt_Shift")]
public class CameraFilterPack_Blur_Tilt_Shift : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.0f, 20f)]
  public float Amount = 3f;
  [Range(2f, 16f)]
  public int FastFilter = 8;
  [Range(0.0f, 1f)]
  public float Smooth = 0.5f;
  [Range(0.0f, 1f)]
  public float Size = 0.5f;
  [Range(-1f, 1f)]
  public float Position = 0.5f;
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
    this.SCShader = Shader.Find("CameraFilterPack/BlurTiltShift");
    if (SystemInfo.supportsImageEffects)
      return;
    this.enabled = false;
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if ((Object) this.SCShader != (Object) null)
    {
      int fastFilter = this.FastFilter;
      this.TimeX += Time.deltaTime;
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Amount", this.Amount);
      this.material.SetFloat("_Value1", this.Smooth);
      this.material.SetFloat("_Value2", this.Size);
      this.material.SetFloat("_Value3", this.Position);
      this.material.SetVector("_ScreenResolution", (Vector4) new Vector2((float) (Screen.width / fastFilter), (float) (Screen.height / fastFilter)));
      int width = sourceTexture.width / fastFilter;
      int height = sourceTexture.height / fastFilter;
      if (this.FastFilter > 1)
      {
        RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 0);
        RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0);
        temporary1.filterMode = FilterMode.Trilinear;
        Graphics.Blit((Texture) sourceTexture, temporary1, this.material, 2);
        Graphics.Blit((Texture) temporary1, temporary2, this.material, 0);
        this.material.SetFloat("_Amount", this.Amount * 2f);
        Graphics.Blit((Texture) temporary2, temporary1, this.material, 2);
        Graphics.Blit((Texture) temporary1, temporary2, this.material, 0);
        this.material.SetTexture("_MainTex2", (Texture) temporary2);
        RenderTexture.ReleaseTemporary(temporary1);
        RenderTexture.ReleaseTemporary(temporary2);
        Graphics.Blit((Texture) sourceTexture, destTexture, this.material, 1);
      }
      else
        Graphics.Blit((Texture) sourceTexture, destTexture, this.material, 0);
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
