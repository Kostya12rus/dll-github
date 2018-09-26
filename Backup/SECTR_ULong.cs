// Decompiled with JetBrains decompiler
// Type: SECTR_ULong
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class SECTR_ULong
{
  [SerializeField]
  private int first;
  [SerializeField]
  private int second;

  public SECTR_ULong(ulong newValue)
  {
    this.value = newValue;
  }

  public SECTR_ULong()
  {
    this.value = 0UL;
  }

  public ulong value
  {
    get
    {
      return (ulong) this.second << 32 | (ulong) this.first;
    }
    set
    {
      this.first = (int) ((long) value & (long) uint.MaxValue);
      this.second = (int) (value >> 32);
    }
  }

  public override string ToString()
  {
    return string.Format("[ULong: value={0}, firstHalf={1}, secondHalf={2}]", (object) this.value, (object) this.first, (object) this.second);
  }

  public static bool operator >(SECTR_ULong a, ulong b)
  {
    return a.value > b;
  }

  public static bool operator >(ulong a, SECTR_ULong b)
  {
    return a > b.value;
  }

  public static bool operator <(SECTR_ULong a, ulong b)
  {
    return a.value < b;
  }

  public static bool operator <(ulong a, SECTR_ULong b)
  {
    return a < b.value;
  }
}
