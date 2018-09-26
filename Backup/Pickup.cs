// Decompiled with JetBrains decompiler
// Type: Pickup
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour
{
  public float searchTime;
  [SyncVar(hook = "SyncPickup")]
  public Pickup.PickupInfo info;
  public static Inventory inv;
  public static List<Pickup> instances;
  private int previousId;
  private GameObject model;

  public Pickup()
  {
    base.\u002Ector();
  }

  private void SyncPickup(Pickup.PickupInfo pickupInfo)
  {
    this.Networkinfo = pickupInfo;
  }

  public void SetupPickup(Pickup.PickupInfo pickupInfo)
  {
    this.Networkinfo = pickupInfo;
    ((Component) this).get_transform().set_position(this.info.position);
    ((Component) this).get_transform().set_rotation(this.info.rotation);
    this.RefreshModel();
    this.UpdatePosition();
  }

  [ServerCallback]
  private void UpdatePosition()
  {
    if (!NetworkServer.get_active())
      return;
    Pickup.PickupInfo info = this.info;
    info.position = ((Component) this).get_transform().get_position();
    info.rotation = ((Component) this).get_transform().get_rotation();
    this.SyncPickup(info);
  }

  public void CheckForRefresh()
  {
    this.UpdatePosition();
    if (this.previousId == this.info.itemId && !Object.op_Equality((Object) this.model, (Object) null))
      return;
    this.previousId = this.info.itemId;
    this.RefreshModel();
  }

  private void RefreshModel()
  {
    if (Object.op_Inequality((Object) this.model, (Object) null))
      Object.Destroy((Object) this.model.get_gameObject());
    this.model = (GameObject) Object.Instantiate<GameObject>((M0) Pickup.inv.availableItems[this.info.itemId].prefab, ((Component) this).get_transform());
    this.model.get_transform().set_localPosition(Vector3.get_zero());
    this.searchTime = Pickup.inv.availableItems[this.info.itemId].pickingtime;
    ((Component) this).get_transform().set_position(this.info.position);
    ((Component) this).get_transform().set_rotation(this.info.rotation);
  }

  public void Delete()
  {
    NetworkServer.Destroy(((Component) this).get_gameObject());
  }

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Pickup.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void OnDestroy()
  {
    if (Pickup.instances == null)
      return;
    Pickup.instances.Remove(this);
    Inventory.collectionModified = true;
  }

  public void RefreshDurability(bool allowAmmoRenew = false)
  {
    if (Pickup.inv.availableItems[this.info.itemId].noEquipable && !allowAmmoRenew)
      return;
    this.info.durability = Pickup.inv.availableItems[this.info.itemId].durability;
  }

  private void UNetVersion()
  {
  }

  public Pickup.PickupInfo Networkinfo
  {
    get
    {
      return this.info;
    }
    [param: In] set
    {
      Pickup.PickupInfo pickupInfo = value;
      ref Pickup.PickupInfo local = ref this.info;
      int num = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SyncPickup(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<Pickup.PickupInfo>((M0) pickupInfo, (M0&) ref local, (uint) num);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      GeneratedNetworkCode._WritePickupInfo_Pickup(writer, this.info);
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
      GeneratedNetworkCode._WritePickupInfo_Pickup(writer, this.info);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.info = GeneratedNetworkCode._ReadPickupInfo_Pickup(reader);
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SyncPickup(GeneratedNetworkCode._ReadPickupInfo_Pickup(reader));
    }
  }

  [Serializable]
  public struct PickupInfo
  {
    public Vector3 position;
    public Quaternion rotation;
    public int itemId;
    public float durability;
    public int ownerPlayerID;
  }
}
