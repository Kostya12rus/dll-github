// Decompiled with JetBrains decompiler
// Type: GrenadeManager
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

public class GrenadeManager : NetworkBehaviour
{
  private static int kCmdCmdThrowGrenade = 724004359;
  public GrenadeSettings[] availableGrenades;
  public static List<Grenade> grenadesOnScene;
  private Inventory inv;
  public static bool flashfire;
  private bool isThrowing;
  private int throwInteger;
  [SyncVar]
  private bool _syncFlashfire;
  private static int kRpcRpcThrowGrenade;
  private static int kRpcRpcExplode;
  private static int kRpcRpcUpdate;

  public GrenadeManager()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (NetworkServer.get_active())
      this.Network_syncFlashfire = ConfigFile.ServerConfig.GetBool("friendly_flash", false);
    this.inv = (Inventory) ((Component) this).GetComponent<Inventory>();
    if (!this.get_isLocalPlayer())
      return;
    GrenadeManager.grenadesOnScene = new List<Grenade>();
  }

  private void Update()
  {
    if (this.get_isLocalPlayer())
      this.CheckForInput();
    if (!(((Object) this).get_name() == "Host"))
      return;
    GrenadeManager.flashfire = this._syncFlashfire;
  }

  private void CheckForInput()
  {
    bool keyDown1 = Input.GetKeyDown(NewInput.GetKey("Shoot"));
    bool keyDown2 = Input.GetKeyDown(NewInput.GetKey("Zoom"));
    if (this.isThrowing || !keyDown1 && !keyDown2 || ((double) Inventory.inventoryCooldown > 0.0 || Cursor.get_visible()))
      return;
    for (int gId = 0; gId < this.availableGrenades.Length; ++gId)
    {
      if (this.availableGrenades[gId].inventoryID == this.inv.curItem)
      {
        this.isThrowing = true;
        Timing.RunCoroutine(this._ThrowGrenade(gId, keyDown2), (Segment) 1);
        break;
      }
    }
  }

  [Server]
  public void ChangeIntoGrenade(Pickup pickup, int id, int ti_pid, int ti_int, Vector3 dir, Vector3 pos)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogWarning((object) "[Server] function 'System.Void GrenadeManager::ChangeIntoGrenade(Pickup,System.Int32,System.Int32,System.Int32,UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
    }
    else
    {
      pickup.Delete();
      this.CallRpcThrowGrenade(id, ti_pid, ti_int, dir, true, pos, false);
    }
  }

  [DebuggerHidden]
  private IEnumerator<float> _ThrowGrenade(int gId, bool slow)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new GrenadeManager.\u003C_ThrowGrenade\u003Ec__Iterator0()
    {
      gId = gId,
      slow = slow,
      \u0024this = this
    };
  }

  [Command]
  private void CmdThrowGrenade(int id, int ti, Vector3 direction, bool slowThrow)
  {
    for (int index = 0; index < (int) this.inv.items.get_Count(); ++index)
    {
      if (((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(index).id == this.availableGrenades[id].inventoryID)
      {
        this.CallRpcThrowGrenade(id, ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId, ti, ((Vector3) ref direction).get_normalized(), false, Vector3.get_zero(), slowThrow);
        ((SyncList<Inventory.SyncItemInfo>) this.inv.items).RemoveAt(index);
        break;
      }
    }
  }

  [ClientRpc]
  private void RpcThrowGrenade(int id, int ti_pid, int ti_int, Vector3 dir, bool isEnvironmentallyTriggered, Vector3 optionalParam, bool slowThrow)
  {
    Timing.RunCoroutine(this._RpcThrowGrenade(id, ti_pid, ti_int, dir, isEnvironmentallyTriggered, optionalParam, slowThrow), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _RpcThrowGrenade(int id, int ti_pid, int ti_int, Vector3 dir, bool isEnvironmentallyTriggered, Vector3 optionalParamenter, bool slowThrow)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new GrenadeManager.\u003C_RpcThrowGrenade\u003Ec__Iterator1()
    {
      isEnvironmentallyTriggered = isEnvironmentallyTriggered,
      id = id,
      ti_pid = ti_pid,
      ti_int = ti_int,
      slowThrow = slowThrow,
      optionalParamenter = optionalParamenter,
      dir = dir,
      \u0024this = this
    };
  }

  [ClientRpc]
  private void RpcExplode(string id, int playerID)
  {
    foreach (Grenade grenade in GrenadeManager.grenadesOnScene)
    {
      if (grenade.id == id)
      {
        grenade.Explode(playerID);
        break;
      }
    }
  }

  [ClientRpc]
  private void RpcUpdate(string id, Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
  {
    if (NetworkServer.get_active())
      return;
    foreach (Grenade grenade in GrenadeManager.grenadesOnScene)
    {
      if (grenade.id == id)
        grenade.SyncMovement(pos, vel, rot, angVel);
    }
  }

  private void UNetVersion()
  {
  }

  public bool Network_syncFlashfire
  {
    get
    {
      return this._syncFlashfire;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this._syncFlashfire, 1U);
    }
  }

  protected static void InvokeCmdCmdThrowGrenade(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdThrowGrenade called on client.");
    else
      ((GrenadeManager) obj).CmdThrowGrenade((int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (Vector3) reader.ReadVector3(), reader.ReadBoolean());
  }

  public void CallCmdThrowGrenade(int id, int ti, Vector3 direction, bool slowThrow)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdThrowGrenade called on server.");
    else if (this.get_isServer())
    {
      this.CmdThrowGrenade(id, ti, direction, slowThrow);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) GrenadeManager.kCmdCmdThrowGrenade);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) id);
      networkWriter.WritePackedUInt32((uint) ti);
      networkWriter.Write((Vector3) direction);
      networkWriter.Write(slowThrow);
      this.SendCommandInternal(networkWriter, 0, "CmdThrowGrenade");
    }
  }

  protected static void InvokeRpcRpcThrowGrenade(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcThrowGrenade called on server.");
    else
      ((GrenadeManager) obj).RpcThrowGrenade((int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (Vector3) reader.ReadVector3(), reader.ReadBoolean(), (Vector3) reader.ReadVector3(), reader.ReadBoolean());
  }

  protected static void InvokeRpcRpcExplode(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcExplode called on server.");
    else
      ((GrenadeManager) obj).RpcExplode(reader.ReadString(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcRpcUpdate(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcUpdate called on server.");
    else
      ((GrenadeManager) obj).RpcUpdate(reader.ReadString(), (Vector3) reader.ReadVector3(), (Quaternion) reader.ReadQuaternion(), (Vector3) reader.ReadVector3(), (Vector3) reader.ReadVector3());
  }

  public void CallRpcThrowGrenade(int id, int ti_pid, int ti_int, Vector3 dir, bool isEnvironmentallyTriggered, Vector3 optionalParam, bool slowThrow)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcThrowGrenade called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) GrenadeManager.kRpcRpcThrowGrenade);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) id);
      networkWriter.WritePackedUInt32((uint) ti_pid);
      networkWriter.WritePackedUInt32((uint) ti_int);
      networkWriter.Write((Vector3) dir);
      networkWriter.Write(isEnvironmentallyTriggered);
      networkWriter.Write((Vector3) optionalParam);
      networkWriter.Write(slowThrow);
      this.SendRPCInternal(networkWriter, 0, "RpcThrowGrenade");
    }
  }

  public void CallRpcExplode(string id, int playerID)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcExplode called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) GrenadeManager.kRpcRpcExplode);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(id);
      networkWriter.WritePackedUInt32((uint) playerID);
      this.SendRPCInternal(networkWriter, 0, "RpcExplode");
    }
  }

  public void CallRpcUpdate(string id, Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcUpdate called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) GrenadeManager.kRpcRpcUpdate);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(id);
      networkWriter.Write((Vector3) pos);
      networkWriter.Write((Quaternion) rot);
      networkWriter.Write((Vector3) vel);
      networkWriter.Write((Vector3) angVel);
      this.SendRPCInternal(networkWriter, 0, "RpcUpdate");
    }
  }

  static GrenadeManager()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (GrenadeManager), GrenadeManager.kCmdCmdThrowGrenade, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdThrowGrenade)));
    GrenadeManager.kRpcRpcThrowGrenade = 807436509;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (GrenadeManager), GrenadeManager.kRpcRpcThrowGrenade, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcThrowGrenade)));
    GrenadeManager.kRpcRpcExplode = 391825004;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (GrenadeManager), GrenadeManager.kRpcRpcExplode, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcExplode)));
    GrenadeManager.kRpcRpcUpdate = 462949854;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (GrenadeManager), GrenadeManager.kRpcRpcUpdate, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcUpdate)));
    NetworkCRC.RegisterBehaviour(nameof (GrenadeManager), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this._syncFlashfire);
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
      writer.Write(this._syncFlashfire);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this._syncFlashfire = reader.ReadBoolean();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this._syncFlashfire = reader.ReadBoolean();
    }
  }
}
