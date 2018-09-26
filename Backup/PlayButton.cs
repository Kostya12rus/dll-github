// Decompiled with JetBrains decompiler
// Type: PlayButton
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using GameConsole;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
  public static int maxPlayers = 20;
  public string Ip;
  public string Port;
  public string InfoType;
  public Text Motd;
  public Text Players;

  public PlayButton()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    PlayButton.maxPlayers = 20;
  }

  private void SetMaxPlayers(string s)
  {
    try
    {
      s = s.Split('/')[1];
      PlayButton.maxPlayers = int.Parse(s);
    }
    catch
    {
      PlayButton.maxPlayers = 20;
    }
  }

  public void Click()
  {
    if (CrashDetector.Show())
      return;
    CustomNetworkManager objectOfType = (CustomNetworkManager) Object.FindObjectOfType<CustomNetworkManager>();
    if (NetworkClient.get_active())
      objectOfType.StopClient();
    NetworkServer.Reset();
    objectOfType.ShowLog(13);
    objectOfType.set_networkAddress(this.Ip);
    CustomNetworkManager.ConnectionIp = this.Ip;
    try
    {
      objectOfType.set_networkPort(int.Parse(this.Port));
    }
    catch
    {
      Console.singleton.AddLog("Wrong server port, parsing to 7777!", new Color32((byte) 182, (byte) 182, (byte) 182, byte.MaxValue), false);
      objectOfType.set_networkPort(7777);
    }
    Console.singleton.AddLog("Connecting to " + this.Ip + ":" + this.Port + "!", new Color32((byte) 182, (byte) 182, (byte) 182, byte.MaxValue), false);
    objectOfType.StartClient();
    this.SetMaxPlayers(this.Players.get_text());
  }

  public void ShowInfo()
  {
    ServerInfo.ShowInfo(this.InfoType);
  }
}
