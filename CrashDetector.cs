// Decompiled with JetBrains decompiler
// Type: CrashDetector
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CrashDetector : MonoBehaviour
{
  public GameObject root;
  public static CrashDetector singleton;

  private void Awake()
  {
    CrashDetector.singleton = this;
  }

  public static bool Show()
  {
    if (!SystemInfo.graphicsDeviceName.ToUpper().Contains("INTEL") || PlayerPrefs.GetInt("intel_warning") == 1)
      return false;
    PlayerPrefs.SetInt("intel_warning", 1);
    CrashDetector.singleton.RunCoroutine(CrashDetector.singleton._IShow());
    return true;
  }

  public void RunCoroutine(IEnumerator<float> coroutine)
  {
    Timing.RunCoroutine(coroutine, Segment.FixedUpdate);
  }

  [DebuggerHidden]
  public IEnumerator<float> _IShow()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CrashDetector.\u003C_IShow\u003Ec__Iterator0() { \u0024this = this };
  }
}
