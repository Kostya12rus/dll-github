// Decompiled with JetBrains decompiler
// Type: TeslaGate
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

public class TeslaGate : NetworkBehaviour
{
  private static int kRpcRpcPlayAnimation = 178484248;
  public Vector3 localPosition;
  public Vector3 localRotation;
  public Vector3 sizeOfKiller;
  public float sizeOfTrigger;
  public GameObject[] killers;
  public AudioSource source;
  public AudioClip clip_warmup;
  public AudioClip clip_shock;
  public LayerMask killerMask;
  public bool showGizmos;
  private bool inProgress;
  public GameObject particles;

  public TeslaGate()
  {
    base.\u002Ector();
  }

  private void ServerSideCode()
  {
    if (this.inProgress || this.PlayersInRange(false).Length <= 0)
      return;
    this.CallRpcPlayAnimation();
  }

  private void ClientSideCode()
  {
    ((Component) this).get_transform().set_localPosition(this.localPosition);
    ((Component) this).get_transform().set_localRotation(Quaternion.Euler(this.localRotation));
    ((Renderer) ((Component) this).GetComponent<Renderer>()).set_enabled(true);
  }

  [ClientRpc]
  private void RpcPlayAnimation()
  {
    Timing.RunCoroutine(this._PlayAnimation(), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _PlayAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new TeslaGate.\u003C_PlayAnimation\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private PlayerStats[] PlayersInRange(bool hurtRange)
  {
    List<PlayerStats> playerStatsList = new List<PlayerStats>();
    if (hurtRange)
    {
      foreach (GameObject killer in this.killers)
      {
        foreach (Component component in Physics.OverlapBox(Vector3.op_Addition(killer.get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), (float) (this.sizeOfKiller.y / 2.0))), Vector3.op_Division(this.sizeOfKiller, 2f), (Quaternion) null, LayerMask.op_Implicit(this.killerMask)))
        {
          PlayerStats componentInParent = (PlayerStats) component.GetComponentInParent<PlayerStats>();
          if (Object.op_Inequality((Object) componentInParent, (Object) null) && componentInParent.ccm.curClass != 2)
            playerStatsList.Add(componentInParent);
        }
      }
    }
    else
    {
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        if ((double) Vector3.Distance(((Component) this).get_transform().get_position(), player.get_transform().get_position()) < (double) this.sizeOfTrigger && ((CharacterClassManager) player.GetComponent<CharacterClassManager>()).curClass != 2)
          playerStatsList.Add((PlayerStats) player.GetComponent<PlayerStats>());
      }
    }
    return playerStatsList.ToArray();
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.showGizmos)
      return;
    Gizmos.set_color(new Color(1f, 0.0f, 0.0f, 0.2f));
    foreach (GameObject killer in this.killers)
      Gizmos.DrawCube(Vector3.op_Addition(killer.get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), (float) (this.sizeOfKiller.y / 2.0))), this.sizeOfKiller);
    Gizmos.set_color(new Color(1f, 1f, 0.0f, 0.2f));
    Gizmos.DrawSphere(((Component) this).get_transform().get_position(), this.sizeOfTrigger);
  }

  private void Update()
  {
    if (NetworkServer.get_active())
      this.ServerSideCode();
    else
      this.ClientSideCode();
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeRpcRpcPlayAnimation(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcPlayAnimation called on server.");
    else
      ((TeslaGate) obj).RpcPlayAnimation();
  }

  public void CallRpcPlayAnimation()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcPlayAnimation called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) TeslaGate.kRpcRpcPlayAnimation);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 0, "RpcPlayAnimation");
    }
  }

  static TeslaGate()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (TeslaGate), TeslaGate.kRpcRpcPlayAnimation, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcPlayAnimation)));
    NetworkCRC.RegisterBehaviour(nameof (TeslaGate), 0);
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
