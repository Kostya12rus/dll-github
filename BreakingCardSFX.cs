﻿// Decompiled with JetBrains decompiler
// Type: BreakingCardSFX
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakingCardSFX : MonoBehaviour
{
  public float waitTime = 1.3f;
  public string[] texts;
  private TextMeshProUGUI text;
  private Text txt;

  private void OnEnable()
  {
    Timing.KillCoroutines(this.gameObject);
    Timing.RunCoroutine(this._DoAnimation(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _DoAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new BreakingCardSFX.\u003C_DoAnimation\u003Ec__Iterator0() { \u0024this = this };
  }
}
