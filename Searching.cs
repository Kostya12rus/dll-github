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

  public Searching()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.fpc = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
    this.cam = ((Scp049PlayerScript) ((Component) this).GetComponent<Scp049PlayerScript>()).plyCam.get_transform();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.inv = (Inventory) ((Component) this).GetComponent<Inventory>();
    this.overloaderror = UserMainInterface.singleton.overloadMsg;
    this.progress = UserMainInterface.singleton.searchProgress;
    this.progressGO = UserMainInterface.singleton.searchOBJ;
    this.ammobox = (AmmoBox) ((Component) this).GetComponent<AmmoBox>();
  }

  public void Init(bool isNotHuman)
  {
    this.isHuman = !isNotHuman;
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer())
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
      this.errorMsgDur -= Time.get_deltaTime();
    this.overloaderror.SetActive((double) this.errorMsgDur > 0.0);
  }

  private void ContinuePickup()
  {
    if (Object.op_Inequality((Object) this.pickup, (Object) null))
    {
      if (!Input.GetKey(NewInput.GetKey("Interact")))
      {
        this.pickup = (GameObject) null;
        this.fpc.isSearching = (__Null) 0;
        this.progressGO.SetActive(false);
      }
      else
      {
        this.timeToPickUp -= Time.get_deltaTime();
        this.progressGO.SetActive(true);
        this.progress.set_value(this.progress.get_maxValue() - this.timeToPickUp);
        if ((double) this.timeToPickUp > 0.0)
          return;
        if (Object.op_Inequality((Object) this.pickup.GetComponent<Pickup>(), (Object) null))
        {
          foreach (WeaponManager.Weapon weapon in ((WeaponManager) ((Component) this).GetComponent<WeaponManager>()).weapons)
          {
            if (weapon.inventoryID == ((Pickup) this.pickup.GetComponent<Pickup>()).info.itemId)
              AchievementManager.Achieve("thatcanbeusefull");
          }
        }
        this.progressGO.SetActive(false);
        this.CallCmdPickupItem(this.pickup);
        this.fpc.isSearching = (__Null) 0;
        this.pickup = (GameObject) null;
      }
    }
    else
    {
      this.fpc.isSearching = (__Null) 0;
      this.progressGO.SetActive(false);
    }
  }

  private void Raycast()
  {
    RaycastHit raycastHit;
    if (!Input.GetKeyDown(NewInput.GetKey("Interact")) || !this.AllowPickup() || !Physics.Raycast(new Ray(this.cam.get_position(), this.cam.get_forward()), ref raycastHit, this.rayDistance, LayerMask.op_Implicit(((PlayerInteract) ((Component) this).GetComponent<PlayerInteract>()).mask)))
      return;
    Pickup componentInParent1 = (Pickup) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<Pickup>();
    Locker componentInParent2 = (Locker) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<Locker>();
    if (Object.op_Inequality((Object) componentInParent1, (Object) null))
    {
      if (this.inv.items.get_Count() < (ushort) 8 || this.inv.availableItems[componentInParent1.info.itemId].noEquipable)
      {
        this.timeToPickUp = componentInParent1.searchTime;
        this.progress.set_maxValue(componentInParent1.searchTime);
        this.fpc.isSearching = (__Null) 1;
        this.pickup = ((Component) componentInParent1).get_gameObject();
      }
      else
        this.ShowErrorMessage();
    }
    if (!Object.op_Inequality((Object) componentInParent2, (Object) null))
      return;
    if (this.inv.items.get_Count() < (ushort) 8)
    {
      this.timeToPickUp = componentInParent2.searchTime;
      this.progress.set_maxValue(componentInParent2.searchTime);
      this.fpc.isSearching = (__Null) 1;
      this.pickup = ((Component) componentInParent2).get_gameObject();
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
      if (Object.op_Equality((Object) ((Handcuffs) player.GetComponent<Handcuffs>()).cuffTarget, (Object) ((Component) this).get_gameObject()))
        return false;
    }
    return true;
  }

  [Command(channel = 2)]
  private void CmdPickupItem(GameObject t)
  {
    if (Object.op_Equality((Object) t, (Object) null) || !this.ccm.IsHuman() || (double) Vector3.Distance(((PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>()).position, t.get_transform().get_position()) > 3.5)
      return;
    int id = -1;
    Pickup component1 = (Pickup) t.GetComponent<Pickup>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      id = component1.info.itemId;
      component1.Delete();
    }
    Locker component2 = (Locker) t.GetComponent<Locker>();
    if (Object.op_Inequality((Object) component2, (Object) null) && !component2.isTaken)
    {
      id = component2.GetItem();
      component2.SetTaken(true);
    }
    if (id == -1)
      return;
    this.AddItem(id, !Object.op_Equality((Object) t.GetComponent<Pickup>(), (Object) null) ? component1.info.durability : -1f);
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
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdPickupItem called on client.");
    else
      ((Searching) obj).CmdPickupItem((GameObject) reader.ReadGameObject());
  }

  public void CallCmdPickupItem(GameObject t)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdPickupItem called on server.");
    else if (this.get_isServer())
    {
      this.CmdPickupItem(t);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Searching.kCmdCmdPickupItem);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) t);
      this.SendCommandInternal(networkWriter, 2, "CmdPickupItem");
    }
  }

  static Searching()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Searching), Searching.kCmdCmdPickupItem, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdPickupItem)));
    NetworkCRC.RegisterBehaviour(nameof (Searching), 0);
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
