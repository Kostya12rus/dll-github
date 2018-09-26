// Decompiled with JetBrains decompiler
// Type: SteamServerManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Steamworks;
using UnityEngine;

public class SteamServerManager : MonoBehaviour
{
  public static SteamServerManager Instance;
  private bool _gsInitialized;
  private Callback<SteamServersConnected_t> Callback_ServerConnected;

  public SteamServerManager()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    SteamServerManager.Instance = this;
  }
}
