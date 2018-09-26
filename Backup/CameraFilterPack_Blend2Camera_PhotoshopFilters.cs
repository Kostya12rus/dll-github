// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Blend2Camera_PhotoshopFilters
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Blend 2 Camera/PhotoshopFilters")]
[ExecuteInEditMode]
public class CameraFilterPack_Blend2Camera_PhotoshopFilters : MonoBehaviour
{
  private string ShaderName;
  public Shader SCShader;
  public Camera Camera2;
  public CameraFilterPack_Blend2Camera_PhotoshopFilters.filters filterchoice;
  private CameraFilterPack_Blend2Camera_PhotoshopFilters.filters filterchoicememo;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float SwitchCameraToCamera2;
  [Range(0.0f, 1f)]
  public float BlendFX;
  private RenderTexture Camera2tex;

  public CameraFilterPack_Blend2Camera_PhotoshopFilters()
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
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Darken)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Darken";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Multiply)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Multiply";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.ColorBurn)
      this.ShaderName = "CameraFilterPack/Blend2Camera_ColorBurn";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.LinearBurn)
      this.ShaderName = "CameraFilterPack/Blend2Camera_LinearBurn";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.DarkerColor)
      this.ShaderName = "CameraFilterPack/Blend2Camera_DarkerColor";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Lighten)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Lighten";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Screen)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Screen";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.ColorDodge)
      this.ShaderName = "CameraFilterPack/Blend2Camera_ColorDodge";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.LinearDodge)
      this.ShaderName = "CameraFilterPack/Blend2Camera_LinearDodge";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.LighterColor)
      this.ShaderName = "CameraFilterPack/Blend2Camera_LighterColor";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Overlay)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Overlay";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.SoftLight)
      this.ShaderName = "CameraFilterPack/Blend2Camera_SoftLight";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.HardLight)
      this.ShaderName = "CameraFilterPack/Blend2Camera_HardLight";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.VividLight)
      this.ShaderName = "CameraFilterPack/Blend2Camera_VividLight";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.LinearLight)
      this.ShaderName = "CameraFilterPack/Blend2Camera_LinearLight";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.PinLight)
      this.ShaderName = "CameraFilterPack/Blend2Camera_PinLight";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.HardMix)
      this.ShaderName = "CameraFilterPack/Blend2Camera_HardMix";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Difference)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Difference";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Exclusion)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Exclusion";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Subtract)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Subtract";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Divide)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Divide";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Hue)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Hue";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Saturation)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Saturation";
    if (this.filterchoice == CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Color)
      this.ShaderName = "CameraFilterPack/Blend2Camera_Color";
    if (this.filterchoice != CameraFilterPack_Blend2Camera_PhotoshopFilters.filters.Luminosity)
      return;
    this.ShaderName = "CameraFilterPack/Blend2Camera_Luminosity";
  }

  private void Start()
  {
    this.filterchoicememo = this.filterchoice;
    if (Object.op_Inequality((Object) this.Camera2, (Object) null))
    {
      this.Camera2tex = new RenderTexture(Screen.get_width(), Screen.get_height(), 24);
      this.Camera2.set_targetTexture(this.Camera2tex);
    }
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
    if (this.filterchoice != this.filterchoicememo)
    {
      this.ChangeFilters();
      this.SCShader = Shader.Find(this.ShaderName);
      Object.DestroyImmediate((Object) this.SCMaterial);
      if (Object.op_Equality((Object) this.SCMaterial, (Object) null))
      {
        this.SCMaterial = new Material(this.SCShader);
        ((Object) this.SCMaterial).set_hideFlags((HideFlags) 61);
      }
    }
    this.filterchoicememo = this.filterchoice;
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

  public enum filters
  {
    Darken,
    Multiply,
    ColorBurn,
    LinearBurn,
    DarkerColor,
    Lighten,
    Screen,
    ColorDodge,
    LinearDodge,
    LighterColor,
    Overlay,
    SoftLight,
    HardLight,
    VividLight,
    LinearLight,
    PinLight,
    HardMix,
    Difference,
    Exclusion,
    Subtract,
    Divide,
    Hue,
    Saturation,
    Color,
    Luminosity,
  }
}
