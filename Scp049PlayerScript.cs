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
  [Header("Attack & Recall")]
  public float distance = 2.4f;
  public float recallDistance = 3.5f;
  [Header("Player Properties")]
  public GameObject plyCam;
  public bool iAm049;
  public bool sameClass;
  public GameObject scpInstance;
  [Header("Infection")]
  public float currentInfection;
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

  private void Start()
  {
    this.interfaces = Object.FindObjectOfType<ScpInterfaces>();
    this.loadingCircle = this.interfaces.Scp049_loading;
    if (!this.isLocalPlayer)
      return;
    this.fpc = this.GetComponent<FirstPersonController>();
  }

  public void Init(int classID, Class c)
  {
    this.sameClass = c.team == Team.SCP;
    this.iAm049 = classID == 5;
    if (!this.isLocalPlayer)
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
      this.currentInfection -= Time.deltaTime;
    if ((double) this.currentInfection >= 0.0)
      return;
    this.currentInfection = 0.0f;
  }

  private void UpdateInput()
  {
    if (!this.isLocalPlayer)
      return;
    if (Input.GetKeyDown(NewInput.GetKey("Shoot")))
      this.Attack();
    if (Input.GetKeyDown(NewInput.GetKey("Interact")))
      this.Surgery();
    this.Recalling();
  }

  private void Attack()
  {
    RaycastHit hitInfo;
    if (!this.iAm049 || !Physics.Raycast(this.plyCam.transform.position, this.plyCam.transform.forward, out hitInfo, this.distance))
      return;
    Scp049PlayerScript component = hitInfo.transform.GetComponent<Scp049PlayerScript>();
    if (!((Object) component != (Object) null) || component.sameClass)
      return;
    this.InfectPlayer(component.gameObject, this.GetComponent<HlapiPlayer>().PlayerId);
  }

  private void Surgery()
  {
    RaycastHit hitInfo;
    if (!this.iAm049 || !Physics.Raycast(this.plyCam.transform.position, this.plyCam.transform.forward, out hitInfo, this.recallDistance))
      return;
    Ragdoll componentInParent = hitInfo.transform.GetComponentInParent<Ragdoll>();
    if (!((Object) componentInParent != (Object) null) || !componentInParent.allowRecall)
      return;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (player.GetComponent<HlapiPlayer>().PlayerId == componentInParent.owner.ownerHLAPI_id && (double) player.GetComponent<Scp049PlayerScript>().currentInfection > 0.0 && componentInParent.allowRecall)
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
    if (this.iAm049 && Input.GetKey(NewInput.GetKey("Interact")) && (Object) this.recallingObject != (Object) null)
    {
      this.fpc.lookingAtMe = true;
      this.recallProgress += Time.deltaTime / this.boost_recallTime.Evaluate(this.GetComponent<PlayerStats>().GetHealthPercent());
      if ((double) this.recallProgress >= 1.0)
      {
        ++this.CuredPlayers;
        if (this.CuredPlayers > 9)
          AchievementManager.Achieve("turnthemall");
        this.CallCmdRecallPlayer(this.recallingObject, this.recallingRagdoll.gameObject);
        this.recallProgress = 0.0f;
        this.recallingObject = (GameObject) null;
      }
    }
    else
    {
      this.recallingObject = (GameObject) null;
      this.recallProgress = 0.0f;
      if (this.iAm049)
        this.fpc.lookingAtMe = false;
    }
    this.loadingCircle.fillAmount = this.recallProgress;
  }

  private void InfectPlayer(GameObject target, string id)
  {
    this.CallCmdInfectPlayer(target, id);
    Hitmarker.Hit(1f);
  }

  [Command(channel = 2)]
  private void CmdInfectPlayer(GameObject target, string id)
  {
    if (this.GetComponent<CharacterClassManager>().curClass != 5 || (double) Vector3.Distance(target.transform.position, this.GetComponent<PlyMovementSync>().position) >= (double) this.distance * 1.29999995231628)
      return;
    this.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(4949f, id, "SCP:049", this.GetComponent<QueryProcessor>().PlayerId), target);
    this.CallRpcInfectPlayer(target, this.boost_infectTime.Evaluate(this.GetComponent<PlayerStats>().GetHealthPercent()));
  }

  [ClientRpc(channel = 2)]
  private void RpcInfectPlayer(GameObject target, float infTime)
  {
    target.GetComponent<Scp049PlayerScript>().currentInfection = infTime;
  }

  [Command(channel = 2)]
  private void CmdRecallPlayer(GameObject target, GameObject ragdoll)
  {
    CharacterClassManager component1 = target.GetComponent<CharacterClassManager>();
    Ragdoll component2 = ragdoll.GetComponent<Ragdoll>();
    if (!((Object) component2 != (Object) null) || !((Object) component1 != (Object) null) || (component1.curClass != 2 || this.GetComponent<CharacterClassManager>().curClass != 5) || !(component2.owner.deathCause.tool == "SCP:049"))
      return;
    ++RoundSummary.changed_into_zombies;
    component1.SetClassID(10);
    target.GetComponent<PlayerStats>().Networkhealth = component1.klasy[10].maxHP;
    this.DestroyPlayer(ragdoll);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdInfectPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdInfectPlayer called on client.");
    else
      ((Scp049PlayerScript) obj).CmdInfectPlayer(reader.ReadGameObject(), reader.ReadString());
  }

  protected static void InvokeCmdCmdRecallPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdRecallPlayer called on client.");
    else
      ((Scp049PlayerScript) obj).CmdRecallPlayer(reader.ReadGameObject(), reader.ReadGameObject());
  }

  public void CallCmdInfectPlayer(GameObject target, string id)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdInfectPlayer called on server.");
    else if (this.isServer)
    {
      this.CmdInfectPlayer(target, id);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Scp049PlayerScript.kCmdCmdInfectPlayer);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(target);
      writer.Write(id);
      this.SendCommandInternal(writer, 2, "CmdInfectPlayer");
    }
  }

  public void CallCmdRecallPlayer(GameObject target, GameObject ragdoll)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdRecallPlayer called on server.");
    else if (this.isServer)
    {
      this.CmdRecallPlayer(target, ragdoll);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Scp049PlayerScript.kCmdCmdRecallPlayer);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(target);
      writer.Write(ragdoll);
      this.SendCommandInternal(writer, 2, "CmdRecallPlayer");
    }
  }

  protected static void InvokeRpcRpcInfectPlayer(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcInfectPlayer called on server.");
    else
      ((Scp049PlayerScript) obj).RpcInfectPlayer(reader.ReadGameObject(), reader.ReadSingle());
  }

  public void CallRpcInfectPlayer(GameObject target, float infTime)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcInfectPlayer called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Scp049PlayerScript.kRpcRpcInfectPlayer);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(target);
      writer.Write(infTime);
      this.SendRPCInternal(writer, 2, "RpcInfectPlayer");
    }
  }

  static Scp049PlayerScript()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp049PlayerScript), Scp049PlayerScript.kCmdCmdInfectPlayer, new NetworkBehaviour.CmdDelegate(Scp049PlayerScript.InvokeCmdCmdInfectPlayer));
    Scp049PlayerScript.kCmdCmdRecallPlayer = 1670066835;
    NetworkBehaviour.RegisterCommandDelegate(typeof (Scp049PlayerScript), Scp049PlayerScript.kCmdCmdRecallPlayer, new NetworkBehaviour.CmdDelegate(Scp049PlayerScript.InvokeCmdCmdRecallPlayer));
    Scp049PlayerScript.kRpcRpcInfectPlayer = -1920658579;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Scp049PlayerScript), Scp049PlayerScript.kRpcRpcInfectPlayer, new NetworkBehaviour.CmdDelegate(Scp049PlayerScript.InvokeRpcRpcInfectPlayer));
    NetworkCRC.RegisterBehaviour(nameof (Scp049PlayerScript), 0);
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
