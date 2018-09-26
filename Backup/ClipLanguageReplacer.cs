// Decompiled with JetBrains decompiler
// Type: ClipLanguageReplacer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ClipLanguageReplacer : MonoBehaviour
{
  [SerializeField]
  public AudioClip englishVersion;

  public ClipLanguageReplacer()
  {
    base.\u002Ector();
  }

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ClipLanguageReplacer.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
