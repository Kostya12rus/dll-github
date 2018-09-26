﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Color_Adjust_Levels
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Colors/Levels")]
[ExecuteInEditMode]
public class CameraFilterPack_Color_Adjust_Levels : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.0f, 1f)]
  public float levelMiddle = 0.5f;
  [Range(0.0f, 1f)]
  public float levelMaximum = 1f;
  [Range(0.0f, 1f)]
  public float maxOutput = 1f;
  public Shader SCShader;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float levelMinimum;
  [Range(0.0f, 1f)]
  public float minOutput;

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
    this.SCShader = Shader.Find("CameraFilterPack/Color_Levels");
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
      this.material.SetFloat("levelMinimum", this.levelMinimum);
      this.material.SetFloat("levelMiddle", this.levelMiddle);
      this.material.SetFloat("levelMaximum", this.levelMaximum);
      this.material.SetFloat("minOutput", this.minOutput);
      this.material.SetFloat("maxOutput", this.maxOutput);
      this.material.SetVector("_ScreenResolution", new Vector4((float) sourceTexture.width, (float) sourceTexture.height, 0.0f, 0.0f));
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
