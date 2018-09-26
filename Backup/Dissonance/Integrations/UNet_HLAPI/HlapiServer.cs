// Decompiled with JetBrains decompiler
// Type: Dissonance.Integrations.UNet_HLAPI.HlapiServer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance.Networking;
using Dissonance.Networking.Server;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
  public class HlapiServer : BaseServer<HlapiServer, HlapiClient, HlapiConn>
  {
    [NotNull]
    private readonly HlapiCommsNetwork _network;
    private readonly NetworkWriter _sendWriter;
    private readonly byte[] _receiveBuffer;
    private readonly List<NetworkConnection> _addedConnections;
    public static HlapiServer _instance;

    public HlapiServer([NotNull] HlapiCommsNetwork network)
    {
      this.\u002Ector();
      if (Object.op_Equality((Object) network, (Object) null))
        throw new ArgumentNullException(nameof (network));
      this._network = network;
      HlapiServer._instance = this;
    }

    public virtual void Connect()
    {
      // ISSUE: method pointer
      NetworkServer.RegisterHandler(this._network.TypeCode, new NetworkMessageDelegate((object) this, __methodptr(OnMessageReceivedHandler)));
      base.Connect();
    }

    private void OnMessageReceivedHandler([NotNull] NetworkMessage netmsg)
    {
      this.NetworkReceivedPacket(new HlapiConn((NetworkConnection) netmsg.conn), this._network.CopyToArraySegment((NetworkReader) netmsg.reader, new ArraySegment<byte>(this._receiveBuffer)));
    }

    protected virtual void AddClient([NotNull] ClientInfo<HlapiConn> client)
    {
      base.AddClient(client);
      if (!(client.get_PlayerName() != this._network.get_PlayerName()))
        return;
      this._addedConnections.Add(client.get_Connection().Connection);
    }

    public virtual void Disconnect()
    {
      base.Disconnect();
      int typeCode = (int) this._network.TypeCode;
      // ISSUE: reference to a compiler-generated field
      if (HlapiServer.\u003C\u003Ef__mg\u0024cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method pointer
        HlapiServer.\u003C\u003Ef__mg\u0024cache0 = new NetworkMessageDelegate((object) null, __methodptr(NullMessageReceivedHandler));
      }
      // ISSUE: reference to a compiler-generated field
      NetworkMessageDelegate fMgCache0 = HlapiServer.\u003C\u003Ef__mg\u0024cache0;
      NetworkServer.RegisterHandler((short) typeCode, fMgCache0);
    }

    protected virtual void ReadMessages()
    {
    }

    public static void OnServerDisconnect(NetworkConnection connection)
    {
      if (HlapiServer._instance == null)
        return;
      HlapiServer._instance.OnServerDisconnect(new HlapiConn(connection));
    }

    private void OnServerDisconnect(HlapiConn conn)
    {
      int index = this._addedConnections.IndexOf(conn.Connection);
      if (index < 0)
        return;
      this._addedConnections.RemoveAt(index);
      this.ClientDisconnected(conn);
    }

    public virtual ServerState Update()
    {
      for (int index = this._addedConnections.Count - 1; index >= 0; --index)
      {
        NetworkConnection addedConnection = this._addedConnections[index];
        if (!addedConnection.get_isConnected() || addedConnection.get_lastError() == 6 || !NetworkServer.get_connections().Contains(this._addedConnections[index]))
        {
          this.ClientDisconnected(new HlapiConn(this._addedConnections[index]));
          this._addedConnections.RemoveAt(index);
        }
      }
      return base.Update();
    }

    protected virtual void SendReliable(HlapiConn connection, ArraySegment<byte> packet)
    {
      if (this.Send(packet, connection, this._network.ReliableSequencedChannel))
        return;
      this.FatalError("Failed to send reliable packet (unknown HLAPI error)");
    }

    protected virtual void SendUnreliable(HlapiConn connection, ArraySegment<byte> packet)
    {
      this.Send(packet, connection, this._network.UnreliableChannel);
    }

    private bool Send(ArraySegment<byte> packet, HlapiConn connection, byte channel)
    {
      if (this._network.PreprocessPacketToClient(packet, connection) || !connection.Connection.get_isConnected() || connection.Connection.get_lastError() == 6)
        return true;
      int networkWriter = this._network.CopyPacketToNetworkWriter(packet, this._sendWriter);
      if (connection.Connection == null)
      {
        ((Log) this.Log).Error("Cannot send to a null destination");
        return false;
      }
      return connection.Connection.SendBytes(this._sendWriter.AsArray(), networkWriter, (int) channel);
    }
  }
}
