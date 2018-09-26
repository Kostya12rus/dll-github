// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Oculus_NightVision2
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Night Vision/Night Vision 2")]
public class CameraFilterPack_Oculus_NightVision2 : MonoBehaviour
{
  private string ShaderName;
  public Shader SCShader;
  [Range(0.0f, 1f)]
  public float FadeFX;
  private float TimeX;
  private Material SCMaterial;
  private float[] Matrix9;

  public CameraFilterPack_Oculus_NightVision2()
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

  private void ChangeFilters()
  {
    this.Matrix9 = new float[12]
    {
      200f,
      -200f,
      -200f,
      195f,
      4f,
      -160f,
      200f,
      -200f,
      -200f,
      -200f,
      10f,
      -200f
    };
  }

  private void Start()
  {
    this.ChangeFilters();
    this.SCShader = Shader.Find(this.ShaderName);
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
      this.material.SetFloat("_Red_R", this.Matrix9[0] / 100f);
      this.material.SetFloat("_Red_G", this.Matrix9[1] / 100f);
      this.material.SetFloat("_Red_B", this.Matrix9[2] / 100f);
      this.material.SetFloat("_Green_R", this.Matrix9[3] / 100f);
      this.material.SetFloat("_Green_G", this.Matrix9[4] / 100f);
      this.material.SetFloat("_Green_B", this.Matrix9[5] / 100f);
      this.material.SetFloat("_Blue_R", this.Matrix9[6] / 100f);
      this.material.SetFloat("_Blue_G", this.Matrix9[7] / 100f);
      this.material.SetFloat("_Blue_B", this.Matrix9[8] / 100f);
      this.material.SetFloat("_Red_C", this.Matrix9[9] / 100f);
      this.material.SetFloat("_Green_C", this.Matrix9[10] / 100f);
      this.material.SetFloat("_Blue_C", this.Matrix9[11] / 100f);
      this.material.SetFloat("_FadeFX", this.FadeFX);
      this.material.SetVector("_ScreenResolution", new Vector4((float) ((Texture) sourceTexture).get_width(), (float) ((Texture) sourceTexture).get_height(), 0.0f, 0.0f));
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void OnValidate()
  {
    this.ChangeFilters();
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
