// Decompiled with JetBrains decompiler
// Type: Inventory
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Inventory : NetworkBehaviour
{
  public static float targetCrosshairAlpha = 1f;
  private static int kCmdCmdSetUnic = 1995465433;
  public Inventory.SyncListItemInfo items = new Inventory.SyncListItemInfo();
  private int prevIt = -10;
  private float crosshairAlpha = 1f;
  public Item[] availableItems;
  private AnimationController ac;
  private WeaponManager weaponManager;
  public static float inventoryCooldown;
  [SyncVar(hook = "SetCurItem")]
  public int curItem;
  public GameObject camera;
  [SyncVar(hook = "SetUniq")]
  public int itemUniq;
  public GameObject pickupPrefab;
  private RawImage crosshair;
  private CharacterClassManager ccm;
  private static int uniqid;
  public static bool collectionModified;
  private bool gotO5;
  private float pickupanimation;
  private static int kListitems;
  private static int kCmdCmdSyncItem;
  private static int kCmdCmdDropItem;

  public void SetUniq(int i)
  {
    this.NetworkitemUniq = i;
  }

  [Command(channel = 2)]
  public void CmdSetUnic(int i)
  {
    this.NetworkitemUniq = i;
  }

  private void Awake()
  {
    for (int index = 0; index < this.availableItems.Length; ++index)
      this.availableItems[index].id = index;
    this.items.InitializeBehaviour((NetworkBehaviour) this, Inventory.kListitems);
  }

  private void Log(string msg)
  {
  }

  public void SetCurItem(int ci)
  {
    if (this.GetComponent<MicroHID_GFX>().onFire)
      return;
    this.NetworkcurItem = ci;
  }

  [DebuggerHidden]
  private IEnumerator<float> _RefreshPickups()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    Inventory.\u003C_RefreshPickups\u003Ec__Iterator0 pickupsCIterator0 = new Inventory.\u003C_RefreshPickups\u003Ec__Iterator0();
    return (IEnumerator<float>) pickupsCIterator0;
  }

  private void Start()
  {
    this.weaponManager = this.GetComponent<WeaponManager>();
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.crosshair = GameObject.Find("CrosshairImage").GetComponent<RawImage>();
    this.ac = this.GetComponent<AnimationController>();
    if (!this.isLocalPlayer)
      return;
    Pickup.inv = this;
    Pickup.instances = new List<Pickup>();
    Timing.RunCoroutine(this._RefreshPickups(), Segment.FixedUpdate);
    Object.FindObjectOfType<InventoryDisplay>().localplayer = this;
  }

  private void RefreshModels()
  {
    for (int index = 0; index < this.availableItems.Length; ++index)
    {
      try
      {
        this.availableItems[index].firstpersonModel.SetActive(this.isLocalPlayer & index == this.curItem);
      }
      catch
      {
      }
    }
  }

  public void DropItem(int id)
  {
    if (!this.isLocalPlayer)
      return;
    if (this.items[id].id == this.curItem)
      this.NetworkcurItem = -1;
    this.CallCmdDropItem(id, this.items[id].id);
  }

  public void ServerDropAll()
  {
    foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) this.items)
      this.SetPickup(syncItemInfo.id, syncItemInfo.durability, this.transform.position, this.camera.transform.rotation);
    AmmoBox component = this.GetComponent<AmmoBox>();
    for (int type = 0; type < 3; ++type)
    {
      if (component.GetAmmo(type) != 0)
        this.SetPickup(component.types[type].inventoryID, (float) component.GetAmmo(type), this.transform.position, this.camera.transform.rotation);
    }
    this.items.Clear();
    component.Networkamount = "0:0:0";
  }

  public void Clear()
  {
    this.items.Clear();
    this.GetComponent<AmmoBox>().Networkamount = "0:0:0";
  }

  public int GetItemIndex()
  {
    int num = 0;
    foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) this.items)
    {
      if (this.itemUniq == syncItemInfo.uniq)
        return num;
      ++num;
    }
    return -1;
  }

  public void AddNewItem(int id, float dur = -4.656647E+11f)
  {
    ++Inventory.uniqid;
    if (TutorialManager.status)
    {
      PickupTrigger[] objectsOfType = Object.FindObjectsOfType<PickupTrigger>();
      PickupTrigger pickupTrigger1 = (PickupTrigger) null;
      foreach (PickupTrigger pickupTrigger2 in objectsOfType)
      {
        if ((pickupTrigger2.filter == -1 || pickupTrigger2.filter == id) && ((Object) pickupTrigger1 == (Object) null || pickupTrigger2.prioirty < pickupTrigger1.prioirty))
          pickupTrigger1 = pickupTrigger2;
      }
      try
      {
        if ((Object) pickupTrigger1 != (Object) null)
          pickupTrigger1.Trigger(id);
      }
      catch
      {
        MonoBehaviour.print((object) "Error");
      }
    }
    Item obj = new Item(this.availableItems[id]);
    if (this.items.Count >= (ushort) 8 && !obj.noEquipable)
      return;
    if ((double) dur != -465664671744.0)
      obj.durability = dur;
    this.items.Add(new Inventory.SyncItemInfo()
    {
      id = obj.id,
      durability = obj.durability,
      uniq = Inventory.uniqid
    });
  }

  [Command(channel = 3)]
  private void CmdSyncItem(int i)
  {
    foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) this.items)
    {
      if (syncItemInfo.id == i)
      {
        this.NetworkcurItem = i;
        return;
      }
    }
    this.NetworkcurItem = -1;
  }

  private void Update()
  {
    if (this.isLocalPlayer)
    {
      if ((double) this.pickupanimation > 0.0)
        this.pickupanimation -= Time.deltaTime;
      if (!this.gotO5 && this.curItem == 11)
      {
        this.gotO5 = true;
        AchievementManager.Achieve("power");
      }
      Inventory.inventoryCooldown -= Time.deltaTime;
      this.CallCmdSyncItem(this.curItem);
      int index = Mathf.Clamp(this.curItem, 0, this.availableItems.Length - 1);
      if (this.ccm.curClass >= 0 && this.ccm.klasy[this.ccm.curClass].forcedCrosshair != -1)
        index = this.ccm.klasy[this.ccm.curClass].forcedCrosshair;
      this.crosshairAlpha = Mathf.Lerp(this.crosshairAlpha, Inventory.targetCrosshairAlpha, Time.deltaTime * 5f);
      this.crosshair.texture = this.availableItems[index].crosshair;
      this.crosshair.color = Color.Lerp(Color.clear, this.availableItems[index].crosshairColor, this.crosshairAlpha);
    }
    if (this.prevIt == this.curItem)
      return;
    this.RefreshModels();
    this.prevIt = this.curItem;
    if (this.isLocalPlayer)
    {
      foreach (WeaponManager.Weapon weapon in this.weaponManager.weapons)
      {
        if (weapon.inventoryID == this.curItem)
        {
          if (weapon.useProceduralPickupAnimation)
            this.weaponManager.weaponInventoryGroup.localPosition = Vector3.down * 0.4f;
          this.pickupanimation = 4f;
        }
      }
    }
    if (!NetworkServer.active)
      return;
    this.RefreshWeapon();
  }

  public bool WeaponReadyToInstantPickup()
  {
    return (double) this.pickupanimation <= 0.0;
  }

  [ServerCallback]
  private void RefreshWeapon()
  {
    if (!NetworkServer.active)
      return;
    int num1 = 0;
    int num2 = -1;
    foreach (WeaponManager.Weapon weapon in this.weaponManager.weapons)
    {
      if (weapon.inventoryID == this.curItem)
        num2 = num1;
      ++num1;
    }
    this.weaponManager.NetworkcurWeapon = num2;
  }

  [Command(channel = 2)]
  private void CmdDropItem(int itemInventoryIndex, int itemId)
  {
    if (this.items[itemInventoryIndex].id != itemId)
      return;
    this.SetPickup(itemId, this.items[itemInventoryIndex].durability, this.transform.position, this.camera.transform.rotation);
    this.items.RemoveAt(itemInventoryIndex);
  }

  public void SetPickup(int dropedItemID, float dur, Vector3 pos, Quaternion rot)
  {
    GameObject gameObject = Object.Instantiate<GameObject>(this.pickupPrefab);
    NetworkServer.Spawn(gameObject);
    gameObject.GetComponent<Pickup>().SetupPickup(new Pickup.PickupInfo()
    {
      position = pos,
      rotation = rot,
      itemId = dropedItemID,
      durability = dur,
      ownerPlayerID = this.GetComponent<QueryProcessor>().PlayerId
    });
  }

  static Inventory()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Inventory), Inventory.kCmdCmdSetUnic, new NetworkBehaviour.CmdDelegate(Inventory.InvokeCmdCmdSetUnic));
    Inventory.kCmdCmdSyncItem = 2140153578;
    NetworkBehaviour.RegisterCommandDelegate(typeof (Inventory), Inventory.kCmdCmdSyncItem, new NetworkBehaviour.CmdDelegate(Inventory.InvokeCmdCmdSyncItem));
    Inventory.kCmdCmdDropItem = -109121218;
    NetworkBehaviour.RegisterCommandDelegate(typeof (Inventory), Inventory.kCmdCmdDropItem, new NetworkBehaviour.CmdDelegate(Inventory.InvokeCmdCmdDropItem));
    Inventory.kListitems = 1683194626;
    NetworkBehaviour.RegisterSyncListDelegate(typeof (Inventory), Inventory.kListitems, new NetworkBehaviour.CmdDelegate(Inventory.InvokeSyncListitems));
    NetworkCRC.RegisterBehaviour(nameof (Inventory), 0);
  }

  private void UNetVersion()
  {
  }

  public int NetworkcurItem
  {
    get
    {
      return this.curItem;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.curItem;
      int num2 = 2;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetCurItem(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<int>(num1, ref local, (uint) num2);
    }
  }

  public int NetworkitemUniq
  {
    get
    {
      return this.itemUniq;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.itemUniq;
      int num2 = 4;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetUniq(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<int>(num1, ref local, (uint) num2);
    }
  }

  protected static void InvokeSyncListitems(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "SyncList items called on server.");
    else
      ((Inventory) obj).items.HandleMsg(reader);
  }

  protected static void InvokeCmdCmdSetUnic(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSetUnic called on client.");
    else
      ((Inventory) obj).CmdSetUnic((int) reader.ReadPackedUInt32());
  }

  protected static void InvokeCmdCmdSyncItem(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSyncItem called on client.");
    else
      ((Inventory) obj).CmdSyncItem((int) reader.ReadPackedUInt32());
  }

  protected static void InvokeCmdCmdDropItem(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdDropItem called on client.");
    else
      ((Inventory) obj).CmdDropItem((int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32());
  }

  public void CallCmdSetUnic(int i)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSetUnic called on server.");
    else if (this.isServer)
    {
      this.CmdSetUnic(i);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Inventory.kCmdCmdSetUnic);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) i);
      this.SendCommandInternal(writer, 2, "CmdSetUnic");
    }
  }

  public void CallCmdSyncItem(int i)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSyncItem called on server.");
    else if (this.isServer)
    {
      this.CmdSyncItem(i);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Inventory.kCmdCmdSyncItem);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) i);
      this.SendCommandInternal(writer, 3, "CmdSyncItem");
    }
  }

  public void CallCmdDropItem(int itemInventoryIndex, int itemId)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdDropItem called on server.");
    else if (this.isServer)
    {
      this.CmdDropItem(itemInventoryIndex, itemId);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Inventory.kCmdCmdDropItem);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) itemInventoryIndex);
      writer.WritePackedUInt32((uint) itemId);
      this.SendCommandInternal(writer, 2, "CmdDropItem");
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      GeneratedNetworkCode._WriteStructSyncListItemInfo_Inventory(writer, this.items);
      writer.WritePackedUInt32((uint) this.curItem);
      writer.WritePackedUInt32((uint) this.itemUniq);
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
      GeneratedNetworkCode._WriteStructSyncListItemInfo_Inventory(writer, this.items);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.curItem);
    }
    if (((int) this.syncVarDirtyBits & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.itemUniq);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      GeneratedNetworkCode._ReadStructSyncListItemInfo_Inventory(reader, this.items);
      this.curItem = (int) reader.ReadPackedUInt32();
      this.itemUniq = (int) reader.ReadPackedUInt32();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        GeneratedNetworkCode._ReadStructSyncListItemInfo_Inventory(reader, this.items);
      if ((num & 2) != 0)
        this.SetCurItem((int) reader.ReadPackedUInt32());
      if ((num & 4) == 0)
        return;
      this.SetUniq((int) reader.ReadPackedUInt32());
    }
  }

  [Serializable]
  public struct SyncItemInfo
  {
    public int id;
    public float durability;
    public int uniq;
  }

  public class SyncListItemInfo : SyncListStruct<Inventory.SyncItemInfo>
  {
    public void ModifyDuration(int index, float value)
    {
      Inventory.SyncItemInfo syncItemInfo = this[index];
      syncItemInfo.durability = value;
      this[index] = syncItemInfo;
    }

    public override void SerializeItem(NetworkWriter writer, Inventory.SyncItemInfo item)
    {
      writer.WritePackedUInt32((uint) item.id);
      writer.Write(item.durability);
      writer.WritePackedUInt32((uint) item.uniq);
    }

    public override Inventory.SyncItemInfo DeserializeItem(NetworkReader reader)
    {
      return new Inventory.SyncItemInfo() { id = (int) reader.ReadPackedUInt32(), durability = reader.ReadSingle(), uniq = (int) reader.ReadPackedUInt32() };
    }
  }
}
