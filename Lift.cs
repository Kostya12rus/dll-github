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
  public Lift.Elevator[] elevators;
  public float movingSpeed;
  public float maxDistance;
  public bool lockable;
  public bool operative;
  public Lift.Status status;
  [SyncVar(hook = "SetLock")]
  private bool locked;
  [SyncVar(hook = "SetStatus")]
  public int statusID;
  public Text monitor;
  private static int kTargetRpcTargetBeingMoved;

  public Lift()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    foreach (Lift.Elevator elevator in this.elevators)
      ((Component) elevator.target).set_tag("LiftTarget");
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
    if (!b || !Object.op_Inequality((Object) this.monitor, (Object) null))
      return;
    this.monitor.set_text("ELEVATOR SYSTEM <color=#e00>DISABLED</color>");
  }

  public void Lock()
  {
    if (!this.lockable)
      return;
    this.SetLock(true);
    Timing.RunCoroutine(this._LockdownUpdate(), (Segment) 0);
  }

  public void UseLift()
  {
    if (!this.operative || (double) AlphaWarheadController.host.timeToDetonation == 0.0 || this.locked)
      return;
    Timing.RunCoroutine(this._LiftAnimation(), (Segment) 0);
    this.operative = false;
  }

  [DebuggerHidden]
  private IEnumerator<float> _LiftAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Lift.\u003C_LiftAnimation\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator<float> _LockdownUpdate()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Lift.\u003C_LockdownUpdate\u003Ec__Iterator1()
    {
      \u0024this = this
    };
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
    Gizmos.set_color(Color.get_red());
    foreach (Lift.Elevator elevator in this.elevators)
      Gizmos.DrawWireCube(((Component) elevator.target).get_transform().get_position(), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_one(), this.maxDistance), 2f));
  }

  private void MovePlayers(Transform target)
  {
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      GameObject which = (GameObject) null;
      if (this.InRange(player.get_transform().get_position(), out which, 1f) && !Object.op_Equality((Object) which.get_transform(), (Object) target))
      {
        PlyMovementSync component = (PlyMovementSync) player.GetComponent<PlyMovementSync>();
        player.get_transform().set_parent(which.get_transform());
        Vector3 localPosition = player.get_transform().get_localPosition();
        player.get_transform().set_parent(((Component) target).get_transform());
        player.get_transform().set_localPosition(localPosition);
        player.get_transform().set_parent((Transform) null);
        component.SetPosition(player.get_transform().get_position());
        PlyMovementSync plyMovementSync = component;
        Quaternion rotation1 = ((Component) target).get_transform().get_rotation();
        // ISSUE: variable of the null type
        __Null y1 = ((Quaternion) ref rotation1).get_eulerAngles().y;
        Quaternion rotation2 = which.get_transform().get_rotation();
        // ISSUE: variable of the null type
        __Null y2 = ((Quaternion) ref rotation2).get_eulerAngles().y;
        // ISSUE: variable of the null type
        __Null local = y1 - y2;
        plyMovementSync.SetRotation((float) local);
        this.CallTargetBeingMoved(((NetworkIdentity) player.GetComponent<NetworkIdentity>()).get_connectionToClient());
        player.get_transform().set_parent((Transform) null);
      }
    }
    foreach (Lift.Elevator elevator in this.elevators)
    {
      foreach (Collider collider in Physics.OverlapBox(((Component) elevator.target).get_transform().get_position(), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_one(), this.maxDistance), 2f)))
      {
        if (Object.op_Inequality((Object) ((Component) collider).GetComponent<Pickup>(), (Object) null) || Object.op_Inequality((Object) ((Component) collider).GetComponent<Grenade>(), (Object) null))
        {
          GameObject which = (GameObject) null;
          if (this.InRange(((Component) collider).get_transform().get_position(), out which, 1.3f) && !Object.op_Equality((Object) which.get_transform(), (Object) target))
          {
            ((Component) collider).get_transform().set_parent(which.get_transform());
            Vector3 localPosition = ((Component) collider).get_transform().get_localPosition();
            Quaternion localRotation = ((Component) collider).get_transform().get_localRotation();
            ((Component) collider).get_transform().set_parent(((Component) target).get_transform());
            ((Component) collider).get_transform().set_localPosition(localPosition);
            ((Component) collider).get_transform().set_localRotation(localRotation);
            ((Component) collider).get_transform().set_parent((Transform) null);
          }
        }
      }
    }
  }

  [TargetRpc(channel = 4)]
  private void TargetBeingMoved(NetworkConnection target)
  {
    ((ExplosionCameraShake) Object.FindObjectOfType<ExplosionCameraShake>()).Shake(0.15f);
  }

  public bool InRange(Vector3 pos, out GameObject which, float maxDistanceMultiplier = 1f)
  {
    foreach (Lift.Elevator elevator in this.elevators)
    {
      bool flag = (double) Mathf.Abs((float) (elevator.target.get_position().x - pos.x)) <= (double) this.maxDistance * (double) maxDistanceMultiplier;
      if ((double) Mathf.Abs((float) (elevator.target.get_position().y - pos.y)) > (double) this.maxDistance * (double) maxDistanceMultiplier)
        flag = false;
      if ((double) Mathf.Abs((float) (elevator.target.get_position().z - pos.z)) > (double) this.maxDistance * (double) maxDistanceMultiplier)
        flag = false;
      if (flag)
      {
        which = ((Component) elevator.target).get_gameObject();
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetLock(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetStatus(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  protected static void InvokeRpcRpcPlayMusic(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcPlayMusic called on server.");
    else
      ((Lift) obj).RpcPlayMusic();
  }

  protected static void InvokeRpcTargetBeingMoved(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetBeingMoved called on server.");
    else
      ((Lift) obj).TargetBeingMoved(ClientScene.get_readyConnection());
  }

  public void CallRpcPlayMusic()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcPlayMusic called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Lift.kRpcRpcPlayMusic);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 4, "RpcPlayMusic");
    }
  }

  public void CallTargetBeingMoved(NetworkConnection target)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetBeingMoved called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetBeingMoved called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Lift.kTargetRpcTargetBeingMoved);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendTargetRPCInternal(target, networkWriter, 4, "TargetBeingMoved");
    }
  }

  static Lift()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Lift), Lift.kRpcRpcPlayMusic, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcPlayMusic)));
    Lift.kTargetRpcTargetBeingMoved = -1324102726;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Lift), Lift.kTargetRpcTargetBeingMoved, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetBeingMoved)));
    NetworkCRC.RegisterBehaviour(nameof (Lift), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.locked);
      writer.WritePackedUInt32((uint) this.statusID);
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
      writer.Write(this.locked);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.statusID);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
      this.pos = this.target.get_position();
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
