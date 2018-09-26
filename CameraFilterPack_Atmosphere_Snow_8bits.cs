// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Atmosphere_Snow_8bits
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Pixel/Snow_8bits")]
[ExecuteInEditMode]
public class CameraFilterPack_Atmosphere_Snow_8bits : MonoBehaviour
{
  private float TimeX = 1f;
  [Range(0.9f, 2f)]
  public float Threshold = 1f;
  [Range(8f, 256f)]
  public float Size = 64f;
  [Range(0.0f, 1f)]
  public float Fade = 1f;
  public Shader SCShader;
  private Material SCMaterial;
  [Range(-0.5f, 0.5f)]
  public float DirectionX;

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

  private void Start()
  {
    this.SCShader = Shader.Find("CameraFilterPack/Atmosphere_Snow_8bits");
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
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.Threshold);
      this.material.SetFloat("_Value2", this.Size);
      this.material.SetFloat("_Value3", this.DirectionX);
      this.material.SetFloat("_Value4", this.Fade);
      this.material.SetVector("_ScreenResolution", new Vector4((float) sourceTexture.width, (float) sourceTexture.height, 0.0f, 0.0f));
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
    if (!(bool) ((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
