﻿// Decompiled with JetBrains decompiler
// Type: NGSS_NoiseTexture
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class NGSS_NoiseTexture : MonoBehaviour
{
  public Texture noiseTex;
  [Range(0.0f, 1f)]
  public float noiseScale;
  private bool isTextureSet;

  public NGSS_NoiseTexture()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    Shader.SetGlobalFloat("NGSS_NOISE_TEXTURE_SCALE", this.noiseScale);
    if (this.isTextureSet || Object.op_Equality((Object) this.noiseTex, (Object) null))
      return;
    Shader.SetGlobalTexture("NGSS_NOISE_TEXTURE", this.noiseTex);
    this.isTextureSet = true;
  }
}
