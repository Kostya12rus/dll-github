// Decompiled with JetBrains decompiler
// Type: ExplosionCameraShake
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Kino;
using UnityEngine;

public class ExplosionCameraShake : MonoBehaviour
{
  public float force;
  public float deductSpeed;
  public AnalogGlitch glitch;
  public static ExplosionCameraShake singleton;

  private void Update()
  {
    this.glitch.enabled = (double) this.glitch.horizontalShake > 0.0;
    this.force -= Time.deltaTime / this.deductSpeed;
    this.force = Mathf.Clamp01(this.force);
    this.glitch.scanLineJitter = this.force;
    this.glitch.horizontalShake = this.force;
    this.glitch.colorDrift = this.force;
  }

  private void Awake()
  {
    ExplosionCameraShake.singleton = this;
  }

  public void Shake(float explosionForce)
  {
    if ((double) explosionForce <= (double) this.force)
      return;
    this.force = explosionForce;
  }
}
