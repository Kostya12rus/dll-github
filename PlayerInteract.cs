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

  private void Start()
  {
    this._ccm = this.GetComponent<CharacterClassManager>();
    this._sr = this.GetComponent<ServerRoles>();
    this._inv = this.GetComponent<Inventory>();
  }

  private void Update()
  {
    RaycastHit hitInfo;
    if (!this.isLocalPlayer || !Input.GetKeyDown(NewInput.GetKey("Interact")) || (this.GetComponent<CharacterClassManager>().curClass == 2 || !Physics.Raycast(this.playerCamera.transform.position, this.playerCamera.transform.forward, out hitInfo, this.raycastMaxDistance, (int) this.mask)))
      return;
    if ((Object) hitInfo.transform.GetComponentInParent<Door>() != (Object) null)
      this.CallCmdOpenDoor(hitInfo.transform.GetComponentInParent<Door>().gameObject);
    else if (hitInfo.transform.CompareTag("AW_Button"))
    {
      if (this._inv.curItem != 0 && ((IEnumerable<string>) this._inv.availableItems[Mathf.Clamp(this._inv.curItem, 0, this._inv.availableItems.Length - 1)].permissions).Any<string>((Func<string, bool>) (item => item == "CONT_LVL_3")))
      {
        this.CallCmdSwitchAWButton();
      }
      else
      {
        GameObject.Find("Keycard Denied Text").GetComponent<Text>().enabled = true;
        this.Invoke("DisableDeniedText", 1f);
      }
    }
    else if (hitInfo.transform.CompareTag("AW_Detonation"))
    {
      if (!AlphaWarheadOutsitePanel.nukeside.enabled || AlphaWarheadController.host.inProgress)
        return;
      this.CallCmdDetonateWarhead();
    }
    else if (hitInfo.transform.CompareTag("AW_Panel"))
      this.CallCmdUsePanel(hitInfo.transform.name);
    else if (hitInfo.transform.CompareTag("914_use"))
      this.CallCmdUse914();
    else if (hitInfo.transform.CompareTag("914_knob"))
      this.CallCmdChange914knob();
    else if (hitInfo.transform.CompareTag("ElevatorButton"))
    {
      foreach (Lift lift in Object.FindObjectsOfType<Lift>())
      {
        foreach (Lift.Elevator elevator in lift.elevators)
        {
          if (this.ChckDis(elevator.door.transform.position, 1f))
            this.CallCmdUseElevator(lift.transform.gameObject);
        }
      }
    }
    else if (hitInfo.transform.CompareTag("FemurBreaker"))
    {
      this.CallCmdContain106();
    }
    else
    {
      if (!hitInfo.collider.CompareTag("WS"))
        return;
      hitInfo.collider.GetComponentInParent<WorkStation>().UseButton(hitInfo.collider.GetComponent<Button>());
    }
  }

  [Command(channel = 4)]
  private void CmdUse914()
  {
    if (Scp914.singleton.working || !this.ChckDis(GameObject.FindGameObjectWithTag("914_use").transform.position, 1f))
      return;
    this.CallRpcUse914();
  }

  [Command(channel = 4)]
  private void CmdChange914knob()
  {
    if (Scp914.singleton.working || !this.ChckDis(GameObject.FindGameObjectWithTag("914_use").transform.position, 1f))
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
    if (!this.ChckDis(station.transform.position, 1f))
      return;
    station.GetComponent<WorkStation>().ConnectTablet(this.gameObject);
  }

  [Command(channel = 4)]
  public void CmdUseWorkStation_Take(GameObject station)
  {
    if (!this.ChckDis(station.transform.position, 1f))
      return;
    station.GetComponent<WorkStation>().UnconnectTablet(this.gameObject);
  }

  [Command(channel = 4)]
  private void CmdUsePanel(string n)
  {
    AlphaWarheadNukesitePanel nukeside = AlphaWarheadOutsitePanel.nukeside;
    if (!this.ChckDis(nukeside.transform.position, 1f))
      return;
    if (n.Contains("cancel"))
    {
      AlphaWarheadController.host.CancelDetonation(this.gameObject);
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
    AlphaWarheadOutsitePanel.nukeside.lever.GetComponent<AudioSource>().Play();
  }

  [Command(channel = 4)]
  private void CmdUseElevator(GameObject elevator)
  {
    foreach (Lift.Elevator elevator1 in elevator.GetComponent<Lift>().elevators)
    {
      if (this.ChckDis(elevator1.door.transform.position, 1f))
        elevator.GetComponent<Lift>().UseLift();
    }
  }

  [Command(channel = 4)]
  private void CmdSwitchAWButton()
  {
    GameObject gameObject = GameObject.Find("OutsitePanelScript");
    if (!this.ChckDis(gameObject.transform.position, 1f) || !((IEnumerable<string>) this._inv.availableItems[this._inv.curItem].permissions).Any<string>((Func<string, bool>) (item => item == "CONT_LVL_3")))
      return;
    gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().SetKeycardState(true);
  }

  [Command(channel = 4)]
  private void CmdDetonateWarhead()
  {
    GameObject gameObject = GameObject.Find("OutsitePanelScript");
    if (!this.ChckDis(gameObject.transform.position, 1f) || !AlphaWarheadOutsitePanel.nukeside.enabled || !gameObject.GetComponent<AlphaWarheadOutsitePanel>().keycardEntered)
      return;
    AlphaWarheadController.host.StartDetonation();
  }

  [Command(channel = 14)]
  private void CmdOpenDoor(GameObject doorId)
  {
    Door door = doorId.GetComponent<Door>();
    if (!(door.buttons.Count != 0 ? door.buttons.Any<GameObject>((Func<GameObject, bool>) (item => this.ChckDis(item.transform.position, 1f))) : this.ChckDis(doorId.transform.position, 1f)))
      return;
    Scp096PlayerScript component = this.GetComponent<Scp096PlayerScript>();
    if ((Object) door.destroyedPrefab != (Object) null && (!door.isOpen || (double) door.curCooldown > 0.0) && (component.iAm096 && component.enraged == Scp096PlayerScript.RageState.Enraged))
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
        if (this.GetComponent<CharacterClassManager>().klasy[this.GetComponent<CharacterClassManager>().curClass].team == Team.SCP)
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
    Timing.RunCoroutine(door.GetComponent<Door>()._Denied(), Segment.Update);
  }

  private bool ChckDis(Vector3 pos, float distanceMultiplier = 1f)
  {
    if (TutorialManager.status)
      return true;
    return (double) Vector3.Distance(this.GetComponent<PlyMovementSync>().position, pos) < (double) this.raycastMaxDistance * 1.5;
  }

  [Command(channel = 4)]
  private void CmdContain106()
  {
    if (!Object.FindObjectOfType<LureSubjectContainer>().allowContain || this._ccm.klasy[this._ccm.curClass].team == Team.SCP && this._ccm.curClass != 3 || (!this.ChckDis(GameObject.FindGameObjectWithTag("FemurBreaker").transform.position, 1f) || Object.FindObjectOfType<OneOhSixContainer>().used || this._ccm.klasy[this._ccm.curClass].team == Team.RIP))
      return;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (player.GetComponent<CharacterClassManager>().curClass == 3)
        player.GetComponent<Scp106PlayerScript>().Contain(this._ccm);
    }
    this.CallRpcContain106(this.gameObject);
    Object.FindObjectOfType<OneOhSixContainer>().SetState(true);
  }

  [ClientRpc(channel = 4)]
  private void RpcContain106(GameObject executor)
  {
    Object.Instantiate<GameObject>(this.GetComponent<Scp106PlayerScript>().screamsPrefab);
    if ((Object) executor != (Object) this.gameObject)
      return;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (player.GetComponent<CharacterClassManager>().curClass == 3)
        AchievementManager.Achieve("securecontainprotect");
    }
  }

  private void DisableDeniedText()
  {
    GameObject.Find("Keycard Denied Text").GetComponent<Text>().enabled = false;
    HintManager.singleton.AddHint(1);
  }

  private void DisableAlphaText()
  {
    GameObject.Find("Alpha Denied Text").GetComponent<Text>().enabled = false;
    HintManager.singleton.AddHint(2);
  }

  private void DisableLockText()
  {
    GameObject.Find("Lock Denied Text").GetComponent<Text>().enabled = false;
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdUse914(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUse914 called on client.");
    else
      ((PlayerInteract) obj).CmdUse914();
  }

  protected static void InvokeCmdCmdChange914knob(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdChange914knob called on client.");
    else
      ((PlayerInteract) obj).CmdChange914knob();
  }

  protected static void InvokeCmdCmdUseWorkStation_Place(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUseWorkStation_Place called on client.");
    else
      ((PlayerInteract) obj).CmdUseWorkStation_Place(reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdUseWorkStation_Take(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUseWorkStation_Take called on client.");
    else
      ((PlayerInteract) obj).CmdUseWorkStation_Take(reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdUsePanel(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUsePanel called on client.");
    else
      ((PlayerInteract) obj).CmdUsePanel(reader.ReadString());
  }

  protected static void InvokeCmdCmdUseElevator(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUseElevator called on client.");
    else
      ((PlayerInteract) obj).CmdUseElevator(reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdSwitchAWButton(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSwitchAWButton called on client.");
    else
      ((PlayerInteract) obj).CmdSwitchAWButton();
  }

  protected static void InvokeCmdCmdDetonateWarhead(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdDetonateWarhead called on client.");
    else
      ((PlayerInteract) obj).CmdDetonateWarhead();
  }

  protected static void InvokeCmdCmdOpenDoor(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdOpenDoor called on client.");
    else
      ((PlayerInteract) obj).CmdOpenDoor(reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdContain106(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdContain106 called on client.");
    else
      ((PlayerInteract) obj).CmdContain106();
  }

  public void CallCmdUse914()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUse914 called on server.");
    else if (this.isServer)
    {
      this.CmdUse914();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUse914);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 4, "CmdUse914");
    }
  }

  public void CallCmdChange914knob()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdChange914knob called on server.");
    else if (this.isServer)
    {
      this.CmdChange914knob();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdChange914knob);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 4, "CmdChange914knob");
    }
  }

  public void CallCmdUseWorkStation_Place(GameObject station)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUseWorkStation_Place called on server.");
    else if (this.isServer)
    {
      this.CmdUseWorkStation_Place(station);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUseWorkStation_Place);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(station);
      this.SendCommandInternal(writer, 4, "CmdUseWorkStation_Place");
    }
  }

  public void CallCmdUseWorkStation_Take(GameObject station)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUseWorkStation_Take called on server.");
    else if (this.isServer)
    {
      this.CmdUseWorkStation_Take(station);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUseWorkStation_Take);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(station);
      this.SendCommandInternal(writer, 4, "CmdUseWorkStation_Take");
    }
  }

  public void CallCmdUsePanel(string n)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUsePanel called on server.");
    else if (this.isServer)
    {
      this.CmdUsePanel(n);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUsePanel);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(n);
      this.SendCommandInternal(writer, 4, "CmdUsePanel");
    }
  }

  public void CallCmdUseElevator(GameObject elevator)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUseElevator called on server.");
    else if (this.isServer)
    {
      this.CmdUseElevator(elevator);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdUseElevator);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(elevator);
      this.SendCommandInternal(writer, 4, "CmdUseElevator");
    }
  }

  public void CallCmdSwitchAWButton()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSwitchAWButton called on server.");
    else if (this.isServer)
    {
      this.CmdSwitchAWButton();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdSwitchAWButton);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 4, "CmdSwitchAWButton");
    }
  }

  public void CallCmdDetonateWarhead()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdDetonateWarhead called on server.");
    else if (this.isServer)
    {
      this.CmdDetonateWarhead();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdDetonateWarhead);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 4, "CmdDetonateWarhead");
    }
  }

  public void CallCmdOpenDoor(GameObject doorId)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdOpenDoor called on server.");
    else if (this.isServer)
    {
      this.CmdOpenDoor(doorId);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdOpenDoor);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(doorId);
      this.SendCommandInternal(writer, 14, "CmdOpenDoor");
    }
  }

  public void CallCmdContain106()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdContain106 called on server.");
    else if (this.isServer)
    {
      this.CmdContain106();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlayerInteract.kCmdCmdContain106);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 4, "CmdContain106");
    }
  }

  protected static void InvokeRpcRpcUse914(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcUse914 called on server.");
    else
      ((PlayerInteract) obj).RpcUse914();
  }

  protected static void InvokeRpcRpcLeverSound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcLeverSound called on server.");
    else
      ((PlayerInteract) obj).RpcLeverSound();
  }

  protected static void InvokeRpcRpcDenied(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcDenied called on server.");
    else
      ((PlayerInteract) obj).RpcDenied(reader.ReadGameObject());
  }

  protected static void InvokeRpcRpcContain106(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcContain106 called on server.");
    else
      ((PlayerInteract) obj).RpcContain106(reader.ReadGameObject());
  }

  public void CallRpcUse914()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcUse914 called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerInteract.kRpcRpcUse914);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 4, "RpcUse914");
    }
  }

  public void CallRpcLeverSound()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcLeverSound called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerInteract.kRpcRpcLeverSound);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 4, "RpcLeverSound");
    }
  }

  public void CallRpcDenied(GameObject door)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcDenied called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerInteract.kRpcRpcDenied);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(door);
      this.SendRPCInternal(writer, 14, "RpcDenied");
    }
  }

  public void CallRpcContain106(GameObject executor)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcContain106 called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerInteract.kRpcRpcContain106);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(executor);
      this.SendRPCInternal(writer, 4, "RpcContain106");
    }
  }

  static PlayerInteract()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUse914, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUse914));
    PlayerInteract.kCmdCmdChange914knob = -845424245;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdChange914knob, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdChange914knob));
    PlayerInteract.kCmdCmdUseWorkStation_Place = 1646281979;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUseWorkStation_Place, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUseWorkStation_Place));
    PlayerInteract.kCmdCmdUseWorkStation_Take = -1055163885;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUseWorkStation_Take, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUseWorkStation_Take));
    PlayerInteract.kCmdCmdUsePanel = 1853207668;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUsePanel, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUsePanel));
    PlayerInteract.kCmdCmdUseElevator = 339400830;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdUseElevator, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUseElevator));
    PlayerInteract.kCmdCmdSwitchAWButton = -710673229;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdSwitchAWButton, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdSwitchAWButton));
    PlayerInteract.kCmdCmdDetonateWarhead = -151679759;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdDetonateWarhead, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdDetonateWarhead));
    PlayerInteract.kCmdCmdOpenDoor = 1645579471;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdOpenDoor, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdOpenDoor));
    PlayerInteract.kCmdCmdContain106 = 1084648090;
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlayerInteract), PlayerInteract.kCmdCmdContain106, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdContain106));
    PlayerInteract.kRpcRpcUse914 = -637254142;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerInteract), PlayerInteract.kRpcRpcUse914, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeRpcRpcUse914));
    PlayerInteract.kRpcRpcLeverSound = -829118990;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerInteract), PlayerInteract.kRpcRpcLeverSound, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeRpcRpcLeverSound));
    PlayerInteract.kRpcRpcDenied = -1136563096;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerInteract), PlayerInteract.kRpcRpcDenied, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeRpcRpcDenied));
    PlayerInteract.kRpcRpcContain106 = -1051575568;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerInteract), PlayerInteract.kRpcRpcContain106, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeRpcRpcContain106));
    NetworkCRC.RegisterBehaviour(nameof (PlayerInteract), 0);
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
