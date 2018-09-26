// Decompiled with JetBrains decompiler
// Type: Scp096PlayerScript
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class Scp096PlayerScript : NetworkBehaviour
{
  private static int kCmdCmdHurtPlayer = 787420137;
  [Space]
  public float ragemultiplier_deduct = 0.08f;
  public float ragemultiplier_coodownduration = 20f;
  public static Scp096PlayerScript instance;
  public GameObject camera;
  public bool sameClass;
  public bool iAm096;
  public LayerMask layerMask;
  private AnimationController animationController;
  private float cooldown;
  public SoundtrackManager.Track[] tracks;
  [Space]
  [SyncVar(hook = "SetRage")]
  public Scp096PlayerScript.RageState enraged;
  public float rageProgress;
  [Space]
  public float ragemultiplier_looking;
  public AnimationCurve lookingTolerance;
  private float t;
  private CharacterClassManager ccm;
  private FirstPersonController fpc;
  private float normalSpeed;

  private void SetRage(Scp096PlayerScript.RageState b)
  {
    this.Networkenraged = b;
  }

  public void IncreaseRage(float amount)
  {
    if (this.enraged != Scp096PlayerScript.RageState.NotEnraged)
      return;
    this.rageProgress += amount;
    this.rageProgress = Mathf.Clamp01(this.rageProgress);
    if ((double) this.rageProgress != 1.0)
      return;
    this.SetRage(Scp096PlayerScript.RageState.Panic);
    this.Invoke("StartRage", 5f);
  }

  private void StartRage()
  {
    this.SetRage(Scp096PlayerScript.RageState.Enraged);
  }

  private void Update()
  {
    this.ExecuteClientsideCode();
    this.Animator();
  }

  [DebuggerHidden]
  private IEnumerator<float> _UpdateAudios()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp096PlayerScript.\u003C_UpdateAudios\u003Ec__Iterator0() { \u0024this = this };
  }

  private void Animator()
  {
    if (this.isLocalPlayer || !((Object) this.animationController.animator != (Object) null) || !this.iAm096)
      return;
    this.animationController.animator.SetBool("Rage", this.enraged == Scp096PlayerScript.RageState.Enraged || this.enraged == Scp096PlayerScript.RageState.Panic);
  }

  private void ExecuteClientsideCode()
  {
    if (!this.isLocalPlayer || !this.iAm096)
      return;
    FirstPersonController fpc1 = this.fpc;
    FirstPersonController fpc2 = this.fpc;
    double normalSpeed = (double) this.normalSpeed;
    double num1 = this.enraged != Scp096PlayerScript.RageState.Panic ? (this.enraged != Scp096PlayerScript.RageState.Enraged ? 1.0 : 2.79999995231628) : 0.0;
    double num2;
    float num3 = (float) (num2 = normalSpeed * num1);
    fpc2.m_RunSpeed = (float) num2;
    double num4 = (double) num3;
    fpc1.m_WalkSpeed = (float) num4;
    if (this.enraged != Scp096PlayerScript.RageState.Enraged || !Input.GetKey(NewInput.GetKey("Shoot")))
      return;
    this.Shoot();
  }

  public void DeductRage()
  {
    if (this.enraged != Scp096PlayerScript.RageState.Enraged)
      return;
    this.rageProgress -= Time.fixedDeltaTime * this.ragemultiplier_deduct;
    this.rageProgress = Mathf.Clamp01(this.rageProgress);
    if ((double) this.rageProgress != 0.0)
      return;
    this.cooldown = this.ragemultiplier_coodownduration;
    this.SetRage(Scp096PlayerScript.RageState.Cooldown);
  }

  public void DeductCooldown()
  {
    if (this.enraged != Scp096PlayerScript.RageState.Cooldown)
      return;
    this.cooldown -= 0.02f;
    this.cooldown = Mathf.Clamp(this.cooldown, 0.0f, this.ragemultiplier_coodownduration);
    if ((double) this.cooldown != 0.0)
      return;
    this.SetRage(Scp096PlayerScript.RageState.NotEnraged);
  }

  [DebuggerHidden]
  [ServerCallback]
  private IEnumerator<float> _ExecuteServersideCode_Looking()
  {
    if (!NetworkServer.active)
      return (IEnumerator<float>) null;
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp096PlayerScript.\u003C_ExecuteServersideCode_Looking\u003Ec__Iterator1() { \u0024this = this };
  }

  [DebuggerHidden]
  [ServerCallback]
  private IEnumerator<float> _ExecuteServersideCode_RageHandler()
  {
    if (!NetworkServer.active)
      return (IEnumerator<float>) null;
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp096PlayerScript.\u003C_ExecuteServersideCode_RageHandler\u003Ec__Iterator2() { \u0024this = this };
  }

  private void Shoot()
  {
    RaycastHit hitInfo;
    if (!Physics.Raycast(this.camera.transform.position, this.camera.transform.forward, out hitInfo, 1.5f))
      return;
    CharacterClassManager component = hitInfo.transform.GetComponent<CharacterClassManager>();
    if (!((Object) component != (Object) null) || component.klasy[component.curClass].team == Team.SCP)
      return;
    Hitmarker.Hit(1f);
    this.CallCmdHurtPlayer(hitInfo.transform.gameObject);
  }

  [Command(channel = 2)]
  private void CmdHurtPlayer(GameObject target)
  {
    if (this.ccm.curClass != 9 || (double) Vector3.Distance(this.GetComponent<PlyMovementSync>().position, target.transform.position) >= 3.0 || this.enraged != Scp096PlayerScript.RageState.Enraged)
      return;
    this.GetComponent<CharacterClassManager>().CallRpcPlaceBlood(target.transform.position, 0, 3.1f);
    target.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(99999f, "SCP096", "SCP:096", this.GetComponent<QueryProcessor>().PlayerId), target);
  }

  public void Init(int classID, Class c)
  {
    this.sameClass = c.team == Team.SCP;
    this.iAm096 = classID == 9;
    if (!this.iAm096)
      return;
    Scp096PlayerScript.instance = this;
  }

  private void Start()
  {
    this.animationController = this.GetComponent<AnimationController>();
    this.fpc = this.GetComponent<FirstPersonController>();
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.normalSpeed = this.ccm.klasy[9].runSpeed;
    Timing.RunCoroutine(this._UpdateAudios(), Segment.FixedUpdate);
    if (!this.isLocalPlayer || !this.isServer)
      return;
    Timing.RunCoroutine(this._ExecuteServersideCode_Looking(), Segment.FixedUpdate);
    Timing.RunCoroutine(this._ExecuteServersideCode_RageHandler(), Segment.FixedUpdate);
  }

  private void UNetVersion()
  {
  }

  public Scp096PlayerScript.RageState Networkenraged
  {
    get
    {
      return this.enraged;
    }
    [param: In] set
    {
      int num1 = (int) value;
      ref Scp096PlayerScript.RageState local = ref this.enraged;
      int num2 = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetRage(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<Scp096PlayerScript.RageState>((Scp096PlayerScript.RageState) num1, ref local, (uint) num2);
    }
  }

  protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdHurtPlayer called on client.");
    else
      ((Scp096PlayerScript) obj).CmdHurtPlayer(reader.ReadGameObject());
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
      writer.WritePackedUInt32((uint) Scp096PlayerScript.kCmdCmdHurtPlayer);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(target);
      this.SendCommandInternal(writer, 2, "CmdHurtPlayer");
    }
  }

  static Scp096PlayerScript()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp096PlayerScript), Scp096PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate(Scp096PlayerScript.InvokeCmdCmdHurtPlayer));
    NetworkCRC.RegisterBehaviour(nameof (Scp096PlayerScript), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write((int) this.enraged);
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
      writer.Write((int) this.enraged);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.enraged = (Scp096PlayerScript.RageState) reader.ReadInt32();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetRage((Scp096PlayerScript.RageState) reader.ReadInt32());
    }
  }

  public enum RageState
  {
    NotEnraged,
    Panic,
    Enraged,
    Cooldown,
  }
}
