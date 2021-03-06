﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Blur_Blurry
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Blur/Blurry")]
public class CameraFilterPack_Blur_Blurry : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 20f)]
  public float Amount;
  [Range(1f, 8f)]
  public int FastFilter;

  public CameraFilterPack_Blur_Blurry()
  {
    base.\u002Ector();
  }

  private Material material
  {
    get
    {
      if (Object.op_Equality((Object) this.SCMaterial, (Object) null))
      {
        this.SCMaterial = new Material(this.SCShader);
        ((Object) this.SCMaterial).set_hideFlags((HideFlags) 61);
      }
      return this.SCMaterial;
    }
  }

  private void Start()
  {
    this.SCShader = Shader.Find("CameraFilterPack/Blur_Blurry");
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      int fastFilter = this.FastFilter;
      this.TimeX += Time.get_deltaTime();
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Amount", this.Amount);
      this.material.SetVector("_ScreenResolution", Vector4.op_Implicit(new Vector2((float) (Screen.get_width() / fastFilter), (float) (Screen.get_height() / fastFilter))));
      int num1 = ((Texture) sourceTexture).get_width() / fastFilter;
      int num2 = ((Texture) sourceTexture).get_height() / fastFilter;
      if (this.FastFilter > 1)
      {
        RenderTexture temporary = RenderTexture.GetTemporary(num1, num2, 0);
        ((Texture) temporary).set_filterMode((FilterMode) 2);
        Graphics.Blit((Texture) sourceTexture, temporary, this.material);
        Graphics.Blit((Texture) temporary, destTexture);
        RenderTexture.ReleaseTemporary(temporary);
      }
      else
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
    if (!Object.op_Implicit((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
