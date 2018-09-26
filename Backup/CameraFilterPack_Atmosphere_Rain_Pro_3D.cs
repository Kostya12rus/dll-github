// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Atmosphere_Rain_Pro_3D
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Weather/Rain_Pro_3D")]
public class CameraFilterPack_Atmosphere_Rain_Pro_3D : MonoBehaviour
{
  public Shader SCShader;
  public bool _Visualize;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 100f)]
  public float _FixDistance;
  [Range(0.0f, 1f)]
  public float Fade;
  [Range(0.0f, 2f)]
  public float Intensity;
  public bool DirectionFollowCameraZ;
  [Range(-0.45f, 0.45f)]
  public float DirectionX;
  [Range(0.4f, 2f)]
  public float Size;
  [Range(0.0f, 0.5f)]
  public float Speed;
  [Range(0.0f, 0.5f)]
  public float Distortion;
  [Range(0.0f, 1f)]
  public float StormFlashOnOff;
  [Range(0.0f, 1f)]
  public float DropOnOff;
  [Range(-0.5f, 0.99f)]
  public float Drop_Near;
  [Range(0.0f, 1f)]
  public float Drop_Far;
  [Range(0.0f, 1f)]
  public float Drop_With_Obj;
  [Range(0.0f, 1f)]
  public float Myst;
  [Range(0.0f, 1f)]
  public float Drop_Floor_Fluid;
  public Color Myst_Color;
  private Texture2D Texture2;

  public CameraFilterPack_Atmosphere_Rain_Pro_3D()
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
    this.Texture2 = Resources.Load("CameraFilterPack_Atmosphere_Rain_FX") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/Atmosphere_Rain_Pro_3D");
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
      this.material.SetFloat("_Value", this.Fade);
      this.material.SetFloat("_Value2", this.Intensity);
      if (this.DirectionFollowCameraZ)
      {
        float z = (float) ((Component) ((Component) this).GetComponent<Camera>()).get_transform().get_rotation().z;
        if ((double) z > 0.0 && (double) z < 360.0)
          this.material.SetFloat("_Value3", z);
        if ((double) z < 0.0)
          this.material.SetFloat("_Value3", z);
      }
      else
        this.material.SetFloat("_Value3", this.DirectionX);
      this.material.SetFloat("_Value4", this.Speed);
      this.material.SetFloat("_Value5", this.Size);
      this.material.SetFloat("_Value6", this.Distortion);
      this.material.SetFloat("_Value7", this.StormFlashOnOff);
      this.material.SetFloat("_Value8", this.DropOnOff);
      this.material.SetFloat("_FixDistance", this._FixDistance);
      this.material.SetFloat("_Visualize", !this._Visualize ? 0.0f : 1f);
      this.material.SetFloat("Drop_Near", this.Drop_Near);
      this.material.SetFloat("Drop_Far", this.Drop_Far);
      this.material.SetFloat("Drop_With_Obj", 1f - this.Drop_With_Obj);
      this.material.SetFloat("Myst", this.Myst);
      this.material.SetColor("Myst_Color", this.Myst_Color);
      this.material.SetFloat("Drop_Floor_Fluid", this.Drop_Floor_Fluid);
      this.material.SetVector("_ScreenResolution", new Vector4((float) ((Texture) sourceTexture).get_width(), (float) ((Texture) sourceTexture).get_height(), 0.0f, 0.0f));
      this.material.SetTexture("Texture2", (Texture) this.Texture2);
      ((Camera) ((Component) this).GetComponent<Camera>()).set_depthTextureMode((DepthTextureMode) 1);
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
