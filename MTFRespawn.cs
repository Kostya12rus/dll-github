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
  public GameObject ciTheme;
  private ChopperAutostart mtf_a;
  private CharacterClassManager _hostCcm;
  [Range(30f, 1000f)]
  public int minMtfTimeToRespawn;
  [Range(40f, 1200f)]
  public int maxMtfTimeToRespawn;
  public float CI_Time_Multiplier;
  public float CI_Percent;
  private float decontaminationCooldown;
  [Space(10f)]
  [Range(2f, 15f)]
  public int maxRespawnAmount;
  public float timeToNextRespawn;
  public bool nextWaveIsCI;
  public List<GameObject> playersToNTF;
  private bool loaded;
  private bool chopperStarted;
  [HideInInspector]
  public float respawnCooldown;
  private static int kRpcRpcVan;
  private static int kRpcRpcAnnouncCI;

  public MTFRespawn()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.minMtfTimeToRespawn = ConfigFile.ServerConfig.GetInt("minimum_MTF_time_to_spawn", 200);
    this.maxMtfTimeToRespawn = ConfigFile.ServerConfig.GetInt("maximum_MTF_time_to_spawn", 400);
    this.CI_Percent = (float) ConfigFile.ServerConfig.GetInt("ci_respawn_percent", 35);
    if (!NetworkServer.get_active() || !this.get_isServer() || (!this.get_isLocalPlayer() || TutorialManager.status))
      return;
    Timing.RunCoroutine(this._Update(), (Segment) 1);
  }

  public void SetDecontCooldown(float f)
  {
    this.decontaminationCooldown = f;
  }

  [DebuggerHidden]
  private IEnumerator<float> _Update()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new MTFRespawn.\u003C_Update\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void RespawnDeadPlayers()
  {
    int num = 0;
    List<GameObject> list = ((IEnumerable<GameObject>) PlayerManager.singleton.players).Where<GameObject>((Func<GameObject, bool>) (item =>
    {
      if (((CharacterClassManager) item.GetComponent<CharacterClassManager>()).curClass == 2)
        return !((ServerRoles) item.GetComponent<ServerRoles>()).OverwatchEnabled;
      return false;
    })).ToList<GameObject>();
    while (list.Count > this.maxRespawnAmount)
      list.RemoveAt(Random.Range(0, list.Count));
    if (this.nextWaveIsCI && AlphaWarheadController.host.detonated)
      this.nextWaveIsCI = false;
    using (List<GameObject>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        GameObject current = enumerator.Current;
        if (!Object.op_Equality((Object) current, (Object) null))
        {
          ++num;
          if (this.nextWaveIsCI)
            ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SetPlayersClass(8, current);
          else
            this.playersToNTF.Add(current);
        }
      }
    }
    if (num > 0)
    {
      ServerLogs.AddLog(ServerLogs.Modules.ClassChange, (!this.nextWaveIsCI ? "MTF" : "Chaos Insurgency") + " respawned!", ServerLogs.ServerLogType.GameEvent);
      if (this.nextWaveIsCI)
        ((MonoBehaviour) this).Invoke("CmdDelayCIAnnounc", 1f);
    }
    this.SummonNTF();
  }

  [ServerCallback]
  public void SummonNTF()
  {
    if (!NetworkServer.get_active() || this.playersToNTF.Count <= 0)
      return;
    char letter;
    int number;
    this.SetUnit(this.playersToNTF.ToArray(), out letter, out number);
    for (int index = 0; index < this.playersToNTF.Count; ++index)
    {
      if (index == 0)
        ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SetPlayersClass(12, this.playersToNTF[index]);
      else if (index <= 3)
        ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SetPlayersClass(11, this.playersToNTF[index]);
      else
        ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SetPlayersClass(13, this.playersToNTF[index]);
    }
    this.playersToNTF.Clear();
    this.CallRpcPlayAnnouncement(letter, number, ((IEnumerable<GameObject>) PlayerManager.singleton.players).Where<GameObject>((Func<GameObject, bool>) (item => ((CharacterClassManager) item.GetComponent<CharacterClassManager>()).IsScpButNotZombie())).ToArray<GameObject>().Length);
  }

  [ClientRpc]
  private void RpcPlayAnnouncement(char natoLetter, int natoNumber, int scpsLeft)
  {
    NineTailedFoxAnnouncer.singleton.AnnounceNtfEntrance(scpsLeft, natoNumber, natoLetter);
  }

  [ServerCallback]
  private void SetUnit(GameObject[] ply, out char letter, out int number)
  {
    if (!NetworkServer.get_active())
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref letter = 0;
      number = 0;
    }
    else
    {
      int unit = ((NineTailedFoxUnits) ((Component) this).GetComponent<NineTailedFoxUnits>()).NewName(out number, out letter);
      foreach (GameObject gameObject in ply)
        ((CharacterClassManager) gameObject.GetComponent<CharacterClassManager>()).SetUnit(unit);
    }
  }

  [ServerCallback]
  private void SummonChopper(bool state)
  {
    if (!NetworkServer.get_active())
      return;
    this.mtf_a.SetState(state);
  }

  [ServerCallback]
  private void SummonVan()
  {
    if (!NetworkServer.get_active())
      return;
    this.CallRpcVan();
  }

  [ClientRpc(channel = 2)]
  private void RpcVan()
  {
    ((Animator) GameObject.Find("CIVanArrive").GetComponent<Animator>()).SetTrigger("Arrive");
  }

  private void CmdDelayCIAnnounc()
  {
    this.PlayAnnoncCI();
  }

  [ServerCallback]
  private void PlayAnnoncCI()
  {
    if (!NetworkServer.get_active())
      return;
    this.CallRpcAnnouncCI();
  }

  [ClientRpc(channel = 2)]
  private void RpcAnnouncCI()
  {
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      CharacterClassManager component = (CharacterClassManager) player.GetComponent<CharacterClassManager>();
      if (component.get_isLocalPlayer())
      {
        switch (component.klasy[component.curClass].team)
        {
          case Team.CHI:
          case Team.CDP:
            Object.Instantiate<GameObject>((M0) this.ciTheme);
            continue;
          default:
            if (!((ServerRoles) ((Component) component).GetComponent<ServerRoles>()).OverwatchEnabled)
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
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcPlayAnnouncement called on server.");
    else
      ((MTFRespawn) obj).RpcPlayAnnouncement((char) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcRpcVan(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcVan called on server.");
    else
      ((MTFRespawn) obj).RpcVan();
  }

  protected static void InvokeRpcRpcAnnouncCI(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcAnnouncCI called on server.");
    else
      ((MTFRespawn) obj).RpcAnnouncCI();
  }

  public void CallRpcPlayAnnouncement(char natoLetter, int natoNumber, int scpsLeft)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcPlayAnnouncement called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) MTFRespawn.kRpcRpcPlayAnnouncement);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) natoLetter);
      networkWriter.WritePackedUInt32((uint) natoNumber);
      networkWriter.WritePackedUInt32((uint) scpsLeft);
      this.SendRPCInternal(networkWriter, 0, "RpcPlayAnnouncement");
    }
  }

  public void CallRpcVan()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcVan called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) MTFRespawn.kRpcRpcVan);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 2, "RpcVan");
    }
  }

  public void CallRpcAnnouncCI()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcAnnouncCI called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) MTFRespawn.kRpcRpcAnnouncCI);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 2, "RpcAnnouncCI");
    }
  }

  static MTFRespawn()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (MTFRespawn), MTFRespawn.kRpcRpcPlayAnnouncement, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcPlayAnnouncement)));
    MTFRespawn.kRpcRpcVan = -871850524;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (MTFRespawn), MTFRespawn.kRpcRpcVan, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcVan)));
    MTFRespawn.kRpcRpcAnnouncCI = -699664669;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (MTFRespawn), MTFRespawn.kRpcRpcAnnouncCI, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcAnnouncCI)));
    NetworkCRC.RegisterBehaviour(nameof (MTFRespawn), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
