// Decompiled with JetBrains decompiler
// Type: WarheadLight
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WarheadLight : MonoBehaviour
{
  public float IntensityMultiplier = 1f;
  public Color NormalColor;
  public Color RedColor;
  private float startIntensity;

  private void Awake()
  {
    this.startIntensity = this.GetComponent<Light>().intensity;
  }

  private void Start()
  {
    WarheadLightManager.AddLight(this);
  }

  public void WarheadEnable()
  {
    Timing.KillCoroutines(this.gameObject);
    this.GetComponent<Light>().color = this.RedColor;
    this.GetComponent<Light>().intensity = this.startIntensity * this.IntensityMultiplier;
  }

  public void WarheadDisable()
  {
    Timing.KillCoroutines(this.gameObject);
    Timing.RunCoroutine(this._FadeoffAnimation(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _FadeoffAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WarheadLight.\u003C_FadeoffAnimation\u003Ec__Iterator0() { \u0024this = this };
  }
}
