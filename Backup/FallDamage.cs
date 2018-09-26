// Decompiled with JetBrains decompiler
// Type: FallDamage
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;

public class FallDamage : NetworkBehaviour
{
  private static int kCmdCmdFall = -1476756283;
  public bool isGrounded;
  public LayerMask groundMask;
  [SerializeField]
  private float groundMaxDistance;
  public AudioClip sound;
  public AudioSource sfxsrc;
  private float previousHeight;
  public AnimationCurve damageOverDistance;
  private CharacterClassManager ccm;
  public string zone;
  private static int kRpcRpcDoSound;

  public FallDamage()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer())
      return;
    this.CalculateGround();
  }

  private void CalculateGround()
  {
    if (TutorialManager.status)
      return;
    RaycastHit raycastHit;
    bool flag = Physics.Raycast(new Ray(((Component) this).get_transform().get_position(), Vector3.get_down()), ref raycastHit, this.groundMaxDistance, LayerMask.op_Implicit(this.groundMask));
    if (flag && this.zone != ((Object) ((RaycastHit) ref raycastHit).get_transform().get_root()).get_name())
    {
      this.zone = ((Object) ((RaycastHit) ref raycastHit).get_transform().get_root()).get_name();
      SoundtrackManager.singleton.mainIndex = !this.zone.Contains("Heavy") ? (!this.zone.Contains("Out") ? 0 : 2) : 1;
    }
    if (flag == this.isGrounded)
      return;
    this.isGrounded = flag;
    if (this.isGrounded)
      this.OnTouchdown();
    else
      this.OnLoseContactWithGround();
  }

  private void OnLoseContactWithGround()
  {
    this.previousHeight = (float) ((Component) this).get_transform().get_position().y;
  }

  private void OnTouchdown()
  {
    float dmg = this.damageOverDistance.Evaluate(this.previousHeight - (float) ((Component) this).get_transform().get_position().y);
    if ((double) dmg <= 5.0 || this.ccm.klasy[this.ccm.curClass].team == Team.SCP)
      return;
    if ((double) ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).health - (double) dmg <= 0.0)
      AchievementManager.Achieve("gravity");
    this.CallCmdFall(dmg);
  }

  [Command(channel = 2)]
  private void CmdFall(float dmg)
  {
    if (((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).GodMode)
      return;
    this.CallRpcDoSound(((Component) this).get_transform().get_position(), dmg);
    ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallRpcPlaceBlood(((Component) this).get_transform().get_position(), 0, Mathf.Clamp(dmg / 30f, 0.8f, 2f));
    ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(Mathf.Abs(dmg), "WORLD", "FALLDOWN", 0), ((Component) this).get_gameObject());
  }

  [ClientRpc]
  private void RpcDoSound(Vector3 pos, float dmg)
  {
    this.sfxsrc.PlayOneShot(this.sound);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdFall(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdFall called on client.");
    else
      ((FallDamage) obj).CmdFall(reader.ReadSingle());
  }

  public void CallCmdFall(float dmg)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdFall called on server.");
    else if (this.get_isServer())
    {
      this.CmdFall(dmg);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) FallDamage.kCmdCmdFall);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(dmg);
      this.SendCommandInternal(networkWriter, 2, "CmdFall");
    }
  }

  protected static void InvokeRpcRpcDoSound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcDoSound called on server.");
    else
      ((FallDamage) obj).RpcDoSound((Vector3) reader.ReadVector3(), reader.ReadSingle());
  }

  public void CallRpcDoSound(Vector3 pos, float dmg)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcDoSound called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) FallDamage.kRpcRpcDoSound);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((Vector3) pos);
      networkWriter.Write(dmg);
      this.SendRPCInternal(networkWriter, 0, "RpcDoSound");
    }
  }

  static FallDamage()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (FallDamage), FallDamage.kCmdCmdFall, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdFall)));
    FallDamage.kRpcRpcDoSound = 675793188;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (FallDamage), FallDamage.kRpcRpcDoSound, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcDoSound)));
    NetworkCRC.RegisterBehaviour(nameof (FallDamage), 0);
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
