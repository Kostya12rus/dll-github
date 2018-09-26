// Decompiled with JetBrains decompiler
// Type: Dissonance.Integrations.UNet_HLAPI.HlapiClient
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance.Networking;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
  public class HlapiClient : BaseClient<HlapiServer, HlapiClient, HlapiConn>
  {
    private readonly HlapiCommsNetwork _network;
    private readonly NetworkWriter _sendWriter;
    private readonly byte[] _receiveBuffer;

    public HlapiClient([NotNull] HlapiCommsNetwork network)
    {
      this.\u002Ector((ICommsNetworkState) network);
      if (Object.op_Equality((Object) network, (Object) null))
        throw new ArgumentNullException(nameof (network));
      this._network = network;
      this._sendWriter = new NetworkWriter(new byte[1024]);
    }

    public virtual void Connect()
    {
      if (!NetworkModeExtensions.IsServerEnabled(this._network.get_Mode()))
      {
        // ISSUE: method pointer
        ((NetworkClient) ((NetworkManager) NetworkManager.singleton).client).RegisterHandler(this._network.TypeCode, new NetworkMessageDelegate((object) this, __methodptr(OnMessageReceivedHandler)));
      }
      this.Connected();
    }

    public virtual void Disconnect()
    {
      if (!NetworkModeExtensions.IsServerEnabled(this._network.get_Mode()) && ((NetworkManager) NetworkManager.singleton).client != null)
      {
        // ISSUE: variable of the null type
        __Null client = ((NetworkManager) NetworkManager.singleton).client;
        int typeCode = (int) this._network.TypeCode;
        // ISSUE: reference to a compiler-generated field
        if (HlapiClient.\u003C\u003Ef__mg\u0024cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          HlapiClient.\u003C\u003Ef__mg\u0024cache0 = new NetworkMessageDelegate((object) null, __methodptr(NullMessageReceivedHandler));
        }
        // ISSUE: reference to a compiler-generated field
        NetworkMessageDelegate fMgCache0 = HlapiClient.\u003C\u003Ef__mg\u0024cache0;
        ((NetworkClient) client).RegisterHandler((short) typeCode, fMgCache0);
      }
      base.Disconnect();
    }

    private void OnMessageReceivedHandler([CanBeNull] NetworkMessage netMsg)
    {
      if (netMsg == null)
        return;
      this.NetworkReceivedPacket(this._network.CopyToArraySegment((NetworkReader) netMsg.reader, new ArraySegment<byte>(this._receiveBuffer)));
    }

    protected virtual void ReadMessages()
    {
    }

    protected virtual void SendReliable(ArraySegment<byte> packet)
    {
      if (this.Send(packet, this._network.ReliableSequencedChannel))
        return;
      this.FatalError("Failed to send reliable packet (unknown HLAPI error)");
    }

    protected virtual void SendUnreliable(ArraySegment<byte> packet)
    {
      this.Send(packet, this._network.UnreliableChannel);
    }

    private bool Send(ArraySegment<byte> packet, byte channel)
    {
      if (this._network.PreprocessPacketToServer(packet))
        return true;
      int networkWriter = this._network.CopyPacketToNetworkWriter(packet, this._sendWriter);
      return ((NetworkClient) ((NetworkManager) NetworkManager.singleton).client).get_connection().SendBytes(this._sendWriter.AsArray(), networkWriter, (int) channel);
    }
  }
}
