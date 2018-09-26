﻿// Decompiled with JetBrains decompiler
// Type: TimeBehaviour
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TimeBehaviour : MonoBehaviour
{
  public static long CurrentTimestamp()
  {
    return DateTime.UtcNow.Ticks;
  }

  public static bool ValidateTimestamp(long timestampentry, long timestampexit, long limit)
  {
    return timestampexit - timestampentry < limit;
  }
}
