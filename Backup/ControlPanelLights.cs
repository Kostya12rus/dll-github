// Decompiled with JetBrains decompiler
// Type: ControlPanelLights
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ControlPanelLights : MonoBehaviour
{
  public Texture[] emissions;
  public Material targetMat;

  public ControlPanelLights()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    Timing.RunCoroutine(this._Animate(), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Animate()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ControlPanelLights.\u003C_Animate\u003Ec__Iterator0() { \u0024this = this };
  }
}
