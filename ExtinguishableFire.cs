// Decompiled with JetBrains decompiler
// Type: ExtinguishableFire
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ExtinguishableFire : MonoBehaviour
{
  public ParticleSystem fireParticleSystem;
  public ParticleSystem smokeParticleSystem;
  protected bool m_isExtinguished;
  private const float m_FireStartingTime = 2f;

  private void Start()
  {
    this.m_isExtinguished = true;
    this.smokeParticleSystem.Stop();
    this.fireParticleSystem.Stop();
    this.StartCoroutine(this.StartingFire());
  }

  public void Extinguish()
  {
    if (this.m_isExtinguished)
      return;
    this.m_isExtinguished = true;
    this.StartCoroutine(this.Extinguishing());
  }

  [DebuggerHidden]
  private IEnumerator Extinguishing()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ExtinguishableFire.\u003CExtinguishing\u003Ec__Iterator0() { \u0024this = this };
  }

  [DebuggerHidden]
  private IEnumerator StartingFire()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ExtinguishableFire.\u003CStartingFire\u003Ec__Iterator1() { \u0024this = this };
  }
}
