// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_TV_Vignetting
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/TV/Vignetting")]
[ExecuteInEditMode]
public class CameraFilterPack_TV_Vignetting : MonoBehaviour
{
  public Shader SCShader;
  private Material SCMaterial;
  private Texture2D Vignette;
  [Range(0.0f, 1f)]
  public float Vignetting;
  [Range(0.0f, 1f)]
  public float VignettingFull;
  [Range(0.0f, 1f)]
  public float VignettingDirt;
  public Color VignettingColor;

  public CameraFilterPack_TV_Vignetting()
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
    this.SCShader = Shader.Find("CameraFilterPack/TV_Vignetting");
    this.Vignette = Resources.Load("CameraFilterPack_TV_Vignetting1") as Texture2D;
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      this.material.SetTexture("Vignette", (Texture) this.Vignette);
      this.material.SetFloat("_Vignetting", this.Vignetting);
      this.material.SetFloat("_Vignetting2", this.VignettingFull);
      this.material.SetColor("_VignettingColor", this.VignettingColor);
      this.material.SetFloat("_VignettingDirt", this.VignettingDirt);
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
