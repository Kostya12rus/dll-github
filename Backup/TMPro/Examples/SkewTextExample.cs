// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.SkewTextExample
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class SkewTextExample : MonoBehaviour
  {
    private TMP_Text m_TextComponent;
    public AnimationCurve VertexCurve;
    public float CurveScale;
    public float ShearAmount;

    public SkewTextExample()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this.m_TextComponent = (TMP_Text) ((Component) this).get_gameObject().GetComponent<TMP_Text>();
    }

    private void Start()
    {
      this.StartCoroutine(this.WarpText());
    }

    private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
    {
      AnimationCurve animationCurve = new AnimationCurve();
      animationCurve.set_keys(curve.get_keys());
      return animationCurve;
    }

    [DebuggerHidden]
    private IEnumerator WarpText()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new SkewTextExample.\u003CWarpText\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
