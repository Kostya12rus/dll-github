// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_FX_Drunk
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/FX/Drunk")]
public class CameraFilterPack_FX_Drunk : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 20f)]
  [HideInInspector]
  public float Value;
  [Range(0.0f, 10f)]
  public float Speed;
  [Range(0.0f, 1f)]
  public float Wavy;
  [Range(0.0f, 1f)]
  public float Distortion;
  [Range(0.0f, 1f)]
  public float DistortionWave;
  [Range(0.0f, 1f)]
  public float Fade;
  [Range(-2f, 2f)]
  public float ColoredSaturate;
  [Range(-1f, 2f)]
  public float ColoredChange;
  [Range(-1f, 1f)]
  public float ChangeRed;
  [Range(-1f, 1f)]
  public float ChangeGreen;
  [Range(-1f, 1f)]
  public float ChangeBlue;

  public CameraFilterPack_FX_Drunk()
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
    this.SCShader = Shader.Find("CameraFilterPack/FX_Drunk");
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
      this.material.SetFloat("_Value", this.Value);
      this.material.SetFloat("_Speed", this.Speed);
      this.material.SetFloat("_Distortion", this.Distortion);
      this.material.SetFloat("_DistortionWave", this.DistortionWave);
      this.material.SetFloat("_Wavy", this.Wavy);
      this.material.SetFloat("_Fade", this.Fade);
      this.material.SetFloat("_ColoredChange", this.ColoredChange);
      this.material.SetFloat("_ChangeRed", this.ChangeRed);
      this.material.SetFloat("_ChangeGreen", this.ChangeGreen);
      this.material.SetFloat("_ChangeBlue", this.ChangeBlue);
      this.material.SetFloat("_Colored", this.ColoredSaturate);
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
