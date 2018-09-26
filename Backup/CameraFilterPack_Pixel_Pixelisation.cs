// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Pixel_Pixelisation
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Pixel/Pixelisation")]
[ExecuteInEditMode]
public class CameraFilterPack_Pixel_Pixelisation : MonoBehaviour
{
  public Shader SCShader;
  [Range(0.6f, 120f)]
  public float _Pixelisation;
  [Range(0.6f, 120f)]
  public float _SizeX;
  [Range(0.6f, 120f)]
  public float _SizeY;
  private Material SCMaterial;

  public CameraFilterPack_Pixel_Pixelisation()
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
    this.SCShader = Shader.Find("CameraFilterPack/Pixel_Pixelisation");
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      this.material.SetFloat("_Val", this._Pixelisation);
      this.material.SetFloat("_Val2", this._SizeX);
      this.material.SetFloat("_Val3", this._SizeY);
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
