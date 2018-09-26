// Decompiled with JetBrains decompiler
// Type: ProjectorInitializer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class ProjectorInitializer : MonoBehaviour
{
  public ProjectorInitializer.LightStruct[] lights;
  public TextMeshProUGUI projector_label;
  public AudioSource src;
  public AudioClip c_st;
  public AudioClip c_lp;
  public AudioClip c_sp;
  public Transform[] spools;
  private float time;
  public bool started;
  private bool prevStarted;
  private bool dir;

  [DebuggerHidden]
  private IEnumerator<float> _StartProjector()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ProjectorInitializer.\u003C_StartProjector\u003Ec__Iterator0() { \u0024this = this };
  }

  [DebuggerHidden]
  private IEnumerator<float> _StopProjector()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ProjectorInitializer.\u003C_StopProjector\u003Ec__Iterator1() { \u0024this = this };
  }

  private void InitLoop()
  {
    this.src.Stop();
    this.src.PlayOneShot(this.c_lp);
  }

  private void Update()
  {
    if (this.started != this.prevStarted)
    {
      if (this.started)
      {
        Timing.RunCoroutine(this._StartProjector());
        this.prevStarted = true;
      }
      else
      {
        Timing.RunCoroutine(this._StopProjector());
        this.prevStarted = false;
      }
    }
    this.time += Time.deltaTime * (!this.dir ? -2f : 2f);
    this.time = Mathf.Clamp01(this.time / 4f) * 4f;
    foreach (Transform spool in this.spools)
      spool.Rotate(Vector3.up * this.time / 4f);
    foreach (ProjectorInitializer.LightStruct light in this.lights)
      light.SetLight(this.time);
  }

  [Serializable]
  public class LightStruct
  {
    public string label;
    public Color normalColor;
    public Light targetLight;
    public AnimationCurve curve;

    public void SetLight(float time)
    {
      this.targetLight.color = Color.Lerp(Color.black, this.normalColor, this.curve.Evaluate(time));
    }
  }
}
