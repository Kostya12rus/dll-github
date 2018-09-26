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
  [Space]
  public float ragemultiplier_deduct;
  public float ragemultiplier_coodownduration;
  public AnimationCurve lookingTolerance;
  private float t;
  private CharacterClassManager ccm;
  private FirstPersonController fpc;
  private float normalSpeed;

  public Scp096PlayerScript()
  {
    base.\u002Ector();
  }

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
    ((MonoBehaviour) this).Invoke("StartRage", 5f);
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
    return (IEnumerator<float>) new Scp096PlayerScript.\u003C_UpdateAudios\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void Animator()
  {
    if (this.get_isLocalPlayer() || !Object.op_Inequality((Object) this.animationController.animator, (Object) null) || !this.iAm096)
      return;
    this.animationController.animator.SetBool("Rage", this.enraged == Scp096PlayerScript.RageState.Enraged || this.enraged == Scp096PlayerScript.RageState.Panic);
  }

  private void ExecuteClientsideCode()
  {
    if (!this.get_isLocalPlayer() || !this.iAm096)
      return;
    FirstPersonController fpc1 = this.fpc;
    FirstPersonController fpc2 = this.fpc;
    double normalSpeed = (double) this.normalSpeed;
    double num1 = this.enraged != Scp096PlayerScript.RageState.Panic ? (this.enraged != Scp096PlayerScript.RageState.Enraged ? 1.0 : 2.79999995231628) : 0.0;
    double num2;
    float num3 = (float) (num2 = normalSpeed * num1);
    fpc2.m_RunSpeed = (__Null) num2;
    double num4 = (double) num3;
    fpc1.m_WalkSpeed = (__Null) num4;
    if (this.enraged != Scp096PlayerScript.RageState.Enraged || !Input.GetKey(NewInput.GetKey("Shoot")))
      return;
    this.Shoot();
  }

  public void DeductRage()
  {
    if (this.enraged != Scp096PlayerScript.RageState.Enraged)
      return;
    this.rageProgress -= Time.get_fixedDeltaTime() * this.ragemultiplier_deduct;
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
    if (!NetworkServer.get_active())
      return (IEnumerator<float>) null;
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp096PlayerScript.\u003C_ExecuteServersideCode_Looking\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  [ServerCallback]
  private IEnumerator<float> _ExecuteServersideCode_RageHandler()
  {
    if (!NetworkServer.get_active())
      return (IEnumerator<float>) null;
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp096PlayerScript.\u003C_ExecuteServersideCode_RageHandler\u003Ec__Iterator2()
    {
      \u0024this = this
    };
  }

  private void Shoot()
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(this.camera.get_transform().get_position(), this.camera.get_transform().get_forward(), ref raycastHit, 1.5f))
      return;
    CharacterClassManager component = (CharacterClassManager) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponent<CharacterClassManager>();
    if (!Object.op_Inequality((Object) component, (Object) null) || component.klasy[component.curClass].team == Team.SCP)
      return;
    Hitmarker.Hit(1f);
    this.CallCmdHurtPlayer(((Component) ((RaycastHit) ref raycastHit).get_transform()).get_gameObject());
  }

  [Command(channel = 2)]
  private void CmdHurtPlayer(GameObject target)
  {
    if (this.ccm.curClass != 9 || (double) Vector3.Distance(((PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>()).position, target.get_transform().get_position()) >= 3.0 || this.enraged != Scp096PlayerScript.RageState.Enraged)
      return;
    ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallRpcPlaceBlood(target.get_transform().get_position(), 0, 3.1f);
    ((PlayerStats) target.GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(99999f, "SCP096", "SCP:096", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), target);
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
    this.animationController = (AnimationController) ((Component) this).GetComponent<AnimationController>();
    this.fpc = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.normalSpeed = this.ccm.klasy[9].runSpeed;
    Timing.RunCoroutine(this._UpdateAudios(), (Segment) 1);
    if (!this.get_isLocalPlayer() || !this.get_isServer())
      return;
    Timing.RunCoroutine(this._ExecuteServersideCode_Looking(), (Segment) 1);
    Timing.RunCoroutine(this._ExecuteServersideCode_RageHandler(), (Segment) 1);
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetRage(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<Scp096PlayerScript.RageState>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdHurtPlayer called on client.");
    else
      ((Scp096PlayerScript) obj).CmdHurtPlayer((GameObject) reader.ReadGameObject());
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
      networkWriter.WritePackedUInt32((uint) Scp096PlayerScript.kCmdCmdHurtPlayer);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) target);
      this.SendCommandInternal(networkWriter, 2, "CmdHurtPlayer");
    }
  }

  static Scp096PlayerScript()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp096PlayerScript), Scp096PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdHurtPlayer)));
    NetworkCRC.RegisterBehaviour(nameof (Scp096PlayerScript), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write((int) this.enraged);
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
      writer.Write((int) this.enraged);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
