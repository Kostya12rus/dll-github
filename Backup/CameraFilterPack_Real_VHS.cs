// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Real_VHS
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/VHS/Real VHS HQ")]
public class CameraFilterPack_Real_VHS : MonoBehaviour
{
  public Shader SCShader;
  private Material SCMaterial;
  private Texture2D VHS;
  private Texture2D VHS2;
  [Range(0.0f, 1f)]
  public float TRACKING;
  [Range(0.0f, 1f)]
  public float JITTER;
  [Range(0.0f, 1f)]
  public float GLITCH;
  [Range(0.0f, 1f)]
  public float NOISE;
  [Range(-1f, 1f)]
  public float Brightness;
  [Range(0.0f, 1.5f)]
  public float Constrast;

  public CameraFilterPack_Real_VHS()
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
    this.SCShader = Shader.Find("CameraFilterPack/Real_VHS");
    this.VHS = Resources.Load("CameraFilterPack_VHS1") as Texture2D;
    this.VHS2 = Resources.Load("CameraFilterPack_VHS2") as Texture2D;
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  public static Texture2D GetRTPixels(Texture2D t, RenderTexture rt, int sx, int sy)
  {
    RenderTexture active = RenderTexture.get_active();
    RenderTexture.set_active(rt);
    t.ReadPixels(new Rect(0.0f, 0.0f, (float) ((Texture) t).get_width(), (float) ((Texture) t).get_height()), 0, 0);
    RenderTexture.set_active(active);
    return t;
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      this.material.SetTexture("VHS", (Texture) this.VHS);
      this.material.SetTexture("VHS2", (Texture) this.VHS2);
      this.material.SetFloat("TRACKING", this.TRACKING);
      this.material.SetFloat("JITTER", this.JITTER);
      this.material.SetFloat("GLITCH", this.GLITCH);
      this.material.SetFloat("NOISE", this.NOISE);
      this.material.SetFloat("Brightness", this.Brightness);
      this.material.SetFloat("CONTRAST", 1f - this.Constrast);
      RenderTexture temporary = RenderTexture.GetTemporary(382, 576, 0);
      ((Texture) temporary).set_filterMode((FilterMode) 2);
      Graphics.Blit((Texture) sourceTexture, temporary, this.material);
      Graphics.Blit((Texture) temporary, destTexture);
      RenderTexture.ReleaseTemporary(temporary);
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
