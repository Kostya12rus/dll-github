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
  public static Recoil singleton;
  public GameObject plyCam;
  private float recoil;
  private float backSpeed;
  private float lerpSpeed;
  private Vector3 target;
  private MouseLook mlook;
  public float positionOffset;

  public Recoil()
  {
    base.\u002Ector();
  }

  public void DoRecoil(RecoilProperties r, float multip)
  {
    if (this.mlook == null)
      return;
    this.backSpeed = r.backSpeed;
    this.lerpSpeed = r.lerpSpeed;
    this.recoil += r.shockSize * multip;
    Vector3 vector3_1 = (Vector3) null;
    vector3_1.x = (__Null) (double) Random.Range(-60, -50);
    vector3_1.y = (__Null) (double) Random.Range(-5, 5);
    vector3_1.z = (__Null) (double) Random.Range(-5, 5);
    this.target = vector3_1;
    Vector3 vector3_2 = Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(multip, ((Vector3) ref this.target).get_normalized()), 13f), r.upSize);
    this.mlook.Recoil((float) -vector3_2.x, (float) vector3_2.y);
  }

  private void Start()
  {
    Timing.RunCoroutine(this._Start(), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Recoil.\u003C_Start\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public static void StaticDoRecoil(RecoilProperties r, float multip)
  {
    Recoil.singleton.DoRecoil(r, multip);
  }
}
