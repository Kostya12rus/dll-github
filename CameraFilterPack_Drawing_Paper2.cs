// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Drawing_Paper2
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Drawing/Paper2")]
public class CameraFilterPack_Drawing_Paper2 : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  public Color Pencil_Color;
  [Range(0.0001f, 0.0022f)]
  public float Pencil_Size;
  [Range(0.0f, 2f)]
  public float Pencil_Correction;
  [Range(0.0f, 1f)]
  public float Intensity;
  [Range(0.0f, 2f)]
  public float Speed_Animation;
  [Range(0.0f, 1f)]
  public float Corner_Lose;
  [Range(0.0f, 1f)]
  public float Fade_Paper_to_BackColor;
  [Range(0.0f, 1f)]
  public float Fade_With_Original;
  public Color Back_Color;
  private Material SCMaterial;
  private Texture2D Texture2;

  public CameraFilterPack_Drawing_Paper2()
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
    this.Texture2 = Resources.Load("CameraFilterPack_Paper3") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/Drawing_Paper2");
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
      this.material.SetColor("_PColor", this.Pencil_Color);
      this.material.SetFloat("_Value1", this.Pencil_Size);
      this.material.SetFloat("_Value2", this.Pencil_Correction);
      this.material.SetFloat("_Value3", this.Intensity);
      this.material.SetFloat("_Value4", this.Speed_Animation);
      this.material.SetFloat("_Value5", this.Corner_Lose);
      this.material.SetFloat("_Value6", this.Fade_Paper_to_BackColor);
      this.material.SetFloat("_Value7", this.Fade_With_Original);
      this.material.SetColor("_PColor2", this.Back_Color);
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
