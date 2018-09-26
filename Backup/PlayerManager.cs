// Decompiled with JetBrains decompiler
// Type: PlayerManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using RemoteAdmin;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
  public GameObject[] players;
  public static PlayerManager singleton;
  public static int playerID;
  public static GameObject localPlayer;
  public static SpectatorManager spect;

  public PlayerManager()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    PlayerManager.singleton = this;
  }

  public void AddPlayer(GameObject player)
  {
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (GameObject player1 in this.players)
      gameObjectList.Add(player1);
    if (!gameObjectList.Contains(player))
      gameObjectList.Add(player);
    this.players = gameObjectList.ToArray();
    DiscordManager.singleton.ChangeLobbyStatus(this.players.Length, PlayButton.maxPlayers);
    PlayerList.AddPlayer(player);
    if (Object.op_Inequality((Object) PlayerManager.spect, (Object) null))
      PlayerManager.spect.RefreshList();
    QueryProcessor.StaticRefreshPlayerList();
  }

  public void RemovePlayer(GameObject player)
  {
    PlayerList.DestroyPlayer(player);
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (GameObject player1 in this.players)
      gameObjectList.Add(player1);
    if (gameObjectList.Contains(player))
      gameObjectList.Remove(player);
    this.players = gameObjectList.ToArray();
    DiscordManager.singleton.ChangeLobbyStatus(this.players.Length, PlayButton.maxPlayers);
    if (Object.op_Inequality((Object) PlayerManager.spect, (Object) null))
      PlayerManager.spect.RefreshList();
    QueryProcessor.StaticRefreshPlayerList();
  }
}
