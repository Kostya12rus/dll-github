// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Colors_HSV
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/Colors/HSV")]
[ExecuteInEditMode]
public class CameraFilterPack_Colors_HSV : MonoBehaviour
{
  public Shader SCShader;
  [Range(0.0f, 360f)]
  public float _HueShift;
  [Range(-32f, 32f)]
  public float _Saturation;
  [Range(-32f, 32f)]
  public float _ValueBrightness;
  private Material SCMaterial;

  public CameraFilterPack_Colors_HSV()
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
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      this.material.SetFloat("_HueShift", this._HueShift);
      this.material.SetFloat("_Sat", this._Saturation);
      this.material.SetFloat("_Val", this._ValueBrightness);
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
