// Decompiled with JetBrains decompiler
// Type: Scp173PlayerScript
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance.Integrations.UNet_HLAPI;
using RemoteAdmin;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.ImageEffects;

public class Scp173PlayerScript : NetworkBehaviour
{
  private static int kCmdCmdHurtPlayer = 1571551081;
  [Header("Player Properties")]
  public bool iAm173;
  public bool sameClass;
  [Header("Raycasting")]
  public GameObject cam;
  public float range;
  public LayerMask layerMask;
  public LayerMask teleportMask;
  public LayerMask hurtLayer;
  [Header("Blinking")]
  public float minBlinkTime;
  public float maxBlinkTime;
  public float blinkDuration_notsee;
  public float blinkDuration_see;
  private float remainingTime;
  private VignetteAndChromaticAberration blinkCtrl;
  private FirstPersonController fpc;
  private PlyMovementSync pms;
  private CharacterClassManager public_ccm;
  private PlayerStats ps;
  public GameObject weaponCameras;
  public GameObject hitbox;
  public AudioClip[] necksnaps;
  [Header("Boosts")]
  public AnimationCurve boost_teleportDistance;
  public AnimationCurve boost_speed;
  private bool allowMove;
  private static float blinkTimeRemaining;
  private static bool localplayerIs173;
  public static bool isBlinking;
  private FlashEffect flash;
  private static int kRpcRpcBlinkTime;
  private static int kRpcRpcSyncAudio;

  public Scp173PlayerScript()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.flash = (FlashEffect) ((Component) this).GetComponent<FlashEffect>();
    this.ps = (PlayerStats) ((Component) this).GetComponent<PlayerStats>();
    this.public_ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    if (!this.get_isLocalPlayer())
      return;
    this.blinkCtrl = (VignetteAndChromaticAberration) ((Component) this).GetComponentInChildren<VignetteAndChromaticAberration>();
    this.fpc = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
    this.pms = (PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>();
    Scp173PlayerScript.isBlinking = false;
  }

  public void Init(int classID, Class c)
  {
    this.sameClass = c.team == Team.SCP;
    if (this.get_isLocalPlayer())
      this.fpc.lookingAtMe = (__Null) 0;
    this.iAm173 = classID == 0;
    if (this.get_isLocalPlayer())
      Scp173PlayerScript.localplayerIs173 = this.iAm173;
    this.hitbox.SetActive(!this.get_isLocalPlayer() && Scp173PlayerScript.localplayerIs173);
  }

