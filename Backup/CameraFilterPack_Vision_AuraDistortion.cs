﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Vision_AuraDistortion
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Vision/AuraDistortion")]
[ExecuteInEditMode]
public class CameraFilterPack_Vision_AuraDistortion : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 2f)]
  public float Twist;
  [Range(-4f, 4f)]
  public float Speed;
  public Color Color;
  [Range(-1f, 2f)]
  public float PosX;
  [Range(-1f, 2f)]
  public float PosY;

  public CameraFilterPack_Vision_AuraDistortion()
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
    this.SCShader = Shader.Find("CameraFilterPack/Vision_AuraDistortion");
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
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.Twist);
      this.material.SetColor("_Value2", this.Color);
      this.material.SetFloat("_Value3", this.PosX);
      this.material.SetFloat("_Value4", this.PosY);
      this.material.SetFloat("_Value5", this.Speed);
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
