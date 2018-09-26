// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Blend2Camera_Luminosity
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Blend 2 Camera/Luminosity")]
[ExecuteInEditMode]
public class CameraFilterPack_Blend2Camera_Luminosity : MonoBehaviour
{
  private string ShaderName;
  public Shader SCShader;
  public Camera Camera2;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float SwitchCameraToCamera2;
  [Range(0.0f, 1f)]
  public float BlendFX;
  private RenderTexture Camera2tex;

  public CameraFilterPack_Blend2Camera_Luminosity()
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
    if (Object.op_Inequality((Object) this.Camera2, (Object) null))
    {
      this.Camera2tex = new RenderTexture(Screen.get_width(), Screen.get_height(), 24);
      this.Camera2.set_targetTexture(this.Camera2tex);
    }
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
      if (Object.op_Inequality((Object) this.Camera2, (Object) null))
        this.material.SetTexture("_MainTex2", (Texture) this.Camera2tex);
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.BlendFX);
      this.material.SetFloat("_Value2", this.SwitchCameraToCamera2);
      this.material.SetVector("_ScreenResolution", new Vector4((float) ((Texture) sourceTexture).get_width(), (float) ((Texture) sourceTexture).get_height(), 0.0f, 0.0f));
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void OnValidate()
  {
    if (!Object.op_Inequality((Object) this.Camera2, (Object) null))
      return;
    this.Camera2tex = new RenderTexture(Screen.get_width(), Screen.get_height(), 24);
    this.Camera2.set_targetTexture(this.Camera2tex);
  }

  private void Update()
  {
  }

  private void OnEnable()
  {
    if (!Object.op_Inequality((Object) this.Camera2, (Object) null))
      return;
    this.Camera2tex = new RenderTexture(Screen.get_width(), Screen.get_height(), 24);
    this.Camera2.set_targetTexture(this.Camera2tex);
  }

  private void OnDisable()
  {
    if (Object.op_Inequality((Object) this.Camera2, (Object) null))
      this.Camera2.set_targetTexture((RenderTexture) null);
    if (!Object.op_Implicit((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
