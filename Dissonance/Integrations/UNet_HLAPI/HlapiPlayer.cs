// Decompiled with JetBrains decompiler
// Type: Dissonance.Integrations.UNet_HLAPI.HlapiPlayer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
  [RequireComponent(typeof (NetworkIdentity))]
  public class HlapiPlayer : NetworkBehaviour, IDissonancePlayer
  {
    private static readonly Log Log = Logs.Create(LogCategory.Network, "HLAPI Player Component");
    private static int kCmdCmdSetPlayerName = -1254064873;
    private DissonanceComms _comms;
    [SyncVar]
    private string _playerId;
    private static int kRpcRpcSetPlayerName;

    public bool IsTracking { get; private set; }

    public string PlayerId
    {
      get
      {
        return this._playerId;
      }
    }

    public Vector3 Position
    {
      get
      {
        return this.transform.position;
      }
    }

    public Quaternion Rotation
    {
      get
      {
        return this.transform.rotation;
      }
    }

    public NetworkPlayerType Type
    {
      get
      {
        return this.isLocalPlayer ? NetworkPlayerType.Local : NetworkPlayerType.Remote;
      }
    }

    public void OnDestroy()
    {
      if (!((Object) this._comms != (Object) null))
        return;
      this._comms.LocalPlayerNameChanged -= new Action<string>(this.SetPlayerName);
    }

    public void OnEnable()
    {
      this._comms = Object.FindObjectOfType<DissonanceComms>();
    }

    public void OnDisable()
    {
      if (!this.IsTracking)
        return;
      this.StopTracking();
    }

    public override void OnStartAuthority()
    {
      base.OnStartAuthority();
      DissonanceComms objectOfType = Object.FindObjectOfType<DissonanceComms>();
      if ((Object) objectOfType == (Object) null)
        throw HlapiPlayer.Log.CreateUserErrorException("cannot find DissonanceComms component in scene", "not placing a DissonanceComms component on a game object in the scene", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "9A79FDCB-263E-4124-B54D-67EDA39C09A5");
      if (objectOfType.LocalPlayerName != null)
        this.SetPlayerName(objectOfType.LocalPlayerName);
      objectOfType.LocalPlayerNameChanged += new Action<string>(this.SetPlayerName);
    }

    private void SetPlayerName(string playerName)
    {
      if (this.IsTracking)
        this.StopTracking();
      this.Network_playerId = playerName;
      this.StartTracking();
      if (!this.hasAuthority)
        return;
      this.CallCmdSetPlayerName(playerName);
    }

    public override void OnStartClient()
    {
      base.OnStartClient();
      if (string.IsNullOrEmpty(this.PlayerId))
        return;
      this.StartTracking();
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
      this.Network_playerId = playerName;
      this.CallRpcSetPlayerName(playerName);
    }

    [ClientRpc]
    private void RpcSetPlayerName(string playerName)
    {
      if (this.hasAuthority)
        return;
      this.SetPlayerName(playerName);
    }

    private void StartTracking()
    {
      if (this.IsTracking)
        throw HlapiPlayer.Log.CreatePossibleBugException("Attempting to start player tracking, but tracking is already started", "B7D1F25E-72AF-4E93-8CFF-90CEBEAC68CF");
      if (!((Object) this._comms != (Object) null))
        return;
      this._comms.TrackPlayerPosition((IDissonancePlayer) this);
      this.IsTracking = true;
    }

    private void StopTracking()
    {
      if (!this.IsTracking)
        throw HlapiPlayer.Log.CreatePossibleBugException("Attempting to stop player tracking, but tracking is not started", "EC5C395D-B544-49DC-B33C-7D7533349134");
      if (!((Object) this._comms != (Object) null))
        return;
      this._comms.StopTracking((IDissonancePlayer) this);
      this.IsTracking = false;
    }

    static HlapiPlayer()
    {
      NetworkBehaviour.RegisterCommandDelegate(typeof (HlapiPlayer), HlapiPlayer.kCmdCmdSetPlayerName, new NetworkBehaviour.CmdDelegate(HlapiPlayer.InvokeCmdCmdSetPlayerName));
      HlapiPlayer.kRpcRpcSetPlayerName = 1332331777;
      NetworkBehaviour.RegisterRpcDelegate(typeof (HlapiPlayer), HlapiPlayer.kRpcRpcSetPlayerName, new NetworkBehaviour.CmdDelegate(HlapiPlayer.InvokeRpcRpcSetPlayerName));
      NetworkCRC.RegisterBehaviour(nameof (HlapiPlayer), 0);
    }

    private void UNetVersion()
    {
    }

    public string Network_playerId
    {
      get
      {
        return this._playerId;
      }
      [param: In] set
      {
        this.SetSyncVar<string>(value, ref this._playerId, 1U);
      }
    }

    protected static void InvokeCmdCmdSetPlayerName(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "Command CmdSetPlayerName called on client.");
      else
        ((HlapiPlayer) obj).CmdSetPlayerName(reader.ReadString());
    }

    public void CallCmdSetPlayerName(string playerName)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "Command function CmdSetPlayerName called on server.");
      else if (this.isServer)
      {
        this.CmdSetPlayerName(playerName);
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 5);
        writer.WritePackedUInt32((uint) HlapiPlayer.kCmdCmdSetPlayerName);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.Write(playerName);
        this.SendCommandInternal(writer, 0, "CmdSetPlayerName");
      }
    }

    protected static void InvokeRpcRpcSetPlayerName(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "RPC RpcSetPlayerName called on server.");
      else
        ((HlapiPlayer) obj).RpcSetPlayerName(reader.ReadString());
    }

    public void CallRpcSetPlayerName(string playerName)
    {
      if (!NetworkServer.active)
      {
        Debug.LogError((object) "RPC Function RpcSetPlayerName called on client.");
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 2);
        writer.WritePackedUInt32((uint) HlapiPlayer.kRpcRpcSetPlayerName);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.Write(playerName);
        this.SendRPCInternal(writer, 0, "RpcSetPlayerName");
      }
    }

    public override bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
      if (forceAll)
      {
        writer.Write(this._playerId);
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
        writer.Write(this._playerId);
      }
      if (!flag)
        writer.WritePackedUInt32(this.syncVarDirtyBits);
      return flag;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
      if (initialState)
      {
        this._playerId = reader.ReadString();
      }
      else
      {
        if (((int) reader.ReadPackedUInt32() & 1) == 0)
          return;
        this._playerId = reader.ReadString();
      }
    }
  }
}
