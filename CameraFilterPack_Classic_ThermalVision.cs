// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Classic_ThermalVision
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Classic/ThermalVision")]
[ExecuteInEditMode]
public class CameraFilterPack_Classic_ThermalVision : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float __Speed;
  [Range(0.0f, 1f)]
  public float _Fade;
  [Range(0.0f, 1f)]
  public float _Crt;
  [Range(0.0f, 1f)]
  public float _Curve;
  [Range(0.0f, 1f)]
  public float _Color1;
  [Range(0.0f, 1f)]
  public float _Color2;
  [Range(0.0f, 1f)]
  public float _Color3;

  public CameraFilterPack_Classic_ThermalVision()
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
    this.SCShader = Shader.Find("CameraFilterPack/CameraFilterPack_Classic_ThermalVision");
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
      this.material.SetFloat("_Speed", this.__Speed);
      this.material.SetFloat("Fade", this._Fade);
      this.material.SetFloat("Crt", this._Crt);
      this.material.SetFloat("Curve", this._Curve);
      this.material.SetFloat("Color1", this._Color1);
      this.material.SetFloat("Color2", this._Color2);
      this.material.SetFloat("Color3", this._Color3);
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
