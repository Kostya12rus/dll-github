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
  public PlayerStats.HitInfo lastHitInfo;
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

  public PlayerStats()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this._pocketCleanup = ConfigFile.ServerConfig.GetBool("SCP106_CLEANUP", false);
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this._ui = UserMainInterface.singleton;
    if (PlayerStats._lifts.Length != 0)
      return;
    PlayerStats._lifts = (Lift[]) Object.FindObjectsOfType<Lift>();
  }

  private void Update()
  {
    if (this.get_isLocalPlayer() && this.ccm.curClass != 2)
      this._ui.SetHP(this.health, this.maxHP);
    if (!this.get_isLocalPlayer())
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
    this.HurtPlayer(info, ((Component) this).get_gameObject());
  }

  public bool Explode(bool inWarhead)
  {
    bool flag = this.health > 0 && (inWarhead || ((Component) this).get_transform().get_position().y < 900.0);
    if (this.ccm.curClass == 3)
    {
      Scp106PlayerScript component = (Scp106PlayerScript) ((Component) this).GetComponent<Scp106PlayerScript>();
      component.DeletePortal();
      if (component.goingViaThePortal)
        flag = true;
    }
    if (flag)
      this.HurtPlayer(new PlayerStats.HitInfo(999999f, "WORLD", "NUKE", 0), ((Component) this).get_gameObject());
    return flag;
  }

  [Command(channel = 2)]
  public void CmdTesla()
  {
    this.HurtPlayer(new PlayerStats.HitInfo((float) Random.Range(100, 200), ((HlapiPlayer) ((Component) this).GetComponent<HlapiPlayer>()).PlayerId, "TESLA", 0), ((Component) this).get_gameObject());
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
    PlayerStats component1 = (PlayerStats) go.GetComponent<PlayerStats>();
    CharacterClassManager component2 = (CharacterClassManager) go.GetComponent<CharacterClassManager>();
    if (component2.GodMode)
      return false;
    PlayerStats playerStats = component1;
    playerStats.Networkhealth = playerStats.health - Mathf.CeilToInt(info.amount);
    component1.lastHitInfo = info;
    if (component1.health < 1 && component2.curClass != 2)
    {
      if (RoundSummary.RoundInProgress() && RoundSummary.roundTime < 60)
        this.CallTargetAchieve(component2.get_connectionToClient(), "wowreally");
      flag = true;
      if (component2.curClass == 9 && ((Scp096PlayerScript) go.GetComponent<Scp096PlayerScript>()).enraged == Scp096PlayerScript.RageState.Panic)
        this.CallTargetAchieve(component2.get_connectionToClient(), "unvoluntaryragequit");
      else if (info.tool == "POCKET")
        this.CallTargetAchieve(component2.get_connectionToClient(), "newb");
      else if (info.tool == "SCP:173")
        this.CallTargetAchieve(component2.get_connectionToClient(), "firsttime");
      else if ((info.tool == "GRENADE" || info.tool == "FRAG") && info.plyID == ((QueryProcessor) go.GetComponent<QueryProcessor>()).PlayerId)
        this.CallTargetAchieve(component2.get_connectionToClient(), "iwanttobearocket");
      else if (info.tool.ToUpper().Contains("WEAPON"))
      {
        if (component2.curClass == 6 && ((Inventory) ((Component) component2).GetComponent<Inventory>()).curItem >= 0 && (((Inventory) ((Component) component2).GetComponent<Inventory>()).curItem <= 11 && ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).curClass == 1))
          this.CallTargetAchieve(this.get_connectionToClient(), "betrayal");
        if ((double) Time.get_realtimeSinceStartup() - (double) this.killstreak_time > 30.0 || this.killstreak == 0)
        {
          this.killstreak = 0;
          this.killstreak_time = Time.get_realtimeSinceStartup();
        }
        if (((WeaponManager) ((Component) this).GetComponent<WeaponManager>()).GetShootPermission(component2, true))
          ++this.killstreak;
        if (this.killstreak > 5)
          this.CallTargetAchieve(this.get_connectionToClient(), "pewpew");
        if ((this.ccm.klasy[this.ccm.curClass].team == Team.MTF || this.ccm.klasy[this.ccm.curClass].team == Team.RSC) && component2.curClass == 1)
          this.CallTargetStats(this.get_connectionToClient(), "dboys_killed", "justresources", 50);
        if (this.ccm.klasy[this.ccm.curClass].team == Team.RSC && this.ccm.klasy[component2.curClass].team == Team.SCP)
          this.CallTargetAchieve(this.get_connectionToClient(), "timetodoitmyself");
      }
      ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + ((NicknameSync) go.GetComponent<NicknameSync>()).myNick + " (" + ((CharacterClassManager) go.GetComponent<CharacterClassManager>()).SteamId + ") killed by " + info.attacker + " using " + info.tool + ".", ServerLogs.ServerLogType.KillLog);
      if (!this._pocketCleanup || info.tool != "POCKET")
      {
        ((Inventory) go.GetComponent<Inventory>()).ServerDropAll();
        if (component2.curClass >= 0 && info.tool != "RAGDOLL-LESS")
          ((RagdollManager) ((Component) this).GetComponent<RagdollManager>()).SpawnRagdoll(go.get_transform().get_position(), go.get_transform().get_rotation(), component2.curClass, info, component2.klasy[component2.curClass].team != Team.SCP, ((HlapiPlayer) go.GetComponent<HlapiPlayer>()).PlayerId, ((NicknameSync) go.GetComponent<NicknameSync>()).myNick);
      }
      else
        ((Inventory) go.GetComponent<Inventory>()).Clear();
      component2.NetworkdeathPosition = go.get_transform().get_position();
      if (component2.curClass != 10 && component2.klasy[component2.curClass].team == Team.SCP)
      {
        GameObject exec = (GameObject) null;
        foreach (GameObject player in PlayerManager.singleton.players)
        {
          if (((QueryProcessor) player.GetComponent<QueryProcessor>()).PlayerId == info.plyID)
            exec = player;
        }
        this.CallRpcAnnounceScpKill(component2.klasy[component2.curClass].fullName, exec);
      }
      component1.SetHPAmount(100);
      component2.SetClassID(2);
      if (TutorialManager.status)
        ((TutorialManager) PlayerManager.localPlayer.GetComponent<TutorialManager>()).KillNPC();
    }
    else
    {
      Vector3 pos = Vector3.get_zero();
      float num = 40f;
      if (info.tool.StartsWith("Weapon:"))
      {
        GameObject playerOfId = this.GetPlayerOfID(info.plyID);
        if (Object.op_Inequality((Object) playerOfId, (Object) null))
        {
          Vector3 vector3 = go.get_transform().InverseTransformPoint(playerOfId.get_transform().get_position());
          pos = ((Vector3) ref vector3).get_normalized();
          num = 100f;
        }
      }
      if (component2.klasy[component2.curClass].fullName.Contains("939"))
        ((Scp939PlayerScript) ((Component) component2).GetComponent<Scp939PlayerScript>()).NetworkspeedMultiplier = 1.25f;
      this.CallTargetOofEffect(((NetworkIdentity) go.GetComponent<NetworkIdentity>()).get_connectionToClient(), pos, Mathf.Clamp01(info.amount / num));
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
    Debug.Log((object) (scpnum + Object.op_Equality((Object) exec, (Object) null).ToString()));
    NineTailedFoxAnnouncer.singleton.AnnounceScpKill(scpnum, !Object.op_Equality((Object) exec, (Object) null) ? (CharacterClassManager) exec.GetComponent<CharacterClassManager>() : (CharacterClassManager) null);
  }

  [TargetRpc]
  public void TargetStats(NetworkConnection conn, string key, string targetAchievement, int maxValue)
  {
    AchievementManager.StatsProgress(key, targetAchievement, maxValue);
  }

  private GameObject GetPlayerOfID(int id)
  {
    return ((IEnumerable<GameObject>) PlayerManager.singleton.players).FirstOrDefault<GameObject>((Func<GameObject, bool>) (ply => ((QueryProcessor) ply.GetComponent<QueryProcessor>()).PlayerId == id));
  }

  [TargetRpc]
  private void TargetOofEffect(NetworkConnection conn, Vector3 pos, float overall)
  {
    OOF_Controller.singleton.AddBlood(pos, overall);
  }

  [ClientRpc(channel = 7)]
  private void RpcRoundrestart()
  {
    if (this.get_isServer())
      return;
    ((CustomNetworkManager) Object.FindObjectOfType<CustomNetworkManager>()).reconnect = true;
    ((MonoBehaviour) this).Invoke("ChangeLevel", 0.5f);
  }

  public void Roundrestart()
  {
    this.CallRpcRoundrestart();
    ((MonoBehaviour) this).Invoke("ChangeLevel", 2.5f);
  }

  private void ChangeLevel()
  {
    if (this.get_isServer())
    {
      GC.Collect();
      ((NetworkManager) NetworkManager.singleton).ServerChangeScene(((NetworkManager) NetworkManager.singleton).get_onlineScene());
    }
    else
      ((NetworkManager) NetworkManager.singleton).StopClient();
  }

  public string HealthToString()
  {
    return this.health.ToString() + "/" + (object) this.maxHP + "(" + (object) ((double) this.health / (double) this.maxHP * 100.0) + "%)";
  }

  static PlayerStats()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerStats), PlayerStats.kCmdCmdSelfDeduct, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSelfDeduct)));
    PlayerStats.kCmdCmdTesla = -1109720487;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerStats), PlayerStats.kCmdCmdTesla, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdTesla)));
    PlayerStats.kRpcRpcAnnounceScpKill = 530564897;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kRpcRpcAnnounceScpKill, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcAnnounceScpKill)));
    PlayerStats.kRpcRpcRoundrestart = 907411477;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kRpcRpcRoundrestart, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcRoundrestart)));
    PlayerStats.kTargetRpcTargetAchieve = 1310991230;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kTargetRpcTargetAchieve, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetAchieve)));
    PlayerStats.kTargetRpcTargetStats = 662062348;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kTargetRpcTargetStats, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetStats)));
    PlayerStats.kTargetRpcTargetOofEffect = -1463723612;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerStats), PlayerStats.kTargetRpcTargetOofEffect, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetOofEffect)));
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetHPAmount(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  protected static void InvokeCmdCmdSelfDeduct(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSelfDeduct called on client.");
    else
      ((PlayerStats) obj).CmdSelfDeduct(GeneratedNetworkCode._ReadHitInfo_PlayerStats(reader));
  }

  protected static void InvokeCmdCmdTesla(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdTesla called on client.");
    else
      ((PlayerStats) obj).CmdTesla();
  }

  public void CallCmdSelfDeduct(PlayerStats.HitInfo info)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSelfDeduct called on server.");
    else if (this.get_isServer())
    {
      this.CmdSelfDeduct(info);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerStats.kCmdCmdSelfDeduct);
      writer.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      GeneratedNetworkCode._WriteHitInfo_PlayerStats(writer, info);
      this.SendCommandInternal(writer, 2, "CmdSelfDeduct");
    }
  }

  public void CallCmdTesla()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdTesla called on server.");
    else if (this.get_isServer())
    {
      this.CmdTesla();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerStats.kCmdCmdTesla);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdTesla");
    }
  }

  protected static void InvokeRpcRpcAnnounceScpKill(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcAnnounceScpKill called on server.");
    else
      ((PlayerStats) obj).RpcAnnounceScpKill(reader.ReadString(), (GameObject) reader.ReadGameObject());
  }

  protected static void InvokeRpcRpcRoundrestart(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcRoundrestart called on server.");
    else
      ((PlayerStats) obj).RpcRoundrestart();
  }

  protected static void InvokeRpcTargetAchieve(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetAchieve called on server.");
    else
      ((PlayerStats) obj).TargetAchieve(ClientScene.get_readyConnection(), reader.ReadString());
  }

  protected static void InvokeRpcTargetStats(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetStats called on server.");
    else
      ((PlayerStats) obj).TargetStats(ClientScene.get_readyConnection(), reader.ReadString(), reader.ReadString(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcTargetOofEffect(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetOofEffect called on server.");
    else
      ((PlayerStats) obj).TargetOofEffect(ClientScene.get_readyConnection(), (Vector3) reader.ReadVector3(), reader.ReadSingle());
  }

  public void CallRpcAnnounceScpKill(string scpnum, GameObject exec)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcAnnounceScpKill called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerStats.kRpcRpcAnnounceScpKill);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(scpnum);
      networkWriter.Write((GameObject) exec);
      this.SendRPCInternal(networkWriter, 0, "RpcAnnounceScpKill");
    }
  }

  public void CallRpcRoundrestart()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcRoundrestart called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerStats.kRpcRpcRoundrestart);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 7, "RpcRoundrestart");
    }
  }

  public void CallTargetAchieve(NetworkConnection conn, string key)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetAchieve called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetAchieve called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerStats.kTargetRpcTargetAchieve);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(key);
      this.SendTargetRPCInternal(conn, networkWriter, 0, "TargetAchieve");
    }
  }

  public void CallTargetStats(NetworkConnection conn, string key, string targetAchievement, int maxValue)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetStats called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetStats called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerStats.kTargetRpcTargetStats);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(key);
      networkWriter.Write(targetAchievement);
      networkWriter.WritePackedUInt32((uint) maxValue);
      this.SendTargetRPCInternal(conn, networkWriter, 0, "TargetStats");
    }
  }

  public void CallTargetOofEffect(NetworkConnection conn, Vector3 pos, float overall)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetOofEffect called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetOofEffect called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerStats.kTargetRpcTargetOofEffect);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((Vector3) pos);
      networkWriter.Write(overall);
      this.SendTargetRPCInternal(conn, networkWriter, 0, "TargetOofEffect");
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.health);
      return true;
    }
    bool flag = false;
    if (((int) this.get_syncVarDirtyBits() & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.health);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
        if (((QueryProcessor) player.GetComponent<QueryProcessor>()).PlayerId == this.plyID)
          return player;
      }
      return (GameObject) null;
    }
  }
}
