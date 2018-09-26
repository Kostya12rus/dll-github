// Decompiled with JetBrains decompiler
// Type: Scp049PlayerScript
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance.Integrations.UNet_HLAPI;
using RemoteAdmin;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Scp049PlayerScript : NetworkBehaviour
{
  private static int kCmdCmdInfectPlayer = -2004090729;
  [Header("Player Properties")]
  public GameObject plyCam;
  public bool iAm049;
  public bool sameClass;
  public GameObject scpInstance;
  [Header("Infection")]
  public float currentInfection;
  [Header("Attack & Recall")]
  public float distance;
  public float recallDistance;
  public float recallProgress;
  public int CuredPlayers;
  private GameObject recallingObject;
  private Ragdoll recallingRagdoll;
  private ScpInterfaces interfaces;
  private Image loadingCircle;
  private FirstPersonController fpc;
  [Header("Boosts")]
  public AnimationCurve boost_recallTime;
  public AnimationCurve boost_infectTime;
  private static int kRpcRpcInfectPlayer;
  private static int kCmdCmdRecallPlayer;

  public Scp049PlayerScript()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.interfaces = (ScpInterfaces) Object.FindObjectOfType<ScpInterfaces>();
    this.loadingCircle = this.interfaces.Scp049_loading;
    if (!this.get_isLocalPlayer())
      return;
    this.fpc = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
  }

  public void Init(int classID, Class c)
  {
    this.sameClass = c.team == Team.SCP;
    this.iAm049 = classID == 5;
    if (!this.get_isLocalPlayer())
      return;
    this.interfaces.Scp049_eq.SetActive(this.iAm049);
  }

  private void Update()
  {
    this.DeductInfection();
    this.UpdateInput();
  }

  private void DeductInfection()
  {
    if ((double) this.currentInfection > 0.0)
      this.currentInfection -= Time.get_deltaTime();
    if ((double) this.currentInfection >= 0.0)
      return;
    this.currentInfection = 0.0f;
  }

  private void UpdateInput()
  {
    if (!this.get_isLocalPlayer())
      return;
    if (Input.GetKeyDown(NewInput.GetKey("Shoot")))
      this.Attack();
    if (Input.GetKeyDown(NewInput.GetKey("Interact")))
      this.Surgery();
    this.Recalling();
  }

  private void Attack()
  {
    RaycastHit raycastHit;
    if (!this.iAm049 || !Physics.Raycast(this.plyCam.get_transform().get_position(), this.plyCam.get_transform().get_forward(), ref raycastHit, this.distance))
      return;
    Scp049PlayerScript component = (Scp049PlayerScript) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponent<Scp049PlayerScript>();
    if (!Object.op_Inequality((Object) component, (Object) null) || component.sameClass)
      return;
    this.InfectPlayer(((Component) component).get_gameObject(), ((HlapiPlayer) ((Component) this).GetComponent<HlapiPlayer>()).PlayerId);
  }

  private void Surgery()
  {
    RaycastHit raycastHit;
    if (!this.iAm049 || !Physics.Raycast(this.plyCam.get_transform().get_position(), this.plyCam.get_transform().get_forward(), ref raycastHit, this.recallDistance))
      return;
    Ragdoll componentInParent = (Ragdoll) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<Ragdoll>();
    if (!Object.op_Inequality((Object) componentInParent, (Object) null) || !componentInParent.allowRecall)
      return;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (((HlapiPlayer) player.GetComponent<HlapiPlayer>()).PlayerId == componentInParent.owner.ownerHLAPI_id && (double) ((Scp049PlayerScript) player.GetComponent<Scp049PlayerScript>()).currentInfection > 0.0 && componentInParent.allowRecall)
      {
        this.recallingObject = player;
        this.recallingRagdoll = componentInParent;
      }
    }
  }

  private void DestroyPlayer(GameObject recallingRagdoll)
  {
    if (!recallingRagdoll.CompareTag("Ragdoll"))
      return;
    NetworkServer.Destroy(recallingRagdoll);
  }

  private void Recalling()
  {
    if (this.iAm049 && Input.GetKey(NewInput.GetKey("Interact")) && Object.op_Inequality((Object) this.recallingObject, (Object) null))
    {
      this.fpc.lookingAtMe = (__Null) 1;
      this.recallProgress += Time.get_deltaTime() / this.boost_recallTime.Evaluate(((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).GetHealthPercent());
      if ((double) this.recallProgress >= 1.0)
      {
        ++this.CuredPlayers;
        if (this.CuredPlayers > 9)
          AchievementManager.Achieve("turnthemall");
        this.CallCmdRecallPlayer(this.recallingObject, ((Component) this.recallingRagdoll).get_gameObject());
        this.recallProgress = 0.0f;
        this.recallingObject = (GameObject) null;
      }
    }
    else
    {
      this.recallingObject = (GameObject) null;
      this.recallProgress = 0.0f;
      if (this.iAm049)
        this.fpc.lookingAtMe = (__Null) 0;
    }
    this.loadingCircle.set_fillAmount(this.recallProgress);
  }

  private void InfectPlayer(GameObject target, string id)
  {
    this.CallCmdInfectPlayer(target, id);
    Hitmarker.Hit(1f);
  }

  [Command(channel = 2)]
  private void CmdInfectPlayer(GameObject target, string id)
  {
    if (((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).curClass != 5 || (double) Vector3.Distance(target.get_transform().get_position(), ((PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>()).position) >= (double) this.distance * 1.29999995231628)
      return;
    ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(4949f, id, "SCP:049", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), target);
    this.CallRpcInfectPlayer(target, this.boost_infectTime.Evaluate(((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).GetHealthPercent()));
  }

  [ClientRpc(channel = 2)]
  private void RpcInfectPlayer(GameObject target, float infTime)
  {
    ((Scp049PlayerScript) target.GetComponent<Scp049PlayerScript>()).currentInfection = infTime;
  }

  [Command(channel = 2)]
  private void CmdRecallPlayer(GameObject target, GameObject ragdoll)
  {
    CharacterClassManager component1 = (CharacterClassManager) target.GetComponent<CharacterClassManager>();
    Ragdoll component2 = (Ragdoll) ragdoll.GetComponent<Ragdoll>();
    if (!Object.op_Inequality((Object) component2, (Object) null) || !Object.op_Inequality((Object) component1, (Object) null) || (component1.curClass != 2 || ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).curClass != 5) || !(component2.owner.deathCause.tool == "SCP:049"))
      return;
    ++RoundSummary.changed_into_zombies;
    component1.SetClassID(10);
    ((PlayerStats) target.GetComponent<PlayerStats>()).Networkhealth = component1.klasy[10].maxHP;
    this.DestroyPlayer(ragdoll);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdInfectPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdInfectPlayer called on client.");
    else
      ((Scp049PlayerScript) obj).CmdInfectPlayer((GameObject) reader.ReadGameObject(), reader.ReadString());
  }

  protected static void InvokeCmdCmdRecallPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRecallPlayer called on client.");
    else
      ((Scp049PlayerScript) obj).CmdRecallPlayer((GameObject) reader.ReadGameObject(), (GameObject) reader.ReadGameObject());
  }

  public void CallCmdInfectPlayer(GameObject target, string id)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdInfectPlayer called on server.");
    else if (this.get_isServer())
    {
      this.CmdInfectPlayer(target, id);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp049PlayerScript.kCmdCmdInfectPlayer);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) target);
      networkWriter.Write(id);
      this.SendCommandInternal(networkWriter, 2, "CmdInfectPlayer");
    }
  }

  public void CallCmdRecallPlayer(GameObject target, GameObject ragdoll)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRecallPlayer called on server.");
    else if (this.get_isServer())
    {
      this.CmdRecallPlayer(target, ragdoll);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Scp049PlayerScript.kCmdCmdRecallPlayer);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) target);
      networkWriter.Write((GameObject) ragdoll);
      this.SendCommandInternal(networkWriter, 2, "CmdRecallPlayer");
    }
  }

  protected static void InvokeRpcRpcInfectPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcInfectPlayer called on server.");
    else
      ((Scp049PlayerScript) obj).RpcInfectPlayer((GameObject) reader.ReadGameObject(), reader.ReadSingle());
  }

  public void CallRpcInfectPlayer(GameObject target, float infTime)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcInfectPlayer called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Scp049PlayerScript.kRpcRpcInfectPlayer);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) target);
      networkWriter.Write(infTime);
      this.SendRPCInternal(networkWriter, 2, "RpcInfectPlayer");
    }
  }

  static Scp049PlayerScript()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp049PlayerScript), Scp049PlayerScript.kCmdCmdInfectPlayer, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdInfectPlayer)));
    Scp049PlayerScript.kCmdCmdRecallPlayer = 1670066835;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp049PlayerScript), Scp049PlayerScript.kCmdCmdRecallPlayer, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRecallPlayer)));
    Scp049PlayerScript.kRpcRpcInfectPlayer = -1920658579;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp049PlayerScript), Scp049PlayerScript.kRpcRpcInfectPlayer, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcInfectPlayer)));
    NetworkCRC.RegisterBehaviour(nameof (Scp049PlayerScript), 0);
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
