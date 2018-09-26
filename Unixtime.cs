// Decompiled with JetBrains decompiler
// Type: Unixtime
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

public class Unixtime : MonoBehaviour
{
  private DiscordController controller;

  private void Start()
  {
    this.ResetTime();
  }

  public void ResetTime()
  {
    this.controller = this.GetComponent<DiscordController>();
    this.controller.presence.startTimestamp = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
  }
}
