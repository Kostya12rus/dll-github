﻿// Decompiled with JetBrains decompiler
// Type: DiscordManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DiscordManager : MonoBehaviour
{
  private CharacterClassManager ccm;
  private DiscordController discordController;
  private CustomNetworkManager nm;
  public static DiscordManager singleton;
  public DiscordRpc.RichPresence menuPreset;
  public DiscordRpc.RichPresence waitingPreset;
  public DiscordRpc.RichPresence[] classPresets;

  public DiscordManager()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    DiscordManager.singleton = this;
    this.nm = (CustomNetworkManager) ((Component) this).GetComponent<CustomNetworkManager>();
    this.ccm = (CharacterClassManager) Resources.FindObjectsOfTypeAll<CharacterClassManager>()[0];
    this.discordController = (DiscordController) ((Component) this).GetComponent<DiscordController>();
    // ISSUE: method pointer
    SceneManager.add_sceneLoaded(new UnityAction<Scene, LoadSceneMode>((object) this, __methodptr(OnLevelFinishedLoading)));
  }

  public void ChangePreset(int classID)
  {
    if (classID < 0)
    {
      this.discordController.presence = classID != -1 ? this.menuPreset : this.waitingPreset;
    }
    else
    {
      try
      {
        this.discordController.presence.state = this.classPresets[classID].state;
        this.discordController.presence.largeImageKey = this.classPresets[classID].largeImageKey;
        this.discordController.presence.smallImageKey = this.classPresets[classID].smallImageKey;
      }
      catch
      {
      }
    }
    this.discordController.presence.startTimestamp = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    string str1;
    if (classID != -2 && this.nm.get_networkAddress().Contains("."))
      str1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.nm.get_networkAddress() + ":" + (object) this.nm.get_networkPort() + ":" + this.nm.CompatibleVersions[0]));
    else
      str1 = string.Empty;
    string str2 = str1;
    this.discordController.presence.joinSecret = str2;
    this.discordController.presence.partyId = "LOBBY#" + str2;
    if (str2 == string.Empty)
    {
      this.discordController.presence.partySize = 0;
      this.discordController.presence.partyMax = 0;
    }
    DiscordRpc.UpdatePresence(ref this.discordController.presence);
  }

  public void ChangeLobbyStatus(int cur, int max)
  {
    this.discordController.presence.partySize = cur;
    this.discordController.presence.partyMax = max;
    DiscordRpc.UpdatePresence(ref this.discordController.presence);
  }

  public void PrintMessage(string msg)
  {
    Debug.Log((object) msg);
  }

  private void Update()
  {
    if (!Input.GetKey((KeyCode) 306))
      return;
    if (Input.GetKeyDown((KeyCode) 121))
      this.discordController.RequestRespondYes();
    if (!Input.GetKeyDown((KeyCode) 110))
      return;
    this.discordController.RequestRespondNo();
  }

  private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
  {
    if (((Scene) ref scene).get_buildIndex() == 1)
    {
      this.discordController.presence.partySize = 0;
      this.discordController.presence.partyMax = 0;
      this.ChangePreset(-2);
    }
    if (!(((Scene) ref scene).get_name() == "Facility"))
      return;
    this.ChangePreset(-1);
  }
}
