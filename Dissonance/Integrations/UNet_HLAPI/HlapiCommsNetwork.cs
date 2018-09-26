// Decompiled with JetBrains decompiler
// Type: Dissonance.Integrations.UNet_HLAPI.HlapiCommsNetwork
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance.Datastructures;
using Dissonance.Extensions;
using Dissonance.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
  [HelpURL("https://placeholder-software.co.uk/dissonance/docs/Basics/Quick-Start-UNet-HLAPI/")]
  public class HlapiCommsNetwork : BaseCommsNetwork<HlapiServer, HlapiClient, HlapiConn, Unit, Unit>
  {
    public byte UnreliableChannel = 1;
    public short TypeCode = 18385;
    private readonly ConcurrentPool<byte[]> _loopbackBuffers = new ConcurrentPool<byte[]>(8, (Func<byte[]>) (() => new byte[1024]));
    private readonly List<ArraySegment<byte>> _loopbackQueue = new List<ArraySegment<byte>>();
    public byte ReliableSequencedChannel;

    protected override HlapiServer CreateServer(Unit details)
    {
      return new HlapiServer(this);
    }

    protected override HlapiClient CreateClient(Unit details)
    {
      return new HlapiClient(this);
    }

    protected override void Update()
    {
      if (this.IsInitialized)
      {
        if ((Object) NetworkManager.singleton != (Object) null && NetworkManager.singleton.isNetworkActive && (NetworkServer.active || NetworkClient.active) && (!NetworkClient.active || NetworkManager.singleton.client != null && NetworkManager.singleton.client.connection != null))
        {
          bool active1 = NetworkServer.active;
          bool active2 = NetworkClient.active;
          if (this.Mode.IsServerEnabled() != active1 || this.Mode.IsClientEnabled() != active2)
          {
            if (active1 && active2)
              this.RunAsHost(Unit.None, Unit.None);
            else if (active1)
              this.RunAsDedicatedServer(Unit.None);
            else if (active2)
              this.RunAsClient(Unit.None);
          }
        }
        else if (this.Mode != NetworkMode.None)
        {
          this.Stop();
          this._loopbackQueue.Clear();
        }
        for (int index = 0; index < this._loopbackQueue.Count; ++index)
        {
          if (this.Client != null)
            this.Client.NetworkReceivedPacket(this._loopbackQueue[index]);
          this._loopbackBuffers.Put(this._loopbackQueue[index].Array);
        }
        this._loopbackQueue.Clear();
      }
      base.Update();
    }

    protected override void Initialize()
    {
      if ((int) this.UnreliableChannel >= NetworkManager.singleton.channels.Count)
        throw this.Log.CreateUserErrorException("configured 'unreliable' channel is out of range", "set the wrong channel number in the HLAPI Comms Network component", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "B19B4916-8709-490B-8152-A646CCAD788E");
      QosType channel1 = NetworkManager.singleton.channels[(int) this.UnreliableChannel];
      if (channel1 != QosType.Unreliable)
        throw this.Log.CreateUserErrorException(string.Format("configured 'unreliable' channel has QoS type '{0}'", (object) channel1), "not creating the channel with the correct QoS type", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "24ee53b1-7517-4672-8a4a-64a3e3c87ef6");
      if ((int) this.ReliableSequencedChannel >= NetworkManager.singleton.channels.Count)
        throw this.Log.CreateUserErrorException("configured 'reliable' channel is out of range", "set the wrong channel number in the HLAPI Comms Network component", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "5F5F2875-ECC8-433D-B0CB-97C151B8094D");
      QosType channel2 = NetworkManager.singleton.channels[(int) this.ReliableSequencedChannel];
      if (channel2 != QosType.ReliableSequenced)
        throw this.Log.CreateUserErrorException(string.Format("configured 'reliable sequenced' channel has QoS type '{0}'", (object) channel2), "not creating the channel with the correct QoS type", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "035773ec-aef3-477a-8eeb-c234d416171c");
      int typeCode = (int) this.TypeCode;
      // ISSUE: reference to a compiler-generated field
      if (HlapiCommsNetwork.\u003C\u003Ef__mg\u0024cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        HlapiCommsNetwork.\u003C\u003Ef__mg\u0024cache0 = new NetworkMessageDelegate(HlapiCommsNetwork.NullMessageReceivedHandler);
      }
      // ISSUE: reference to a compiler-generated field
      NetworkMessageDelegate fMgCache0 = HlapiCommsNetwork.\u003C\u003Ef__mg\u0024cache0;
      NetworkServer.RegisterHandler((short) typeCode, fMgCache0);
      base.Initialize();
    }

    internal bool PreprocessPacketToClient(ArraySegment<byte> packet, HlapiConn destination)
    {
      if (this.Server == null)
        throw this.Log.CreatePossibleBugException("server packet preprocessing running, but this peer is not a server", "8f9dc0a0-1b48-4a7f-9bb6-f767b2542ab1");
      if (this.Client == null || NetworkManager.singleton.client.connection != destination.Connection)
        return false;
      if (this.Client != null)
        this._loopbackQueue.Add(packet.CopyTo<byte>(this._loopbackBuffers.Get(), 0));
      return true;
    }

    internal bool PreprocessPacketToServer(ArraySegment<byte> packet)
    {
      if (this.Client == null)
        throw this.Log.CreatePossibleBugException("client packet processing running, but this peer is not a client", "dd75dce4-e85c-4bb3-96ec-3a3636cc4fbe");
      if (this.Server == null)
        return false;
      this.Server.NetworkReceivedPacket(new HlapiConn(NetworkManager.singleton.client.connection), packet);
      return true;
    }

    internal static void NullMessageReceivedHandler([NotNull] NetworkMessage netmsg)
    {
      if (netmsg == null)
        throw new ArgumentNullException(nameof (netmsg));
      if (Logs.GetLogLevel(LogCategory.Network) <= LogLevel.Trace)
        Debug.Log((object) "Discarding Dissonance network message");
      int num1 = (int) netmsg.reader.ReadPackedUInt32();
      for (int index = 0; index < num1; ++index)
      {
        int num2 = (int) netmsg.reader.ReadByte();
      }
    }

    internal ArraySegment<byte> CopyToArraySegment([NotNull] NetworkReader msg, ArraySegment<byte> segment)
    {
      if (msg == null)
        throw new ArgumentNullException(nameof (msg));
      byte[] array = segment.Array;
      if (array == null)
        throw new ArgumentNullException(nameof (segment));
      int count = (int) msg.ReadPackedUInt32();
      if (count > segment.Count)
        throw this.Log.CreatePossibleBugException("receive buffer is too small", "A7387195-BF3D-4796-A362-6C64BB546445");
      for (int index = 0; index < count; ++index)
        array[segment.Offset + index] = msg.ReadByte();
      return new ArraySegment<byte>(array, segment.Offset, count);
    }

    internal int CopyPacketToNetworkWriter(ArraySegment<byte> packet, [NotNull] NetworkWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      byte[] array = packet.Array;
      if (array == null)
        throw new ArgumentNullException(nameof (packet));
      writer.SeekZero();
      writer.StartMessage(this.TypeCode);
      writer.WritePackedUInt32((uint) packet.Count);
      for (int index = 0; index < packet.Count; ++index)
        writer.Write(array[packet.Offset + index]);
      writer.FinishMessage();
      return (int) writer.Position;
    }
  }
}
