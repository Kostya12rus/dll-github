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

  private void Start()
  {
    if ((Object) Scp106PlayerScript.blastDoor == (Object) null)
      Scp106PlayerScript.blastDoor = Object.FindObjectOfType<BlastDoor>();
    this.cooldownImg = GameObject.Find("Cooldown106").GetComponent<Image>();
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.fpc = this.GetComponent<FirstPersonController>();
    this.InvokeRepeating("ExitDoor", 1f, 2f);
    if (this.isLocalPlayer && NetworkServer.active)
      this.InvokeRepeating("HumanPocketLoss", 1f, 1f);
    this.modelOffset = this.ccm.klasy[3].model_offset;
    if (!this.isLocalPlayer)
      return;
    this.pointsText = Object.FindObjectOfType<ScpInterfaces>().Scp106_ability_points;
    this.pointsText.text = TranslationReader.Get("Legancy_Interfaces", 11);
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
    if (!NetworkServer.active)
    {
      Debug.LogWarning((object) "[Server] function 'System.Void Scp106PlayerScript::HumanPocketLoss()' called on client");
    }
    else
    {
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        if ((double) player.transform.position.y < -1500.0 && player.GetComponent<CharacterClassManager>().IsHuman())
          player.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(1f, "WORLD", "POCKET", this.GetComponent<QueryProcessor>().PlayerId), player);
      }
    }
  }

  private void CheckForShootInput()
  {
    if (!this.isLocalPlayer || !this.iAm106)
      return;
    this.cooldownImg.fillAmount = Mathf.Clamp01((double) this.attackCooldown > 0.0 ? (float) (1.0 - (double) this.attackCooldown * 2.0) : 0.0f);
    if ((double) this.attackCooldown > 0.0)
      this.attackCooldown -= Time.deltaTime;
    if (!Input.GetKeyDown(NewInput.GetKey("Shoot")) || (double) this.attackCooldown > 0.0 || (double) Inventory.inventoryCooldown > 0.0)
      return;
    this.attackCooldown = 0.5f;
    this.Shoot();
  }

  private void Shoot()
  {
    RaycastHit hitInfo;
    if (!Physics.Raycast(this.plyCam.transform.position, this.plyCam.transform.forward, out hitInfo, 1.5f))
      return;
    CharacterClassManager component = hitInfo.transform.GetComponent<CharacterClassManager>();
    if ((Object) component == (Object) null || component.klasy[component.curClass].team == Team.SCP)
      return;
    this.CallCmdMovePlayer(hitInfo.transform.gameObject, ServerTime.time);
    Hitmarker.Hit(1.5f);
  }

  private void UpdatePointText()
  {
    if (!this.isServer)
      return;
    Scp106PlayerScript scp106PlayerScript = this;
    scp106PlayerScript.NetworkultimatePoints = scp106PlayerScript.ultimatePoints + Time.deltaTime * 6.66f * this.teleportSpeed;
    this.NetworkultimatePoints = Mathf.Clamp(this.ultimatePoints, 0.0f, 100f);
  }

  private bool BuyAbility(int cost)
  {
    if ((double) cost > (double) this.ultimatePoints)
      return false;
    if (this.isServer)
    {
      Scp106PlayerScript scp106PlayerScript = this;
      scp106PlayerScript.NetworkultimatePoints = scp106PlayerScript.ultimatePoints - (float) cost;
    }
    return true;
  }

  private void AnimateHighlightedText()
  {
    if ((Object) this.highlightedAbilityText == (Object) null)
    {
      this.highlightedAbilityText = Object.FindObjectOfType<ScpInterfaces>().Scp106_ability_highlight;
    }
    else
    {
      this.highlightedString = string.Empty;
      if (this.highlightID == 1)
        this.highlightedString = TranslationReader.Get("Legancy_Interfaces", 12);
      if (this.highlightID == 2)
        this.highlightedString = TranslationReader.Get("Legancy_Interfaces", 13);
      if (this.highlightedString != this.highlightedAbilityText.text)
      {
        if ((double) this.highlightedAbilityText.canvasRenderer.GetAlpha() > 0.0)
          this.highlightedAbilityText.canvasRenderer.SetAlpha(this.highlightedAbilityText.canvasRenderer.GetAlpha() - Time.deltaTime * 4f);
        else
          this.highlightedAbilityText.text = this.highlightedString;
      }
      else
      {
        if ((double) this.highlightedAbilityText.canvasRenderer.GetAlpha() >= 1.0 || !(this.highlightedString != string.Empty))
          return;
        this.highlightedAbilityText.canvasRenderer.SetAlpha(this.highlightedAbilityText.canvasRenderer.GetAlpha() + Time.deltaTime * 4f);
      }
    }
  }

  private void CheckForInventoryInput()
  {
    if (!this.isLocalPlayer)
      return;
    if ((Object) this.popup106 == (Object) null)
    {
      this.popup106 = Object.FindObjectOfType<ScpInterfaces>().Scp106_eq;
    }
    else
    {
      bool flag = this.iAm106 & Input.GetKey(NewInput.GetKey("Inventory"));
      CursorManager.scp106 = flag;
      this.popup106.SetActive(flag);
      this.fpc.m_MouseLook.scp106_eq = flag;
    }
  }

  public void Init(int classID, Class c)
  {
    this.iAm106 = classID == 3;
    this.sameClass = c.team == Team.SCP;
  }

  public void SetDoors()
  {
    if (!this.isLocalPlayer)
      return;
    foreach (Door door in Object.FindObjectsOfType<Door>())
    {
      if (door.permissionLevel != "UNACCESSIBLE" && !door.locked)
      {
        foreach (Collider componentsInChild in door.GetComponentsInChildren<Collider>())
        {
          if (componentsInChild.tag != "DoorButton")
          {
            try
            {
              componentsInChild.isTrigger = this.iAm106;
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
    if (!NetworkServer.active)
    {
      Debug.LogWarning((object) "[Server] function 'System.Void Scp106PlayerScript::Contain(CharacterClassManager)' called on client");
    }
    else
    {
      this.NetworkultimatePoints = 0.0f;
      Timing.RunCoroutine(this._ContainAnimation(ccm), Segment.Update);
    }
  }

  public void DeletePortal()
  {
    if ((double) this.portalPosition.y >= 900.0)
      return;
    this.portalPrefab = (GameObject) null;
    this.NetworkportalPosition = Vector3.zero;
  }

  public void UseTeleport()
  {
    if ((Object) this.portalPrefab == (Object) null)
      return;
    if (this.BuyAbility(100) && this.portalPosition != Vector3.zero)
      this.CallCmdUsePortal();
    else
      Timing.RunCoroutine(this._HighlightPointsText(), Segment.FixedUpdate);
  }

  private void SetPortalPosition(Vector3 pos)
  {
    this.NetworkportalPosition = pos;
    Timing.RunCoroutine(this._DoPortalSetupAnimation(), Segment.Update);
  }

  public void CreatePortalInCurrentPosition()
  {
    if (this.BuyAbility(100))
    {
      if (!this.isLocalPlayer)
        return;
      this.CallCmdMakePortal();
    }
    else
      Timing.RunCoroutine(this._HighlightPointsText(), Segment.FixedUpdate);
  }

  [Server]
  [DebuggerHidden]
  private IEnumerator<float> _ContainAnimation(CharacterClassManager ccm)
  {
    if (!NetworkServer.active)
    {
      Debug.LogWarning((object) "[Server] function 'System.Collections.Generic.IEnumerator`1<System.Single> Scp106PlayerScript::_ContainAnimation(CharacterClassManager)' called on client");
      return (IEnumerator<float>) null;
    }
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_ContainAnimation\u003Ec__Iterator0() { ccm = ccm, \u0024this = this };
  }

  [DebuggerHidden]
  private IEnumerator<float> _ClientContainAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_ClientContainAnimation\u003Ec__Iterator1() { \u0024this = this };
  }

  [ClientRpc]
  private void RpcContainAnimation()
  {
    Timing.RunCoroutine(this._ClientContainAnimation(), Segment.FixedUpdate);
  }

  private void LateUpdate()
  {
    Animator animator = this.GetComponent<AnimationController>().animator;
    if (!((Object) animator != (Object) null) || !this.iAm106 || this.isLocalPlayer)
      return;
    AnimationFloatValue component = this.ccm.myModel.GetComponent<AnimationFloatValue>();
    Offset modelOffset = this.modelOffset;
    modelOffset.position -= component.v3_value * component.f_value;
    animator.transform.localPosition = modelOffset.position;
    animator.transform.localRotation = Quaternion.Euler(modelOffset.rotation);
  }

  [Server]
  public void Kill(CharacterClassManager ccm)
  {
    if (!NetworkServer.active)
      Debug.LogWarning((object) "[Server] function 'System.Void Scp106PlayerScript::Kill(CharacterClassManager)' called on client");
    else
      this.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo()
      {
        plyID = ccm.GetComponent<QueryProcessor>().PlayerId,
        amount = 999799f,
        attacker = string.Empty,
        time = 0,
        tool = "RAGDOLL-LESS"
      }, this.gameObject);
  }

  [DebuggerHidden]
  private IEnumerator<float> _HighlightPointsText()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_HighlightPointsText\u003Ec__Iterator2() { \u0024this = this };
  }

  [DebuggerHidden]
  private IEnumerator<float> _DoPortalSetupAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_DoPortalSetupAnimation\u003Ec__Iterator3() { \u0024this = this };
  }

  [Server]
  [DebuggerHidden]
  private IEnumerator<float> _DoTeleportAnimation()
  {
    if (!NetworkServer.active)
    {
      Debug.LogWarning((object) "[Server] function 'System.Collections.Generic.IEnumerator`1<System.Single> Scp106PlayerScript::_DoTeleportAnimation()' called on client");
      return (IEnumerator<float>) null;
    }
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_DoTeleportAnimation\u003Ec__Iterator4() { \u0024this = this };
  }

  [ClientRpc]
  public void RpcTeleportAnimation()
  {
    Timing.RunCoroutine(this._ClientTeleportAnimation(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _ClientTeleportAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp106PlayerScript.\u003C_ClientTeleportAnimation\u003Ec__Iterator5() { \u0024this = this };
  }

  [Command(channel = 4)]
  private void CmdMakePortal()
  {
    Debug.DrawRay(this.transform.position, -this.transform.up, Color.red, 10f);
    RaycastHit hitInfo;
    if (!this.iAm106 || this.goingViaThePortal || !Physics.Raycast(new Ray(this.transform.position, -this.transform.up), out hitInfo, 10f, (int) this.teleportPlacementMask))
      return;
    this.SetPortalPosition(hitInfo.point - Vector3.up);
  }

  [Command(channel = 4)]
  public void CmdUsePortal()
  {
    if (!this.iAm106 || !(this.portalPosition != Vector3.zero) || this.goingViaThePortal)
      return;
    Timing.RunCoroutine(this._DoTeleportAnimation(), Segment.Update);
  }

  [Command(channel = 2)]
  private void CmdMovePlayer(GameObject ply, int t)
  {
    if (!ServerTime.CheckSynchronization(t) || !this.iAm106 || ((double) Vector3.Distance(this.GetComponent<PlyMovementSync>().position, ply.transform.position) >= 3.0 || !ply.GetComponent<CharacterClassManager>().IsHuman()))
      return;
    CharacterClassManager component = ply.GetComponent<CharacterClassManager>();
    if (component.GodMode || component.klasy[component.curClass].team == Team.SCP)
      return;
    this.GetComponent<CharacterClassManager>().CallRpcPlaceBlood(ply.transform.position, 1, 2f);
    if (Scp106PlayerScript.blastDoor.isClosed)
    {
      this.GetComponent<CharacterClassManager>().CallRpcPlaceBlood(ply.transform.position, 1, 2f);
      this.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(500f, "SCP:106", "SCP:106", this.GetComponent<QueryProcessor>().PlayerId), ply);
    }
    else
    {
      this.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(40f, "SCP:106", "SCP:106", this.GetComponent<QueryProcessor>().PlayerId), ply);
      ply.GetComponent<PlyMovementSync>().SetPosition(Vector3.down * 1997f);
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!this.isLocalPlayer || this.ccm.curClass != 3)
      return;
    Door componentInParent = other.GetComponentInParent<Door>();
    if ((Object) componentInParent == (Object) null)
      return;
    this.doorCurrentlyIn = componentInParent;
    this.isCollidingDoorOpen = false;
    this.fpc.m_WalkSpeed = 1f;
    this.fpc.m_RunSpeed = 1f;
    if (!componentInParent.isOpen || (double) componentInParent.curCooldown > 0.0)
      return;
    this.fpc.m_WalkSpeed = this.ccm.klasy[this.ccm.curClass].walkSpeed;
    this.fpc.m_RunSpeed = this.ccm.klasy[this.ccm.curClass].runSpeed;
    this.isCollidingDoorOpen = true;
  }

  private void ExitDoor()
  {
    if (!this.isLocalPlayer || this.ccm.curClass != 3)
      return;
    this.fpc.m_WalkSpeed = this.ccm.klasy[this.ccm.curClass].walkSpeed;
    this.fpc.m_RunSpeed = this.ccm.klasy[this.ccm.curClass].runSpeed;
    this.doorCurrentlyIn = (Door) null;
  }

  private void OnTriggerExit(Collider other)
  {
    this.ExitDoor();
  }

  private void DoorCollisionCheck()
  {
    if ((Object) this.doorCurrentlyIn != (Object) null && this.doorCurrentlyIn.destroyed)
      this.ExitDoor();
    else if (!this.isCollidingDoorOpen && (Object) this.doorCurrentlyIn != (Object) null && (this.doorCurrentlyIn.isOpen && (double) this.doorCurrentlyIn.curCooldown <= 0.0) && !this.isCollidingDoorOpen)
    {
      this.fpc.m_WalkSpeed = this.ccm.klasy[this.ccm.curClass].walkSpeed;
      this.fpc.m_RunSpeed = this.ccm.klasy[this.ccm.curClass].runSpeed;
      this.isCollidingDoorOpen = true;
    }
    else
    {
      if (!this.isCollidingDoorOpen || !((Object) this.doorCurrentlyIn != (Object) null) || this.doorCurrentlyIn.isOpen)
        return;
      this.isCollidingDoorOpen = false;
      this.fpc.m_WalkSpeed = 1f;
      this.fpc.m_RunSpeed = 1f;
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
      this.SetSyncVar<float>(value, ref this.ultimatePoints, 1U);
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
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetPortalPosition(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<Vector3>(vector3, ref local, (uint) num);
    }
  }

  protected static void InvokeCmdCmdMakePortal(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdMakePortal called on client.");
    else
      ((Scp106PlayerScript) obj).CmdMakePortal();
  }

  protected static void InvokeCmdCmdUsePortal(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUsePortal called on client.");
    else
      ((Scp106PlayerScript) obj).CmdUsePortal();
  }

  protected static void InvokeCmdCmdMovePlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdMovePlayer called on client.");
    else
      ((Scp106PlayerScript) obj).CmdMovePlayer(reader.ReadGameObject(), (int) reader.ReadPackedUInt32());
  }

  public void CallCmdMakePortal()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdMakePortal called on server.");
    else if (this.isServer)
    {
      this.CmdMakePortal();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Scp106PlayerScript.kCmdCmdMakePortal);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 4, "CmdMakePortal");
    }
  }

  public void CallCmdUsePortal()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUsePortal called on server.");
    else if (this.isServer)
    {
      this.CmdUsePortal();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Scp106PlayerScript.kCmdCmdUsePortal);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 4, "CmdUsePortal");
    }
  }

  public void CallCmdMovePlayer(GameObject ply, int t)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdMovePlayer called on server.");
    else if (this.isServer)
    {
      this.CmdMovePlayer(ply, t);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Scp106PlayerScript.kCmdCmdMovePlayer);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(ply);
      writer.WritePackedUInt32((uint) t);
      this.SendCommandInternal(writer, 2, "CmdMovePlayer");
    }
  }

  protected static void InvokeRpcRpcContainAnimation(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcContainAnimation called on server.");
    else
      ((Scp106PlayerScript) obj).RpcContainAnimation();
  }

  protected static void InvokeRpcRpcTeleportAnimation(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcTeleportAnimation called on server.");
    else
      ((Scp106PlayerScript) obj).RpcTeleportAnimation();
  }

  public void CallRpcContainAnimation()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcContainAnimation called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Scp106PlayerScript.kRpcRpcContainAnimation);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 0, "RpcContainAnimation");
    }
  }

  public void CallRpcTeleportAnimation()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcTeleportAnimation called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Scp106PlayerScript.kRpcRpcTeleportAnimation);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 0, "RpcTeleportAnimation");
    }
  }

  static Scp106PlayerScript()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kCmdCmdMakePortal, new NetworkBehaviour.CmdDelegate(Scp106PlayerScript.InvokeCmdCmdMakePortal));
    Scp106PlayerScript.kCmdCmdUsePortal = 1611005744;
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kCmdCmdUsePortal, new NetworkBehaviour.CmdDelegate(Scp106PlayerScript.InvokeCmdCmdUsePortal));
    Scp106PlayerScript.kCmdCmdMovePlayer = -1259313323;
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kCmdCmdMovePlayer, new NetworkBehaviour.CmdDelegate(Scp106PlayerScript.InvokeCmdCmdMovePlayer));
    Scp106PlayerScript.kRpcRpcContainAnimation = -1083358231;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kRpcRpcContainAnimation, new NetworkBehaviour.CmdDelegate(Scp106PlayerScript.InvokeRpcRpcContainAnimation));
    Scp106PlayerScript.kRpcRpcTeleportAnimation = 660537568;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp106PlayerScript), Scp106PlayerScript.kRpcRpcTeleportAnimation, new NetworkBehaviour.CmdDelegate(Scp106PlayerScript.InvokeRpcRpcTeleportAnimation));
    NetworkCRC.RegisterBehaviour(nameof (Scp106PlayerScript), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.ultimatePoints);
      writer.Write(this.portalPosition);
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
      writer.Write(this.ultimatePoints);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.portalPosition);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.ultimatePoints = reader.ReadSingle();
      this.portalPosition = reader.ReadVector3();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.ultimatePoints = reader.ReadSingle();
      if ((num & 2) == 0)
        return;
      this.SetPortalPosition(reader.ReadVector3());
    }
  }
}
