// Decompiled with JetBrains decompiler
// Type: CentralServer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

public class CentralServer : MonoBehaviour
{
  public static object RefreshLock;
  private static string _serversPath;
  private static string[] _servers;
  private static List<string> _workingServers;
  private static DateTime _lastReset;

  public static string MasterUrl { get; private set; }

  public static string StandardUrl { get; private set; }

  public static string SelectedServer { get; private set; }

  public static bool TestServer { get; private set; }

  public static bool ServerSelected { get; set; }

  public void Start()
  {
    if (File.Exists(FileManager.AppFolder + "testserver.txt"))
    {
      CentralServer.StandardUrl = "https://test.scpslgame.com/";
      CentralServer.MasterUrl = "https://test.scpslgame.com/";
      CentralServer.TestServer = true;
      CentralServer.ServerSelected = true;
      ServerConsole.AddLog("Using TEST central server: " + CentralServer.MasterUrl);
    }
    else
    {
      CentralServer.MasterUrl = "https://api.scpslgame.com/";
      CentralServer.StandardUrl = "https://api.scpslgame.com/";
      CentralServer.TestServer = false;
      CentralServer._lastReset = DateTime.MinValue;
      CentralServer._servers = new string[0];
      CentralServer._workingServers = new List<string>();
      CentralServer.RefreshLock = new object();
      if (!Directory.Exists(FileManager.AppFolder + "internal/"))
        Directory.CreateDirectory(FileManager.AppFolder + "internal/");
      CentralServer._serversPath = FileManager.AppFolder + "internal/CentralServers";
      if (File.Exists(CentralServer._serversPath))
      {
        CentralServer._servers = FileManager.ReadAllLines(CentralServer._serversPath);
        CentralServer._workingServers = ((IEnumerable<string>) CentralServer._servers).ToList<string>();
        if (!ServerStatic.IsDedicated)
          GameConsole.Console.singleton.AddLog("Cached central servers count: " + (object) CentralServer._servers.Length, (Color32) Color.grey, false);
        if (CentralServer._servers.Length != 0)
        {
          Random random = new Random();
          CentralServer.SelectedServer = CentralServer._servers[random.Next(CentralServer._servers.Length)];
          CentralServer.StandardUrl = "https://" + CentralServer.SelectedServer.ToLower() + ".scpslgame.com/";
          if (ServerStatic.IsDedicated)
            ServerConsole.AddLog("Selected central server: " + CentralServer.SelectedServer + " (" + CentralServer.StandardUrl + ")");
          else
            GameConsole.Console.singleton.AddLog("Selected central server: " + CentralServer.SelectedServer + " (" + CentralServer.StandardUrl + ")", (Color32) Color.grey, false);
        }
      }
      Timing.RunCoroutine(CentralServer._RefreshServerList(true), Segment.FixedUpdate);
    }
  }

  [DebuggerHidden]
  public static IEnumerator<float> _RefreshServerList(bool planned = false)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CentralServer.\u003C_RefreshServerList\u003Ec__Iterator0() { planned = planned };
  }

  public static bool ChangeCentralServer(bool remove)
  {
    CentralServer.ServerSelected = false;
    if (CentralServer.SelectedServer == "Primary API")
    {
      if (CentralServer._lastReset >= DateTime.Now.AddMinutes(-2.0))
        return false;
      CentralServer._RefreshServerList(true);
      return true;
    }
    if (CentralServer._workingServers.Count == 0)
    {
      GameConsole.Console.singleton.AddLog("All known central servers aren't working.", (Color32) Color.yellow, false);
      CentralServer._workingServers.Add("API");
      CentralServer.SelectedServer = "Primary API";
      CentralServer.StandardUrl = "https://api.scpslgame.com/";
      GameConsole.Console.singleton.AddLog("Changed central server: " + CentralServer.SelectedServer + " (" + CentralServer.StandardUrl + ")", (Color32) Color.yellow, false);
      return true;
    }
    if (remove && CentralServer._workingServers.Contains(CentralServer.SelectedServer))
      CentralServer._workingServers.Remove(CentralServer.SelectedServer);
    if (CentralServer._workingServers.Count == 0)
    {
      CentralServer._workingServers.Add("API");
      CentralServer.SelectedServer = "Primary API";
      CentralServer.StandardUrl = "https://api.scpslgame.com/";
      GameConsole.Console.singleton.AddLog("Changed central server: " + CentralServer.SelectedServer + " (" + CentralServer.StandardUrl + ")", (Color32) Color.yellow, false);
      return true;
    }
    Random random = new Random();
    CentralServer.SelectedServer = CentralServer._workingServers[random.Next(0, CentralServer._workingServers.Count)];
    CentralServer.StandardUrl = "https://" + CentralServer.SelectedServer.ToLower() + ".scpslgame.com/";
    GameConsole.Console.singleton.AddLog("Changed central server: " + CentralServer.SelectedServer + " (" + CentralServer.StandardUrl + ")", (Color32) Color.yellow, false);
    return true;
  }
}
