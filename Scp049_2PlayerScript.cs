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
  [Header("Attack")]
  public float distance = 2.4f;
  public int damage = 60;
  [Header("Player Properties")]
  public Transform plyCam;
  public Animator animator;
  public bool iAm049_2;
  public bool sameClass;
  [Header("Boosts")]
  public AnimationCurve multiplier;
  private static int kCmdCmdShootAnim;
  private static int kRpcRpcShootAnim;

  private void Start()
  {
    if (!this.isLocalPlayer)
      return;
    Timing.RunCoroutine(this._UpdateInput(), Segment.FixedUpdate);
  }

  public void Init(int classID, Class c)
  {
    this.sameClass = c.team == Team.SCP;
    this.iAm049_2 = classID == 10;
    this.animator.gameObject.SetActive(this.isLocalPlayer && this.iAm049_2);
  }

  [DebuggerHidden]
  private IEnumerator<float> _UpdateInput()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Scp049_2PlayerScript.\u003C_UpdateInput\u003Ec__Iterator0() { \u0024this = this };
  }

  private void Attack()
  {
    RaycastHit hitInfo;
    if (!Physics.Raycast(this.plyCam.transform.position, this.plyCam.transform.forward, out hitInfo, this.distance))
      return;
    Scp049_2PlayerScript scp0492PlayerScript = hitInfo.transform.GetComponent<Scp049_2PlayerScript>();
    if ((Object) scp0492PlayerScript == (Object) null)
      scp0492PlayerScript = hitInfo.transform.GetComponentInParent<Scp049_2PlayerScript>();
    if (!((Object) scp0492PlayerScript != (Object) null) || scp0492PlayerScript.sameClass)
      return;
    Hitmarker.Hit(1f);
    this.CallCmdHurtPlayer(hitInfo.transform.gameObject, this.GetComponent<HlapiPlayer>().PlayerId);
  }

  [Command(channel = 2)]
  private void CmdHurtPlayer(GameObject ply, string id)
  {
    if ((double) Vector3.Distance(this.GetComponent<PlyMovementSync>().position, ply.transform.position) > (double) this.distance * 1.5 || this.GetComponent<CharacterClassManager>().curClass != 10)
      return;
    Vector3 position = ply.transform.position;
    this.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo((float) this.damage, id, "SCP:0492", this.GetComponent<QueryProcessor>().PlayerId), ply);
    if (ply.GetComponent<CharacterClassManager>().curClass == 2)
      this.GetComponent<CharacterClassManager>().CallRpcPlaceBlood(position, 0, 1.3f);
    else
      this.GetComponent<CharacterClassManager>().CallRpcPlaceBlood(position, 0, 0.5f);
  }

  [Command(channel = 1)]
  private void CmdShootAnim()
  {
    this.CallRpcShootAnim();
  }

  [ClientRpc]
  private void RpcShootAnim()
  {
    this.GetComponent<AnimationController>().DoAnimation("Shoot");
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdHurtPlayer called on client.");
    else
      ((Scp049_2PlayerScript) obj).CmdHurtPlayer(reader.ReadGameObject(), reader.ReadString());
  }

  protected static void InvokeCmdCmdShootAnim(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdShootAnim called on client.");
    else
      ((Scp049_2PlayerScript) obj).CmdShootAnim();
  }

  public void CallCmdHurtPlayer(GameObject ply, string id)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdHurtPlayer called on server.");
    else if (this.isServer)
    {
      this.CmdHurtPlayer(ply, id);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Scp049_2PlayerScript.kCmdCmdHurtPlayer);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(ply);
      writer.Write(id);
      this.SendCommandInternal(writer, 2, "CmdHurtPlayer");
    }
  }

  public void CallCmdShootAnim()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdShootAnim called on server.");
    else if (this.isServer)
    {
      this.CmdShootAnim();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Scp049_2PlayerScript.kCmdCmdShootAnim);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 1, "CmdShootAnim");
    }
  }

  protected static void InvokeRpcRpcShootAnim(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcShootAnim called on server.");
    else
      ((Scp049_2PlayerScript) obj).RpcShootAnim();
  }

  public void CallRpcShootAnim()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcShootAnim called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Scp049_2PlayerScript.kRpcRpcShootAnim);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 0, "RpcShootAnim");
    }
  }

  static Scp049_2PlayerScript()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp049_2PlayerScript), Scp049_2PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate(Scp049_2PlayerScript.InvokeCmdCmdHurtPlayer));
    Scp049_2PlayerScript.kCmdCmdShootAnim = 1794565020;
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp049_2PlayerScript), Scp049_2PlayerScript.kCmdCmdShootAnim, new NetworkBehaviour.CmdDelegate(Scp049_2PlayerScript.InvokeCmdCmdShootAnim));
    Scp049_2PlayerScript.kRpcRpcShootAnim = 201633926;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp049_2PlayerScript), Scp049_2PlayerScript.kRpcRpcShootAnim, new NetworkBehaviour.CmdDelegate(Scp049_2PlayerScript.InvokeRpcRpcShootAnim));
    NetworkCRC.RegisterBehaviour(nameof (Scp049_2PlayerScript), 0);
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
