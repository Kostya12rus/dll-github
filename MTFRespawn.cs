// Decompiled with JetBrains decompiler
// Type: MTFRespawn
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MTFRespawn : NetworkBehaviour
{
  private static int kRpcRpcPlayAnnouncement = 418834810;
  [Range(30f, 1000f)]
  public int minMtfTimeToRespawn = 200;
  [Range(40f, 1200f)]
  public int maxMtfTimeToRespawn = 400;
  public float CI_Time_Multiplier = 2f;
  public float CI_Percent = 20f;
  [Space(10f)]
  [Range(2f, 15f)]
  public int maxRespawnAmount = 15;
  public List<GameObject> playersToNTF = new List<GameObject>();
  public GameObject ciTheme;
  private ChopperAutostart mtf_a;
  private CharacterClassManager _hostCcm;
  private float decontaminationCooldown;
  public float timeToNextRespawn;
  public bool nextWaveIsCI;
  private bool loaded;
  private bool chopperStarted;
  [HideInInspector]
  public float respawnCooldown;
  private static int kRpcRpcVan;
  private static int kRpcRpcAnnouncCI;

  private void Start()
  {
    this.minMtfTimeToRespawn = ConfigFile.ServerConfig.GetInt("minimum_MTF_time_to_spawn", 200);
    this.maxMtfTimeToRespawn = ConfigFile.ServerConfig.GetInt("maximum_MTF_time_to_spawn", 400);
    this.CI_Percent = (float) ConfigFile.ServerConfig.GetInt("ci_respawn_percent", 35);
    if (!NetworkServer.active || !this.isServer || (!this.isLocalPlayer || TutorialManager.status))
      return;
    Timing.RunCoroutine(this._Update(), Segment.FixedUpdate);
  }

  public void SetDecontCooldown(float f)
  {
    this.decontaminationCooldown = f;
  }

  [DebuggerHidden]
  private IEnumerator<float> _Update()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new MTFRespawn.\u003C_Update\u003Ec__Iterator0() { \u0024this = this };
  }

  private void RespawnDeadPlayers()
  {
    int num = 0;
    List<GameObject> list = ((IEnumerable<GameObject>) PlayerManager.singleton.players).Where<GameObject>((Func<GameObject, bool>) (item =>
    {
      if (item.GetComponent<CharacterClassManager>().curClass == 2)
        return !item.GetComponent<ServerRoles>().OverwatchEnabled;
      return false;
    })).ToList<GameObject>();
    while (list.Count > this.maxRespawnAmount)
      list.RemoveAt(Random.Range(0, list.Count));
    if (this.nextWaveIsCI && AlphaWarheadController.host.detonated)
      this.nextWaveIsCI = false;
    foreach (GameObject ply in list)
    {
      if (!((Object) ply == (Object) null))
      {
        ++num;
        if (this.nextWaveIsCI)
          this.GetComponent<CharacterClassManager>().SetPlayersClass(8, ply);
        else
          this.playersToNTF.Add(ply);
      }
    }
    if (num > 0)
    {
      ServerLogs.AddLog(ServerLogs.Modules.ClassChange, (!this.nextWaveIsCI ? "MTF" : "Chaos Insurgency") + " respawned!", ServerLogs.ServerLogType.GameEvent);
      if (this.nextWaveIsCI)
        this.Invoke("CmdDelayCIAnnounc", 1f);
    }
    this.SummonNTF();
  }

  [ServerCallback]
  public void SummonNTF()
  {
    if (!NetworkServer.active || this.playersToNTF.Count <= 0)
      return;
    char letter;
    int number;
    this.SetUnit(this.playersToNTF.ToArray(), out letter, out number);
    for (int index = 0; index < this.playersToNTF.Count; ++index)
    {
      if (index == 0)
        this.GetComponent<CharacterClassManager>().SetPlayersClass(12, this.playersToNTF[index]);
      else if (index <= 3)
        this.GetComponent<CharacterClassManager>().SetPlayersClass(11, this.playersToNTF[index]);
      else
        this.GetComponent<CharacterClassManager>().SetPlayersClass(13, this.playersToNTF[index]);
    }
    this.playersToNTF.Clear();
    this.CallRpcPlayAnnouncement(letter, number, ((IEnumerable<GameObject>) PlayerManager.singleton.players).Where<GameObject>((Func<GameObject, bool>) (item => item.GetComponent<CharacterClassManager>().IsScpButNotZombie())).ToArray<GameObject>().Length);
  }

  [ClientRpc]
  private void RpcPlayAnnouncement(char natoLetter, int natoNumber, int scpsLeft)
  {
    NineTailedFoxAnnouncer.singleton.AnnounceNtfEntrance(scpsLeft, natoNumber, natoLetter);
  }

  [ServerCallback]
  private void SetUnit(GameObject[] ply, out char letter, out int number)
  {
    if (!NetworkServer.active)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref letter = 0;
      number = 0;
    }
    else
    {
      int unit = this.GetComponent<NineTailedFoxUnits>().NewName(out number, out letter);
      foreach (GameObject gameObject in ply)
        gameObject.GetComponent<CharacterClassManager>().SetUnit(unit);
    }
  }

  [ServerCallback]
  private void SummonChopper(bool state)
  {
    if (!NetworkServer.active)
      return;
    this.mtf_a.SetState(state);
  }

  [ServerCallback]
  private void SummonVan()
  {
    if (!NetworkServer.active)
      return;
    this.CallRpcVan();
  }

  [ClientRpc(channel = 2)]
  private void RpcVan()
  {
    GameObject.Find("CIVanArrive").GetComponent<Animator>().SetTrigger("Arrive");
  }

  private void CmdDelayCIAnnounc()
  {
    this.PlayAnnoncCI();
  }

  [ServerCallback]
  private void PlayAnnoncCI()
  {
    if (!NetworkServer.active)
      return;
    this.CallRpcAnnouncCI();
  }

  [ClientRpc(channel = 2)]
  private void RpcAnnouncCI()
  {
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      CharacterClassManager component = player.GetComponent<CharacterClassManager>();
      if (component.isLocalPlayer)
      {
        switch (component.klasy[component.curClass].team)
        {
          case Team.CHI:
          case Team.CDP:
            Object.Instantiate<GameObject>(this.ciTheme);
            continue;
          default:
            if (!component.GetComponent<ServerRoles>().OverwatchEnabled)
              continue;
            goto case Team.CHI;
        }
      }
    }
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeRpcRpcPlayAnnouncement(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcPlayAnnouncement called on server.");
    else
      ((MTFRespawn) obj).RpcPlayAnnouncement((char) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcRpcVan(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcVan called on server.");
    else
      ((MTFRespawn) obj).RpcVan();
  }

  protected static void InvokeRpcRpcAnnouncCI(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcAnnouncCI called on server.");
    else
      ((MTFRespawn) obj).RpcAnnouncCI();
  }

  public void CallRpcPlayAnnouncement(char natoLetter, int natoNumber, int scpsLeft)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcPlayAnnouncement called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) MTFRespawn.kRpcRpcPlayAnnouncement);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) natoLetter);
      writer.WritePackedUInt32((uint) natoNumber);
      writer.WritePackedUInt32((uint) scpsLeft);
      this.SendRPCInternal(writer, 0, "RpcPlayAnnouncement");
    }
  }

  public void CallRpcVan()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcVan called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) MTFRespawn.kRpcRpcVan);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 2, "RpcVan");
    }
  }

  public void CallRpcAnnouncCI()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcAnnouncCI called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) MTFRespawn.kRpcRpcAnnouncCI);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 2, "RpcAnnouncCI");
    }
  }

  static MTFRespawn()
  {
    NetworkBehaviour.RegisterRpcDelegate(typeof (MTFRespawn), MTFRespawn.kRpcRpcPlayAnnouncement, new NetworkBehaviour.CmdDelegate(MTFRespawn.InvokeRpcRpcPlayAnnouncement));
    MTFRespawn.kRpcRpcVan = -871850524;
    NetworkBehaviour.RegisterRpcDelegate(typeof (MTFRespawn), MTFRespawn.kRpcRpcVan, new NetworkBehaviour.CmdDelegate(MTFRespawn.InvokeRpcRpcVan));
    MTFRespawn.kRpcRpcAnnouncCI = -699664669;
    NetworkBehaviour.RegisterRpcDelegate(typeof (MTFRespawn), MTFRespawn.kRpcRpcAnnouncCI, new NetworkBehaviour.CmdDelegate(MTFRespawn.InvokeRpcRpcAnnouncCI));
    NetworkCRC.RegisterBehaviour(nameof (MTFRespawn), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
