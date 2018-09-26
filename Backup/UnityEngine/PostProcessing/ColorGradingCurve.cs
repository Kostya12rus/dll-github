// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.ColorGradingCurve
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public sealed class ColorGradingCurve
  {
    public AnimationCurve curve;
    [SerializeField]
    private bool m_Loop;
    [SerializeField]
    private float m_ZeroValue;
    [SerializeField]
    private float m_Range;
    private AnimationCurve m_InternalLoopingCurve;

    public ColorGradingCurve(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
    {
      this.curve = curve;
      this.m_ZeroValue = zeroValue;
      this.m_Loop = loop;
      this.m_Range = ((Vector2) ref bounds).get_magnitude();
    }

    public void Cache()
    {
      if (!this.m_Loop)
        return;
      int length = this.curve.get_length();
      if (length < 2)
        return;
      if (this.m_InternalLoopingCurve == null)
        this.m_InternalLoopingCurve = new AnimationCurve();
      Keyframe keyframe1 = this.curve.get_Item(length - 1);
      ref Keyframe local1 = ref keyframe1;
      ((Keyframe) ref local1).set_time(((Keyframe) ref local1).get_time() - this.m_Range);
      Keyframe keyframe2 = this.curve.get_Item(0);
      ref Keyframe local2 = ref keyframe2;
      ((Keyframe) ref local2).set_time(((Keyframe) ref local2).get_time() + this.m_Range);
      this.m_InternalLoopingCurve.set_keys(this.curve.get_keys());
      this.m_InternalLoopingCurve.AddKey(keyframe1);
      this.m_InternalLoopingCurve.AddKey(keyframe2);
    }

    public float Evaluate(float t)
    {
      if (this.curve.get_length() == 0)
        return this.m_ZeroValue;
      if (!this.m_Loop || this.curve.get_length() == 1)
        return this.curve.Evaluate(t);
      return this.m_InternalLoopingCurve.Evaluate(t);
    }
  }
}
