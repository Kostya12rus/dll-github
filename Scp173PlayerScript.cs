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
  private bool allowMove = true;
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
  private static float blinkTimeRemaining;
  private static bool localplayerIs173;
  public static bool isBlinking;
  private FlashEffect flash;
  private static int kRpcRpcBlinkTime;
  private static int kRpcRpcSyncAudio;

  private void Start()
  {
    this.flash = this.GetComponent<FlashEffect>();
    this.ps = this.GetComponent<PlayerStats>();
    this.public_ccm = this.GetComponent<CharacterClassManager>();
    if (!this.isLocalPlayer)
      return;
    this.blinkCtrl = this.GetComponentInChildren<VignetteAndChromaticAberration>();
    this.fpc = this.GetComponent<FirstPersonController>();
    this.pms = this.GetComponent<PlyMovementSync>();
    Scp173PlayerScript.isBlinking = false;
  }

  public void Init(int classID, Class c)
  {
    this.sameClass = c.team == Team.SCP;
    if (this.isLocalPlayer)
      this.fpc.lookingAtMe = false;
    this.iAm173 = classID == 0;
    if (this.isLocalPlayer)
      Scp173PlayerScript.localplayerIs173 = this.iAm173;
    this.hitbox.SetActive(!this.isLocalPlayer && Scp173PlayerScript.localplayerIs173);
  }

  private void FixedUpdate()
  {
    this.DoBlinkingSequence();
    if (!this.iAm173 || !this.isLocalPlayer && !NetworkServer.active)
      return;
    this.allowMove = true;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      Scp173PlayerScript component = player.GetComponent<Scp173PlayerScript>();
      if (!component.sameClass && component.LookFor173(this.gameObject, true) && this.LookFor173(component.gameObject, false))
      {
        this.allowMove = false;
        break;
      }
    }
    if (!this.isLocalPlayer)
      return;
    this.CheckForInput();
    this.fpc.lookingAtMe = !this.allowMove;
    float num = this.boost_speed.Evaluate(this.ps.GetHealthPercent());
    this.fpc.m_WalkSpeed = num;
    this.fpc.m_RunSpeed = num;
  }

  public bool LookFor173(GameObject scp, bool angleCheck)
  {
    RaycastHit hitInfo;
    return (bool) ((Object) scp) && this.public_ccm.curClass != 2 && !this.flash.sync_blind && (!angleCheck || (double) Vector3.Dot(this.cam.transform.forward, (this.cam.transform.position - scp.transform.position).normalized) < -0.649999976158142) && (Physics.Raycast(this.cam.transform.position, (scp.transform.position - this.cam.transform.position).normalized, out hitInfo, this.range, (int) this.layerMask) && hitInfo.transform.name == scp.name);
  }

  public bool CanMove()
  {
    if (!this.allowMove)
      return (double) Scp173PlayerScript.blinkTimeRemaining > 0.0;
    return true;
  }

  private void DoBlinkingSequence()
  {
    if (!this.isServer || !this.isLocalPlayer)
      return;
    this.remainingTime -= Time.fixedDeltaTime;
    Scp173PlayerScript.blinkTimeRemaining -= Time.fixedDeltaTime;
    if ((double) this.remainingTime >= 0.0)
      return;
    Scp173PlayerScript.blinkTimeRemaining = this.blinkDuration_see + 0.5f;
    this.remainingTime = Random.Range(this.minBlinkTime, this.maxBlinkTime);
    foreach (Scp173PlayerScript scp173PlayerScript in Object.FindObjectsOfType<Scp173PlayerScript>())
      scp173PlayerScript.CallRpcBlinkTime();
  }

  public void Boost()
  {
    if (!this.isLocalPlayer)
      return;
    this.pms.ClientSetRotation(this.transform.rotation.eulerAngles.y);
    if (this.fpc.lookingAtMe)
    {
      bool flag = false;
      RaycastHit hitInfo1;
      if (Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out hitInfo1, 100f, (int) this.teleportMask) && (Object) hitInfo1.transform.GetComponent<CharacterClassManager>() != (Object) null && ((double) Input.GetAxisRaw("Vertical") > 0.0 && (double) Input.GetAxisRaw("Horizontal") == 0.0))
        flag = true;
      float num1 = this.boost_teleportDistance.Evaluate(this.ps.GetHealthPercent());
      Vector3 position1 = this.transform.position;
      if (flag)
      {
        Vector3 vector3 = hitInfo1.transform.position - this.transform.position;
        this.transform.position += vector3.normalized * Mathf.Clamp(vector3.magnitude - 1f, 0.0f, num1);
      }
      else
      {
        RaycastHit hitInfo2;
        Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out hitInfo2, 100f, (int) this.teleportMask);
        float b = Vector3.Distance(this.transform.position, new Vector3(hitInfo2.point.x, this.transform.position.y, hitInfo2.point.z));
        float num2 = Mathf.Min(num1, b);
        for (int index = 0; index < 1000 && (double) Vector3.Distance(position1, this.transform.position) < (double) num2; ++index)
        {
          Vector3 position2 = this.transform.position;
          this.Forward();
          if (position2 == this.transform.position)
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
    this.fpc.blinkAddition = 0.8f;
    this.fpc.MotorPlayer();
    this.fpc.blinkAddition = 0.0f;
  }

  public void Blink()
  {
    if (!this.isLocalPlayer)
      return;
    Scp173PlayerScript.isBlinking = true;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (player.GetComponent<Scp173PlayerScript>().iAm173)
      {
        bool flag = this.LookFor173(player, true);
        if (flag)
        {
          this.blinkCtrl.intensity = 1f;
          this.weaponCameras.SetActive(false);
        }
        this.Invoke("UnBlink", !flag ? this.blinkDuration_notsee : this.blinkDuration_see);
      }
    }
  }

  private void UnBlink()
  {
    this.blinkCtrl.intensity = 0.036f;
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
    RaycastHit hitInfo;
    if (!Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out hitInfo, 1.5f, (int) this.hurtLayer))
      return;
    CharacterClassManager component = hitInfo.transform.GetComponent<CharacterClassManager>();
    if (!((Object) component != (Object) null) || component.klasy[component.curClass].team == Team.SCP)
      return;
    this.HurtPlayer(hitInfo.transform.gameObject, this.GetComponent<HlapiPlayer>().PlayerId);
  }

  private void HurtPlayer(GameObject go, string plyID)
  {
    Hitmarker.Hit(1f);
    this.CallCmdHurtPlayer(go);
  }

  [Command(channel = 2)]
  private void CmdHurtPlayer(GameObject target)
  {
    if (this.GetComponent<CharacterClassManager>().curClass != 0 || !this.CanMove() || (double) Vector3.Distance(this.GetComponent<PlyMovementSync>().position, target.transform.position) >= 3.0 + (double) this.boost_teleportDistance.Evaluate(this.ps.GetHealthPercent()))
      return;
    this.CallRpcSyncAudio();
    this.GetComponent<CharacterClassManager>().CallRpcPlaceBlood(target.transform.position, 0, 2.2f);
    target.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(999990f, "SCP173", "SCP:173", this.GetComponent<QueryProcessor>().PlayerId), target);
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
    this.GetComponent<AnimationController>().gunSource.PlayOneShot(this.necksnaps[Random.Range(0, this.necksnaps.Length)]);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdHurtPlayer called on client.");
    else
      ((Scp173PlayerScript) obj).CmdHurtPlayer(reader.ReadGameObject());
  }

  public void CallCmdHurtPlayer(GameObject target)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdHurtPlayer called on server.");
    else if (this.isServer)
    {
      this.CmdHurtPlayer(target);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Scp173PlayerScript.kCmdCmdHurtPlayer);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(target);
      this.SendCommandInternal(writer, 2, "CmdHurtPlayer");
    }
  }

  protected static void InvokeRpcRpcBlinkTime(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcBlinkTime called on server.");
    else
      ((Scp173PlayerScript) obj).RpcBlinkTime();
  }

  protected static void InvokeRpcRpcSyncAudio(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcSyncAudio called on server.");
    else
      ((Scp173PlayerScript) obj).RpcSyncAudio();
  }

  public void CallRpcBlinkTime()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcBlinkTime called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Scp173PlayerScript.kRpcRpcBlinkTime);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 0, "RpcBlinkTime");
    }
  }

  public void CallRpcSyncAudio()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcSyncAudio called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Scp173PlayerScript.kRpcRpcSyncAudio);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 1, "RpcSyncAudio");
    }
  }

  static Scp173PlayerScript()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp173PlayerScript), Scp173PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate(Scp173PlayerScript.InvokeCmdCmdHurtPlayer));
    Scp173PlayerScript.kRpcRpcBlinkTime = -1078791558;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp173PlayerScript), Scp173PlayerScript.kRpcRpcBlinkTime, new NetworkBehaviour.CmdDelegate(Scp173PlayerScript.InvokeRpcRpcBlinkTime));
    Scp173PlayerScript.kRpcRpcSyncAudio = -769227732;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp173PlayerScript), Scp173PlayerScript.kRpcRpcSyncAudio, new NetworkBehaviour.CmdDelegate(Scp173PlayerScript.InvokeRpcRpcSyncAudio));
    NetworkCRC.RegisterBehaviour(nameof (Scp173PlayerScript), 0);
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
