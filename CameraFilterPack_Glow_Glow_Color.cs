// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Glow_Glow_Color
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Glow/Glow_Color")]
[ExecuteInEditMode]
public class CameraFilterPack_Glow_Glow_Color : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 20f)]
  public float Amount;
  [Range(2f, 16f)]
  public int FastFilter;
  [Range(0.0f, 1f)]
  public float Threshold;
  [Range(0.0f, 3f)]
  public float Intensity;
  [Range(-1f, 1f)]
  public float Precision;
  public Color GlowColor;

  public CameraFilterPack_Glow_Glow_Color()
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
    this.SCShader = Shader.Find("CameraFilterPack/Glow_Glow_Color");
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      int fastFilter = this.FastFilter;
      this.TimeX += Time.get_deltaTime();
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Amount", this.Amount);
      this.material.SetFloat("_Value1", this.Threshold);
      this.material.SetFloat("_Value2", this.Intensity);
      this.material.SetFloat("_Value3", this.Precision);
      this.material.SetColor("_GlowColor", this.GlowColor);
      this.material.SetVector("_ScreenResolution", Vector4.op_Implicit(new Vector2((float) (Screen.get_width() / fastFilter), (float) (Screen.get_height() / fastFilter))));
      int num1 = ((Texture) sourceTexture).get_width() / fastFilter;
      int num2 = ((Texture) sourceTexture).get_height() / fastFilter;
      if (this.FastFilter > 1)
      {
        RenderTexture temporary1 = RenderTexture.GetTemporary(num1, num2, 0);
        RenderTexture temporary2 = RenderTexture.GetTemporary(num1, num2, 0);
        ((Texture) temporary1).set_filterMode((FilterMode) 2);
        Graphics.Blit((Texture) sourceTexture, temporary1, this.material, 3);
        Graphics.Blit((Texture) temporary1, temporary2, this.material, 2);
        Graphics.Blit((Texture) temporary2, temporary1, this.material, 0);
        this.material.SetFloat("_Amount", this.Amount * 2f);
        Graphics.Blit((Texture) temporary1, temporary2, this.material, 2);
        Graphics.Blit((Texture) temporary2, temporary1, this.material, 0);
        this.material.SetTexture("_MainTex2", (Texture) temporary1);
        RenderTexture.ReleaseTemporary(temporary1);
        RenderTexture.ReleaseTemporary(temporary2);
        Graphics.Blit((Texture) sourceTexture, destTexture, this.material, 1);
      }
      else
        Graphics.Blit((Texture) sourceTexture, destTexture, this.material, 0);
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
