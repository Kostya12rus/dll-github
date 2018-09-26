// Decompiled with JetBrains decompiler
// Type: UnitySA.Utility.LerpCtrlBob
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;

namespace UnitySA.Utility
{
  [Serializable]
  public class LerpCtrlBob
  {
    public float BobDuration;
    public float BobAmount;
    private float m_Offset;

    public float Offset()
    {
      return this.m_Offset;
    }

    [DebuggerHidden]
    public IEnumerator DoBobCycle()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new LerpCtrlBob.\u003CDoBobCycle\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
