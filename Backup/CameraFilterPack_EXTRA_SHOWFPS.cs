// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_EXTRA_SHOWFPS
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Camera Filter Pack/EXTRA/SHOWFPS")]
[ExecuteInEditMode]
public class CameraFilterPack_EXTRA_SHOWFPS : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(8f, 42f)]
  public float Size;
  [Range(0.0f, 100f)]
  private int FPS;
  [Range(0.0f, 10f)]
  private float Value3;
  [Range(0.0f, 10f)]
  private float Value4;
  private float accum;
  private int frames;
  public float frequency;

  public CameraFilterPack_EXTRA_SHOWFPS()
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
    this.FPS = 0;
    this.StartCoroutine(this.FPSX());
    this.SCShader = Shader.Find("CameraFilterPack/EXTRA_SHOWFPS");
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
      this.material.SetFloat("_Value", this.Size);
      this.material.SetFloat("_Value2", (float) this.FPS);
      this.material.SetFloat("_Value3", this.Value3);
      this.material.SetFloat("_Value4", this.Value4);
      this.material.SetVector("_ScreenResolution", new Vector4((float) ((Texture) sourceTexture).get_width(), (float) ((Texture) sourceTexture).get_height(), 0.0f, 0.0f));
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  [DebuggerHidden]
  private IEnumerator FPSX()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new CameraFilterPack_EXTRA_SHOWFPS.\u003CFPSX\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void Update()
  {
    this.accum += Time.get_timeScale() / Time.get_deltaTime();
    ++this.frames;
  }

  private void OnDisable()
  {
    if (!Object.op_Implicit((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
