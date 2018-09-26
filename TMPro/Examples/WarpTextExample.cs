// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.WarpTextExample
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class WarpTextExample : MonoBehaviour
  {
    public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe[5]{ new Keyframe(0.0f, 0.0f), new Keyframe(0.25f, 2f), new Keyframe(0.5f, 0.0f), new Keyframe(0.75f, 2f), new Keyframe(1f, 0.0f) });
    public float AngleMultiplier = 1f;
    public float SpeedMultiplier = 1f;
    public float CurveScale = 1f;
    private TMP_Text m_TextComponent;

    private void Awake()
    {
      this.m_TextComponent = this.gameObject.GetComponent<TMP_Text>();
    }

    private void Start()
    {
      this.StartCoroutine(this.WarpText());
    }

    private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
    {
      return new AnimationCurve() { keys = curve.keys };
    }

    [DebuggerHidden]
    private IEnumerator WarpText()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new WarpTextExample.\u003CWarpText\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
