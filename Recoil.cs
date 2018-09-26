// Decompiled with JetBrains decompiler
// Type: Recoil
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Recoil : MonoBehaviour
{
  private float backSpeed = 1f;
  private float lerpSpeed = 1f;
  public static Recoil singleton;
  public GameObject plyCam;
  private float recoil;
  private Vector3 target;
  private MouseLook mlook;
  public float positionOffset;

  public void DoRecoil(RecoilProperties r, float multip)
  {
    if (this.mlook == null)
      return;
    this.backSpeed = r.backSpeed;
    this.lerpSpeed = r.lerpSpeed;
    this.recoil += r.shockSize * multip;
    this.target = new Vector3()
    {
      x = (float) Random.Range(-60, -50),
      y = (float) Random.Range(-5, 5),
      z = (float) Random.Range(-5, 5)
    };
    Vector3 vector3 = multip * this.target.normalized * 13f * r.upSize;
    this.mlook.Recoil(-vector3.x, vector3.y);
  }

  private void Start()
  {
    Timing.RunCoroutine(this._Start(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Recoil.\u003C_Start\u003Ec__Iterator0() { \u0024this = this };
  }

  public static void StaticDoRecoil(RecoilProperties r, float multip)
  {
    Recoil.singleton.DoRecoil(r, multip);
  }
}
