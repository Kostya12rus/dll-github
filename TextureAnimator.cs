// Decompiled with JetBrains decompiler
// Type: TextureAnimator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
  public Material[] textures;
  public Renderer targetRenderer;
  public float cooldown;
  public Light optionalLight;
  public int lightRange;

  private void Start()
  {
    Timing.RunCoroutine(this._Animate(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Animate()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new TextureAnimator.\u003C_Animate\u003Ec__Iterator0() { \u0024this = this };
  }
}
