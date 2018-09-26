// Decompiled with JetBrains decompiler
// Type: Scp106PlayerScript
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Scp106PlayerScript : NetworkBehaviour
{
  private static int kCmdCmdMakePortal = 582440253;
  [Header("Player Properties")]
  public Transform plyCam;
  public bool iAm106;
  public bool sameClass;
  [SyncVar]
  private float ultimatePoints;
  public float teleportSpeed;
  public GameObject screamsPrefab;
  [Header("Portal")]
  [SyncVar(hook = "SetPortalPosition")]
  public Vector3 portalPosition;
  public GameObject portalPrefab;
  private Vector3 previousPortalPosition;
  private Offset modelOffset;
  private CharacterClassManager ccm;
  private FirstPersonController fpc;
  private GameObject popup106;
  private TextMeshProUGUI highlightedAbilityText;
  private Text pointsText;
  private string highlightedString;
  public int highlightID;
  private Image cooldownImg;
  private static BlastDoor blastDoor;
  private float attackCooldown;
  public bool goingViaThePortal;
  private bool isCollidingDoorOpen;
  private Door doorCurrentlyIn;
  private bool isHighlightingPoints;
  public LayerMask teleportPlacementMask;
  private static int kRpcRpcContainAnimation;
  private static int kRpcRpcTeleportAnimation;
  private static int kCmdCmdUsePortal;
  private static int kCmdCmdMovePlayer;

  public Scp106PlayerScript()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (Object.op_Equality((Object) Scp106PlayerScript.blastDoor, (Object) null))
      Scp106PlayerScript.blastDoor = (BlastDoor) Object.FindObjectOfType<BlastDoor>();
    this.cooldownImg = (Image) GameObject.Find("Cooldown106").GetComponent<Image>();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.fpc = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
    ((MonoBehaviour) this).InvokeRepeating("ExitDoor", 1f, 2f);
    if (this.get_isLocalPlayer() && NetworkServer.get_active())
      ((MonoBehaviour) this).InvokeRepeating("HumanPocketLoss", 1f, 1f);
    this.modelOffset = this.ccm.klasy[3].model_offset;
    if (!this.get_isLocalPlayer())
      return;
    this.pointsText = ((ScpInterfaces) Object.FindObjectOfType<ScpInterfaces>()).Scp106_ability_points;
    this.pointsText.set_text(TranslationReader.Get("Legancy_Interfaces", 11));
  }

  private void Update()
  {
    this.CheckForInventoryInput();
    this.CheckForShootInput();
    this.AnimateHighlightedText();
    this.UpdatePointText();
    this.DoorCollisionCheck();
  }

  [Server]
  private void HumanPocketLoss()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogWarning((object) "[Server] function 'System.Void Scp106PlayerScript::HumanPocketLoss()' called on client");
    }
    else
    {
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        if (player.get_transform().get_position().y < -1500.0 && ((CharacterClassManager) player.GetComponent<CharacterClassManager>()).IsHuman())
          ((PlayerStats) player.GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(1f, "WORLD", "POCKET", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), player);
      }
    }
  }

  private void CheckForShootInput()
  {
    if (!this.get_isLocalPlayer() || !this.iAm106)
      return;
    this.cooldownImg.set_fillAmount(Mathf.Clamp01((double) this.attackCooldown > 0.0 ? (float) (1.0 - (double) this.attackCooldown * 2.0) : 0.0f));
    if ((double) this.attackCooldown > 0.0)
      this.attackCooldown -= Time.get_deltaTime();
    if (!Input.GetKeyDown(NewInput.GetKey("Shoot")) || (double) this.attackCooldown > 0.0 || (double) Inventory.inventoryCooldown > 0.0)
      return;
    this.attackCooldown = 0.5f;
    this.Shoot();
  }

  private void Shoot()
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(((Component) this.plyCam).get_transform().get_position(), ((Component) this.plyCam).get_transform().get_forward(), ref raycastHit, 1.5f))
      return;
    CharacterClassManager component = (CharacterClassManager) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponent<CharacterClassManager>();
    if (Object.op_Equality((Object) component, (Object) null) || component.klasy[component.curClass].team == Team.SCP)
      return;
    this.CallCmdMovePlayer(((Component) ((RaycastHit) ref raycastHit).get_transform()).get_gameObject(), ServerTime.time);
    Hitmarker.Hit(1.5f);
  }

  private void UpdatePointText()
  {
    if (!this.get_isServer())
      return;
    Scp106PlayerScript scp106PlayerScript = this;
    scp106PlayerScript.NetworkultimatePoints = scp106PlayerScript.ultimatePoints + Time.get_deltaTime() * 6.66f * this.teleportSpeed;
    this.NetworkultimatePoints = Mathf.Clamp(this.ultimatePoints, 0.0f, 100f);
  }

  private bool BuyAbility(int cost)
  {
    if ((double) cost > (double) this.ultimatePoints)
      return false;
    if (this.get_isServer())
    {
      Scp106PlayerScript scp106PlayerScript = this;
      scp106PlayerScript.NetworkultimatePoints = scp106PlayerScript.ultimatePoints - (float) cost;
    }
    return true;
  }

  private void AnimateHighlightedText()
  {
    if (Object.op_Equality((Object) this.highlightedAbilityText, (Object) null))
    {
      this.highlightedAbilityText = ((ScpInterfaces) Object.FindObjectOfType<ScpInterfaces>()).Scp106_ability_highlight;
    }
    else
    {
      this.highlightedString = string.Empty;
      if (this.highlightID == 1)
        this.highlightedString = TranslationReader.Get("Legancy_Interfaces", 12);
      if (this.highlightID == 2)
        this.highlightedString = TranslationReader.Get("Legancy_Interfaces", 13);
      if (this.highlightedString != ((TMP_Text) this.highlightedAbilityText).get_text())
      {
        if ((double) this.highlightedAbilityText.get_canvasRenderer().GetAlpha() > 0.0)
          this.highlightedAbilityText.get_canvasRenderer().SetAlpha(this.highlightedAbilityText.get_canvasRenderer().GetAlpha() - Time.get_deltaTime() * 4f);
        else
          ((TMP_Text) this.highlightedAbilityText).set_text(this.highlightedString);
      }
      else
      {
        if ((double) this.highlightedAbilityText.get_canvasRenderer().GetAlpha() >= 1.0 || !(this.highlightedString != string.Empty))
          return;
        this.highlightedAbilityText.get_canvasRenderer().SetAlpha(this.highlightedAbilityText.get_canvasRenderer().GetAlpha() + Time.get_deltaTime() * 4f);
      }
    }
  }

  private void CheckForInventoryInput()
  {
    if (!this.get_isLocalPlayer())
      return;
    if (Object.op_Equality((Object) this.popup106, (Object) null))
    {
      this.popup106 = ((ScpInterfaces) Object.FindObjectOfType<ScpInterfaces>()).Scp106_eq;
    }
    else
    {
      bool flag = this.iAm106 & Input.GetKey(NewInput.GetKey("Inventory"));
      CursorManager.scp106 = flag;
      this.popup106.SetActive(flag);
      ((MouseLook) this.fpc.m_MouseLook).scp106_eq = (__Null) (flag ? 1 : 0);
    }
  }

  public void Init(int classID, Class c)
  {
    this.iAm106 = classID == 3;
    this.sameClass = c.team == Team.SCP;
  }

  public void SetDoors()
  {
    if (!this.get_isLocalPlayer())
      return;
    foreach (Door door in (Door[]) Object.FindObjectsOfType<Door>())
    {
      if (door.permissionLevel != "UNACCESSIBLE" && !door.locked)
      {
        foreach (Collider componentsInChild in (Collider[]) ((Component) door).GetComponentsInChildren<Collider>())
        {
          if (((Component) componentsInChild).get_tag() != "DoorButton")
          {
            try
            {
              componentsInChild.set_isTrigger(this.iAm106);
            }
            catch
            {
            }
          }
        }
      }
    }
  }

  [Server]
  public void Contain(CharacterClassManager ccm)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogWarning((object) "[Server] function 'System.Void Scp106PlayerScript::Contain(CharacterClassManager)' called on client");
    }
    else
    {
      this.NetworkultimatePoints = 0.0f;
      Timing.RunCoroutine(this._ContainAnimation(ccm), (Segment) 0);
    }
  }

  public void DeletePortal()
  {
    if (this.portalPosition.y >= 900.0)
      return;
    this.portalPrefab = (GameObject) null;
    this.NetworkportalPosition = Vector3.get_zero();
  }

  public void UseTeleport()
  {
    if (Object.op_Equality((Object) this.portalPrefab, (Object) null))
      return;
    if (this.BuyAbility(100) && Vector3.op_Inequality(this.portalPosition, Vector3.get_zero()))
      this.CallCmdUsePortal();
    else
      Timing.RunCoroutine(this._HighlightPointsText(), (Segment) 1);
  }

  private void SetPortalPosition(Vector3 pos)
  {
    this.NetworkportalPosition = pos;
    Timing.RunCoroutine(this._DoPortalSetupAnimation(), (Segment) 0);
  }

  public void CreatePortalInCurrentPosition()
  {
    if (this.BuyAbility(100))
    {
      if (!this.get_isLocalPlayer())
        return;
      this.CallCmdMakePortal();
    }
    else
      Timing.RunCoroutine(this._HighlightPointsText(), (Segment) 1);
  }

  [Server]
  [DebuggerHidden]
  private IEnumerator<float> _ContainAnimation(CharacterClassManager ccm)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogWarning((object) "[Server] function 'System.Collections.Generic.IEnumerator`1<System.Single> Scp106PlayerScript::_ContainAnimation(CharacterClassManager)' called on client");
      return (IEnumerator<float>) null;
    }
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_ContainAnimation\u003Ec__Iterator0()
    {
      ccm = ccm,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator<float> _ClientContainAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_ClientContainAnimation\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  [ClientRpc]
  private void RpcContainAnimation()
  {
    Timing.RunCoroutine(this._ClientContainAnimation(), (Segment) 1);
  }

  private void LateUpdate()
  {
    Animator animator = ((AnimationController) ((Component) this).GetComponent<AnimationController>()).animator;
    if (!Object.op_Inequality((Object) animator, (Object) null) || !this.iAm106 || this.get_isLocalPlayer())
      return;
    AnimationFloatValue component = (AnimationFloatValue) this.ccm.myModel.GetComponent<AnimationFloatValue>();
    Offset modelOffset = this.modelOffset;
    ref Offset local = ref modelOffset;
    local.position = Vector3.op_Subtraction(local.position, Vector3.op_Multiply(component.v3_value, component.f_value));
    ((Component) animator).get_transform().set_localPosition(modelOffset.position);
    ((Component) animator).get_transform().set_localRotation(Quaternion.Euler(modelOffset.rotation));
  }

  [Server]
  public void Kill(CharacterClassManager ccm)
  {
    if (!NetworkServer.get_active())
      Debug.LogWarning((object) "[Server] function 'System.Void Scp106PlayerScript::Kill(CharacterClassManager)' called on client");
    else
      ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo()
      {
        plyID = ((QueryProcessor) ((Component) ccm).GetComponent<QueryProcessor>()).PlayerId,
        amount = 999799f,
        attacker = string.Empty,
        time = 0,
        tool = "RAGDOLL-LESS"
      }, ((Component) this).get_gameObject());
  }

  [DebuggerHidden]
  private IEnumerator<float> _HighlightPointsText()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_HighlightPointsText\u003Ec__Iterator2()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator<float> _DoPortalSetupAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_DoPortalSetupAnimation\u003Ec__Iterator3()
    {
      \u0024this = this
    };
  }

  [Server]
  [DebuggerHidden]
  private IEnumerator<float> _DoTeleportAnimation()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogWarning((object) "[Server] function 'System.Collections.Generic.IEnumerator`1<System.Single> Scp106PlayerScript::_DoTeleportAnimation()' called on client");
      return (IEnumerator<float>) null;
    }
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_DoTeleportAnimation\u003Ec__Iterator4()
    {
      \u0024this = this
    };
  }

  [ClientRpc]
  public void RpcTeleportAnimation()
  {
    Timing.RunCoroutine(this._ClientTeleportAnimation(), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _ClientTeleportAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_ClientTeleportAnimation\u003Ec__Iterator5()
    {
      \u0024this = this
    };
  }

  [Command(channel = 4)]
  private void CmdMakePortal()
  {
    Debug.DrawRay(((Component) this).get_transform().get_position(), Vector3.op_UnaryNegation(((Component) this).get_transform().get_up()), Color.get_red(), 10f);
    RaycastHit raycastHit;
    if (!this.iAm106 || this.goingViaThePortal || !Physics.Raycast(new Ray(((Component) this).get_transform().get_position(), Vector3.op_UnaryNegation(((Component) this).get_transform().get_up())), ref raycastHit, 10f, LayerMask.op_Implicit(this.teleportPlacementMask)))
      return;
    this.SetPortalPosition(Vector3.op_Subtraction(((RaycastHit) ref raycastHit).get_point(), Vector3.get_up()));
  }

  [Command(channel = 4)]
  public void CmdUsePortal()
  {
    if (!this.iAm106 || !Vector3.op_Inequality(this.portalPosition, Vector3.get_zero()) || this.goingViaThePortal)
      return;
    Timing.RunCoroutine(this._DoTeleportAnimation(), (Segment) 0);
  }

  [Command(channel = 2)]
  private void CmdMovePlayer(GameObject ply, int t)
  {
    if (!ServerTime.CheckSynchronization(t) || !this.iAm106 || ((double) Vector3.Distance(((PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>()).position, ply.get_transform().get_position()) >= 3.0 || !((CharacterClassManager) ply.GetComponent<CharacterClassManager>()).IsHuman()))
      return;
    CharacterClassManager component = (CharacterClassManager) ply.GetComponent<CharacterClassManager>();
    if (component.GodMode || component.klasy[component.curClass].team == Team.SCP)
      return;
    ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallRpcPlaceBlood(ply.get_transform().get_position(), 1, 2f);
    if (Scp106PlayerScript.blastDoor.isClosed)
    {
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallRpcPlaceBlood(ply.get_transform().get_position(), 1, 2f);
      ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(500f, "SCP:106", "SCP:106", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), ply);
    }
    else
    {
      ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(40f, "SCP:106", "SCP:106", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), ply);
      ((PlyMovementSync) ply.GetComponent<PlyMovementSync>()).SetPosition(Vector3.op_Multiply(Vector3.get_down(), 1997f));
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!this.get_isLocalPlayer() || this.ccm.curClass != 3)
      return;
    Door componentInParent = (Door) ((Component) other).GetComponentInParent<Door>();
    if (Object.op_Equality((Object) componentInParent, (Object) null))
      return;
    this.doorCurrentlyIn = componentInParent;
    this.isCollidingDoorOpen = false;
    this.fpc.m_WalkSpeed = (__Null) 1.0;
    this.fpc.m_RunSpeed = (__Null) 1.0;
    if (!componentInParent.isOpen || (double) componentInParent.curCooldown > 0.0)
      return;
    this.fpc.m_WalkSpeed = (__Null) (double) this.ccm.klasy[this.ccm.curClass].walkSpeed;
    this.fpc.m_RunSpeed = (__Null) (double) this.ccm.klasy[this.ccm.curClass].runSpeed;
    this.isCollidingDoorOpen = true;
  }

  private void ExitDoor()
  {
    if (!this.get_isLocalPlayer() || this.ccm.curClass != 3)
      return;
    this.fpc.m_WalkSpeed = (__Null) (double) this.ccm.klasy[this.ccm.curClass].walkSpeed;
    this.fpc.m_RunSpeed = (__Null) (double) this.ccm.klasy[this.ccm.curClass].runSpeed;
    this.doorCurrentlyIn = (Door) null;
  }

  private void OnTriggerExit(Collider other)
  {
    this.ExitDoor();
  }

  private void DoorCollisionCheck()
  {
    if (Object.op_Inequality((Object) this.doorCurrentlyIn, (Object) null) && this.doorCurrentlyIn.destroyed)
      this.ExitDoor();
    else if (!this.isCollidingDoorOpen && Object.op_Inequality((Object) this.doorCurrentlyIn, (Object) null) && (this.doorCurrentlyIn.isOpen && (double) this.doorCurrentlyIn.curCooldown <= 0.0) && !this.isCollidingDoorOpen)
    {
      this.fpc.m_WalkSpeed = (__Null) (double) this.ccm.klasy[this.ccm.curClass].walkSpeed;
      this.fpc.m_RunSpeed = (__Null) (double) this.ccm.klasy[this.ccm.curClass].runSpeed;
      this.isCollidingDoorOpen = true;
    }
    else
    {
      if (!this.isCollidingDoorOpen || !Object.op_Inequality((Object) this.doorCurrentlyIn, (Object) null) || this.doorCurrentlyIn.isOpen)
        return;
      this.isCollidingDoorOpen = false;
      this.fpc.m_WalkSpeed = (__Null) 1.0;
      this.fpc.m_RunSpeed = (__Null) 1.0;
    }
  }

  private void UNetVersion()
  {
  }

  public float NetworkultimatePoints
  {
    get
    {
      return this.ultimatePoints;
    }
    [param: In] set
    {
      this.SetSyncVar<float>((M0) (double) value, (M0&) ref this.ultimatePoints, 1U);
    }
  }

  public Vector3 NetworkportalPosition
  {
    get
    {
      return this.portalPosition;
    }
    [param: In] set
    {
      Vector3 vector3 = value;
      ref Vector3 local = ref this.portalPosition;
      int num = 2;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetPortalPosition(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<Vector3>((M0) vector3, (M0&) ref local, (uint) num);
    }
  }

  protected static void InvokeCmdCmdMakePortal(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdMakePortal called on client.");
    else
      ((Scp106PlayerScript) obj).CmdMakePortal();
  }

  protected static void InvokeCmdCmdUsePortal(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUsePortal called on client.");
    else
      ((Scp106PlayerScript) obj).CmdUsePortal();
  }

  protected static void InvokeCmdCmdMovePlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdMovePlayer called on client.");
    else
      ((Scp106PlayerScript) obj).CmdMovePlayer((GameObject) reader.ReadGameObject(), (int) reader.ReadPackedUInt32());
  }

  public void CallCmdMakePortal()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdMakePortal called on server.");
    else if (this.get_isServer())
    {
      this.CmdMakePortal();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp106PlayerScript.kCmdCmdMakePortal);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 4, "CmdMakePortal");
    }
  }

  public void CallCmdUsePortal()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUsePortal called on server.");
    else if (this.get_isServer())
    {
      this.CmdUsePortal();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp106PlayerScript.kCmdCmdUsePortal);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 4, "CmdUsePortal");
    }
  }

  public void CallCmdMovePlayer(GameObject ply, int t)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdMovePlayer called on server.");
    else if (this.get_isServer())
    {
      this.CmdMovePlayer(ply, t);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp106PlayerScript.kCmdCmdMovePlayer);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) ply);
      networkWriter.WritePackedUInt32((uint) t);
      this.SendCommandInternal(networkWriter, 2, "CmdMovePlayer");
    }
  }

  protected static void InvokeRpcRpcContainAnimation(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcContainAnimation called on server.");
    else
      ((Scp106PlayerScript) obj).RpcContainAnimation();
  }

  protected static void InvokeRpcRpcTeleportAnimation(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcTeleportAnimation called on server.");
    else
      ((Scp106PlayerScript) obj).RpcTeleportAnimation();
  }

  public void CallRpcContainAnimation()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcContainAnimation called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Scp106PlayerScript.kRpcRpcContainAnimation);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 0, "RpcContainAnimation");
    }
  }

  public void CallRpcTeleportAnimation()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcTeleportAnimation called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Scp106PlayerScript.kRpcRpcTeleportAnimation);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 0, "RpcTeleportAnimation");
    }
  }

  static Scp106PlayerScript()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kCmdCmdMakePortal, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdMakePortal)));
    Scp106PlayerScript.kCmdCmdUsePortal = 1611005744;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kCmdCmdUsePortal, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUsePortal)));
    Scp106PlayerScript.kCmdCmdMovePlayer = -1259313323;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kCmdCmdMovePlayer, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdMovePlayer)));
    Scp106PlayerScript.kRpcRpcContainAnimation = -1083358231;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kRpcRpcContainAnimation, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcContainAnimation)));
    Scp106PlayerScript.kRpcRpcTeleportAnimation = 660537568;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kRpcRpcTeleportAnimation, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcTeleportAnimation)));
    NetworkCRC.RegisterBehaviour(nameof (Scp106PlayerScript), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.ultimatePoints);
      writer.Write((Vector3) this.portalPosition);
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
      writer.Write(this.ultimatePoints);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write((Vector3) this.portalPosition);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.ultimatePoints = reader.ReadSingle();
      this.portalPosition = (Vector3) reader.ReadVector3();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.ultimatePoints = reader.ReadSingle();
      if ((num & 2) == 0)
        return;
      this.SetPortalPosition((Vector3) reader.ReadVector3());
    }
  }
}
