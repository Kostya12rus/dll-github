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

  private void Start()
  {
    this.pmng = PlayerManager.singleton;
    this.invdis = Object.FindObjectOfType<InventoryDisplay>();
    this.plyid = this.GetComponent<HlapiPlayer>();
  }

  private void Update()
  {
    if (!this.isLocalPlayer || !Input.GetKeyDown(NewInput.GetKey("Shoot")) || (this.GetComponent<Inventory>().curItem != 16 || this.onFire) || (double) Inventory.inventoryCooldown > 0.0 || (double) this.GetComponent<Inventory>().items[this.GetComponent<Inventory>().GetItemIndex()].durability <= 0.0)
      return;
    this.onFire = true;
    this.CallCmdUse();
    Timing.RunCoroutine(this._PlayAnimation(), Segment.Update);
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
    if (this.GetComponent<Inventory>().curItem != 16 || (double) Vector3.Distance(this.GetComponent<PlyMovementSync>().position, ply.transform.position) >= (double) this.range || !this.GetComponent<WeaponManager>().GetShootPermission(ply.GetComponent<CharacterClassManager>(), false))
      return;
    bool flag = ply.GetComponent<CharacterClassManager>().klasy[ply.GetComponent<CharacterClassManager>().curClass].team == Team.SCP;
    if (!this.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo((float) Random.Range(200, 300), string.Empty, "TESLA", this.GetComponent<QueryProcessor>().PlayerId), ply) || !flag)
      return;
    PlayerManager.localPlayer.GetComponent<PlayerStats>().CallTargetAchieve(this.connectionToClient, "zap");
  }

  [Command(channel = 2)]
  private void CmdUse()
  {
    Inventory component = this.GetComponent<Inventory>();
    if (component.curItem != 16)
      return;
    if (component.GetItemIndex() >= 0 && component.items[component.GetItemIndex()].id == 16)
    {
      component.items.ModifyDuration(component.GetItemIndex(), 0.0f);
    }
    else
    {
      for (int index = 0; index < (int) component.items.Count; ++index)
      {
        if (component.items[index].id == 16)
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
    if (this.isLocalPlayer)
      return;
    this.GetComponent<AnimationController>().PlaySound("HID_Shoot", true);
    this.GetComponent<AnimationController>().DoAnimation("Shoot");
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
    Timing.RunCoroutine(this._SetLightState((float) num1, light, (float) num2), Segment.FixedUpdate);
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
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdHurtPlayersInRange called on client.");
    else
      ((MicroHID_GFX) obj).CmdHurtPlayersInRange(reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdUse(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUse called on client.");
    else
      ((MicroHID_GFX) obj).CmdUse();
  }

  public void CallCmdHurtPlayersInRange(GameObject ply)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdHurtPlayersInRange called on server.");
    else if (this.isServer)
    {
      this.CmdHurtPlayersInRange(ply);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) MicroHID_GFX.kCmdCmdHurtPlayersInRange);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(ply);
      this.SendCommandInternal(writer, 11, "CmdHurtPlayersInRange");
    }
  }

  public void CallCmdUse()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUse called on server.");
    else if (this.isServer)
    {
      this.CmdUse();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) MicroHID_GFX.kCmdCmdUse);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 2, "CmdUse");
    }
  }

  protected static void InvokeRpcRpcSyncAnim(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcSyncAnim called on server.");
    else
      ((MicroHID_GFX) obj).RpcSyncAnim();
  }

  public void CallRpcSyncAnim()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcSyncAnim called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) MicroHID_GFX.kRpcRpcSyncAnim);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 1, "RpcSyncAnim");
    }
  }

  static MicroHID_GFX()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (MicroHID_GFX), MicroHID_GFX.kCmdCmdHurtPlayersInRange, new NetworkBehaviour.CmdDelegate(MicroHID_GFX.InvokeCmdCmdHurtPlayersInRange));
    MicroHID_GFX.kCmdCmdUse = -1833499346;
    NetworkBehaviour.RegisterCommandDelegate(typeof (MicroHID_GFX), MicroHID_GFX.kCmdCmdUse, new NetworkBehaviour.CmdDelegate(MicroHID_GFX.InvokeCmdCmdUse));
    MicroHID_GFX.kRpcRpcSyncAnim = -572266021;
    NetworkBehaviour.RegisterRpcDelegate(typeof (MicroHID_GFX), MicroHID_GFX.kRpcRpcSyncAnim, new NetworkBehaviour.CmdDelegate(MicroHID_GFX.InvokeRpcRpcSyncAnim));
    NetworkCRC.RegisterBehaviour(nameof (MicroHID_GFX), 0);
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
