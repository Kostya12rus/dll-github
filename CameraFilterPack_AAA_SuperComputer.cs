// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_AAA_SuperComputer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/AAA/Super Computer")]
[ExecuteInEditMode]
public class CameraFilterPack_AAA_SuperComputer : MonoBehaviour
{
  public Shader SCShader;
  [Range(0.0f, 1f)]
  public float _AlphaHexa;
  private float TimeX;
  private Material SCMaterial;
  [Range(-20f, 20f)]
  public float ShapeFormula;
  [Range(0.0f, 6f)]
  public float Shape;
  [Range(-4f, 4f)]
  public float _BorderSize;
  public Color _BorderColor;
  public float _SpotSize;
  public Vector2 center;
  public float Radius;

  public CameraFilterPack_AAA_SuperComputer()
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
    this.SCShader = Shader.Find("CameraFilterPack/AAA_Super_Computer");
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      this.TimeX += Time.get_deltaTime() / 4f;
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.ShapeFormula);
      this.material.SetFloat("_Value2", this.Shape);
      this.material.SetFloat("_PositionX", (float) this.center.x);
      this.material.SetFloat("_PositionY", (float) this.center.y);
      this.material.SetFloat("_Radius", this.Radius);
      this.material.SetFloat("_BorderSize", this._BorderSize);
      this.material.SetColor("_BorderColor", this._BorderColor);
      this.material.SetFloat("_AlphaHexa", this._AlphaHexa);
      this.material.SetFloat("_SpotSize", this._SpotSize);
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
