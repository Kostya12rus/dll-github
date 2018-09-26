// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Drawing_EnhancedComics
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Drawing/EnhancedComics")]
[ExecuteInEditMode]
public class CameraFilterPack_Drawing_EnhancedComics : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float DotSize;
  [Range(0.0f, 1f)]
  public float _ColorR;
  [Range(0.0f, 1f)]
  public float _ColorG;
  [Range(0.0f, 1f)]
  public float _ColorB;
  [Range(0.0f, 1f)]
  public float _Blood;
  [Range(0.0f, 1f)]
  public float _SmoothStart;
  [Range(0.0f, 1f)]
  public float _SmoothEnd;
  public Color ColorRGB;

  public CameraFilterPack_Drawing_EnhancedComics()
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
    this.SCShader = Shader.Find("CameraFilterPack/Drawing_EnhancedComics");
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
      this.material.SetFloat("_DotSize", this.DotSize);
      this.material.SetFloat("_ColorR", this._ColorR);
      this.material.SetFloat("_ColorG", this._ColorG);
      this.material.SetFloat("_ColorB", this._ColorB);
      this.material.SetFloat("_Blood", this._Blood);
      this.material.SetColor("_ColorRGB", this.ColorRGB);
      this.material.SetFloat("_SmoothStart", this._SmoothStart);
      this.material.SetFloat("_SmoothEnd", this._SmoothEnd);
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
