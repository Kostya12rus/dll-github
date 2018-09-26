// Decompiled with JetBrains decompiler
// Type: Lift
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Lift : NetworkBehaviour
{
  private static int kRpcRpcPlayMusic = 374858512;
  public bool operative = true;
  public Lift.Elevator[] elevators;
  public float movingSpeed;
  public float maxDistance;
  public bool lockable;
  public Lift.Status status;
  [SyncVar(hook = "SetLock")]
  private bool locked;
  [SyncVar(hook = "SetStatus")]
  public int statusID;
  public Text monitor;
  private static int kTargetRpcTargetBeingMoved;

  private void Awake()
  {
    foreach (Lift.Elevator elevator in this.elevators)
      elevator.target.tag = "LiftTarget";
  }

  private void FixedUpdate()
  {
    for (int index = 0; index < this.elevators.Length; ++index)
    {
      bool flag = this.statusID == index && this.status != Lift.Status.Moving;
      this.elevators[index].door.SetBool("isOpen", flag);
    }
  }

  private void SetStatus(int i)
  {
    this.NetworkstatusID = i;
    this.status = (Lift.Status) i;
  }

  private void SetLock(bool b)
  {
    this.Networklocked = b;
    if (!b || !((Object) this.monitor != (Object) null))
      return;
    this.monitor.text = "ELEVATOR SYSTEM <color=#e00>DISABLED</color>";
  }

  public void Lock()
  {
    if (!this.lockable)
      return;
    this.SetLock(true);
    Timing.RunCoroutine(this._LockdownUpdate(), Segment.Update);
  }

  public void UseLift()
  {
    if (!this.operative || (double) AlphaWarheadController.host.timeToDetonation == 0.0 || this.locked)
      return;
    Timing.RunCoroutine(this._LiftAnimation(), Segment.Update);
    this.operative = false;
  }

  [DebuggerHidden]
  private IEnumerator<float> _LiftAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Lift.\u003C_LiftAnimation\u003Ec__Iterator0() { \u0024this = this };
  }

  [DebuggerHidden]
  private IEnumerator<float> _LockdownUpdate()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Lift.\u003C_LockdownUpdate\u003Ec__Iterator1() { \u0024this = this };
  }

  [ClientRpc(channel = 4)]
  private void RpcPlayMusic()
  {
    foreach (Lift.Elevator elevator in this.elevators)
    {
      try
      {
        elevator.musicSpeaker.Play();
      }
      catch
      {
      }
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    foreach (Lift.Elevator elevator in this.elevators)
      Gizmos.DrawWireCube(elevator.target.transform.position, Vector3.one * this.maxDistance * 2f);
  }

  private void MovePlayers(Transform target)
  {
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      GameObject which = (GameObject) null;
      if (this.InRange(player.transform.position, out which, 1f) && !((Object) which.transform == (Object) target))
      {
        PlyMovementSync component = player.GetComponent<PlyMovementSync>();
        player.transform.parent = which.transform;
        Vector3 localPosition = player.transform.localPosition;
        player.transform.parent = target.transform;
        player.transform.localPosition = localPosition;
        player.transform.parent = (Transform) null;
        component.SetPosition(player.transform.position);
        component.SetRotation(target.transform.rotation.eulerAngles.y - which.transform.rotation.eulerAngles.y);
        this.CallTargetBeingMoved(player.GetComponent<NetworkIdentity>().connectionToClient);
        player.transform.parent = (Transform) null;
      }
    }
    foreach (Lift.Elevator elevator in this.elevators)
    {
      foreach (Collider collider in Physics.OverlapBox(elevator.target.transform.position, Vector3.one * this.maxDistance * 2f))
      {
        if ((Object) collider.GetComponent<Pickup>() != (Object) null || (Object) collider.GetComponent<Grenade>() != (Object) null)
        {
          GameObject which = (GameObject) null;
          if (this.InRange(collider.transform.position, out which, 1.3f) && !((Object) which.transform == (Object) target))
          {
            collider.transform.parent = which.transform;
            Vector3 localPosition = collider.transform.localPosition;
            Quaternion localRotation = collider.transform.localRotation;
            collider.transform.parent = target.transform;
            collider.transform.localPosition = localPosition;
            collider.transform.localRotation = localRotation;
            collider.transform.parent = (Transform) null;
          }
        }
      }
    }
  }

  [TargetRpc(channel = 4)]
  private void TargetBeingMoved(NetworkConnection target)
  {
    Object.FindObjectOfType<ExplosionCameraShake>().Shake(0.15f);
  }

  public bool InRange(Vector3 pos, out GameObject which, float maxDistanceMultiplier = 1f)
  {
    foreach (Lift.Elevator elevator in this.elevators)
    {
      bool flag = (double) Mathf.Abs(elevator.target.position.x - pos.x) <= (double) this.maxDistance * (double) maxDistanceMultiplier;
      if ((double) Mathf.Abs(elevator.target.position.y - pos.y) > (double) this.maxDistance * (double) maxDistanceMultiplier)
        flag = false;
      if ((double) Mathf.Abs(elevator.target.position.z - pos.z) > (double) this.maxDistance * (double) maxDistanceMultiplier)
        flag = false;
      if (flag)
      {
        which = elevator.target.gameObject;
        return true;
      }
    }
    which = (GameObject) null;
    return false;
  }

  private void UNetVersion()
  {
  }

  public bool Networklocked
  {
    get
    {
      return this.locked;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.locked;
      int num2 = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetLock(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<bool>(num1 != 0, ref local, (uint) num2);
    }
  }

  public int NetworkstatusID
  {
    get
    {
      return this.statusID;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.statusID;
      int num2 = 2;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetStatus(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<int>(num1, ref local, (uint) num2);
    }
  }

  protected static void InvokeRpcRpcPlayMusic(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcPlayMusic called on server.");
    else
      ((Lift) obj).RpcPlayMusic();
  }

  protected static void InvokeRpcTargetBeingMoved(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetBeingMoved called on server.");
    else
      ((Lift) obj).TargetBeingMoved(ClientScene.readyConnection);
  }

  public void CallRpcPlayMusic()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcPlayMusic called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Lift.kRpcRpcPlayMusic);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 4, "RpcPlayMusic");
    }
  }

  public void CallTargetBeingMoved(NetworkConnection target)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetBeingMoved called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetBeingMoved called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Lift.kTargetRpcTargetBeingMoved);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendTargetRPCInternal(target, writer, 4, "TargetBeingMoved");
    }
  }

  static Lift()
  {
    NetworkBehaviour.RegisterRpcDelegate(typeof (Lift), Lift.kRpcRpcPlayMusic, new NetworkBehaviour.CmdDelegate(Lift.InvokeRpcRpcPlayMusic));
    Lift.kTargetRpcTargetBeingMoved = -1324102726;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Lift), Lift.kTargetRpcTargetBeingMoved, new NetworkBehaviour.CmdDelegate(Lift.InvokeRpcTargetBeingMoved));
    NetworkCRC.RegisterBehaviour(nameof (Lift), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.locked);
      writer.WritePackedUInt32((uint) this.statusID);
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
      writer.Write(this.locked);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.statusID);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.locked = reader.ReadBoolean();
      this.statusID = (int) reader.ReadPackedUInt32();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.SetLock(reader.ReadBoolean());
      if ((num & 2) == 0)
        return;
      this.SetStatus((int) reader.ReadPackedUInt32());
    }
  }

  [Serializable]
  public struct Elevator
  {
    public Transform target;
    public Animator door;
    public AudioSource musicSpeaker;
    private Vector3 pos;

    public void SetPosition()
    {
      this.pos = this.target.position;
    }

    public Vector3 GetPosition()
    {
      return this.pos;
    }
  }

  public enum Status
  {
    Up,
    Down,
    Moving,
  }
}
