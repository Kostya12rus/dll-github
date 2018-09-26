// Decompiled with JetBrains decompiler
// Type: ServerConsole
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ServerConsole : MonoBehaviour
{
  private static bool _accepted = true;
  public static bool Update = false;
  private static readonly List<string> PrompterQueue = new List<string>();
  public static ServerConsole singleton;
  public static int LogId;
  public static int Cycle;
  public static int Port;
  public static Process ConsoleId;
  public static string Session;
  public static string Password;
  public static string Ip;
  public static AsymmetricKeyParameter Publickey;
  private bool _errorSent;

  private void Start()
  {
    if (!ServerStatic.IsDedicated)
      return;
    ServerConsole.LogId = 0;
    ServerConsole._accepted = true;
    if (Directory.Exists("SCPSL_Data / Dedicated / " + ServerConsole.Session))
      Directory.Delete("SCPSL_Data / Dedicated / " + ServerConsole.Session, true);
    Timing.RunCoroutine(this._Prompt());
    Timing.RunCoroutine(ServerConsole._CheckLog());
  }

  private void Awake()
  {
    ServerConsole.singleton = this;
  }

  [DebuggerHidden]
  private static IEnumerator<float> _CheckLog()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ServerConsole.\u003C_CheckLog\u003Ec__Iterator0 checkLogCIterator0 = new ServerConsole.\u003C_CheckLog\u003Ec__Iterator0();
    return (IEnumerator<float>) checkLogCIterator0;
  }

  public static void AddLog(string q)
  {
    if (ServerStatic.IsDedicated)
      ServerConsole.PrompterQueue.Add(q);
    else
      GameConsole.Console.singleton.AddLog(q, (Color32) Color.grey, false);
  }

  public static string GetClientInfo(NetworkConnection conn)
  {
    GameObject connectedRoot = GameConsole.Console.FindConnectedRoot(conn);
    return connectedRoot.GetComponent<NicknameSync>().myNick + " ( " + connectedRoot.GetComponent<CharacterClassManager>().SteamId + " | " + conn.address + " )";
  }

  public static string GetClientInfo(GameObject gameObject)
  {
    return gameObject.GetComponent<NicknameSync>().myNick + " ( " + gameObject.GetComponent<CharacterClassManager>().SteamId + " | " + gameObject.GetComponent<NetworkBehaviour>().connectionToClient.address + " )";
  }

  public static void Disconnect(GameObject player, string message)
  {
    if ((Object) player == (Object) null)
      return;
    NetworkBehaviour component1 = player.GetComponent<NetworkBehaviour>();
    if ((Object) component1 == (Object) null || !component1.connectionToClient.isConnected)
      return;
    CharacterClassManager component2 = player.GetComponent<CharacterClassManager>();
    if ((Object) component2 == (Object) null)
    {
      component1.connectionToClient.Disconnect();
      component1.connectionToClient.Dispose();
    }
    else
      component2.DisconnectClient(component1.connectionToClient, message);
  }

  public static void Disconnect(NetworkConnection conn, string message)
  {
    GameObject connectedRoot = GameConsole.Console.FindConnectedRoot(conn);
    if ((Object) connectedRoot == (Object) null)
    {
      conn.Disconnect();
      conn.Dispose();
    }
    else
      ServerConsole.Disconnect(connectedRoot, message);
  }

  [DebuggerHidden]
  public IEnumerator<float> _Prompt()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ServerConsole.\u003C_Prompt\u003Ec__Iterator1() { \u0024this = this };
  }

  private static void ColorText(string text)
  {
    Debug.Log((object) string.Format("<color={0}>{1}</color>", (object) ServerConsole.GetColor(text), (object) text), (Object) null);
  }

  private static string GetColor(string text)
  {
    int num = 9;
    if (text.Contains("LOGTYPE"))
    {
      try
      {
        string str = text.Remove(0, text.IndexOf("LOGTYPE", StringComparison.Ordinal) + 7);
        num = int.Parse(!str.Contains("-") ? str : str.Remove(0, 1));
      }
      catch
      {
        num = 9;
      }
    }
    string empty = string.Empty;
    switch (num)
    {
      case 0:
        return "#000";
      case 1:
        return "#183487";
      case 2:
        return "#0b7011";
      case 3:
        return "#0a706c";
      case 4:
        return "#700a0a";
      case 5:
        return "#5b0a40";
      case 6:
        return "#aaa800";
      case 7:
        return "#afafaf";
      case 8:
        return "#5b5b5b";
      case 9:
        return "#0055ff";
      case 10:
        return "#10ce1a";
      case 11:
        return "#0fc7ce";
      case 12:
        return "#ce0e0e";
      case 13:
        return "#c70dce";
      case 14:
        return "#ffff07";
      case 15:
        return "#e0e0e0";
      default:
        return "#fff";
    }
  }

  private static string EnterCommand(string cmds)
  {
    string str = "Command accepted.";
    string[] strArray = cmds.ToUpper().Split(' ');
    if (strArray.Length <= 0)
      return str;
    switch (strArray[0])
    {
      case "FORCESTART":
        bool flag = false;
        GameObject gameObject = GameObject.Find("Host");
        if ((Object) gameObject != (Object) null)
        {
          CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
          if ((Object) component != (Object) null && component.isLocalPlayer && (component.isServer && !component.roundStarted))
          {
            component.ForceRoundStart();
            flag = true;
          }
        }
        str = !flag ? "Failed to force start.LOGTYPE14" : "Forced round start.";
        break;
      case "CONFIG":
        if (File.Exists(ConfigFile.ConfigPath))
        {
          Application.OpenURL(ConfigFile.ConfigPath);
          break;
        }
        str = "Config file not found!";
        break;
      default:
        str = GameConsole.Console.singleton.TypeCommand(cmds);
        break;
    }
    return str;
  }

  public void RunServer()
  {
    Timing.RunCoroutine(this._RefreshSession(), Segment.Update);
  }

  public void RunRefreshPublicKey()
  {
    Timing.RunCoroutine(this._RefreshPublicKey(), Segment.Update);
  }

  public void RunRefreshCentralServers()
  {
    Timing.RunCoroutine(this._RefreshCentralServers(), Segment.Update);
  }

  [DebuggerHidden]
  private IEnumerator<float> _RefreshCentralServers()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ServerConsole.\u003C_RefreshCentralServers\u003Ec__Iterator2 serversCIterator2 = new ServerConsole.\u003C_RefreshCentralServers\u003Ec__Iterator2();
    return (IEnumerator<float>) serversCIterator2;
  }

  [DebuggerHidden]
  private IEnumerator<float> _RefreshPublicKey()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ServerConsole.\u003C_RefreshPublicKey\u003Ec__Iterator3 publicKeyCIterator3 = new ServerConsole.\u003C_RefreshPublicKey\u003Ec__Iterator3();
    return (IEnumerator<float>) publicKeyCIterator3;
  }

  [DebuggerHidden]
  private IEnumerator<float> _RefreshPublicKeyOnce()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ServerConsole.\u003C_RefreshPublicKeyOnce\u003Ec__Iterator4 keyOnceCIterator4 = new ServerConsole.\u003C_RefreshPublicKeyOnce\u003Ec__Iterator4();
    return (IEnumerator<float>) keyOnceCIterator4;
  }

  [DebuggerHidden]
  private IEnumerator<float> _RefreshSession()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ServerConsole.\u003C_RefreshSession\u003Ec__Iterator5() { \u0024this = this };
  }

  public void RefreshToken()
  {
    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/verkey.txt";
    if (!File.Exists(path))
      return;
    StreamReader streamReader = new StreamReader(path);
    string end = streamReader.ReadToEnd();
    if (string.IsNullOrEmpty(ServerConsole.Password) && !string.IsNullOrEmpty(end))
      ServerConsole.AddLog("Verification token loaded! Server probably will be listed on public list.");
    if (ServerConsole.Password != end)
    {
      ServerConsole.AddLog("Verification token reloaded.");
      ServerConsole.Update = true;
    }
    ServerConsole.Password = end;
    ServerStatic.PermissionsHandler.SetServerAsVerified();
    streamReader.Close();
  }

  private static void TerminateProcess()
  {
    ServerStatic.IsDedicated = false;
    Application.Quit();
  }
}
