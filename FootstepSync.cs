// Decompiled with JetBrains decompiler
// Type: FootstepSync
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;

public class FootstepSync : NetworkBehaviour
{
  private static int kCmdCmdSyncFoot = -789180642;
  private AnimationController controller;
  private CharacterClassManager ccm;
  private Scp939_VisionController visionController;
  private static int kRpcRpcSyncFoot;

  private void Start()
  {
    this.visionController = this.GetComponent<Scp939_VisionController>();
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.controller = this.GetComponent<AnimationController>();
  }

  public void SyncFoot(bool run)
  {
    if (!this.isLocalPlayer)
      return;
    this.CallCmdSyncFoot(run);
    AudioClip[] stepClips = this.ccm.klasy[this.ccm.curClass].stepClips;
    this.controller.walkSource.PlayOneShot(stepClips[Random.Range(0, stepClips.Length)], !run ? 0.6f : 1f);
  }

  public void SyncWalk()
  {
    this.SyncFoot(false);
  }

  public void SyncRun()
  {
    this.SyncFoot(true);
  }

  public void SetLoundness(Team t, bool is939)
  {
    if (t == Team.SCP && !is939 || t == Team.CHI)
    {
      this.controller.runSource.maxDistance = 50f;
      this.controller.walkSource.maxDistance = 50f;
    }
    else if (t == Team.CDP || t == Team.RSC)
    {
      this.controller.runSource.maxDistance = 20f;
      this.controller.walkSource.maxDistance = 10f;
    }
    else
    {
      this.controller.runSource.maxDistance = 30f;
      this.controller.walkSource.maxDistance = 15f;
    }
  }

  [Command(channel = 1)]
  private void CmdSyncFoot(bool run)
  {
    this.visionController.MakeNoise(this.controller.runSource.maxDistance * (!run ? 0.4f : 0.7f));
    this.CallRpcSyncFoot(run);
  }

  [ClientRpc(channel = 1)]
  private void RpcSyncFoot(bool run)
  {
    if (this.isLocalPlayer || !((Object) this.ccm != (Object) null))
      return;
    AudioClip[] stepClips = this.ccm.klasy[this.ccm.curClass].stepClips;
    if (run)
      this.controller.runSource.PlayOneShot(stepClips[Random.Range(0, stepClips.Length)]);
    else
      this.controller.walkSource.PlayOneShot(stepClips[Random.Range(0, stepClips.Length)]);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdSyncFoot(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSyncFoot called on client.");
    else
      ((FootstepSync) obj).CmdSyncFoot(reader.ReadBoolean());
  }

  public void CallCmdSyncFoot(bool run)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSyncFoot called on server.");
    else if (this.isServer)
    {
      this.CmdSyncFoot(run);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) FootstepSync.kCmdCmdSyncFoot);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(run);
      this.SendCommandInternal(writer, 1, "CmdSyncFoot");
    }
  }

  protected static void InvokeRpcRpcSyncFoot(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcSyncFoot called on server.");
    else
      ((FootstepSync) obj).RpcSyncFoot(reader.ReadBoolean());
  }

  public void CallRpcSyncFoot(bool run)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcSyncFoot called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) FootstepSync.kRpcRpcSyncFoot);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(run);
      this.SendRPCInternal(writer, 1, "RpcSyncFoot");
    }
  }

  static FootstepSync()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (FootstepSync), FootstepSync.kCmdCmdSyncFoot, new NetworkBehaviour.CmdDelegate(FootstepSync.InvokeCmdCmdSyncFoot));
    FootstepSync.kRpcRpcSyncFoot = -840565516;
    NetworkBehaviour.RegisterRpcDelegate(typeof (FootstepSync), FootstepSync.kRpcRpcSyncFoot, new NetworkBehaviour.CmdDelegate(FootstepSync.InvokeRpcRpcSyncFoot));
    NetworkCRC.RegisterBehaviour(nameof (FootstepSync), 0);
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
