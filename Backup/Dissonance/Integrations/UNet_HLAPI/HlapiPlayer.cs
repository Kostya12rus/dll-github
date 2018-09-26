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
    private static readonly Log Log = Logs.Create((LogCategory) 2, "HLAPI Player Component");
    private static int kCmdCmdSetPlayerName = -1254064873;
    private DissonanceComms _comms;
    [SyncVar]
    private string _playerId;
    private static int kRpcRpcSetPlayerName;

    public HlapiPlayer()
    {
      base.\u002Ector();
    }

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
        return ((Component) this).get_transform().get_position();
      }
    }

    public Quaternion Rotation
    {
      get
      {
        return ((Component) this).get_transform().get_rotation();
      }
    }

    public NetworkPlayerType Type
    {
      get
      {
        if (this.get_isLocalPlayer())
          return (NetworkPlayerType) 1;
        return (NetworkPlayerType) 2;
      }
    }

    public void OnDestroy()
    {
      if (!Object.op_Inequality((Object) this._comms, (Object) null))
        return;
      this._comms.remove_LocalPlayerNameChanged(new Action<string>(this.SetPlayerName));
    }

    public void OnEnable()
    {
      this._comms = (DissonanceComms) Object.FindObjectOfType<DissonanceComms>();
    }

    public void OnDisable()
    {
      if (!this.IsTracking)
        return;
      this.StopTracking();
    }

    public virtual void OnStartAuthority()
    {
      base.OnStartAuthority();
      DissonanceComms objectOfType = (DissonanceComms) Object.FindObjectOfType<DissonanceComms>();
      if (Object.op_Equality((Object) objectOfType, (Object) null))
        throw HlapiPlayer.Log.CreateUserErrorException("cannot find DissonanceComms component in scene", "not placing a DissonanceComms component on a game object in the scene", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "9A79FDCB-263E-4124-B54D-67EDA39C09A5");
      if (objectOfType.get_LocalPlayerName() != null)
        this.SetPlayerName(objectOfType.get_LocalPlayerName());
      objectOfType.add_LocalPlayerNameChanged(new Action<string>(this.SetPlayerName));
    }

    private void SetPlayerName(string playerName)
    {
      if (this.IsTracking)
        this.StopTracking();
      this.Network_playerId = playerName;
      this.StartTracking();
      if (!this.get_hasAuthority())
        return;
      this.CallCmdSetPlayerName(playerName);
    }

    public virtual void OnStartClient()
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
      if (this.get_hasAuthority())
        return;
      this.SetPlayerName(playerName);
    }

    private void StartTracking()
    {
      if (this.IsTracking)
        throw HlapiPlayer.Log.CreatePossibleBugException("Attempting to start player tracking, but tracking is already started", "B7D1F25E-72AF-4E93-8CFF-90CEBEAC68CF");
      if (!Object.op_Inequality((Object) this._comms, (Object) null))
        return;
      this._comms.TrackPlayerPosition((IDissonancePlayer) this);
      this.IsTracking = true;
    }

    private void StopTracking()
    {
      if (!this.IsTracking)
        throw HlapiPlayer.Log.CreatePossibleBugException("Attempting to stop player tracking, but tracking is not started", "EC5C395D-B544-49DC-B33C-7D7533349134");
      if (!Object.op_Inequality((Object) this._comms, (Object) null))
        return;
      this._comms.StopTracking((IDissonancePlayer) this);
      this.IsTracking = false;
    }

    static HlapiPlayer()
    {
      // ISSUE: method pointer
      NetworkBehaviour.RegisterCommandDelegate(typeof (HlapiPlayer), HlapiPlayer.kCmdCmdSetPlayerName, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSetPlayerName)));
      HlapiPlayer.kRpcRpcSetPlayerName = 1332331777;
      // ISSUE: method pointer
      NetworkBehaviour.RegisterRpcDelegate(typeof (HlapiPlayer), HlapiPlayer.kRpcRpcSetPlayerName, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcSetPlayerName)));
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
        this.SetSyncVar<string>((M0) value, (M0&) ref this._playerId, 1U);
      }
    }

    protected static void InvokeCmdCmdSetPlayerName(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkServer.get_active())
        Debug.LogError((object) "Command CmdSetPlayerName called on client.");
      else
        ((HlapiPlayer) obj).CmdSetPlayerName(reader.ReadString());
    }

    public void CallCmdSetPlayerName(string playerName)
    {
      if (!NetworkClient.get_active())
        Debug.LogError((object) "Command function CmdSetPlayerName called on server.");
      else if (this.get_isServer())
      {
        this.CmdSetPlayerName(playerName);
      }
      else
      {
        NetworkWriter networkWriter = new NetworkWriter();
        networkWriter.Write((short) 0);
        networkWriter.Write((short) 5);
        networkWriter.WritePackedUInt32((uint) HlapiPlayer.kCmdCmdSetPlayerName);
        networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
        networkWriter.Write(playerName);
        this.SendCommandInternal(networkWriter, 0, "CmdSetPlayerName");
      }
    }

    protected static void InvokeRpcRpcSetPlayerName(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkClient.get_active())
        Debug.LogError((object) "RPC RpcSetPlayerName called on server.");
      else
        ((HlapiPlayer) obj).RpcSetPlayerName(reader.ReadString());
    }

    public void CallRpcSetPlayerName(string playerName)
    {
      if (!NetworkServer.get_active())
      {
        Debug.LogError((object) "RPC Function RpcSetPlayerName called on client.");
      }
      else
      {
        NetworkWriter networkWriter = new NetworkWriter();
        networkWriter.Write((short) 0);
        networkWriter.Write((short) 2);
        networkWriter.WritePackedUInt32((uint) HlapiPlayer.kRpcRpcSetPlayerName);
        networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
        networkWriter.Write(playerName);
        this.SendRPCInternal(networkWriter, 0, "RpcSetPlayerName");
      }
    }

    public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
      if (forceAll)
      {
        writer.Write(this._playerId);
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
        writer.Write(this._playerId);
      }
      if (!flag)
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
      return flag;
    }

    public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
