// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Blend2Camera_SplitScreen3D
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Split Screen/Split 3D")]
public class CameraFilterPack_Blend2Camera_SplitScreen3D : MonoBehaviour
{
  private string ShaderName = "CameraFilterPack/Blend2Camera_SplitScreen3D";
  private float TimeX = 1f;
  [Range(0.0f, 100f)]
  public float _FixDistance = 1f;
  [Range(-0.99f, 0.99f)]
  public float _Distance = 0.5f;
  [Range(0.0f, 0.5f)]
  public float _Size = 0.1f;
  [Range(0.0f, 1f)]
  public float BlendFX = 1f;
  [Range(-3f, 3f)]
  public float SplitX = 0.5f;
  [Range(-3f, 3f)]
  public float SplitY = 0.5f;
  [Range(0.0f, 2f)]
  public float Smooth = 0.1f;
  [Range(-3.14f, 3.14f)]
  public float Rotation = 3.14f;
  public Shader SCShader;
  public Camera Camera2;
  private Material SCMaterial;
  [Range(0.0f, 1f)]
  public float SwitchCameraToCamera2;
  private bool ForceYSwap;
  private RenderTexture Camera2tex;
  private Vector2 ScreenSize;

  private Material material
  {
    get
    {
      if ((Object) this.SCMaterial == (Object) null)
      {
        this.SCMaterial = new Material(this.SCShader);
        this.SCMaterial.hideFlags = HideFlags.HideAndDontSave;
      }
      return this.SCMaterial;
    }
  }

  private void OnValidate()
  {
    this.ScreenSize.x = (float) Screen.width;
    this.ScreenSize.y = (float) Screen.height;
  }

  private void Start()
  {
    if ((Object) this.Camera2 != (Object) null)
    {
      this.Camera2tex = new RenderTexture((int) this.ScreenSize.x, (int) this.ScreenSize.y, 24);
      this.Camera2.targetTexture = this.Camera2tex;
    }
    this.SCShader = Shader.Find(this.ShaderName);
    if (SystemInfo.supportsImageEffects)
      return;
    this.enabled = false;
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if ((Object) this.SCShader != (Object) null)
    {
      this.TimeX += Time.deltaTime;
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      if ((Object) this.Camera2 != (Object) null)
        this.material.SetTexture("_MainTex2", (Texture) this.Camera2tex);
      this.material.SetFloat("_Near", this._Distance);
      this.material.SetFloat("_Far", this._Size);
      this.material.SetFloat("_FixDistance", this._FixDistance);
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.BlendFX);
      this.material.SetFloat("_Value2", this.SwitchCameraToCamera2);
      this.material.SetFloat("_Value3", this.SplitX);
      this.material.SetFloat("_Value6", this.SplitY);
      this.material.SetFloat("_Value4", this.Smooth);
      this.material.SetFloat("_Value5", this.Rotation);
      this.material.SetInt("_ForceYSwap", !this.ForceYSwap ? 1 : 0);
      this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void Update()
  {
    this.ScreenSize.x = (float) Screen.width;
    this.ScreenSize.y = (float) Screen.height;
  }

  private void OnEnable()
  {
    this.Start();
  }

  private void OnDisable()
  {
    if ((Object) this.Camera2 != (Object) null)
      this.Camera2.targetTexture = (RenderTexture) null;
    if (!(bool) ((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
