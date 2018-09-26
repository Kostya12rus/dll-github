// Decompiled with JetBrains decompiler
// Type: PlayerInteract
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerInteract : NetworkBehaviour
{
  private static int kCmdCmdUse914 = -1419322708;
  public GameObject playerCamera;
  public LayerMask mask;
  public float raycastMaxDistance;
  private CharacterClassManager _ccm;
  private ServerRoles _sr;
  private Inventory _inv;
  private static int kCmdCmdChange914knob;
  private static int kRpcRpcUse914;
  private static int kCmdCmdUseWorkStation_Place;
  private static int kCmdCmdUseWorkStation_Take;
  private static int kCmdCmdUsePanel;
  private static int kRpcRpcLeverSound;
  private static int kCmdCmdUseElevator;
  private static int kCmdCmdSwitchAWButton;
  private static int kCmdCmdDetonateWarhead;
  private static int kCmdCmdOpenDoor;
  private static int kRpcRpcDenied;
  private static int kCmdCmdContain106;
  private static int kRpcRpcContain106;

  public PlayerInteract()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this._ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this._sr = (ServerRoles) ((Component) this).GetComponent<ServerRoles>();
    this._inv = (Inventory) ((Component) this).GetComponent<Inventory>();
  }

  private void Update()
  {
    RaycastHit raycastHit;
    if (!this.get_isLocalPlayer() || !Input.GetKeyDown(NewInput.GetKey("Interact")) || (((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).curClass == 2 || !Physics.Raycast(this.playerCamera.get_transform().get_position(), this.playerCamera.get_transform().get_forward(), ref raycastHit, this.raycastMaxDistance, LayerMask.op_Implicit(this.mask))))
      return;
    if (Object.op_Inequality((Object) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<Door>(), (Object) null))
      this.CallCmdOpenDoor(((Component) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<Door>()).get_gameObject());
    else if (((Component) ((RaycastHit) ref raycastHit).get_transform()).CompareTag("AW_Button"))
    {
      if (this._inv.curItem != 0 && ((IEnumerable<string>) this._inv.availableItems[Mathf.Clamp(this._inv.curItem, 0, this._inv.availableItems.Length - 1)].permissions).Any<string>((Func<string, bool>) (item => item == "CONT_LVL_3")))
      {
        this.CallCmdSwitchAWButton();
      }
      else
      {
        ((Behaviour) GameObject.Find("Keycard Denied Text").GetComponent<Text>()).set_enabled(true);
        ((MonoBehaviour) this).Invoke("DisableDeniedText", 1f);
      }
    }
    else if (((Component) ((RaycastHit) ref raycastHit).get_transform()).CompareTag("AW_Detonation"))
    {
      if (!AlphaWarheadOutsitePanel.nukeside.enabled || AlphaWarheadController.host.inProgress)
        return;
      this.CallCmdDetonateWarhead();
    }
    else if (((Component) ((RaycastHit) ref raycastHit).get_transform()).CompareTag("AW_Panel"))
      this.CallCmdUsePanel(((Object) ((RaycastHit) ref raycastHit).get_transform()).get_name());
    else if (((Component) ((RaycastHit) ref raycastHit).get_transform()).CompareTag("914_use"))
      this.CallCmdUse914();
    else if (((Component) ((RaycastHit) ref raycastHit).get_transform()).CompareTag("914_knob"))
      this.CallCmdChange914knob();
    else if (((Component) ((RaycastHit) ref raycastHit).get_transform()).CompareTag("ElevatorButton"))
    {
      foreach (Lift lift in (Lift[]) Object.FindObjectsOfType<Lift>())
      {
        foreach (Lift.Elevator elevator in lift.elevators)
        {
          if (this.ChckDis(((Component) elevator.door).get_transform().get_position(), 1f))
            this.CallCmdUseElevator(((Component) ((Component) lift).get_transform()).get_gameObject());
        }
      }
    }
    else if (((Component) ((RaycastHit) ref raycastHit).get_transform()).CompareTag("FemurBreaker"))
    {
      this.CallCmdContain106();
    }
    else
    {
      if (!((Component) ((RaycastHit) ref raycastHit).get_collider()).CompareTag("WS"))
        return;
      ((WorkStation) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponentInParent<WorkStation>()).UseButton((Button) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponent<Button>());
    }
  }

  [Command(channel = 4)]
  private void CmdUse914()
  {
    if (Scp914.singleton.working || !this.ChckDis(GameObject.FindGameObjectWithTag("914_use").get_transform().get_position(), 1f))
      return;
    this.CallRpcUse914();
  }

  [Command(channel = 4)]
  private void CmdChange914knob()
  {
    if (Scp914.singleton.working || !this.ChckDis(GameObject.FindGameObjectWithTag("914_use").get_transform().get_position(), 1f))
      return;
    Scp914.singleton.ChangeKnobStatus();
  }

  [ClientRpc(channel = 4)]
  private void RpcUse914()
  {
    Scp914.singleton.StartRefining();
  }

  [Command(channel = 4)]
  public void CmdUseWorkStation_Place(GameObject station)
  {
    if (!this.ChckDis(station.get_transform().get_position(), 1f))
      return;
    ((WorkStation) station.GetComponent<WorkStation>()).ConnectTablet(((Component) this).get_gameObject());
  }

  [Command(channel = 4)]
  public void CmdUseWorkStation_Take(GameObject station)
  {
    if (!this.ChckDis(station.get_transform().get_position(), 1f))
      return;
    ((WorkStation) station.GetComponent<WorkStation>()).UnconnectTablet(((Component) this).get_gameObject());
  }

  [Command(channel = 4)]
  private void CmdUsePanel(string n)
  {
    AlphaWarheadNukesitePanel nukeside = AlphaWarheadOutsitePanel.nukeside;
    if (!this.ChckDis(((Component) nukeside).get_transform().get_position(), 1f))
      return;
    if (n.Contains("cancel"))
    {
      AlphaWarheadController.host.CancelDetonation(((Component) this).get_gameObject());
    }
    else
    {
      if (!n.Contains("lever") || !nukeside.AllowChangeLevelState())
        return;
      nukeside.Networkenabled = !nukeside.enabled;
      this.CallRpcLeverSound();
    }
  }

  [ClientRpc(channel = 4)]
  private void RpcLeverSound()
  {
    ((AudioSource) ((Component) AlphaWarheadOutsitePanel.nukeside.lever).GetComponent<AudioSource>()).Play();
  }

  [Command(channel = 4)]
  private void CmdUseElevator(GameObject elevator)
  {
    foreach (Lift.Elevator elevator1 in ((Lift) elevator.GetComponent<Lift>()).elevators)
    {
      if (this.ChckDis(((Component) elevator1.door).get_transform().get_position(), 1f))
        ((Lift) elevator.GetComponent<Lift>()).UseLift();
    }
  }

  [Command(channel = 4)]
  private void CmdSwitchAWButton()
  {
    GameObject gameObject = GameObject.Find("OutsitePanelScript");
    if (!this.ChckDis(gameObject.get_transform().get_position(), 1f) || !((IEnumerable<string>) this._inv.availableItems[this._inv.curItem].permissions).Any<string>((Func<string, bool>) (item => item == "CONT_LVL_3")))
      return;
    ((AlphaWarheadOutsitePanel) gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>()).SetKeycardState(true);
  }

  [Command(channel = 4)]
  private void CmdDetonateWarhead()
  {
    GameObject gameObject = GameObject.Find("OutsitePanelScript");
    if (!this.ChckDis(gameObject.get_transform().get_position(), 1f) || !AlphaWarheadOutsitePanel.nukeside.enabled || !((AlphaWarheadOutsitePanel) gameObject.GetComponent<AlphaWarheadOutsitePanel>()).keycardEntered)
      return;
    AlphaWarheadController.host.StartDetonation();
  }

  [Command(channel = 14)]
  private void CmdOpenDoor(GameObject doorId)
  {
    Door door = (Door) doorId.GetComponent<Door>();
    if (!(door.buttons.Count != 0 ? ((IEnumerable<GameObject>) door.buttons).Any<GameObject>((Func<GameObject, bool>) (item => this.ChckDis(item.get_transform().get_position(), 1f))) : this.ChckDis(doorId.get_transform().get_position(), 1f)))
      return;
    Scp096PlayerScript component = (Scp096PlayerScript) ((Component) this).GetComponent<Scp096PlayerScript>();
    if (Object.op_Inequality((Object) door.destroyedPrefab, (Object) null) && (!door.isOpen || (double) door.curCooldown > 0.0) && (component.iAm096 && component.enraged == Scp096PlayerScript.RageState.Enraged))
    {
      if (door.locked && !this._sr.BypassMode)
        return;
      door.DestroyDoor(true);
    }
    else if (this._sr.BypassMode)
    {
      door.ChangeState(true);
    }
    else
    {
      if (door.permissionLevel.ToUpper() == "CHCKPOINT_ACC")
      {
        if (((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).klasy[((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).curClass].team == Team.SCP)
        {
          door.ChangeState(false);
          return;
        }
      }
      try
      {
        if (string.IsNullOrEmpty(door.permissionLevel))
        {
          if (door.locked)
            return;
          door.ChangeState(false);
        }
        else if (((IEnumerable<string>) this._inv.availableItems[this._inv.curItem].permissions).Any<string>((Func<string, bool>) (item => item == door.permissionLevel)))
        {
          if (!door.locked)
            door.ChangeState(false);
          else
            this.CallRpcDenied(doorId);
        }
        else
          this.CallRpcDenied(doorId);
      }
      catch
      {
        this.CallRpcDenied(doorId);
      }
    }
  }

  [ClientRpc(channel = 14)]
  private void RpcDenied(GameObject door)
  {
    Timing.RunCoroutine(((Door) door.GetComponent<Door>())._Denied(), (Segment) 0);
  }

  private bool ChckDis(Vector3 pos, float distanceMultiplier = 1f)
  {
    if (TutorialManager.status)
      return true;
    return (double) Vector3.Distance(((PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>()).position, pos) < (double) this.raycastMaxDistance * 1.5;
  }

  [Command(channel = 4)]
  private void CmdContain106()
  {
    if (!((LureSubjectContainer) Object.FindObjectOfType<LureSubjectContainer>()).allowContain || this._ccm.klasy[this._ccm.curClass].team == Team.SCP && this._ccm.curClass != 3 || (!this.ChckDis(GameObject.FindGameObjectWithTag("FemurBreaker").get_transform().get_position(), 1f) || ((OneOhSixContainer) Object.FindObjectOfType<OneOhSixContainer>()).used || this._ccm.klasy[this._ccm.curClass].team == Team.RIP))
      return;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (((CharacterClassManager) player.GetComponent<CharacterClassManager>()).curClass == 3)
        ((Scp106PlayerScript) player.GetComponent<Scp106PlayerScript>()).Contain(this._ccm);
    }
    this.CallRpcContain106(((Component) this).get_gameObject());
    ((OneOhSixContainer) Object.FindObjectOfType<OneOhSixContainer>()).SetState(true);
  }

  [ClientRpc(channel = 4)]
  private void RpcContain106(GameObject executor)
  {
    Object.Instantiate<GameObject>((M0) ((Scp106PlayerScript) ((Component) this).GetComponent<Scp106PlayerScript>()).screamsPrefab);
    if (Object.op_Inequality((Object) executor, (Object) ((Component) this).get_gameObject()))
      return;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (((CharacterClassManager) player.GetComponent<CharacterClassManager>()).curClass == 3)
        AchievementManager.Achieve("securecontainprotect");
    }
  }

  private void DisableDeniedText()
  {
    ((Behaviour) GameObject.Find("Keycard Denied Text").GetComponent<Text>()).set_enabled(false);
    HintManager.singleton.AddHint(1);
  }

  private void DisableAlphaText()
  {
    ((Behaviour) GameObject.Find("Alpha Denied Text").GetComponent<Text>()).set_enabled(false);
    HintManager.singleton.AddHint(2);
  }

  private void DisableLockText()
  {
    ((Behaviour) GameObject.Find("Lock Denied Text").GetComponent<Text>()).set_enabled(false);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdUse914(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUse914 called on client.");
    else
      ((PlayerInteract) obj).CmdUse914();
  }

  protected static void InvokeCmdCmdChange914knob(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdChange914knob called on client.");
    else
      ((PlayerInteract) obj).CmdChange914knob();
  }

  protected static void InvokeCmdCmdUseWorkStation_Place(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUseWorkStation_Place called on client.");
    else
      ((PlayerInteract) obj).CmdUseWorkStation_Place((GameObject) reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdUseWorkStation_Take(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUseWorkStation_Take called on client.");
    else
      ((PlayerInteract) obj).CmdUseWorkStation_Take((GameObject) reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdUsePanel(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUsePanel called on client.");
    else
      ((PlayerInteract) obj).CmdUsePanel(reader.ReadString());
  }

  protected static void InvokeCmdCmdUseElevator(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUseElevator called on client.");
    else
      ((PlayerInteract) obj).CmdUseElevator((GameObject) reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdSwitchAWButton(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSwitchAWButton called on client.");
    else
      ((PlayerInteract) obj).CmdSwitchAWButton();
  }

  protected static void InvokeCmdCmdDetonateWarhead(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdDetonateWarhead called on client.");
    else
      ((PlayerInteract) obj).CmdDetonateWarhead();
  }

  protected static void InvokeCmdCmdOpenDoor(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdOpenDoor called on client.");
    else
      ((PlayerInteract) obj).CmdOpenDoor((GameObject) reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdContain106(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdContain106 called on client.");
    else
      ((PlayerInteract) obj).CmdContain106();
  }

  public void CallCmdUse914()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUse914 called on server.");
    else if (this.get_isServer())
    {
      this.CmdUse914();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUse914);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 4, "CmdUse914");
    }
  }

  public void CallCmdChange914knob()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdChange914knob called on server.");
    else if (this.get_isServer())
    {
      this.CmdChange914knob();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdChange914knob);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 4, "CmdChange914knob");
    }
  }

  public void CallCmdUseWorkStation_Place(GameObject station)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUseWorkStation_Place called on server.");
    else if (this.get_isServer())
    {
      this.CmdUseWorkStation_Place(station);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUseWorkStation_Place);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) station);
      this.SendCommandInternal(networkWriter, 4, "CmdUseWorkStation_Place");
    }
  }

  public void CallCmdUseWorkStation_Take(GameObject station)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUseWorkStation_Take called on server.");
    else if (this.get_isServer())
    {
      this.CmdUseWorkStation_Take(station);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUseWorkStation_Take);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) station);
      this.SendCommandInternal(networkWriter, 4, "CmdUseWorkStation_Take");
    }
  }

  public void CallCmdUsePanel(string n)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUsePanel called on server.");
    else if (this.get_isServer())
    {
      this.CmdUsePanel(n);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUsePanel);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(n);
      this.SendCommandInternal(networkWriter, 4, "CmdUsePanel");
    }
  }

  public void CallCmdUseElevator(GameObject elevator)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUseElevator called on server.");
    else if (this.get_isServer())
    {
      this.CmdUseElevator(elevator);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUseElevator);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) elevator);
      this.SendCommandInternal(networkWriter, 4, "CmdUseElevator");
    }
  }

  public void CallCmdSwitchAWButton()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSwitchAWButton called on server.");
    else if (this.get_isServer())
    {
      this.CmdSwitchAWButton();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdSwitchAWButton);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 4, "CmdSwitchAWButton");
    }
  }

  public void CallCmdDetonateWarhead()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdDetonateWarhead called on server.");
    else if (this.get_isServer())
    {
      this.CmdDetonateWarhead();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdDetonateWarhead);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 4, "CmdDetonateWarhead");
    }
  }

  public void CallCmdOpenDoor(GameObject doorId)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdOpenDoor called on server.");
    else if (this.get_isServer())
    {
      this.CmdOpenDoor(doorId);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdOpenDoor);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) doorId);
      this.SendCommandInternal(networkWriter, 14, "CmdOpenDoor");
    }
  }

  public void CallCmdContain106()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdContain106 called on server.");
    else if (this.get_isServer())
    {
      this.CmdContain106();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kCmdCmdContain106);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 4, "CmdContain106");
    }
  }

  protected static void InvokeRpcRpcUse914(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcUse914 called on server.");
    else
      ((PlayerInteract) obj).RpcUse914();
  }

  protected static void InvokeRpcRpcLeverSound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcLeverSound called on server.");
    else
      ((PlayerInteract) obj).RpcLeverSound();
  }

  protected static void InvokeRpcRpcDenied(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcDenied called on server.");
    else
      ((PlayerInteract) obj).RpcDenied((GameObject) reader.ReadGameObject());
  }

  protected static void InvokeRpcRpcContain106(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcContain106 called on server.");
    else
      ((PlayerInteract) obj).RpcContain106((GameObject) reader.ReadGameObject());
  }

  public void CallRpcUse914()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcUse914 called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kRpcRpcUse914);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 4, "RpcUse914");
    }
  }

  public void CallRpcLeverSound()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcLeverSound called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kRpcRpcLeverSound);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 4, "RpcLeverSound");
    }
  }

  public void CallRpcDenied(GameObject door)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcDenied called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kRpcRpcDenied);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) door);
      this.SendRPCInternal(networkWriter, 14, "RpcDenied");
    }
  }

  public void CallRpcContain106(GameObject executor)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcContain106 called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlayerInteract.kRpcRpcContain106);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) executor);
      this.SendRPCInternal(networkWriter, 4, "RpcContain106");
    }
  }

  static PlayerInteract()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUse914, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUse914)));
    PlayerInteract.kCmdCmdChange914knob = -845424245;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdChange914knob, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdChange914knob)));
    PlayerInteract.kCmdCmdUseWorkStation_Place = 1646281979;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUseWorkStation_Place, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUseWorkStation_Place)));
    PlayerInteract.kCmdCmdUseWorkStation_Take = -1055163885;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUseWorkStation_Take, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUseWorkStation_Take)));
    PlayerInteract.kCmdCmdUsePanel = 1853207668;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUsePanel, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUsePanel)));
    PlayerInteract.kCmdCmdUseElevator = 339400830;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUseElevator, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUseElevator)));
    PlayerInteract.kCmdCmdSwitchAWButton = -710673229;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdSwitchAWButton, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSwitchAWButton)));
    PlayerInteract.kCmdCmdDetonateWarhead = -151679759;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdDetonateWarhead, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdDetonateWarhead)));
    PlayerInteract.kCmdCmdOpenDoor = 1645579471;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdOpenDoor, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdOpenDoor)));
    PlayerInteract.kCmdCmdContain106 = 1084648090;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdContain106, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdContain106)));
    PlayerInteract.kRpcRpcUse914 = -637254142;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerInteract), PlayerInteract.kRpcRpcUse914, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcUse914)));
    PlayerInteract.kRpcRpcLeverSound = -829118990;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerInteract), PlayerInteract.kRpcRpcLeverSound, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcLeverSound)));
    PlayerInteract.kRpcRpcDenied = -1136563096;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerInteract), PlayerInteract.kRpcRpcDenied, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcDenied)));
    PlayerInteract.kRpcRpcContain106 = -1051575568;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerInteract), PlayerInteract.kRpcRpcContain106, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcContain106)));
    NetworkCRC.RegisterBehaviour(nameof (PlayerInteract), 0);
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
