// Decompiled with JetBrains decompiler
// Type: BreakableWindow
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class BreakableWindow : NetworkBehaviour
{
  public GameObject template;
  public Transform parent;
  public Vector3 size;
  private BreakableWindow.BreakableWindowStatus prevStatus;
  [SyncVar(hook = "SetStatus")]
  public BreakableWindow.BreakableWindowStatus syncStatus;
  public float health;
  public bool isBroken;

  public BreakableWindow()
  {
    base.\u002Ector();
  }

  public void SetStatus(BreakableWindow.BreakableWindowStatus s)
  {
    this.NetworksyncStatus = s;
  }

  [ServerCallback]
  private void UpdateStatus(BreakableWindow.BreakableWindowStatus s)
  {
    if (!NetworkServer.get_active())
      return;
    this.SetStatus(s);
  }

  [ServerCallback]
  public void ServerDamageWindow(float damage)
  {
    if (!NetworkServer.get_active())
      return;
    this.health -= damage;
    if ((double) this.health > 0.0)
      return;
    ((MonoBehaviour) this).StartCoroutine(this.BreakWindow());
  }

  private void Update()
  {
    BreakableWindow.BreakableWindowStatus s = new BreakableWindow.BreakableWindowStatus()
    {
      position = ((Component) this).get_transform().get_position(),
      rotation = ((Component) this).get_transform().get_rotation(),
      broken = this.isBroken
    };
    if (s.IsEqual(this.syncStatus))
      return;
    if (NetworkServer.get_active())
    {
      this.UpdateStatus(s);
    }
    else
    {
      if (!this.isBroken && this.syncStatus.broken)
        ((MonoBehaviour) this).StartCoroutine(this.BreakWindow());
      ((Component) this).get_transform().set_position(this.syncStatus.position);
      ((Component) this).get_transform().set_rotation(this.syncStatus.rotation);
      this.isBroken = this.syncStatus.broken;
    }
  }

  [DebuggerHidden]
  private IEnumerator BreakWindow()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BreakableWindow.\u003CBreakWindow\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void UNetVersion()
  {
  }

  public BreakableWindow.BreakableWindowStatus NetworksyncStatus
  {
    get
    {
      return this.syncStatus;
    }
    [param: In] set
    {
      BreakableWindow.BreakableWindowStatus breakableWindowStatus = value;
      ref BreakableWindow.BreakableWindowStatus local = ref this.syncStatus;
      int num = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetStatus(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<BreakableWindow.BreakableWindowStatus>((M0) breakableWindowStatus, (M0&) ref local, (uint) num);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      GeneratedNetworkCode._WriteBreakableWindowStatus_BreakableWindow(writer, this.syncStatus);
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
      GeneratedNetworkCode._WriteBreakableWindowStatus_BreakableWindow(writer, this.syncStatus);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.syncStatus = GeneratedNetworkCode._ReadBreakableWindowStatus_BreakableWindow(reader);
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetStatus(GeneratedNetworkCode._ReadBreakableWindowStatus_BreakableWindow(reader));
    }
  }

  public struct BreakableWindowStatus
  {
    public Vector3 position;
    public Quaternion rotation;
    public bool broken;

    public bool IsEqual(BreakableWindow.BreakableWindowStatus stat)
    {
      if (Vector3.op_Equality(this.position, stat.position) && Quaternion.op_Equality(this.rotation, stat.rotation))
        return this.broken == stat.broken;
      return false;
    }
  }
}
