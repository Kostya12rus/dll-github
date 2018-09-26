// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Distortion_Half_Sphere
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Distortion/Half_Sphere")]
[ExecuteInEditMode]
public class CameraFilterPack_Distortion_Half_Sphere : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  [Range(1f, 6f)]
  private Material SCMaterial;
  public float SphereSize;
  [Range(-1f, 1f)]
  public float SpherePositionX;
  [Range(-1f, 1f)]
  public float SpherePositionY;
  [Range(1f, 10f)]
  public float Strength;

  public CameraFilterPack_Distortion_Half_Sphere()
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
    this.SCShader = Shader.Find("CameraFilterPack/Distortion_Half_Sphere");
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
      this.material.SetFloat("_SphereSize", this.SphereSize);
      this.material.SetFloat("_SpherePositionX", this.SpherePositionX);
      this.material.SetFloat("_SpherePositionY", this.SpherePositionY);
      this.material.SetFloat("_Strength", this.Strength);
      this.material.SetVector("_ScreenResolution", Vector4.op_Implicit(new Vector2((float) Screen.get_width(), (float) Screen.get_height())));
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
