// Decompiled with JetBrains decompiler
// Type: Scp939PlayerScript
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance;
using RemoteAdmin;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class Scp939PlayerScript : NetworkBehaviour
{
  public static List<Scp939PlayerScript> instances = new List<Scp939PlayerScript>();
  private static int kCmdCmdChangeHumanchatThing = 1697597860;
  public bool iAm939;
  public bool sameClass;
  public LayerMask normalVision;
  public LayerMask scpVision;
  public Camera visionCamera;
  public Behaviour[] visualEffects;
  public LayerMask attackMask;
  public AudioClip[] attackSounds;
  public float attackDistance;
  [SyncVar]
  public float speedMultiplier;
  private Camera plyCam;
  [SyncVar]
  public bool usingHumanChat;
  private bool prevuhc;
  private KeyCode speechKey;
  private float cooldown;
  private static int kCmdCmdShoot;
  private static int kRpcRpcShoot;

  public Scp939PlayerScript()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (!this.get_isLocalPlayer())
      return;
    this.speechKey = NewInput.GetKey("939's speech");
    Scp939PlayerScript.instances = new List<Scp939PlayerScript>();
    this.plyCam = (Camera) ((Scp049PlayerScript) ((Component) this).GetComponent<Scp049PlayerScript>()).plyCam.GetComponent<Camera>();
  }

  public void Init(int classID, Class c)
  {
    this.sameClass = c.team == Team.SCP;
    this.iAm939 = c.fullName.Contains("939");
    if (this.iAm939 && !Scp939PlayerScript.instances.Contains(this))
      Scp939PlayerScript.instances.Add(this);
    if (!this.get_isLocalPlayer())
      return;
    foreach (Behaviour visualEffect in this.visualEffects)
      visualEffect.set_enabled(this.iAm939);
    this.plyCam.set_renderingPath(!this.iAm939 ? (RenderingPath) 3 : (RenderingPath) 0);
    this.plyCam.set_cullingMask(LayerMask.op_Implicit(!this.iAm939 ? this.normalVision : this.scpVision));
    ((Component) this.visionCamera).get_gameObject().SetActive(this.iAm939);
    this.visionCamera.set_fieldOfView(this.plyCam.get_fieldOfView());
    this.visionCamera.set_farClipPlane(this.plyCam.get_farClipPlane());
  }

  private void Update()
  {
    this.CheckInstances();
    this.CheckForInput();
    this.ServersideCode();
    this.ClientsideMisc();
  }

  private void ServersideCode()
  {
    if (!NetworkServer.get_active())
      return;
    if ((double) this.cooldown >= 0.0)
      this.cooldown -= Time.get_deltaTime();
    if ((double) this.speedMultiplier > 1.0)
    {
      Scp939PlayerScript scp939PlayerScript = this;
      scp939PlayerScript.NetworkspeedMultiplier = scp939PlayerScript.speedMultiplier - Time.get_deltaTime() / 3f;
    }
    if ((double) this.speedMultiplier >= 1.0)
      return;
    this.NetworkspeedMultiplier = 1f;
  }

  private void ClientsideMisc()
  {
    if (!this.get_isLocalPlayer())
      return;
    if (this.iAm939)
      FirstPersonController.speedMultiplier939 = (__Null) (double) this.speedMultiplier;
    else
      FirstPersonController.speedMultiplier939 = (__Null) 1.0;
  }

  private void CheckForInput()
  {
    if (!this.get_isLocalPlayer())
      return;
    if (this.iAm939 && Input.GetKey(NewInput.GetKey("Shoot")))
      this.Shoot();
    bool newValue = Input.GetKey(this.speechKey) && this.iAm939;
    if (this.prevuhc == newValue)
      return;
    this.prevuhc = newValue;
    this.CallCmdChangeHumanchatThing(newValue);
    VoiceBroadcastTrigger.is939Speaking = (__Null) (newValue ? 1 : 0);
  }

  [Command]
  private void CmdChangeHumanchatThing(bool newValue)
  {
    if (this.iAm939)
      this.NetworkusingHumanChat = newValue;
    else
      this.NetworkusingHumanChat = false;
  }

  private void Shoot()
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(new Ray(((Component) this.plyCam).get_transform().get_position(), ((Component) this.plyCam).get_transform().get_forward()), ref raycastHit, this.attackDistance))
      return;
    Scp939PlayerScript componentInParent = (Scp939PlayerScript) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponentInParent<Scp939PlayerScript>();
    if (!Object.op_Inequality((Object) componentInParent, (Object) null) || componentInParent.sameClass)
      return;
    this.CallCmdShoot(((Component) componentInParent).get_gameObject());
  }

  [Command]
  private void CmdShoot(GameObject attacker)
  {
    if (!this.iAm939 || (double) Vector3.Distance(attacker.get_transform().get_position(), ((Component) this).get_transform().get_position()) >= (double) this.attackDistance * 1.20000004768372 || (double) this.cooldown > 0.0)
      return;
    this.cooldown = 1f;
    ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo((float) Random.Range(50, 80), "SCP:939", "SCP:939", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), attacker);
    this.CallRpcShoot();
  }

  [ClientRpc]
  private void RpcShoot()
  {
    Animator component = (Animator) ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).myModel.GetComponent<Animator>();
    ((AudioSource) ((Component) component).GetComponent<AudioSource>()).PlayOneShot(this.attackSounds[Random.Range(0, this.attackSounds.Length)]);
    if (this.get_isLocalPlayer())
      Hitmarker.Hit(1.5f);
    else
      component.SetTrigger("Attack");
  }

  private void CheckInstances()
  {
    if (!(((Object) this).get_name() == "Host"))
      return;
    foreach (Scp939PlayerScript instance in Scp939PlayerScript.instances)
    {
      if (Object.op_Equality((Object) instance, (Object) null) || !instance.iAm939)
      {
        Scp939PlayerScript.instances.Remove(instance);
        break;
      }
    }
  }

  static Scp939PlayerScript()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp939PlayerScript), Scp939PlayerScript.kCmdCmdChangeHumanchatThing, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdChangeHumanchatThing)));
    Scp939PlayerScript.kCmdCmdShoot = -1473386604;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp939PlayerScript), Scp939PlayerScript.kCmdCmdShoot, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdShoot)));
    Scp939PlayerScript.kRpcRpcShoot = -1725253250;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp939PlayerScript), Scp939PlayerScript.kRpcRpcShoot, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcShoot)));
    NetworkCRC.RegisterBehaviour(nameof (Scp939PlayerScript), 0);
  }

  private void UNetVersion()
  {
  }

  public float NetworkspeedMultiplier
  {
    get
    {
      return this.speedMultiplier;
    }
    [param: In] set
    {
      this.SetSyncVar<float>((M0) (double) value, (M0&) ref this.speedMultiplier, 1U);
    }
  }

  public bool NetworkusingHumanChat
  {
    get
    {
      return this.usingHumanChat;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this.usingHumanChat, 2U);
    }
  }

  protected static void InvokeCmdCmdChangeHumanchatThing(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdChangeHumanchatThing called on client.");
    else
      ((Scp939PlayerScript) obj).CmdChangeHumanchatThing(reader.ReadBoolean());
  }

  protected static void InvokeCmdCmdShoot(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdShoot called on client.");
    else
      ((Scp939PlayerScript) obj).CmdShoot((GameObject) reader.ReadGameObject());
  }

  public void CallCmdChangeHumanchatThing(bool newValue)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdChangeHumanchatThing called on server.");
    else if (this.get_isServer())
    {
      this.CmdChangeHumanchatThing(newValue);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp939PlayerScript.kCmdCmdChangeHumanchatThing);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(newValue);
      this.SendCommandInternal(networkWriter, 0, "CmdChangeHumanchatThing");
    }
  }

  public void CallCmdShoot(GameObject attacker)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdShoot called on server.");
    else if (this.get_isServer())
    {
      this.CmdShoot(attacker);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp939PlayerScript.kCmdCmdShoot);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) attacker);
      this.SendCommandInternal(networkWriter, 0, "CmdShoot");
    }
  }

  protected static void InvokeRpcRpcShoot(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcShoot called on server.");
    else
      ((Scp939PlayerScript) obj).RpcShoot();
  }

  public void CallRpcShoot()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcShoot called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Scp939PlayerScript.kRpcRpcShoot);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 0, "RpcShoot");
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.speedMultiplier);
      writer.Write(this.usingHumanChat);
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
      writer.Write(this.speedMultiplier);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.usingHumanChat);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.speedMultiplier = reader.ReadSingle();
      this.usingHumanChat = reader.ReadBoolean();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.speedMultiplier = reader.ReadSingle();
      if ((num & 2) == 0)
        return;
      this.usingHumanChat = reader.ReadBoolean();
    }
  }
}
