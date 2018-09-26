// Decompiled with JetBrains decompiler
// Type: NetManagerValueSetter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;

public class NetManagerValueSetter : MonoBehaviour
{
  private CustomNetworkManager _singleton;

  private void Start()
  {
    this._singleton = NetworkManager.singleton.GetComponent<CustomNetworkManager>();
  }

  public void ChangeIP(string ip)
  {
    this._singleton.networkAddress = ip;
    CustomNetworkManager.ConnectionIp = ip;
  }

  public void ChangePort(int port)
  {
    this._singleton.networkPort = port;
  }

  public void JoinGame()
  {
    this._singleton.StartClient();
  }

  public void HostGame()
  {
    this._singleton.StartHost();
  }

  public void Disconnect()
  {
    this._singleton.StopHost();
  }
}
