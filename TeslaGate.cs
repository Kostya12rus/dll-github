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

  private void ServerSideCode()
  {
    if (this.inProgress || this.PlayersInRange(false).Length <= 0)
      return;
    this.CallRpcPlayAnimation();
  }

  private void ClientSideCode()
  {
    this.transform.localPosition = this.localPosition;
    this.transform.localRotation = Quaternion.Euler(this.localRotation);
    this.GetComponent<Renderer>().enabled = true;
  }

  [ClientRpc]
  private void RpcPlayAnimation()
  {
    Timing.RunCoroutine(this._PlayAnimation(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _PlayAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new TeslaGate.\u003C_PlayAnimation\u003Ec__Iterator0() { \u0024this = this };
  }

  private PlayerStats[] PlayersInRange(bool hurtRange)
  {
    List<PlayerStats> playerStatsList = new List<PlayerStats>();
    if (hurtRange)
    {
      foreach (GameObject killer in this.killers)
      {
        foreach (Component component in Physics.OverlapBox(killer.transform.position + Vector3.up * (this.sizeOfKiller.y / 2f), this.sizeOfKiller / 2f, new Quaternion(), (int) this.killerMask))
        {
          PlayerStats componentInParent = component.GetComponentInParent<PlayerStats>();
          if ((Object) componentInParent != (Object) null && componentInParent.ccm.curClass != 2)
            playerStatsList.Add(componentInParent);
        }
      }
    }
    else
    {
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        if ((double) Vector3.Distance(this.transform.position, player.transform.position) < (double) this.sizeOfTrigger && player.GetComponent<CharacterClassManager>().curClass != 2)
          playerStatsList.Add(player.GetComponent<PlayerStats>());
      }
    }
    return playerStatsList.ToArray();
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.showGizmos)
      return;
    Gizmos.color = new Color(1f, 0.0f, 0.0f, 0.2f);
    foreach (GameObject killer in this.killers)
      Gizmos.DrawCube(killer.transform.position + Vector3.up * (this.sizeOfKiller.y / 2f), this.sizeOfKiller);
    Gizmos.color = new Color(1f, 1f, 0.0f, 0.2f);
    Gizmos.DrawSphere(this.transform.position, this.sizeOfTrigger);
  }

  private void Update()
  {
    if (NetworkServer.active)
      this.ServerSideCode();
    else
      this.ClientSideCode();
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeRpcRpcPlayAnimation(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcPlayAnimation called on server.");
    else
      ((TeslaGate) obj).RpcPlayAnimation();
  }

  public void CallRpcPlayAnimation()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcPlayAnimation called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) TeslaGate.kRpcRpcPlayAnimation);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 0, "RpcPlayAnimation");
    }
  }

  static TeslaGate()
  {
    NetworkBehaviour.RegisterRpcDelegate(typeof (TeslaGate), TeslaGate.kRpcRpcPlayAnimation, new NetworkBehaviour.CmdDelegate(TeslaGate.InvokeRpcRpcPlayAnimation));
    NetworkCRC.RegisterBehaviour(nameof (TeslaGate), 0);
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
