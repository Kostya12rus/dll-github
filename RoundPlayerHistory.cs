// Decompiled with JetBrains decompiler
// Type: RoundPlayerHistory
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundPlayerHistory : MonoBehaviour
{
  public static RoundPlayerHistory singleton;
  public List<RoundPlayerHistory.PlayerHistoryLog> historyLogs;

  public RoundPlayerHistory()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    RoundPlayerHistory.singleton = this;
  }

  public RoundPlayerHistory.PlayerHistoryLog GetData(int playerID)
  {
    foreach (RoundPlayerHistory.PlayerHistoryLog historyLog in this.historyLogs)
    {
      if (historyLog.PlayerID == playerID)
        return historyLog;
    }
    return (RoundPlayerHistory.PlayerHistoryLog) null;
  }

  public void SetData(int playerID, string _newNick, int _newPlyID, string _newSteamId, int _newConnectionStatus, int _newAliveClass, int _newCurrentClass, DateTime _newStartTime, DateTime _newStopTime)
  {
    int index1 = -1;
    if (playerID == -1)
    {
      this.historyLogs.Add(new RoundPlayerHistory.PlayerHistoryLog()
      {
        Nickname = "Player",
        PlayerID = 0,
        SteamID64 = string.Empty,
        ConnectionStatus = 0,
        LastAliveClass = -1,
        CurrentClass = -1,
        ConnectionStart = DateTime.Now,
        ConnectionStop = new DateTime(0, 0, 0)
      });
      index1 = this.historyLogs.Count - 1;
    }
    else
    {
      for (int index2 = 0; index2 < this.historyLogs.Count; ++index2)
      {
        if (this.historyLogs[index2].PlayerID == playerID)
          index1 = index2;
      }
    }
    if (index1 < 0)
      return;
    if (_newNick != string.Empty)
      this.historyLogs[index1].Nickname = _newNick;
    if (_newPlyID != 0)
      this.historyLogs[index1].PlayerID = _newPlyID;
    if (_newSteamId != string.Empty)
      this.historyLogs[index1].SteamID64 = _newSteamId;
    if (_newConnectionStatus != 0)
      this.historyLogs[index1].ConnectionStatus = _newConnectionStatus;
    if (_newAliveClass != 0)
      this.historyLogs[index1].LastAliveClass = _newAliveClass;
    if (_newCurrentClass != 0)
      this.historyLogs[index1].CurrentClass = _newCurrentClass;
    if (_newStartTime.Year != 0)
      this.historyLogs[index1].ConnectionStart = _newStartTime;
    if (_newStopTime.Year == 0)
      return;
    this.historyLogs[index1].ConnectionStop = _newStopTime;
  }

  [Serializable]
  public class PlayerHistoryLog
  {
    public string Nickname;
    public int PlayerID;
    public string SteamID64;
    public int ConnectionStatus;
    public int LastAliveClass;
    public int CurrentClass;
    public DateTime ConnectionStart;
    public DateTime ConnectionStop;
  }
}
