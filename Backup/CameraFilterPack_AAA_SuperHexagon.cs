// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_AAA_SuperHexagon
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/AAA/Super Hexagon")]
[ExecuteInEditMode]
public class CameraFilterPack_AAA_SuperHexagon : MonoBehaviour
{
  public Shader SCShader;
  [Range(0.0f, 1f)]
  public float _AlphaHexa;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.2f, 10f)]
  public float HexaSize;
  public float _BorderSize;
  public Color _BorderColor;
  public Color _HexaColor;
  public float _SpotSize;
  public Vector2 center;
  public float Radius;

  public CameraFilterPack_AAA_SuperHexagon()
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
    this.SCShader = Shader.Find("CameraFilterPack/AAA_Super_Hexagon");
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
      this.material.SetFloat("_Value", this.HexaSize);
      this.material.SetFloat("_PositionX", (float) this.center.x);
      this.material.SetFloat("_PositionY", (float) this.center.y);
      this.material.SetFloat("_Radius", this.Radius);
      this.material.SetFloat("_BorderSize", this._BorderSize);
      this.material.SetColor("_BorderColor", this._BorderColor);
      this.material.SetColor("_HexaColor", this._HexaColor);
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
