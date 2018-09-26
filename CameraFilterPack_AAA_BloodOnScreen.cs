// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_AAA_BloodOnScreen
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/AAA/Blood On Screen")]
[ExecuteInEditMode]
public class CameraFilterPack_AAA_BloodOnScreen : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  [Range(0.02f, 1.6f)]
  public float Blood_On_Screen;
  public Color Blood_Color;
  [Range(0.0f, 2f)]
  public float Blood_Intensify;
  [Range(0.0f, 2f)]
  public float Blood_Darkness;
  [Range(0.0f, 1f)]
  public float Blood_Distortion_Speed;
  [Range(0.0f, 1f)]
  public float Blood_Fade;
  private Material SCMaterial;
  private Texture2D Texture2;

  public CameraFilterPack_AAA_BloodOnScreen()
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
    this.Texture2 = Resources.Load("CameraFilterPack_AAA_BloodOnScreen1") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/AAA_BloodOnScreen");
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
      this.material.SetFloat("_Value", Mathf.Clamp(this.Blood_On_Screen, 0.02f, 1.6f));
      this.material.SetFloat("_Value2", Mathf.Clamp(this.Blood_Intensify, 0.0f, 2f));
      this.material.SetFloat("_Value3", Mathf.Clamp(this.Blood_Darkness, 0.0f, 2f));
      this.material.SetFloat("_Value4", Mathf.Clamp(this.Blood_Fade, 0.0f, 1f));
      this.material.SetFloat("_Value5", Mathf.Clamp(this.Blood_Distortion_Speed, 0.0f, 2f));
      this.material.SetColor("_Color2", this.Blood_Color);
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
