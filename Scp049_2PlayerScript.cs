// Decompiled with JetBrains decompiler
// Type: Scp049_2PlayerScript
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance.Integrations.UNet_HLAPI;
using MEC;
using RemoteAdmin;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

public class Scp049_2PlayerScript : NetworkBehaviour
{
  private static int kCmdCmdHurtPlayer = 21222532;
  [Header("Player Properties")]
  public Transform plyCam;
  public Animator animator;
  public bool iAm049_2;
  public bool sameClass;
  [Header("Attack")]
  public float distance;
  public int damage;
  [Header("Boosts")]
  public AnimationCurve multiplier;
  private static int kCmdCmdShootAnim;
  private static int kRpcRpcShootAnim;

  public Scp049_2PlayerScript()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (!this.get_isLocalPlayer())
      return;
    Timing.RunCoroutine(this._UpdateInput(), (Segment) 1);
  }

  public void Init(int classID, Class c)
  {
    this.sameClass = c.team == Team.SCP;
    this.iAm049_2 = classID == 10;
    ((Component) this.animator).get_gameObject().SetActive(this.get_isLocalPlayer() && this.iAm049_2);
  }

  [DebuggerHidden]
  private IEnumerator<float> _UpdateInput()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp049_2PlayerScript.\u003C_UpdateInput\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void Attack()
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(((Component) this.plyCam).get_transform().get_position(), ((Component) this.plyCam).get_transform().get_forward(), ref raycastHit, this.distance))
      return;
    Scp049_2PlayerScript scp0492PlayerScript = (Scp049_2PlayerScript) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponent<Scp049_2PlayerScript>();
    if (Object.op_Equality((Object) scp0492PlayerScript, (Object) null))
      scp0492PlayerScript = (Scp049_2PlayerScript) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<Scp049_2PlayerScript>();
    if (!Object.op_Inequality((Object) scp0492PlayerScript, (Object) null) || scp0492PlayerScript.sameClass)
      return;
    Hitmarker.Hit(1f);
    this.CallCmdHurtPlayer(((Component) ((RaycastHit) ref raycastHit).get_transform()).get_gameObject(), ((HlapiPlayer) ((Component) this).GetComponent<HlapiPlayer>()).PlayerId);
  }

  [Command(channel = 2)]
  private void CmdHurtPlayer(GameObject ply, string id)
  {
    if ((double) Vector3.Distance(((PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>()).position, ply.get_transform().get_position()) > (double) this.distance * 1.5 || ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).curClass != 10)
      return;
    Vector3 position = ply.get_transform().get_position();
    ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo((float) this.damage, id, "SCP:0492", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), ply);
    if (((CharacterClassManager) ply.GetComponent<CharacterClassManager>()).curClass == 2)
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallRpcPlaceBlood(position, 0, 1.3f);
    else
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallRpcPlaceBlood(position, 0, 0.5f);
  }

  [Command(channel = 1)]
  private void CmdShootAnim()
  {
    this.CallRpcShootAnim();
  }

  [ClientRpc]
  private void RpcShootAnim()
  {
    ((AnimationController) ((Component) this).GetComponent<AnimationController>()).DoAnimation("Shoot");
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdHurtPlayer called on client.");
    else
      ((Scp049_2PlayerScript) obj).CmdHurtPlayer((GameObject) reader.ReadGameObject(), reader.ReadString());
  }

  protected static void InvokeCmdCmdShootAnim(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdShootAnim called on client.");
    else
      ((Scp049_2PlayerScript) obj).CmdShootAnim();
  }

  public void CallCmdHurtPlayer(GameObject ply, string id)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdHurtPlayer called on server.");
    else if (this.get_isServer())
    {
      this.CmdHurtPlayer(ply, id);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp049_2PlayerScript.kCmdCmdHurtPlayer);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) ply);
      networkWriter.Write(id);
      this.SendCommandInternal(networkWriter, 2, "CmdHurtPlayer");
    }
  }

  public void CallCmdShootAnim()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdShootAnim called on server.");
    else if (this.get_isServer())
    {
      this.CmdShootAnim();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp049_2PlayerScript.kCmdCmdShootAnim);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 1, "CmdShootAnim");
    }
  }

  protected static void InvokeRpcRpcShootAnim(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcShootAnim called on server.");
    else
      ((Scp049_2PlayerScript) obj).RpcShootAnim();
  }

  public void CallRpcShootAnim()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcShootAnim called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Scp049_2PlayerScript.kRpcRpcShootAnim);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 0, "RpcShootAnim");
    }
  }

  static Scp049_2PlayerScript()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp049_2PlayerScript), Scp049_2PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdHurtPlayer)));
    Scp049_2PlayerScript.kCmdCmdShootAnim = 1794565020;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp049_2PlayerScript), Scp049_2PlayerScript.kCmdCmdShootAnim, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdShootAnim)));
    Scp049_2PlayerScript.kRpcRpcShootAnim = 201633926;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp049_2PlayerScript), Scp049_2PlayerScript.kRpcRpcShootAnim, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcShootAnim)));
    NetworkCRC.RegisterBehaviour(nameof (Scp049_2PlayerScript), 0);
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
