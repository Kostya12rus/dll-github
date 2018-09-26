// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_3D_Anomaly
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/3D/Anomaly")]
public class CameraFilterPack_3D_Anomaly : MonoBehaviour
{
  public Shader SCShader;
  public bool _Visualize;
  private float TimeX;
  private Material SCMaterial;
  [Range(0.0f, 100f)]
  public float _FixDistance;
  [Range(-0.5f, 0.99f)]
  public float Anomaly_Near;
  [Range(0.0f, 1f)]
  public float Anomaly_Far;
  [Range(0.0f, 2f)]
  public float Intensity;
  [Range(0.0f, 1f)]
  public float AnomalyWithoutObject;
  [Range(0.1f, 1f)]
  public float Anomaly_Distortion;
  [Range(4f, 64f)]
  public float Anomaly_Distortion_Size;
  [Range(-4f, 8f)]
  public float Anomaly_Intensity;

  public CameraFilterPack_3D_Anomaly()
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
    this.SCShader = Shader.Find("CameraFilterPack/3D_Anomaly");
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
      this.material.SetFloat("_Value2", this.Intensity);
      this.material.SetFloat("Anomaly_Distortion", this.Anomaly_Distortion);
      this.material.SetFloat("Anomaly_Distortion_Size", this.Anomaly_Distortion_Size);
      this.material.SetFloat("Anomaly_Intensity", this.Anomaly_Intensity);
      this.material.SetFloat("_Visualize", !this._Visualize ? 0.0f : 1f);
      this.material.SetFloat("_FixDistance", this._FixDistance);
      this.material.SetFloat("Anomaly_Near", this.Anomaly_Near);
      this.material.SetFloat("Anomaly_Far", this.Anomaly_Far);
      this.material.SetFloat("Anomaly_With_Obj", this.AnomalyWithoutObject);
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
