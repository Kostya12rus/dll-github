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
  public bool isGrounded = true;
  [SerializeField]
  private float groundMaxDistance = 1.3f;
  public LayerMask groundMask;
  public AudioClip sound;
  public AudioSource sfxsrc;
  private float previousHeight;
  public AnimationCurve damageOverDistance;
  private CharacterClassManager ccm;
  public string zone;
  private static int kRpcRpcDoSound;

  private void Start()
  {
    this.ccm = this.GetComponent<CharacterClassManager>();
  }

  private void Update()
  {
    if (!this.isLocalPlayer)
      return;
    this.CalculateGround();
  }

  private void CalculateGround()
  {
    if (TutorialManager.status)
      return;
    RaycastHit hitInfo;
    bool flag = Physics.Raycast(new Ray(this.transform.position, Vector3.down), out hitInfo, this.groundMaxDistance, (int) this.groundMask);
    if (flag && this.zone != hitInfo.transform.root.name)
    {
      this.zone = hitInfo.transform.root.name;
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
    this.previousHeight = this.transform.position.y;
  }

  private void OnTouchdown()
  {
    float dmg = this.damageOverDistance.Evaluate(this.previousHeight - this.transform.position.y);
    if ((double) dmg <= 5.0 || this.ccm.klasy[this.ccm.curClass].team == Team.SCP)
      return;
    if ((double) this.GetComponent<PlayerStats>().health - (double) dmg <= 0.0)
      AchievementManager.Achieve("gravity");
    this.CallCmdFall(dmg);
  }

  [Command(channel = 2)]
  private void CmdFall(float dmg)
  {
    if (this.GetComponent<CharacterClassManager>().GodMode)
      return;
    this.CallRpcDoSound(this.transform.position, dmg);
    this.GetComponent<CharacterClassManager>().CallRpcPlaceBlood(this.transform.position, 0, Mathf.Clamp(dmg / 30f, 0.8f, 2f));
    this.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(Mathf.Abs(dmg), "WORLD", "FALLDOWN", 0), this.gameObject);
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
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdFall called on client.");
    else
      ((FallDamage) obj).CmdFall(reader.ReadSingle());
  }

  public void CallCmdFall(float dmg)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdFall called on server.");
    else if (this.isServer)
    {
      this.CmdFall(dmg);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) FallDamage.kCmdCmdFall);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(dmg);
      this.SendCommandInternal(writer, 2, "CmdFall");
    }
  }

  protected static void InvokeRpcRpcDoSound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcDoSound called on server.");
    else
      ((FallDamage) obj).RpcDoSound(reader.ReadVector3(), reader.ReadSingle());
  }

  public void CallRpcDoSound(Vector3 pos, float dmg)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcDoSound called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) FallDamage.kRpcRpcDoSound);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(pos);
      writer.Write(dmg);
      this.SendRPCInternal(writer, 0, "RpcDoSound");
    }
  }

  static FallDamage()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (FallDamage), FallDamage.kCmdCmdFall, new NetworkBehaviour.CmdDelegate(FallDamage.InvokeCmdCmdFall));
    FallDamage.kRpcRpcDoSound = 675793188;
    NetworkBehaviour.RegisterRpcDelegate(typeof (FallDamage), FallDamage.kRpcRpcDoSound, new NetworkBehaviour.CmdDelegate(FallDamage.InvokeRpcRpcDoSound));
    NetworkCRC.RegisterBehaviour(nameof (FallDamage), 0);
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
