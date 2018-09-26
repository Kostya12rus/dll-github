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
  private int previousId = -1;
  public float searchTime;
  [SyncVar(hook = "SyncPickup")]
  public Pickup.PickupInfo info;
  public static Inventory inv;
  public static List<Pickup> instances;
  private GameObject model;

  private void SyncPickup(Pickup.PickupInfo pickupInfo)
  {
    this.Networkinfo = pickupInfo;
  }

  public void SetupPickup(Pickup.PickupInfo pickupInfo)
  {
    this.Networkinfo = pickupInfo;
    this.transform.position = this.info.position;
    this.transform.rotation = this.info.rotation;
    this.RefreshModel();
    this.UpdatePosition();
  }

  [ServerCallback]
  private void UpdatePosition()
  {
    if (!NetworkServer.active)
      return;
    Pickup.PickupInfo info = this.info;
    info.position = this.transform.position;
    info.rotation = this.transform.rotation;
    this.SyncPickup(info);
  }

  public void CheckForRefresh()
  {
    this.UpdatePosition();
    if (this.previousId == this.info.itemId && !((Object) this.model == (Object) null))
      return;
    this.previousId = this.info.itemId;
    this.RefreshModel();
  }

  private void RefreshModel()
  {
    if ((Object) this.model != (Object) null)
      Object.Destroy((Object) this.model.gameObject);
    this.model = Object.Instantiate<GameObject>(Pickup.inv.availableItems[this.info.itemId].prefab, this.transform);
    this.model.transform.localPosition = Vector3.zero;
    this.searchTime = Pickup.inv.availableItems[this.info.itemId].pickingtime;
    this.transform.position = this.info.position;
    this.transform.rotation = this.info.rotation;
  }

  public void Delete()
  {
    NetworkServer.Destroy(this.gameObject);
  }

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Pickup.\u003CStart\u003Ec__Iterator0() { \u0024this = this };
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
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SyncPickup(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<Pickup.PickupInfo>(pickupInfo, ref local, (uint) num);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      GeneratedNetworkCode._WritePickupInfo_Pickup(writer, this.info);
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
      GeneratedNetworkCode._WritePickupInfo_Pickup(writer, this.info);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
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
