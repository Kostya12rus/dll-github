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
    private readonly NetworkWriter _sendWriter = new NetworkWriter(new byte[1024]);
    private readonly byte[] _receiveBuffer = new byte[1024];
    private readonly List<NetworkConnection> _addedConnections = new List<NetworkConnection>();
    [NotNull]
    private readonly HlapiCommsNetwork _network;
    public static HlapiServer _instance;

    public HlapiServer([NotNull] HlapiCommsNetwork network)
    {
      if ((Object) network == (Object) null)
        throw new ArgumentNullException(nameof (network));
      this._network = network;
      HlapiServer._instance = this;
    }

    public override void Connect()
    {
      NetworkServer.RegisterHandler(this._network.TypeCode, new NetworkMessageDelegate(this.OnMessageReceivedHandler));
      base.Connect();
    }

    private void OnMessageReceivedHandler([NotNull] NetworkMessage netmsg)
    {
      this.NetworkReceivedPacket(new HlapiConn(netmsg.conn), this._network.CopyToArraySegment(netmsg.reader, new ArraySegment<byte>(this._receiveBuffer)));
    }

    protected override void AddClient([NotNull] ClientInfo<HlapiConn> client)
    {
      base.AddClient(client);
      if (!(client.PlayerName != this._network.PlayerName))
        return;
      this._addedConnections.Add(client.Connection.Connection);
    }

    public override void Disconnect()
    {
      base.Disconnect();
      int typeCode = (int) this._network.TypeCode;
      // ISSUE: reference to a compiler-generated field
      if (HlapiServer.\u003C\u003Ef__mg\u0024cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        HlapiServer.\u003C\u003Ef__mg\u0024cache0 = new NetworkMessageDelegate(HlapiCommsNetwork.NullMessageReceivedHandler);
      }
      // ISSUE: reference to a compiler-generated field
      NetworkMessageDelegate fMgCache0 = HlapiServer.\u003C\u003Ef__mg\u0024cache0;
      NetworkServer.RegisterHandler((short) typeCode, fMgCache0);
    }

    protected override void ReadMessages()
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

    public override ServerState Update()
    {
      for (int index = this._addedConnections.Count - 1; index >= 0; --index)
      {
        NetworkConnection addedConnection = this._addedConnections[index];
        if (!addedConnection.isConnected || addedConnection.lastError == NetworkError.Timeout || !NetworkServer.connections.Contains(this._addedConnections[index]))
        {
          this.ClientDisconnected(new HlapiConn(this._addedConnections[index]));
          this._addedConnections.RemoveAt(index);
        }
      }
      return base.Update();
    }

    protected override void SendReliable(HlapiConn connection, ArraySegment<byte> packet)
    {
      if (this.Send(packet, connection, this._network.ReliableSequencedChannel))
        return;
      this.FatalError("Failed to send reliable packet (unknown HLAPI error)");
    }

    protected override void SendUnreliable(HlapiConn connection, ArraySegment<byte> packet)
    {
      this.Send(packet, connection, this._network.UnreliableChannel);
    }

    private bool Send(ArraySegment<byte> packet, HlapiConn connection, byte channel)
    {
      if (this._network.PreprocessPacketToClient(packet, connection) || !connection.Connection.isConnected || connection.Connection.lastError == NetworkError.Timeout)
        return true;
      int networkWriter = this._network.CopyPacketToNetworkWriter(packet, this._sendWriter);
      if (connection.Connection == null)
      {
        this.Log.Error("Cannot send to a null destination");
        return false;
      }
      return connection.Connection.SendBytes(this._sendWriter.AsArray(), networkWriter, (int) channel);
    }
  }
}
