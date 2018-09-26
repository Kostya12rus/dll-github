// Decompiled with JetBrains decompiler
// Type: StartScreen
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
  public GameObject popup;
  public Image black;
  public Text youare;
  public Text wmi;
  public Text wihtd;
  private CoroutineHandle handle;

  public StartScreen()
  {
    base.\u002Ector();
  }

  public void PlayAnimation(int classID)
  {
    Timing.KillCoroutines(this.handle);
    this.handle = Timing.RunCoroutine(this._Animate(classID), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Animate(int classID)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new StartScreen.\u003C_Animate\u003Ec__Iterator0()
    {
      classID = classID,
      \u0024this = this
    };
  }
}