  private void FixedUpdate()
  {
    this.DoBlinkingSequence();
    if (!this.iAm173 || !this.get_isLocalPlayer() && !NetworkServer.get_active())
      return;
    this.allowMove = true;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      Scp173PlayerScript component = (Scp173PlayerScript) player.GetComponent<Scp173PlayerScript>();
      if (!component.sameClass && component.LookFor173(((Component) this).get_gameObject(), true) && this.LookFor173(((Component) component).get_gameObject(), false))
      {
        this.allowMove = false;
        break;
      }
    }
    if (!this.get_isLocalPlayer())
      return;
    this.CheckForInput();
    this.fpc.lookingAtMe = (__Null) (!this.allowMove ? 1 : 0);
    float num = this.boost_speed.Evaluate(this.ps.GetHealthPercent());
    this.fpc.m_WalkSpeed = (__Null) (double) num;
    this.fpc.m_RunSpeed = (__Null) (double) num;
  }

  public bool LookFor173(GameObject scp, bool angleCheck)
  {
    if (!Object.op_Implicit((Object) scp) || this.public_ccm.curClass == 2 || this.flash.sync_blind)
      return false;
    if (angleCheck)
    {
      Vector3 forward = this.cam.get_transform().get_forward();
      Vector3 vector3 = Vector3.op_Subtraction(this.cam.get_transform().get_position(), scp.get_transform().get_position());
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      if ((double) Vector3.Dot(forward, normalized) >= -0.649999976158142)
        goto label_6;
    }
    Vector3 position = this.cam.get_transform().get_position();
    Vector3 vector3_1 = Vector3.op_Subtraction(scp.get_transform().get_position(), this.cam.get_transform().get_position());
    Vector3 normalized1 = ((Vector3) ref vector3_1).get_normalized();
    RaycastHit raycastHit;
    ref RaycastHit local = ref raycastHit;
    double range = (double) this.range;
    int num = LayerMask.op_Implicit(this.layerMask);
    if (Physics.Raycast(position, normalized1, ref local, (float) range, num) && ((Object) ((RaycastHit) ref raycastHit).get_transform()).get_name() == ((Object) scp).get_name())
      return true;
label_6:
    return false;
  }

  public bool CanMove()
  {
    if (!this.allowMove)
      return (double) Scp173PlayerScript.blinkTimeRemaining > 0.0;
    return true;
  }

  private void DoBlinkingSequence()
  {
    if (!this.get_isServer() || !this.get_isLocalPlayer())
      return;
    this.remainingTime -= Time.get_fixedDeltaTime();
    Scp173PlayerScript.blinkTimeRemaining -= Time.get_fixedDeltaTime();
    if ((double) this.remainingTime >= 0.0)
      return;
    Scp173PlayerScript.blinkTimeRemaining = this.blinkDuration_see + 0.5f;
    this.remainingTime = Random.Range(this.minBlinkTime, this.maxBlinkTime);
    foreach (Scp173PlayerScript scp173PlayerScript in (Scp173PlayerScript[]) Object.FindObjectsOfType<Scp173PlayerScript>())
      scp173PlayerScript.CallRpcBlinkTime();
  }

  public void Boost()
  {
    if (!this.get_isLocalPlayer())
      return;
    PlyMovementSync pms = this.pms;
    Quaternion rotation = ((Component) this).get_transform().get_rotation();
    // ISSUE: variable of the null type
    __Null y = ((Quaternion) ref rotation).get_eulerAngles().y;
    pms.ClientSetRotation((float) y);
    if (this.fpc.lookingAtMe != null)
    {
      bool flag = false;
      RaycastHit raycastHit1;
      if (Physics.Raycast(this.cam.get_transform().get_position(), this.cam.get_transform().get_forward(), ref raycastHit1, 100f, LayerMask.op_Implicit(this.teleportMask)) && Object.op_Inequality((Object) ((Component) ((RaycastHit) ref raycastHit1).get_transform()).GetComponent<CharacterClassManager>(), (Object) null) && ((double) Input.GetAxisRaw("Vertical") > 0.0 && (double) Input.GetAxisRaw("Horizontal") == 0.0))
        flag = true;
      float num1 = this.boost_teleportDistance.Evaluate(this.ps.GetHealthPercent());
      Vector3 position1 = ((Component) this).get_transform().get_position();
      if (flag)
      {
        Vector3 vector3_1 = Vector3.op_Subtraction(((RaycastHit) ref raycastHit1).get_transform().get_position(), ((Component) this).get_transform().get_position());
        Vector3 vector3_2 = Vector3.op_Multiply(((Vector3) ref vector3_1).get_normalized(), Mathf.Clamp(((Vector3) ref vector3_1).get_magnitude() - 1f, 0.0f, num1));
        Transform transform = ((Component) this).get_transform();
        transform.set_position(Vector3.op_Addition(transform.get_position(), vector3_2));
      }
      else
      {
        RaycastHit raycastHit2;
        Physics.Raycast(this.cam.get_transform().get_position(), this.cam.get_transform().get_forward(), ref raycastHit2, 100f, LayerMask.op_Implicit(this.teleportMask));
        float num2 = Vector3.Distance(((Component) this).get_transform().get_position(), new Vector3((float) ((RaycastHit) ref raycastHit2).get_point().x, (float) ((Component) this).get_transform().get_position().y, (float) ((RaycastHit) ref raycastHit2).get_point().z));
        float num3 = Mathf.Min(num1, num2);
        for (int index = 0; index < 1000 && (double) Vector3.Distance(position1, ((Component) this).get_transform().get_position()) < (double) num3; ++index)
        {
          Vector3 position2 = ((Component) this).get_transform().get_position();
          this.Forward();
          if (Vector3.op_Equality(position2, ((Component) this).get_transform().get_position()))
            break;
        }
      }
    }
    if (!Input.GetKey(NewInput.GetKey("Shoot")))
      return;
    this.Shoot();
  }

  private void Forward()
  {
    this.fpc.blinkAddition = (__Null) 0.800000011920929;
    this.fpc.MotorPlayer();
    this.fpc.blinkAddition = (__Null) 0.0;
  }

  public void Blink()
  {
    if (!this.get_isLocalPlayer())
      return;
    Scp173PlayerScript.isBlinking = true;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (((Scp173PlayerScript) player.GetComponent<Scp173PlayerScript>()).iAm173)
      {
        bool flag = this.LookFor173(player, true);
        if (flag)
        {
          this.blinkCtrl.intensity = (__Null) 1.0;
          this.weaponCameras.SetActive(false);
        }
        ((MonoBehaviour) this).Invoke("UnBlink", !flag ? this.blinkDuration_notsee : this.blinkDuration_see);
      }
    }
  }

  private void UnBlink()
  {
    this.blinkCtrl.intensity = (__Null) 0.0359999984502792;
    Scp173PlayerScript.isBlinking = false;
    this.weaponCameras.SetActive(true);
  }

  private void CheckForInput()
  {
    if (!Input.GetKeyDown(NewInput.GetKey("Shoot")) || !this.allowMove)
      return;
    this.Shoot();
  }

  private void Shoot()
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(this.cam.get_transform().get_position(), this.cam.get_transform().get_forward(), ref raycastHit, 1.5f, LayerMask.op_Implicit(this.hurtLayer)))
      return;
    CharacterClassManager component = (CharacterClassManager) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponent<CharacterClassManager>();
    if (!Object.op_Inequality((Object) component, (Object) null) || component.klasy[component.curClass].team == Team.SCP)
      return;
    this.HurtPlayer(((Component) ((RaycastHit) ref raycastHit).get_transform()).get_gameObject(), ((HlapiPlayer) ((Component) this).GetComponent<HlapiPlayer>()).PlayerId);
  }

  private void HurtPlayer(GameObject go, string plyID)
  {
    Hitmarker.Hit(1f);
    this.CallCmdHurtPlayer(go);
  }

  [Command(channel = 2)]
  private void CmdHurtPlayer(GameObject target)
  {
    if (((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).curClass != 0 || !this.CanMove() || (double) Vector3.Distance(((PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>()).position, target.get_transform().get_position()) >= 3.0 + (double) this.boost_teleportDistance.Evaluate(this.ps.GetHealthPercent()))
      return;
    this.CallRpcSyncAudio();
    ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallRpcPlaceBlood(target.get_transform().get_position(), 0, 2.2f);
    ((PlayerStats) target.GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(999990f, "SCP173", "SCP:173", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), target);
  }

  [ClientRpc(channel = 0)]
  private void RpcBlinkTime()
  {
    if (this.iAm173)
      this.Boost();
    if (this.sameClass)
      return;
    this.Blink();
  }

  [ClientRpc(channel = 1)]
  private void RpcSyncAudio()
  {
    ((AnimationController) ((Component) this).GetComponent<AnimationController>()).gunSource.PlayOneShot(this.necksnaps[Random.Range(0, this.necksnaps.Length)]);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdHurtPlayer called on client.");
    else
      ((Scp173PlayerScript) obj).CmdHurtPlayer((GameObject) reader.ReadGameObject());
  }

  public void CallCmdHurtPlayer(GameObject target)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdHurtPlayer called on server.");
    else if (this.get_isServer())
    {
      this.CmdHurtPlayer(target);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp173PlayerScript.kCmdCmdHurtPlayer);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) target);
      this.SendCommandInternal(networkWriter, 2, "CmdHurtPlayer");
    }
  }

  protected static void InvokeRpcRpcBlinkTime(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcBlinkTime called on server.");
    else
      ((Scp173PlayerScript) obj).RpcBlinkTime();
  }

  protected static void InvokeRpcRpcSyncAudio(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcSyncAudio called on server.");
    else
      ((Scp173PlayerScript) obj).RpcSyncAudio();
  }

  public void CallRpcBlinkTime()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcBlinkTime called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Scp173PlayerScript.kRpcRpcBlinkTime);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 0, "RpcBlinkTime");
    }
  }

  public void CallRpcSyncAudio()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcSyncAudio called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Scp173PlayerScript.kRpcRpcSyncAudio);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 1, "RpcSyncAudio");
    }
  }

  static Scp173PlayerScript()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp173PlayerScript), Scp173PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdHurtPlayer)));
    Scp173PlayerScript.kRpcRpcBlinkTime = -1078791558;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp173PlayerScript), Scp173PlayerScript.kRpcRpcBlinkTime, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcBlinkTime)));
    Scp173PlayerScript.kRpcRpcSyncAudio = -769227732;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp173PlayerScript), Scp173PlayerScript.kRpcRpcSyncAudio, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcSyncAudio)));
    NetworkCRC.RegisterBehaviour(nameof (Scp173PlayerScript), 0);
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
