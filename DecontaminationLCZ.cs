// Decompiled with JetBrains decompiler
// Type: DecontaminationLCZ
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

public class DecontaminationLCZ : NetworkBehaviour
{
  private static int kRpcRpcPlayAnnouncement = -1569315677;
  public List<DecontaminationLCZ.Announcement> announcements = new List<DecontaminationLCZ.Announcement>();
  public float time;
  private bool smDisableDecontamination;
  private MTFRespawn mtfrespawn;
  private AlphaWarheadController alphaController;
  private PlayerStats ps;
  private CharacterClassManager ccm;
  private int curAnm;

  private void Start()
  {
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.ps = this.GetComponent<PlayerStats>();
    this.mtfrespawn = this.GetComponent<MTFRespawn>();
    this.alphaController = this.GetComponent<AlphaWarheadController>();
    this.smDisableDecontamination = ConfigFile.ServerConfig.GetBool("disable_decontamination", false);
  }

  private void Update()
  {
    if (this.smDisableDecontamination || !this.isLocalPlayer || !(this.name == "Host"))
      return;
    this.DoServersideStuff();
  }

  [DebuggerHidden]
  private IEnumerator<float> _KillPlayersInLCZ()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new DecontaminationLCZ.\u003C_KillPlayersInLCZ\u003Ec__Iterator0() { \u0024this = this };
  }

  [ServerCallback]
  private void DoServersideStuff()
  {
    if (!NetworkServer.active || this.curAnm >= this.announcements.Count || (this.alphaController.inProgress || !this.ccm.roundStarted) || (double) this.mtfrespawn.respawnCooldown > 0.0)
      return;
    this.time += Time.deltaTime;
    if ((double) this.time / 60.0 <= (double) this.announcements[this.curAnm].startTime)
      return;
    this.CallRpcPlayAnnouncement(this.curAnm, this.GetOption("global", this.curAnm));
    AlphaWarheadController alphaController = this.alphaController;
    alphaController.NetworktimeToDetonation = alphaController.timeToDetonation + (this.announcements[this.curAnm].clip.length + 10f);
    PlayerManager.localPlayer.GetComponent<MTFRespawn>().SetDecontCooldown(this.announcements[this.curAnm].clip.length + 10f);
    if (this.GetOption("checkpoints", this.curAnm))
      this.Invoke("CallOpenDoors", 10f);
    ++this.curAnm;
  }

  private bool GetOption(string optionName, int curAnm)
  {
    string options = this.announcements[curAnm].options;
    char[] chArray = new char[1]{ ',' };
    foreach (string str in options.Split(chArray))
    {
      if (str == optionName)
        return true;
    }
    return false;
  }

  private void CallOpenDoors()
  {
    DecontaminationSpeaker.OpenDoors();
  }

  [ClientRpc]
  private void RpcPlayAnnouncement(int id, bool global)
  {
    DecontaminationSpeaker.PlaySound(this.announcements[id].clip, global);
    if (!this.GetOption("decontstart", id))
      return;
    if (NetworkServer.active)
      Timing.RunCoroutine(this._KillPlayersInLCZ(), Segment.Update);
    DecontaminationGas.TurnOn();
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeRpcRpcPlayAnnouncement(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcPlayAnnouncement called on server.");
    else
      ((DecontaminationLCZ) obj).RpcPlayAnnouncement((int) reader.ReadPackedUInt32(), reader.ReadBoolean());
  }

  public void CallRpcPlayAnnouncement(int id, bool global)
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
      writer.WritePackedUInt32((uint) DecontaminationLCZ.kRpcRpcPlayAnnouncement);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) id);
      writer.Write(global);
      this.SendRPCInternal(writer, 0, "RpcPlayAnnouncement");
    }
  }

  static DecontaminationLCZ()
  {
    NetworkBehaviour.RegisterRpcDelegate(typeof (DecontaminationLCZ), DecontaminationLCZ.kRpcRpcPlayAnnouncement, new NetworkBehaviour.CmdDelegate(DecontaminationLCZ.InvokeRpcRpcPlayAnnouncement));
    NetworkCRC.RegisterBehaviour(nameof (DecontaminationLCZ), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }

  [Serializable]
  public class Announcement
  {
    public AudioClip clip;
    public float startTime;
    public string options;
  }
}
