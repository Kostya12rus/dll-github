// Decompiled with JetBrains decompiler
// Type: Door
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Door : NetworkBehaviour, IComparable
{
  private static int kRpcRpcDoSound = 630763456;
  public AudioSource soundsource;
  public AudioClip sound_checkpointWarning;
  public AudioClip sound_denied;
  public MovingStatus moving;
  public GameObject destroyedPrefab;
  public Vector3 localPos;
  public Quaternion localRot;
  private SECTR_Portal _portal;
  public Animator[] parts;
  public AudioClip[] sound_open;
  public AudioClip[] sound_close;
  private Rigidbody[] _destoryedRb;
  public int doorType;
  public int status;
  public float curCooldown;
  public float cooldown;
  public bool dontOpenOnWarhead;
  public bool blockAfterDetonation;
  public bool lockdown;
  public bool warheadlock;
  public bool commandlock;
  public bool decontlock;
  private bool _buffedStatus;
  private bool _wasLocked;
  private bool _prevDestroyed;
  private bool _deniedInProgress;
  public string DoorName;
  public string permissionLevel;
  [HideInInspector]
  public List<GameObject> buttons;
  [SyncVar(hook = "DestroyDoor")]
  public bool destroyed;
  [SyncVar(hook = "SetState")]
  public bool isOpen;
  [SyncVar(hook = "SetLock")]
  public bool locked;

  public Door()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    Timing.RunCoroutine(this._Start(), (Segment) 1);
  }

  private void LateUpdate()
  {
    if (this._prevDestroyed != this.destroyed && Object.op_Inequality((Object) GameObject.Find("Host"), (Object) null) && RandomSeedSync.generated)
      ((MonoBehaviour) this).StartCoroutine(this.RefreshDestroyAnimation());
    if ((double) this.curCooldown >= 0.0)
      this.curCooldown -= Time.get_deltaTime();
    if (!this._deniedInProgress && (!this.locked || this.permissionLevel == "UNACCESSIBLE"))
    {
      if ((double) this.curCooldown >= 0.0 && this.status != 3)
      {
        if (Object.op_Equality((Object) this.sound_checkpointWarning, (Object) null))
        {
          if (Object.op_Inequality((Object) this._portal, (Object) null))
            this._portal.Flags = (SECTR_Portal.PortalFlags) 0;
          this.SetActiveStatus(2);
        }
      }
      else
      {
        if (Object.op_Inequality((Object) this._portal, (Object) null))
          this._portal.Flags = !(this.isOpen | this.destroyed) ? SECTR_Portal.PortalFlags.Closed : (SECTR_Portal.PortalFlags) 0;
        this.SetActiveStatus(!this.isOpen ? 0 : 1);
      }
    }
    if (this.locked && this.permissionLevel != "UNACCESSIBLE")
    {
      if (Object.op_Inequality((Object) this._portal, (Object) null))
        this._portal.Flags = !(this.isOpen | this.destroyed | this.moving.moving) ? SECTR_Portal.PortalFlags.Closed : (SECTR_Portal.PortalFlags) 0;
      if (this._wasLocked)
        return;
      this._wasLocked = true;
      this.SetActiveStatus(4);
    }
    else
    {
      if (!this._wasLocked)
        return;
      this._wasLocked = false;
      if (this.doorType != 3)
        return;
      this.SetState(false);
      this.CallRpcDoSound();
    }
  }

  public int CompareTo(object obj)
  {
    return string.CompareOrdinal(this.DoorName, ((Door) obj).DoorName);
  }

  private void SetLock(bool l)
  {
    this.Networklocked = l;
  }

  public void UpdateLock()
  {
    this.Networklocked = this.permissionLevel != "UNACCESSIBLE" && this.commandlock | this.lockdown | this.warheadlock | this.decontlock;
  }

  public void SetPortal(SECTR_Portal p)
  {
    this._portal = p;
  }

  public void SetLocalPos()
  {
    this.localPos = ((Component) this).get_transform().get_localPosition();
    this.localRot = ((Component) this).get_transform().get_localRotation();
  }

  [DebuggerHidden]
  private IEnumerator<float> _UpdatePosition()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Door.\u003C_UpdatePosition\u003Ec__Iterator0() { \u0024this = this };
  }

  public void SetState(bool open)
  {
    this.NetworkisOpen = open;
    this.ForceCooldown(this.cooldown);
  }

  public void SetStateWithSound(bool open)
  {
    if (this.isOpen != open)
      this.CallRpcDoSound();
    this.NetworkisOpen = open;
    this.ForceCooldown(this.cooldown);
  }

  public void DestroyDoor(bool b)
  {
    if (b && Object.op_Inequality((Object) this.destroyedPrefab, (Object) null))
      this.Networkdestroyed = true;
    else
      this.Networkdestroyed = false;
  }

  [DebuggerHidden]
  private IEnumerator RefreshDestroyAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Door.\u003CRefreshDestroyAnimation\u003Ec__Iterator1() { \u0024this = this };
  }

  [DebuggerHidden]
  private IEnumerator<float> _Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Door.\u003C_Start\u003Ec__Iterator2() { \u0024this = this };
  }

  public void UpdatePos()
  {
    if (Vector3.op_Equality(this.localPos, Vector3.get_zero()))
      return;
    ((Component) this).get_transform().set_localPosition(this.localPos);
    ((Component) this).get_transform().set_localRotation(this.localRot);
  }

  public void SetZero()
  {
    this.localPos = Vector3.get_zero();
  }

  public void ChangeState(bool force = false)
  {
    if ((double) this.curCooldown >= 0.0 || this.moving.moving || this._deniedInProgress || this.locked && !force)
      return;
    this.moving.moving = true;
    this.SetState(!this.isOpen);
    this.CallRpcDoSound();
  }

  public void OpenDecontamination()
  {
    if (this.permissionLevel == "UNACCESSIBLE")
      return;
    this.decontlock = true;
    if (!this.isOpen)
      this.CallRpcDoSound();
    this.moving.moving = true;
    this.SetState(true);
    this.UpdateLock();
  }

  public void CloseDecontamination()
  {
    if (this.permissionLevel == "UNACCESSIBLE" || ((Component) this).get_transform().get_position().y < -100.0 || ((Component) this).get_transform().get_position().y > 100.0)
      return;
    this.decontlock = true;
    if (this.isOpen)
      this.CallRpcDoSound();
    this.moving.moving = true;
    this.SetState(false);
    this.UpdateLock();
  }

  public void OpenWarhead(bool force, bool lockDoor)
  {
    if (this.permissionLevel == "UNACCESSIBLE" || this.dontOpenOnWarhead && !force)
      return;
    if (lockDoor)
      this.warheadlock = true;
    if (this.locked && !force || !force && this.permissionLevel == "CONT_LVL_3")
      return;
    if (!this.isOpen)
      this.CallRpcDoSound();
    this.moving.moving = true;
    this.SetState(true);
    this.UpdateLock();
  }

  [ClientRpc(channel = 14)]
  public void RpcDoSound()
  {
    this.soundsource.PlayOneShot(!this.isOpen ? this.sound_close[Random.Range(0, this.sound_close.Length)] : this.sound_open[Random.Range(0, this.sound_open.Length)]);
  }

  public void SetActiveStatus(int s)
  {
    if (this.status == s)
      return;
    this.status = s;
    using (List<GameObject>.Enumerator enumerator = this.buttons.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        GameObject current = enumerator.Current;
        MeshRenderer component = (MeshRenderer) current.GetComponent<MeshRenderer>();
        Text componentInChildren1 = (Text) current.GetComponentInChildren<Text>();
        Image componentInChildren2 = (Image) current.GetComponentInChildren<Image>();
        if (Object.op_Inequality((Object) component, (Object) null))
          ((Renderer) component).set_material(ButtonStages.types[this.doorType].stages[s].mat);
        if (Object.op_Inequality((Object) componentInChildren1, (Object) null))
          componentInChildren1.set_text(ButtonStages.types[this.doorType].stages[s].info);
        if (Object.op_Inequality((Object) componentInChildren2, (Object) null))
        {
          ((Graphic) componentInChildren2).set_color(!Object.op_Equality((Object) ButtonStages.types[this.doorType].stages[s].texture, (Object) null) ? Color.get_white() : Color.get_clear());
          componentInChildren2.set_sprite(ButtonStages.types[this.doorType].stages[s].texture);
        }
      }
    }
  }

  [DebuggerHidden]
  public IEnumerator<float> _Denied()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Door.\u003C_Denied\u003Ec__Iterator3() { \u0024this = this };
  }

  public void ForceCooldown(float cd)
  {
    this.curCooldown = cd;
    Timing.RunCoroutine(this._UpdatePosition(), (Segment) 0);
  }

  private void UNetVersion()
  {
  }

  public bool Networkdestroyed
  {
    get
    {
      return this.destroyed;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.destroyed;
      int num2 = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.DestroyDoor(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public bool NetworkisOpen
  {
    get
    {
      return this.isOpen;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.isOpen;
      int num2 = 2;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetState(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
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
      int num2 = 4;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetLock(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  protected static void InvokeRpcRpcDoSound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcDoSound called on server.");
    else
      ((Door) obj).RpcDoSound();
  }

  public void CallRpcDoSound()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcDoSound called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Door.kRpcRpcDoSound);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 14, "RpcDoSound");
    }
  }

  static Door()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Door), Door.kRpcRpcDoSound, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcDoSound)));
    NetworkCRC.RegisterBehaviour(nameof (Door), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.destroyed);
      writer.Write(this.isOpen);
      writer.Write(this.locked);
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
      writer.Write(this.destroyed);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.isOpen);
    }
    if (((int) this.get_syncVarDirtyBits() & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.locked);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.destroyed = reader.ReadBoolean();
      this.isOpen = reader.ReadBoolean();
      this.locked = reader.ReadBoolean();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.DestroyDoor(reader.ReadBoolean());
      if ((num & 2) != 0)
        this.SetState(reader.ReadBoolean());
      if ((num & 4) == 0)
        return;
      this.SetLock(reader.ReadBoolean());
    }
  }
}
