// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Blur_BlurHole
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Blur/Blur Hole")]
public class CameraFilterPack_Blur_BlurHole : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  [Range(1f, 16f)]
  public float Size;
  [Range(-1f, 1f)]
  public float _Radius;
  [Range(-4f, 4f)]
  public float _SpotSize;
  [Range(0.0f, 1f)]
  public float _CenterX;
  [Range(0.0f, 1f)]
  public float _CenterY;
  [Range(0.0f, 1f)]
  public float _AlphaBlur;
  [Range(0.0f, 1f)]
  public float _AlphaBlurInside;
  private Material SCMaterial;

  public CameraFilterPack_Blur_BlurHole()
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
    this.SCShader = Shader.Find("CameraFilterPack/BlurHole");
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
      this.material.SetFloat("_Distortion", this.Size);
      this.material.SetFloat("_Radius", this._Radius);
      this.material.SetFloat("_SpotSize", this._SpotSize);
      this.material.SetFloat("_CenterX", this._CenterX);
      this.material.SetFloat("_CenterY", this._CenterY);
      this.material.SetFloat("_Alpha", this._AlphaBlur);
      this.material.SetFloat("_Alpha2", this._AlphaBlurInside);
      this.material.SetVector("_ScreenResolution", Vector4.op_Implicit(new Vector2((float) Screen.get_width(), (float) Screen.get_height())));
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
