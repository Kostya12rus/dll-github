// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Glasses_On_4
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Glasses/Futuristic Desert")]
public class CameraFilterPack_Glasses_On_4 : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  [Range(0.0f, 1f)]
  public float Fade;
  [Range(0.0f, 0.1f)]
  public float VisionBlur;
  public Color GlassesColor;
  public Color GlassesColor2;
  [Range(0.0f, 1f)]
  public float GlassDistortion;
  [Range(0.0f, 1f)]
  public float GlassAberration;
  [Range(0.0f, 1f)]
  public float UseFinalGlassColor;
  [Range(0.0f, 1f)]
  public float UseScanLine;
  [Range(1f, 512f)]
  public float UseScanLineSize;
  public Color GlassColor;
  private Material SCMaterial;
  private Texture2D Texture2;

  public CameraFilterPack_Glasses_On_4()
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
    this.Texture2 = Resources.Load("CameraFilterPack_Glasses_On5") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/Glasses_On");
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
      this.material.SetFloat("UseFinalGlassColor", this.UseFinalGlassColor);
      this.material.SetFloat("Fade", this.Fade);
      this.material.SetFloat("VisionBlur", this.VisionBlur);
      this.material.SetFloat("GlassDistortion", this.GlassDistortion);
      this.material.SetFloat("GlassAberration", this.GlassAberration);
      this.material.SetColor("GlassesColor", this.GlassesColor);
      this.material.SetColor("GlassesColor2", this.GlassesColor2);
      this.material.SetColor("GlassColor", this.GlassColor);
      this.material.SetFloat("UseScanLineSize", this.UseScanLineSize);
      this.material.SetFloat("UseScanLine", this.UseScanLine);
      this.material.SetTexture("_MainTex2", (Texture) this.Texture2);
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
