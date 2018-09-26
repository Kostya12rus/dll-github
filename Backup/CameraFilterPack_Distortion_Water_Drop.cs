﻿// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Distortion_Water_Drop
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Distortion/Water_Drop")]
public class CameraFilterPack_Distortion_Water_Drop : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(-1f, 1f)]
  public float CenterX;
  [Range(-1f, 1f)]
  public float CenterY;
  [Range(0.0f, 10f)]
  public float WaveIntensity;
  [Range(0.0f, 20f)]
  public int NumberOfWaves;

  public CameraFilterPack_Distortion_Water_Drop()
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
    this.SCShader = Shader.Find("CameraFilterPack/Distortion_Water_Drop");
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
      this.material.SetVector("_ScreenResolution", Vector4.op_Implicit(new Vector2((float) Screen.get_width(), (float) Screen.get_height())));
      this.material.SetFloat("_CenterX", this.CenterX);
      this.material.SetFloat("_CenterY", this.CenterY);
      this.material.SetFloat("_WaveIntensity", this.WaveIntensity);
      this.material.SetInt("_NumberOfWaves", this.NumberOfWaves);
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
