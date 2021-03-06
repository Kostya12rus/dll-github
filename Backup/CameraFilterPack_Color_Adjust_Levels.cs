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
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float levelMinimum;
  [Range(0.0f, 1f)]
  public float levelMiddle;
  [Range(0.0f, 1f)]
  public float levelMaximum;
  [Range(0.0f, 1f)]
  public float minOutput;
  [Range(0.0f, 1f)]
  public float maxOutput;

  public CameraFilterPack_Color_Adjust_Levels()
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
    this.SCShader = Shader.Find("CameraFilterPack/Color_Levels");
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      this.TimeX += Time.get_deltaTime();
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      this.material.SetFloat("levelMinimum", this.levelMinimum);
      this.material.SetFloat("levelMiddle", this.levelMiddle);
      this.material.SetFloat("levelMaximum", this.levelMaximum);
      this.material.SetFloat("minOutput", this.minOutput);
      this.material.SetFloat("maxOutput", this.maxOutput);
      this.material.SetVector("_ScreenResolution", new Vector4((float) ((Texture) sourceTexture).get_width(), (float) ((Texture) sourceTexture).get_height(), 0.0f, 0.0f));
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
