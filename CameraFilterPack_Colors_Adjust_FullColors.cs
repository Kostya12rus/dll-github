// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Colors_Adjust_FullColors
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/ColorsAdjust/FullColors")]
[ExecuteInEditMode]
public class CameraFilterPack_Colors_Adjust_FullColors : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(-200f, 200f)]
  public float Red_R;
  [Range(-200f, 200f)]
  public float Red_G;
  [Range(-200f, 200f)]
  public float Red_B;
  [Range(-200f, 200f)]
  public float Red_Constant;
  [Range(-200f, 200f)]
  public float Green_R;
  [Range(-200f, 200f)]
  public float Green_G;
  [Range(-200f, 200f)]
  public float Green_B;
  [Range(-200f, 200f)]
  public float Green_Constant;
  [Range(-200f, 200f)]
  public float Blue_R;
  [Range(-200f, 200f)]
  public float Blue_G;
  [Range(-200f, 200f)]
  public float Blue_B;
  [Range(-200f, 200f)]
  public float Blue_Constant;

  public CameraFilterPack_Colors_Adjust_FullColors()
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
    this.SCShader = Shader.Find("CameraFilterPack/Colors_Adjust_FullColors");
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
      this.material.SetFloat("_Red_R", this.Red_R / 100f);
      this.material.SetFloat("_Red_G", this.Red_G / 100f);
      this.material.SetFloat("_Red_B", this.Red_B / 100f);
      this.material.SetFloat("_Green_R", this.Green_R / 100f);
      this.material.SetFloat("_Green_G", this.Green_G / 100f);
      this.material.SetFloat("_Green_B", this.Green_B / 100f);
      this.material.SetFloat("_Blue_R", this.Blue_R / 100f);
      this.material.SetFloat("_Blue_G", this.Blue_G / 100f);
      this.material.SetFloat("_Blue_B", this.Blue_B / 100f);
      this.material.SetFloat("_Red_C", this.Red_Constant / 100f);
      this.material.SetFloat("_Green_C", this.Green_Constant / 100f);
      this.material.SetFloat("_Blue_C", this.Blue_Constant / 100f);
      this.material.SetVector("_ScreenResolution", new Vector4((float) ((Texture) sourceTexture).get_width(), (float) ((Texture) sourceTexture).get_height(), 0.0f, 0.0f));
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void Update()
  {
    if (Application.get_isPlaying())
      ;
  }

  private void OnDisable()
  {
    if (!Object.op_Implicit((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
