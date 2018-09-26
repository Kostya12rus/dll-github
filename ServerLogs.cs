// Decompiled with JetBrains decompiler
// Type: ServerLogs
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ServerLogs : MonoBehaviour
{
  public static readonly string[] Txt = new string[6]
  {
    "Connection update",
    "Remote Admin",
    "Remote Admin - Misc",
    "Kill",
    "Game Event",
    "Internal"
  };
  public static readonly string[] Modulestxt = new string[6]
  {
    "Warhead",
    "Networking",
    "Class change",
    "Permissions",
    "Administrative",
    "Logger"
  };
  private readonly List<ServerLogs.ServerLog> _logs;
  public static ServerLogs singleton;
  private int _port;
  private int _ready;
  private int _maxlen;
  private int _modulemaxlen;
  private bool _locked;
  private bool _queued;
  private string _roundStartTime;

  public ServerLogs()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    ServerLogs.singleton = this;
    ((IEnumerable<string>) ServerLogs.Txt).ToList<string>().ForEach((Action<string>) (txt => this._maxlen = Math.Max(this._maxlen, txt.Length)));
    ((IEnumerable<string>) ServerLogs.Modulestxt).ToList<string>().ForEach((Action<string>) (txt => this._modulemaxlen = Math.Max(this._modulemaxlen, txt.Length)));
    ++this._ready;
    ServerLogs.AddLog(ServerLogs.Modules.Logger, "Started logging.", ServerLogs.ServerLogType.InternalMessage);
  }

  public static void AddLog(ServerLogs.Modules module, string msg, ServerLogs.ServerLogType type)
  {
    string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
    ServerLogs.singleton._logs.Add(new ServerLogs.ServerLog()
    {
      Content = msg,
      Module = ServerLogs.Modulestxt[(int) module],
      Type = ServerLogs.Txt[(int) type],
      Time = str
    });
    if (!NetworkServer.get_active())
      return;
    ServerLogs.singleton.StartCoroutine(ServerLogs.singleton.AppendLog());
  }

  private void Start()
  {
    this._port = ((NetworkManager) NetworkManager.singleton).get_networkPort();
    this._roundStartTime = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month >= 10 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString()) + "-" + (DateTime.Now.Day >= 10 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day.ToString()) + " " + (DateTime.Now.Hour >= 10 ? DateTime.Now.Hour.ToString() : "0" + DateTime.Now.Hour.ToString()) + "." + (DateTime.Now.Minute >= 10 ? DateTime.Now.Minute.ToString() : "0" + DateTime.Now.Minute.ToString()) + "." + (DateTime.Now.Second >= 10 ? DateTime.Now.Second.ToString() : "0" + DateTime.Now.Second.ToString());
    ++this._ready;
  }

  private void OnDestroy()
  {
    if (!NetworkServer.get_active())
      return;
    this.AppendLog();
  }

  private void Update()
  {
    if (!this._queued)
      return;
    this.StartCoroutine(this.AppendLog());
  }

  [DebuggerHidden]
  private IEnumerator AppendLog()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ServerLogs.\u003CAppendLog\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private static string ToMax(string text, int max)
  {
    while (text.Length < max)
      text += " ";
    return text;
  }

  public enum ServerLogType
  {
    ConnectionUpdate,
    RemoteAdminActivity_GameChanging,
    RemoteAdminActivity_Misc,
    KillLog,
    GameEvent,
    InternalMessage,
  }

  public enum Modules
  {
    Warhead,
    Networking,
    ClassChange,
    Permissions,
    Administrative,
    Logger,
  }

  public class ServerLog
  {
    public string Content;
    public string Type;
    public string Module;
    public string Time;
    public bool Saved;
  }
}
