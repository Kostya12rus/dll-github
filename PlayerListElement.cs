﻿// Decompiled with JetBrains decompiler
// Type: PlayerListElement
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;

public class PlayerListElement : MonoBehaviour
{
  public GameObject instance;

  public void Use(bool b)
  {
    Object.FindObjectOfType<DissonanceComms>().FindPlayer(this.instance.GetComponent<HlapiPlayer>().PlayerId).IsLocallyMuted = b;
  }
}
