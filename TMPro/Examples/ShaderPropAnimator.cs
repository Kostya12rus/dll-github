// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.ShaderPropAnimator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class ShaderPropAnimator : MonoBehaviour
  {
    private Renderer m_Renderer;
    private Material m_Material;
    public AnimationCurve GlowCurve;
    public float m_frame;

    public ShaderPropAnimator()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this.m_Renderer = (Renderer) ((Component) this).GetComponent<Renderer>();
      this.m_Material = this.m_Renderer.get_material();
    }

    private void Start()
    {
      this.StartCoroutine(this.AnimateProperties());
    }

    [DebuggerHidden]
    private IEnumerator AnimateProperties()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ShaderPropAnimator.\u003CAnimateProperties\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
