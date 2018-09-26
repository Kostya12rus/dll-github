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

  private void Start()
  {
    if (NetworkServer.active)
      this.Network_syncFlashfire = ConfigFile.ServerConfig.GetBool("friendly_flash", false);
    this.inv = this.GetComponent<Inventory>();
    if (!this.isLocalPlayer)
      return;
    GrenadeManager.grenadesOnScene = new List<Grenade>();
  }

  private void Update()
  {
    if (this.isLocalPlayer)
      this.CheckForInput();
    if (!(this.name == "Host"))
      return;
    GrenadeManager.flashfire = this._syncFlashfire;
  }

  private void CheckForInput()
  {
    bool keyDown1 = Input.GetKeyDown(NewInput.GetKey("Shoot"));
    bool keyDown2 = Input.GetKeyDown(NewInput.GetKey("Zoom"));
    if (this.isThrowing || !keyDown1 && !keyDown2 || ((double) Inventory.inventoryCooldown > 0.0 || Cursor.visible))
      return;
    for (int gId = 0; gId < this.availableGrenades.Length; ++gId)
    {
      if (this.availableGrenades[gId].inventoryID == this.inv.curItem)
      {
        this.isThrowing = true;
        Timing.RunCoroutine(this._ThrowGrenade(gId, keyDown2), Segment.FixedUpdate);
        break;
      }
    }
  }

  [Server]
  public void ChangeIntoGrenade(Pickup pickup, int id, int ti_pid, int ti_int, Vector3 dir, Vector3 pos)
  {
    if (!NetworkServer.active)
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
    return (IEnumerator<float>) new GrenadeManager.\u003C_ThrowGrenade\u003Ec__Iterator0() { gId = gId, slow = slow, \u0024this = this };
  }

  [Command]
  private void CmdThrowGrenade(int id, int ti, Vector3 direction, bool slowThrow)
  {
    for (int index = 0; index < (int) this.inv.items.Count; ++index)
    {
      if (this.inv.items[index].id == this.availableGrenades[id].inventoryID)
      {
        this.CallRpcThrowGrenade(id, this.GetComponent<QueryProcessor>().PlayerId, ti, direction.normalized, false, Vector3.zero, slowThrow);
        this.inv.items.RemoveAt(index);
        break;
      }
    }
  }

  [ClientRpc]
  private void RpcThrowGrenade(int id, int ti_pid, int ti_int, Vector3 dir, bool isEnvironmentallyTriggered, Vector3 optionalParam, bool slowThrow)
  {
    Timing.RunCoroutine(this._RpcThrowGrenade(id, ti_pid, ti_int, dir, isEnvironmentallyTriggered, optionalParam, slowThrow), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _RpcThrowGrenade(int id, int ti_pid, int ti_int, Vector3 dir, bool isEnvironmentallyTriggered, Vector3 optionalParamenter, bool slowThrow)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new GrenadeManager.\u003C_RpcThrowGrenade\u003Ec__Iterator1() { isEnvironmentallyTriggered = isEnvironmentallyTriggered, id = id, ti_pid = ti_pid, ti_int = ti_int, slowThrow = slowThrow, optionalParamenter = optionalParamenter, dir = dir, \u0024this = this };
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
    if (NetworkServer.active)
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
      this.SetSyncVar<bool>(value, ref this._syncFlashfire, 1U);
    }
  }

  protected static void InvokeCmdCmdThrowGrenade(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdThrowGrenade called on client.");
    else
      ((GrenadeManager) obj).CmdThrowGrenade((int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), reader.ReadVector3(), reader.ReadBoolean());
  }

  public void CallCmdThrowGrenade(int id, int ti, Vector3 direction, bool slowThrow)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdThrowGrenade called on server.");
    else if (this.isServer)
    {
      this.CmdThrowGrenade(id, ti, direction, slowThrow);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) GrenadeManager.kCmdCmdThrowGrenade);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) id);
      writer.WritePackedUInt32((uint) ti);
      writer.Write(direction);
      writer.Write(slowThrow);
      this.SendCommandInternal(writer, 0, "CmdThrowGrenade");
    }
  }

  protected static void InvokeRpcRpcThrowGrenade(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcThrowGrenade called on server.");
    else
      ((GrenadeManager) obj).RpcThrowGrenade((int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), reader.ReadVector3(), reader.ReadBoolean(), reader.ReadVector3(), reader.ReadBoolean());
  }

  protected static void InvokeRpcRpcExplode(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcExplode called on server.");
    else
      ((GrenadeManager) obj).RpcExplode(reader.ReadString(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcRpcUpdate(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcUpdate called on server.");
    else
      ((GrenadeManager) obj).RpcUpdate(reader.ReadString(), reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadVector3(), reader.ReadVector3());
  }

  public void CallRpcThrowGrenade(int id, int ti_pid, int ti_int, Vector3 dir, bool isEnvironmentallyTriggered, Vector3 optionalParam, bool slowThrow)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcThrowGrenade called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) GrenadeManager.kRpcRpcThrowGrenade);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) id);
      writer.WritePackedUInt32((uint) ti_pid);
      writer.WritePackedUInt32((uint) ti_int);
      writer.Write(dir);
      writer.Write(isEnvironmentallyTriggered);
      writer.Write(optionalParam);
      writer.Write(slowThrow);
      this.SendRPCInternal(writer, 0, "RpcThrowGrenade");
    }
  }

  public void CallRpcExplode(string id, int playerID)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcExplode called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) GrenadeManager.kRpcRpcExplode);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(id);
      writer.WritePackedUInt32((uint) playerID);
      this.SendRPCInternal(writer, 0, "RpcExplode");
    }
  }

  public void CallRpcUpdate(string id, Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcUpdate called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) GrenadeManager.kRpcRpcUpdate);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(id);
      writer.Write(pos);
      writer.Write(rot);
      writer.Write(vel);
      writer.Write(angVel);
      this.SendRPCInternal(writer, 0, "RpcUpdate");
    }
  }

  static GrenadeManager()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (GrenadeManager), GrenadeManager.kCmdCmdThrowGrenade, new NetworkBehaviour.CmdDelegate(GrenadeManager.InvokeCmdCmdThrowGrenade));
    GrenadeManager.kRpcRpcThrowGrenade = 807436509;
    NetworkBehaviour.RegisterRpcDelegate(typeof (GrenadeManager), GrenadeManager.kRpcRpcThrowGrenade, new NetworkBehaviour.CmdDelegate(GrenadeManager.InvokeRpcRpcThrowGrenade));
    GrenadeManager.kRpcRpcExplode = 391825004;
    NetworkBehaviour.RegisterRpcDelegate(typeof (GrenadeManager), GrenadeManager.kRpcRpcExplode, new NetworkBehaviour.CmdDelegate(GrenadeManager.InvokeRpcRpcExplode));
    GrenadeManager.kRpcRpcUpdate = 462949854;
    NetworkBehaviour.RegisterRpcDelegate(typeof (GrenadeManager), GrenadeManager.kRpcRpcUpdate, new NetworkBehaviour.CmdDelegate(GrenadeManager.InvokeRpcRpcUpdate));
    NetworkCRC.RegisterBehaviour(nameof (GrenadeManager), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this._syncFlashfire);
      return true;
    }
    bool flag = false;
    if (((int) this.syncVarDirtyBits & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this._syncFlashfire);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
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
