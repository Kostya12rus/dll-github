// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Vision_SniperScore
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Vision/SniperScore")]
public class CameraFilterPack_Vision_SniperScore : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float Fade;
  [Range(0.0f, 1f)]
  public float Size;
  [Range(0.01f, 0.4f)]
  public float Smooth;
  [Range(0.0f, 1f)]
  public float _Cible;
  [Range(0.0f, 1f)]
  public float _Distortion;
  [Range(0.0f, 1f)]
  public float _ExtraColor;
  [Range(0.0f, 1f)]
  public float _ExtraLight;
  public Color _Tint;
  [Range(0.0f, 10f)]
  private float StretchX;
  [Range(0.0f, 10f)]
  private float StretchY;
  [Range(-1f, 1f)]
  public float _PosX;
  [Range(-1f, 1f)]
  public float _PosY;

  public CameraFilterPack_Vision_SniperScore()
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
    this.SCShader = Shader.Find("CameraFilterPack/Vision_SniperScore");
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
      this.material.SetFloat("_Fade", this.Fade);
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.Size);
      this.material.SetFloat("_Value2", this.Smooth);
      this.material.SetFloat("_Value3", this.StretchX);
      this.material.SetFloat("_Value4", this.StretchY);
      this.material.SetFloat("_Cible", this._Cible);
      this.material.SetFloat("_ExtraColor", this._ExtraColor);
      this.material.SetFloat("_Distortion", this._Distortion);
      this.material.SetFloat("_PosX", this._PosX);
      this.material.SetFloat("_PosY", this._PosY);
      this.material.SetColor("_Tint", this._Tint);
      this.material.SetFloat("_ExtraLight", this._ExtraLight);
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector((float) Screen.get_width(), (float) Screen.get_height());
      this.material.SetVector("_ScreenResolution", new Vector4((float) vector2.x, (float) vector2.y, (float) (vector2.y / vector2.x), 0.0f));
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
