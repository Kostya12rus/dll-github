// Decompiled with JetBrains decompiler
// Type: OOF_Controller
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class OOF_Controller : MonoBehaviour
{
  public static OOF_Controller singleton;
  public CameraFilterPack_AAA_Blood_Hit blood;
  public CameraFilterPack_Color_GrayScale grayScale;
  public PlayerStats playerStats;
  public CharacterClassManager ccm;
  public float deductSpeed;
  public AudioSource audioSource;
  public AudioClip[] clips;
  private float minimalDmg;
  public AnimationCurve intensityOverDmg;
  public AnimationCurve grayScaleOverDmg;
  public AnimationCurve overallOverDmg;

  public OOF_Controller()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (!this.ccm.get_isLocalPlayer())
      return;
    Timing.RunCoroutine(this._ContinuousProcessing(), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _ContinuousProcessing()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new OOF_Controller.\u003C_ContinuousProcessing\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public void AddBlood(Vector3 hitDir, float fullBlood)
  {
    if ((double) fullBlood > 0.200000002980232 && this.clips.Length > 0)
      this.audioSource.PlayOneShot(this.clips[Random.Range(0, this.clips.Length)]);
    if (hitDir.x > 0.0)
    {
      this.blood.Hit_Right += (float) (hitDir.x * 2.0);
      this.blood.Blood_Hit_Right += (float) (hitDir.x / 2.0);
    }
    if (hitDir.x < 0.0)
    {
      this.blood.Hit_Left += (float) (-hitDir.x * 2.0);
      this.blood.Blood_Hit_Left += (float) (-hitDir.x / 2.0);
    }
    if (hitDir.z > 0.0)
    {
      this.blood.Hit_Up += (float) (hitDir.z * 2.0);
      this.blood.Blood_Hit_Up += (float) (hitDir.z / 2.0);
    }
    if (hitDir.z < 0.0)
    {
      this.blood.Hit_Down += (float) (-hitDir.z * 2.0);
      this.blood.Blood_Hit_Down += (float) (-hitDir.z / 2.0);
    }
    this.blood.Hit_Full += fullBlood;
    this.blood.Blood_Hit_Full_1 += fullBlood / 3f;
    this.blood.Blood_Hit_Full_2 += fullBlood / 3f;
    this.blood.Blood_Hit_Full_3 += fullBlood / 3f;
  }

  private void DeductBlood(float speed)
  {
    float num = this.intensityOverDmg.Evaluate(this.minimalDmg);
    this.blood.Hit_Right = Mathf.Lerp(this.blood.Hit_Right, 0.0f, speed);
    this.blood.Hit_Left = Mathf.Lerp(this.blood.Hit_Left, 0.0f, speed);
    this.blood.Hit_Up = Mathf.Lerp(this.blood.Hit_Up, 0.0f, speed);
    this.blood.Hit_Down = Mathf.Lerp(this.blood.Hit_Down, 0.0f, speed);
    this.blood.Blood_Hit_Right = Mathf.Lerp(this.blood.Blood_Hit_Right, 0.0f, speed * 1.5f);
    this.blood.Blood_Hit_Left = Mathf.Lerp(this.blood.Blood_Hit_Left, 0.0f, speed * 1.5f);
    this.blood.Blood_Hit_Up = Mathf.Lerp(this.blood.Blood_Hit_Up, 0.0f, speed * 1.5f);
    this.blood.Blood_Hit_Down = Mathf.Lerp(this.blood.Blood_Hit_Down, 0.0f, speed * 1.5f);
    this.blood.Hit_Full = Mathf.Lerp(this.blood.Hit_Full, num, speed);
    this.blood.Blood_Hit_Full_1 = Mathf.Lerp(this.blood.Blood_Hit_Full_1, num, speed);
    this.blood.Blood_Hit_Full_2 = Mathf.Lerp(this.blood.Blood_Hit_Full_2, num, speed);
    this.blood.Blood_Hit_Full_3 = Mathf.Lerp(this.blood.Blood_Hit_Full_3, num, speed);
    this.blood.LightReflect = this.overallOverDmg.Evaluate(this.minimalDmg);
    this.grayScale._Fade = this.grayScaleOverDmg.Evaluate(this.minimalDmg);
  }
}
