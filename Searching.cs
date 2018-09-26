// Decompiled with JetBrains decompiler
// Type: Searching
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Searching : NetworkBehaviour
{
  private static int kCmdCmdPickupItem = 2021286825;
  private CharacterClassManager ccm;
  private Inventory inv;
  private bool isHuman;
  private GameObject pickup;
  private Transform cam;
  private FirstPersonController fpc;
  private AmmoBox ammobox;
  private float timeToPickUp;
  private float errorMsgDur;
  private GameObject overloaderror;
  private Slider progress;
  private GameObject progressGO;
  public float rayDistance;

  private void Start()
  {
    this.fpc = this.GetComponent<FirstPersonController>();
    this.cam = this.GetComponent<Scp049PlayerScript>().plyCam.transform;
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.inv = this.GetComponent<Inventory>();
    this.overloaderror = UserMainInterface.singleton.overloadMsg;
    this.progress = UserMainInterface.singleton.searchProgress;
    this.progressGO = UserMainInterface.singleton.searchOBJ;
    this.ammobox = this.GetComponent<AmmoBox>();
  }

  public void Init(bool isNotHuman)
  {
    this.isHuman = !isNotHuman;
  }

  private void Update()
  {
    if (!this.isLocalPlayer)
      return;
    this.Raycast();
    this.ContinuePickup();
    this.ErrorMessage();
  }

  public void ShowErrorMessage()
  {
    this.errorMsgDur = 2f;
  }

  private void ErrorMessage()
  {
    if ((double) this.errorMsgDur > 0.0)
      this.errorMsgDur -= Time.deltaTime;
    this.overloaderror.SetActive((double) this.errorMsgDur > 0.0);
  }

  private void ContinuePickup()
  {
    if ((Object) this.pickup != (Object) null)
    {
      if (!Input.GetKey(NewInput.GetKey("Interact")))
      {
        this.pickup = (GameObject) null;
        this.fpc.isSearching = false;
        this.progressGO.SetActive(false);
      }
      else
      {
        this.timeToPickUp -= Time.deltaTime;
        this.progressGO.SetActive(true);
        this.progress.value = this.progress.maxValue - this.timeToPickUp;
        if ((double) this.timeToPickUp > 0.0)
          return;
        if ((Object) this.pickup.GetComponent<Pickup>() != (Object) null)
        {
          foreach (WeaponManager.Weapon weapon in this.GetComponent<WeaponManager>().weapons)
          {
            if (weapon.inventoryID == this.pickup.GetComponent<Pickup>().info.itemId)
              AchievementManager.Achieve("thatcanbeusefull");
          }
        }
        this.progressGO.SetActive(false);
        this.CallCmdPickupItem(this.pickup);
        this.fpc.isSearching = false;
        this.pickup = (GameObject) null;
      }
    }
    else
    {
      this.fpc.isSearching = false;
      this.progressGO.SetActive(false);
    }
  }

  private void Raycast()
  {
    RaycastHit hitInfo;
    if (!Input.GetKeyDown(NewInput.GetKey("Interact")) || !this.AllowPickup() || !Physics.Raycast(new Ray(this.cam.position, this.cam.forward), out hitInfo, this.rayDistance, (int) this.GetComponent<PlayerInteract>().mask))
      return;
    Pickup componentInParent1 = hitInfo.transform.GetComponentInParent<Pickup>();
    Locker componentInParent2 = hitInfo.transform.GetComponentInParent<Locker>();
    if ((Object) componentInParent1 != (Object) null)
    {
      if (this.inv.items.Count < (ushort) 8 || this.inv.availableItems[componentInParent1.info.itemId].noEquipable)
      {
        this.timeToPickUp = componentInParent1.searchTime;
        this.progress.maxValue = componentInParent1.searchTime;
        this.fpc.isSearching = true;
        this.pickup = componentInParent1.gameObject;
      }
      else
        this.ShowErrorMessage();
    }
    if (!((Object) componentInParent2 != (Object) null))
      return;
    if (this.inv.items.Count < (ushort) 8)
    {
      this.timeToPickUp = componentInParent2.searchTime;
      this.progress.maxValue = componentInParent2.searchTime;
      this.fpc.isSearching = true;
      this.pickup = componentInParent2.gameObject;
    }
    else
      this.ShowErrorMessage();
  }

  private bool AllowPickup()
  {
    if (!this.isHuman)
      return false;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if ((Object) player.GetComponent<Handcuffs>().cuffTarget == (Object) this.gameObject)
        return false;
    }
    return true;
  }

  [Command(channel = 2)]
  private void CmdPickupItem(GameObject t)
  {
    if ((Object) t == (Object) null || !this.ccm.IsHuman() || (double) Vector3.Distance(this.GetComponent<PlyMovementSync>().position, t.transform.position) > 3.5)
      return;
    int id = -1;
    Pickup component1 = t.GetComponent<Pickup>();
    if ((Object) component1 != (Object) null)
    {
      id = component1.info.itemId;
      component1.Delete();
    }
    Locker component2 = t.GetComponent<Locker>();
    if ((Object) component2 != (Object) null && !component2.isTaken)
    {
      id = component2.GetItem();
      component2.SetTaken(true);
    }
    if (id == -1)
      return;
    this.AddItem(id, !((Object) t.GetComponent<Pickup>() == (Object) null) ? component1.info.durability : -1f);
  }

  public void AddItem(int id, float dur)
  {
    if (id == -1)
      return;
    if (!this.inv.availableItems[id].noEquipable)
    {
      this.inv.AddNewItem(id, (double) dur != -1.0 ? dur : this.inv.availableItems[id].durability);
    }
    else
    {
      string[] strArray = this.ammobox.amount.Split(':');
      for (int type = 0; type < 3; ++type)
      {
        if (this.ammobox.types[type].inventoryID == id)
          strArray[type] = ((float) this.ammobox.GetAmmo(type) + dur).ToString();
      }
      this.ammobox.Networkamount = strArray[0] + ":" + strArray[1] + ":" + strArray[2];
    }
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdPickupItem(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdPickupItem called on client.");
    else
      ((Searching) obj).CmdPickupItem(reader.ReadGameObject());
  }

  public void CallCmdPickupItem(GameObject t)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdPickupItem called on server.");
    else if (this.isServer)
    {
      this.CmdPickupItem(t);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Searching.kCmdCmdPickupItem);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(t);
      this.SendCommandInternal(writer, 2, "CmdPickupItem");
    }
  }

  static Searching()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Searching), Searching.kCmdCmdPickupItem, new NetworkBehaviour.CmdDelegate(Searching.InvokeCmdCmdPickupItem));
    NetworkCRC.RegisterBehaviour(nameof (Searching), 0);
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
