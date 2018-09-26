// Decompiled with JetBrains decompiler
// Type: MicroHID_GFX
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

public class MicroHID_GFX : NetworkBehaviour
{
  private static int kCmdCmdHurtPlayersInRange = 1650017390;
  public Light[] progress;
  public ParticleSystem teslaFX;
  public Animator anim;
  public AudioSource shotSource;
  public bool onFire;
  public float range;
  public GameObject cam;
  private PlayerManager pmng;
  private HlapiPlayer plyid;
  private InventoryDisplay invdis;
  private float damageGiven;
  private static int kCmdCmdUse;
  private static int kRpcRpcSyncAnim;

  public MicroHID_GFX()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.pmng = PlayerManager.singleton;
    this.invdis = (InventoryDisplay) Object.FindObjectOfType<InventoryDisplay>();
    this.plyid = (HlapiPlayer) ((Component) this).GetComponent<HlapiPlayer>();
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer() || !Input.GetKeyDown(NewInput.GetKey("Shoot")) || (((Inventory) ((Component) this).GetComponent<Inventory>()).curItem != 16 || this.onFire) || (double) Inventory.inventoryCooldown > 0.0 || (double) ((SyncList<Inventory.SyncItemInfo>) ((Inventory) ((Component) this).GetComponent<Inventory>()).items).get_Item(((Inventory) ((Component) this).GetComponent<Inventory>()).GetItemIndex()).durability <= 0.0)
      return;
    this.onFire = true;
    this.CallCmdUse();
    Timing.RunCoroutine(this._PlayAnimation(), (Segment) 0);
  }

  [DebuggerHidden]
  private IEnumerator<float> _PlayAnimation()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new MicroHID_GFX.\u003C_PlayAnimation\u003Ec__Iterator0() { \u0024this = this };
  }

  [Command(channel = 11)]
  private void CmdHurtPlayersInRange(GameObject ply)
  {
    if (((Inventory) ((Component) this).GetComponent<Inventory>()).curItem != 16 || (double) Vector3.Distance(((PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>()).position, ply.get_transform().get_position()) >= (double) this.range || !((WeaponManager) ((Component) this).GetComponent<WeaponManager>()).GetShootPermission((CharacterClassManager) ply.GetComponent<CharacterClassManager>(), false))
      return;
    bool flag = ((CharacterClassManager) ply.GetComponent<CharacterClassManager>()).klasy[((CharacterClassManager) ply.GetComponent<CharacterClassManager>()).curClass].team == Team.SCP;
    if (!((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo((float) Random.Range(200, 300), string.Empty, "TESLA", ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), ply) || !flag)
      return;
    ((PlayerStats) PlayerManager.localPlayer.GetComponent<PlayerStats>()).CallTargetAchieve(this.get_connectionToClient(), "zap");
  }

  [Command(channel = 2)]
  private void CmdUse()
  {
    Inventory component = (Inventory) ((Component) this).GetComponent<Inventory>();
    if (component.curItem != 16)
      return;
    if (component.GetItemIndex() >= 0 && ((SyncList<Inventory.SyncItemInfo>) component.items).get_Item(component.GetItemIndex()).id == 16)
    {
      component.items.ModifyDuration(component.GetItemIndex(), 0.0f);
    }
    else
    {
      for (int index = 0; index < (int) component.items.get_Count(); ++index)
      {
        if (((SyncList<Inventory.SyncItemInfo>) component.items).get_Item(index).id == 16)
        {
          component.items.ModifyDuration(index, 0.0f);
          break;
        }
      }
    }
    this.CallRpcSyncAnim();
  }

  [ClientRpc(channel = 1)]
  private void RpcSyncAnim()
  {
    if (this.get_isLocalPlayer())
      return;
    ((AnimationController) ((Component) this).GetComponent<AnimationController>()).PlaySound("HID_Shoot", true);
    ((AnimationController) ((Component) this).GetComponent<AnimationController>()).DoAnimation("Shoot");
  }

  private void GlowLight(int id)
  {
    double num1;
    switch (id)
    {
      case 4:
        num1 = 6.0;
        break;
      case 5:
        num1 = 50.0;
        break;
      default:
        num1 = 3.0;
        break;
    }
    Light light = this.progress[id];
    double num2 = id != 5 ? 2.0 : 50.0;
    Timing.RunCoroutine(this._SetLightState((float) num1, light, (float) num2), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _SetLightState(float targetIntensity, Light light, float speed)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new MicroHID_GFX.\u003C_SetLightState\u003Ec__Iterator1() { light = light, targetIntensity = targetIntensity, speed = speed };
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdHurtPlayersInRange(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdHurtPlayersInRange called on client.");
    else
      ((MicroHID_GFX) obj).CmdHurtPlayersInRange((GameObject) reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdUse(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUse called on client.");
    else
      ((MicroHID_GFX) obj).CmdUse();
  }

  public void CallCmdHurtPlayersInRange(GameObject ply)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdHurtPlayersInRange called on server.");
    else if (this.get_isServer())
    {
      this.CmdHurtPlayersInRange(ply);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) MicroHID_GFX.kCmdCmdHurtPlayersInRange);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) ply);
      this.SendCommandInternal(networkWriter, 11, "CmdHurtPlayersInRange");
    }
  }

  public void CallCmdUse()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUse called on server.");
    else if (this.get_isServer())
    {
      this.CmdUse();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) MicroHID_GFX.kCmdCmdUse);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdUse");
    }
  }

  protected static void InvokeRpcRpcSyncAnim(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcSyncAnim called on server.");
    else
      ((MicroHID_GFX) obj).RpcSyncAnim();
  }

  public void CallRpcSyncAnim()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcSyncAnim called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) MicroHID_GFX.kRpcRpcSyncAnim);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 1, "RpcSyncAnim");
    }
  }

  static MicroHID_GFX()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (MicroHID_GFX), MicroHID_GFX.kCmdCmdHurtPlayersInRange, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdHurtPlayersInRange)));
    MicroHID_GFX.kCmdCmdUse = -1833499346;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (MicroHID_GFX), MicroHID_GFX.kCmdCmdUse, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUse)));
    MicroHID_GFX.kRpcRpcSyncAnim = -572266021;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (MicroHID_GFX), MicroHID_GFX.kRpcRpcSyncAnim, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcSyncAnim)));
    NetworkCRC.RegisterBehaviour(nameof (MicroHID_GFX), 0);
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
