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

  public FootstepSync()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.visionController = (Scp939_VisionController) ((Component) this).GetComponent<Scp939_VisionController>();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.controller = (AnimationController) ((Component) this).GetComponent<AnimationController>();
  }

  public void SyncFoot(bool run)
  {
    if (!this.get_isLocalPlayer())
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
      this.controller.runSource.set_maxDistance(50f);
      this.controller.walkSource.set_maxDistance(50f);
    }
    else if (t == Team.CDP || t == Team.RSC)
    {
      this.controller.runSource.set_maxDistance(20f);
      this.controller.walkSource.set_maxDistance(10f);
    }
    else
    {
      this.controller.runSource.set_maxDistance(30f);
      this.controller.walkSource.set_maxDistance(15f);
    }
  }

  [Command(channel = 1)]
  private void CmdSyncFoot(bool run)
  {
    this.visionController.MakeNoise(this.controller.runSource.get_maxDistance() * (!run ? 0.4f : 0.7f));
    this.CallRpcSyncFoot(run);
  }

  [ClientRpc(channel = 1)]
  private void RpcSyncFoot(bool run)
  {
    if (this.get_isLocalPlayer() || !Object.op_Inequality((Object) this.ccm, (Object) null))
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
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSyncFoot called on client.");
    else
      ((FootstepSync) obj).CmdSyncFoot(reader.ReadBoolean());
  }

  public void CallCmdSyncFoot(bool run)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSyncFoot called on server.");
    else if (this.get_isServer())
    {
      this.CmdSyncFoot(run);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) FootstepSync.kCmdCmdSyncFoot);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(run);
      this.SendCommandInternal(networkWriter, 1, "CmdSyncFoot");
    }
  }

  protected static void InvokeRpcRpcSyncFoot(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcSyncFoot called on server.");
    else
      ((FootstepSync) obj).RpcSyncFoot(reader.ReadBoolean());
  }

  public void CallRpcSyncFoot(bool run)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcSyncFoot called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) FootstepSync.kRpcRpcSyncFoot);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(run);
      this.SendRPCInternal(networkWriter, 1, "RpcSyncFoot");
    }
  }

  static FootstepSync()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (FootstepSync), FootstepSync.kCmdCmdSyncFoot, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSyncFoot)));
    FootstepSync.kRpcRpcSyncFoot = -840565516;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (FootstepSync), FootstepSync.kRpcRpcSyncFoot, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcSyncFoot)));
    NetworkCRC.RegisterBehaviour(nameof (FootstepSync), 0);
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
