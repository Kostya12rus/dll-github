// Decompiled with JetBrains decompiler
// Type: PlayerStats
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance.Integrations.UNet_HLAPI;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStats : NetworkBehaviour
{
  private static Lift[] _lifts = new Lift[0];
  private static int kCmdCmdSelfDeduct = -2147454163;
  public PlayerStats.HitInfo lastHitInfo = new PlayerStats.HitInfo(0.0f, "NONE", "NONE", 0);
  public Transform[] grenadePoints;
  public CharacterClassManager ccm;
  private UserMainInterface _ui;
  public int maxHP;
  public bool used914;
  private bool _pocketCleanup;
  [SyncVar(hook = "SetHPAmount")]
  public int health;
  private float killstreak_time;
  private int killstreak;
  private static int kCmdCmdTesla;
  private static int kTargetRpcTargetAchieve;
  private static int kRpcRpcAnnounceScpKill;
  private static int kTargetRpcTargetStats;
  private static int kTargetRpcTargetOofEffect;
  private static int kRpcRpcRoundrestart;

  private void Start()
  {
    this._pocketCleanup = ConfigFile.ServerConfig.GetBool("SCP106_CLEANUP", false);
    this.ccm = this.GetComponent<CharacterClassManager>();
    this._ui = UserMainInterface.singleton;
    if (PlayerStats._lifts.Length != 0)
      return;
    PlayerStats._lifts = Object.FindObjectsOfType<Lift>();
  }

  private void Update()
  {
    if (this.isLocalPlayer && this.ccm.curClass != 2)
      this._ui.SetHP(this.health, this.maxHP);
    if (!this.isLocalPlayer)
      return;
    this._ui.hpOBJ.SetActive(this.ccm.curClass != 2);
  }

  public float GetHealthPercent()
  {
    if (this.ccm.curClass < 0)
      return 0.0f;
    return Mathf.Clamp01(1f - (float) this.health / (float) this.ccm.klasy[this.ccm.curClass].maxHP);
  }

  [Command(channel = 2)]
  public void CmdSelfDeduct(PlayerStats.HitInfo info)
  {
    this.HurtPlayer(info, this.gameObject);
  }

  public bool Explode(bool inWarhead)
  {
    bool flag = this.health > 0 && (inWarhead || (double) this.transform.position.y < 900.0);
    if (this.ccm.curClass == 3)
    {
      Scp106PlayerScript component = this.GetComponent<Scp106PlayerScript>();
      component.DeletePortal();
      if (component.goingViaThePortal)
        flag = true;
    }
    if (flag)
      this.HurtPlayer(new PlayerStats.HitInfo(999999f, "WORLD", "NUKE", 0), this.gameObject);
    return flag;
  }

  [Command(channel = 2)]
  public void CmdTesla()
  {
    this.HurtPlayer(new PlayerStats.HitInfo((float) Random.Range(100, 200), this.GetComponent<HlapiPlayer>().PlayerId, "TESLA", 0), this.gameObject);
  }

  public void SetHPAmount(int hp)
  {
    this.Networkhealth = hp;
  }

  public bool HurtPlayer(PlayerStats.HitInfo info, GameObject go)
  {
    bool flag = false;
    info.amount = Mathf.Abs(info.amount);
    if ((double) info.amount > 999999.0)
      info.amount = 999999f;
    PlayerStats component1 = go.GetComponent<PlayerStats>();
    CharacterClassManager component2 = go.GetComponent<CharacterClassManager>();
    if (component2.GodMode)
      return false;
    PlayerStats playerStats = component1;
    playerStats.Networkhealth = playerStats.health - Mathf.CeilToInt(info.amount);
    component1.lastHitInfo = info;
    if (component1.health < 1 && component2.curClass != 2)
    {
      if (RoundSummary.RoundInProgress() && RoundSummary.roundTime < 60)
        this.CallTargetAchieve(component2.connectionToClient, "wowreally");
      flag = true;
      if (component2.curClass == 9 && go.GetComponent<Scp096PlayerScript>().enraged == Scp096PlayerScript.RageState.Panic)
        this.CallTargetAchieve(component2.connectionToClient, "unvoluntaryragequit");
      else if (info.tool == "POCKET")
        this.CallTargetAchieve(component2.connectionToClient, "newb");
      else if (info.tool == "SCP:173")
        this.CallTargetAchieve(component2.connectionToClient, "firsttime");
      else if ((info.tool == "GRENADE" || info.tool == "FRAG") && info.plyID == go.GetComponent<QueryProcessor>().PlayerId)
        this.CallTargetAchieve(component2.connectionToClient, "iwanttobearocket");
      else if (info.tool.ToUpper().Contains("WEAPON"))
      {
        if (component2.curClass == 6 && component2.GetComponent<Inventory>().curItem >= 0 && (component2.GetComponent<Inventory>().curItem <= 11 && this.GetComponent<CharacterClassManager>().curClass == 1))
          this.CallTargetAchieve(this.connectionToClient, "betrayal");
        if ((double) Time.realtimeSinceStartup - (double) this.killstreak_time > 30.0 || this.killstreak == 0)
        {
          this.killstreak = 0;
          this.killstreak_time = Time.realtimeSinceStartup;
        }
        if (this.GetComponent<WeaponManager>().GetShootPermission(component2, true))
          ++this.killstreak;
        if (this.killstreak > 5)
          this.CallTargetAchieve(this.connectionToClient, "pewpew");
        if ((this.ccm.klasy[this.ccm.curClass].team == Team.MTF || this.ccm.klasy[this.ccm.curClass].team == Team.RSC) && component2.curClass == 1)
          this.CallTargetStats(this.connectionToClient, "dboys_killed", "justresources", 50);
        if (this.ccm.klasy[this.ccm.curClass].team == Team.RSC && this.ccm.klasy[component2.curClass].team == Team.SCP)
          this.CallTargetAchieve(this.connectionToClient, "timetodoitmyself");
      }
      ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + go.GetComponent<NicknameSync>().myNick + " (" + go.GetComponent<CharacterClassManager>().SteamId + ") killed by " + info.attacker + " using " + info.tool + ".", ServerLogs.ServerLogType.KillLog);
      if (!this._pocketCleanup || info.tool != "POCKET")
      {
        go.GetComponent<Inventory>().ServerDropAll();
        if (component2.curClass >= 0 && info.tool != "RAGDOLL-LESS")
          this.GetComponent<RagdollManager>().SpawnRagdoll(go.transform.position, go.transform.rotation, component2.curClass, info, component2.klasy[component2.curClass].team != Team.SCP, go.GetComponent<HlapiPlayer>().PlayerId, go.GetComponent<NicknameSync>().myNick);
      }
      else
        go.GetComponent<Inventory>().Clear();
      component2.NetworkdeathPosition = go.transform.position;
      if (component2.curClass != 10 && component2.klasy[component2.curClass].team == Team.SCP)
      {
        GameObject exec = (GameObject) null;
        foreach (GameObject player in PlayerManager.singleton.players)
        {
          if (player.GetComponent<QueryProcessor>().PlayerId == info.plyID)
            exec = player;
        }
        this.CallRpcAnnounceScpKill(component2.klasy[component2.curClass].fullName, exec);
      }
      component1.SetHPAmount(100);
      component2.SetClassID(2);
      if (TutorialManager.status)
        PlayerManager.localPlayer.GetComponent<TutorialManager>().KillNPC();
    }
    else
    {
      Vector3 pos = Vector3.zero;
      float num = 40f;
      if (info.tool.StartsWith("Weapon:"))
      {
        GameObject playerOfId = this.GetPlayerOfID(info.plyID);
        if ((Object) playerOfId != (Object) null)
        {
          pos = go.transform.InverseTransformPoint(playerOfId.transform.position).normalized;
          num = 100f;
        }
      }
      if (component2.klasy[component2.curClass].fullName.Contains("939"))
        component2.GetComponent<Scp939PlayerScript>().NetworkspeedMultiplier = 1.25f;
      this.CallTargetOofEffect(go.GetComponent<NetworkIdentity>().connectionToClient, pos, Mathf.Clamp01(info.amount / num));
    }
    return flag;
  }

  [TargetRpc]
  public void TargetAchieve(NetworkConnection conn, string key)
  {
    AchievementManager.Achieve(key);
  }

  [ClientRpc]
  private void RpcAnnounceScpKill(string scpnum, GameObject exec)
  {
    Debug.Log((object) (scpnum + ((Object) exec == (Object) null).ToString()));
    NineTailedFoxAnnouncer.singleton.AnnounceScpKill(scpnum, !((Object) exec == (Object) null) ? exec.GetComponent<CharacterClassManager>() : (CharacterClassManager) null);
  }

  [TargetRpc]
  public void TargetStats(NetworkConnection conn, string key, string targetAchievement, int maxValue)
  {
    AchievementManager.StatsProgress(key, targetAchievement, maxValue);
  }

  private GameObject GetPlayerOfID(int id)
  {
    return ((IEnumerable<GameObject>) PlayerManager.singleton.players).FirstOrDefault<GameObject>((Func<GameObject, bool>) (ply => ply.GetComponent<QueryProcessor>().PlayerId == id));
  }

  [TargetRpc]
  private void TargetOofEffect(NetworkConnection conn, Vector3 pos, float overall)
  {
    OOF_Controller.singleton.AddBlood(pos, overall);
  }

  [ClientRpc(channel = 7)]
  private void RpcRoundrestart()
  {
    if (this.isServer)
      return;
    Object.FindObjectOfType<CustomNetworkManager>().reconnect = true;
    this.Invoke("ChangeLevel", 0.5f);
  }

  public void Roundrestart()
  {
    this.CallRpcRoundrestart();
    this.Invoke("ChangeLevel", 2.5f);
  }

  private void ChangeLevel()
  {
    if (this.isServer)
    {
      GC.Collect();
      NetworkManager.singleton.ServerChangeScene(NetworkManager.singleton.onlineScene);
    }
    else
      NetworkManager.singleton.StopClient();
  }

  public string HealthToString()
  {
    return this.health.ToString() + "/" + (object) this.maxHP + "(" + (object) ((double) this.health / (double) this.maxHP * 100.0) + "%)";
  }

  static PlayerStats()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerStats), PlayerStats.kCmdCmdSelfDeduct, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeCmdCmdSelfDeduct));
    PlayerStats.kCmdCmdTesla = -1109720487;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerStats), PlayerStats.kCmdCmdTesla, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeCmdCmdTesla));
    PlayerStats.kRpcRpcAnnounceScpKill = 530564897;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kRpcRpcAnnounceScpKill, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeRpcRpcAnnounceScpKill));
    PlayerStats.kRpcRpcRoundrestart = 907411477;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kRpcRpcRoundrestart, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeRpcRpcRoundrestart));
    PlayerStats.kTargetRpcTargetAchieve = 1310991230;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kTargetRpcTargetAchieve, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeRpcTargetAchieve));
    PlayerStats.kTargetRpcTargetStats = 662062348;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kTargetRpcTargetStats, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeRpcTargetStats));
    PlayerStats.kTargetRpcTargetOofEffect = -1463723612;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kTargetRpcTargetOofEffect, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeRpcTargetOofEffect));
    NetworkCRC.RegisterBehaviour(nameof (PlayerStats), 0);
  }

  private void UNetVersion()
  {
  }

  public int Networkhealth
  {
    get
    {
      return this.health;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.health;
      int num2 = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetHPAmount(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<int>(num1, ref local, (uint) num2);
    }
  }

  protected static void InvokeCmdCmdSelfDeduct(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSelfDeduct called on client.");
    else
      ((PlayerStats) obj).CmdSelfDeduct(GeneratedNetworkCode._ReadHitInfo_PlayerStats(reader));
  }

  protected static void InvokeCmdCmdTesla(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdTesla called on client.");
    else
      ((PlayerStats) obj).CmdTesla();
  }

  public void CallCmdSelfDeduct(PlayerStats.HitInfo info)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSelfDeduct called on server.");
    else if (this.isServer)
    {
      this.CmdSelfDeduct(info);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerStats.kCmdCmdSelfDeduct);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      GeneratedNetworkCode._WriteHitInfo_PlayerStats(writer, info);
      this.SendCommandInternal(writer, 2, "CmdSelfDeduct");
    }
  }

  public void CallCmdTesla()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdTesla called on server.");
    else if (this.isServer)
    {
      this.CmdTesla();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerStats.kCmdCmdTesla);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 2, "CmdTesla");
    }
  }

  protected static void InvokeRpcRpcAnnounceScpKill(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcAnnounceScpKill called on server.");
    else
      ((PlayerStats) obj).RpcAnnounceScpKill(reader.ReadString(), reader.ReadGameObject());
  }

  protected static void InvokeRpcRpcRoundrestart(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcRoundrestart called on server.");
    else
      ((PlayerStats) obj).RpcRoundrestart();
  }

  protected static void InvokeRpcTargetAchieve(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetAchieve called on server.");
    else
      ((PlayerStats) obj).TargetAchieve(ClientScene.readyConnection, reader.ReadString());
  }

  protected static void InvokeRpcTargetStats(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetStats called on server.");
    else
      ((PlayerStats) obj).TargetStats(ClientScene.readyConnection, reader.ReadString(), reader.ReadString(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcTargetOofEffect(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetOofEffect called on server.");
    else
      ((PlayerStats) obj).TargetOofEffect(ClientScene.readyConnection, reader.ReadVector3(), reader.ReadSingle());
  }

  public void CallRpcAnnounceScpKill(string scpnum, GameObject exec)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcAnnounceScpKill called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerStats.kRpcRpcAnnounceScpKill);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(scpnum);
      writer.Write(exec);
      this.SendRPCInternal(writer, 0, "RpcAnnounceScpKill");
    }
  }

  public void CallRpcRoundrestart()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcRoundrestart called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerStats.kRpcRpcRoundrestart);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 7, "RpcRoundrestart");
    }
  }

  public void CallTargetAchieve(NetworkConnection conn, string key)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetAchieve called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetAchieve called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerStats.kTargetRpcTargetAchieve);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(key);
      this.SendTargetRPCInternal(conn, writer, 0, "TargetAchieve");
    }
  }

  public void CallTargetStats(NetworkConnection conn, string key, string targetAchievement, int maxValue)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetStats called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetStats called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerStats.kTargetRpcTargetStats);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(key);
      writer.Write(targetAchievement);
      writer.WritePackedUInt32((uint) maxValue);
      this.SendTargetRPCInternal(conn, writer, 0, "TargetStats");
    }
  }

  public void CallTargetOofEffect(NetworkConnection conn, Vector3 pos, float overall)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetOofEffect called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetOofEffect called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerStats.kTargetRpcTargetOofEffect);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(pos);
      writer.Write(overall);
      this.SendTargetRPCInternal(conn, writer, 0, "TargetOofEffect");
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.health);
      return true;
    }
    bool flag = false;
    if (((int) this.syncVarDirtyBits & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.health);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.health = (int) reader.ReadPackedUInt32();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetHPAmount((int) reader.ReadPackedUInt32());
    }
  }

  [Serializable]
  public struct HitInfo
  {
    public float amount;
    public string tool;
    public int time;
    public string attacker;
    public int plyID;

    public HitInfo(float amnt, string attackerName, string weapon, int attackerID)
    {
      this.amount = amnt;
      this.tool = weapon;
      this.attacker = attackerName;
      this.plyID = attackerID;
      this.time = ServerTime.time;
    }

    public GameObject GetPlayerObject()
    {
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        if (player.GetComponent<QueryProcessor>().PlayerId == this.plyID)
          return player;
      }
      return (GameObject) null;
    }
  }
}
