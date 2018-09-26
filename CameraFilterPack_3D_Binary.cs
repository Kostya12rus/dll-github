// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_3D_Binary
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/3D/Binary")]
[ExecuteInEditMode]
public class CameraFilterPack_3D_Binary : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  public bool _Visualize;
  private Material SCMaterial;
  [Range(0.0f, 100f)]
  public float _FixDistance;
  [Range(-5f, 5f)]
  public float LightIntensity;
  [Range(0.0f, 8f)]
  public float MatrixSize;
  [Range(-4f, 4f)]
  public float MatrixSpeed;
  [Range(0.0f, 1f)]
  public float Fade;
  [Range(0.0f, 1f)]
  public float FadeFromBinary;
  public Color _MatrixColor;
  public static Color ChangeColorRGB;
  private Texture2D Texture2;

  public CameraFilterPack_3D_Binary()
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
    this.Texture2 = Resources.Load("CameraFilterPack_3D_Binary1") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/3D_Binary");
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
      this.material.SetFloat("_DepthLevel", this.Fade);
      this.material.SetFloat("_FadeFromBinary", this.FadeFromBinary);
      this.material.SetFloat("_FixDistance", this._FixDistance);
      this.material.SetFloat("_MatrixSize", this.MatrixSize);
      this.material.SetColor("_MatrixColor", this._MatrixColor);
      this.material.SetFloat("_MatrixSpeed", this.MatrixSpeed * 2f);
      this.material.SetFloat("_Visualize", !this._Visualize ? 0.0f : 1f);
      this.material.SetFloat("_LightIntensity", this.LightIntensity);
      this.material.SetTexture("_MainTex2", (Texture) this.Texture2);
      this.material.SetFloat("_FarCamera", 1000f / ((Camera) ((Component) this).GetComponent<Camera>()).get_farClipPlane());
      this.material.SetVector("_ScreenResolution", new Vector4((float) ((Texture) sourceTexture).get_width(), (float) ((Texture) sourceTexture).get_height(), 0.0f, 0.0f));
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
