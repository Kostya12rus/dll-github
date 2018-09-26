// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.DoorPrinter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace RemoteAdmin
{
  internal class DoorPrinter : MonoBehaviour
  {
    public static readonly string[] SpecialValues = new string[2]{ "*", "!*" };
    public static readonly string[] SpecialTexts = new string[2]{ "(All listed)", "(All not listed)" };
    public GameObject Template;
    public Transform Parent;
    public static string SelectedDoors;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DoorPrinter.\u003CStart\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
