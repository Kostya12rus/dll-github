// Decompiled with JetBrains decompiler
// Type: MarkupTransceiver
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using RemoteAdmin;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MarkupTransceiver : NetworkBehaviour
{
  private static int kTargetRpcTargetRpcDownloadStyle = 2037165113;
  private static int kTargetRpcTargetRpcReceiveData;

  [ServerCallback]
  public void Transmit(string code, int[] playerIDs)
  {
    if (!NetworkServer.active)
      return;
    foreach (NetworkConnection target in this.GetTargets(playerIDs))
      this.CallTargetRpcReceiveData(target, code);
  }

  [ServerCallback]
  public void RequestStyleDownload(string url, int[] playerIDs)
  {
    if (!NetworkServer.active)
      return;
    foreach (NetworkConnection target in this.GetTargets(playerIDs))
      this.CallTargetRpcDownloadStyle(target, url);
  }

  [TargetRpc]
  private void TargetRpcDownloadStyle(NetworkConnection conn, string url)
  {
    MarkupReader.singleton.AddStyleFromURL(url);
  }

  public NetworkConnection[] GetTargets(int[] playerIDs)
  {
    List<NetworkConnection> networkConnectionList = new List<NetworkConnection>();
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      QueryProcessor component = player.GetComponent<QueryProcessor>();
      foreach (int playerId in playerIDs)
      {
        if (component.PlayerId == playerId)
          networkConnectionList.Add(component.connectionToClient);
      }
    }
    return networkConnectionList.ToArray();
  }

  [TargetRpc]
  private void TargetRpcReceiveData(NetworkConnection target, string code)
  {
    MarkupWriter.singleton.ReadTag(code);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeRpcTargetRpcDownloadStyle(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetRpcDownloadStyle called on server.");
    else
      ((MarkupTransceiver) obj).TargetRpcDownloadStyle(ClientScene.readyConnection, reader.ReadString());
  }

  protected static void InvokeRpcTargetRpcReceiveData(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetRpcReceiveData called on server.");
    else
      ((MarkupTransceiver) obj).TargetRpcReceiveData(ClientScene.readyConnection, reader.ReadString());
  }

  public void CallTargetRpcDownloadStyle(NetworkConnection conn, string url)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetRpcDownloadStyle called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetRpcDownloadStyle called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) MarkupTransceiver.kTargetRpcTargetRpcDownloadStyle);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(url);
      this.SendTargetRPCInternal(conn, writer, 0, "TargetRpcDownloadStyle");
    }
  }

  public void CallTargetRpcReceiveData(NetworkConnection target, string code)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetRpcReceiveData called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetRpcReceiveData called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) MarkupTransceiver.kTargetRpcTargetRpcReceiveData);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(code);
      this.SendTargetRPCInternal(target, writer, 0, "TargetRpcReceiveData");
    }
  }

  static MarkupTransceiver()
  {
    NetworkBehaviour.RegisterRpcDelegate(typeof (MarkupTransceiver), MarkupTransceiver.kTargetRpcTargetRpcDownloadStyle, new NetworkBehaviour.CmdDelegate(MarkupTransceiver.InvokeRpcTargetRpcDownloadStyle));
    MarkupTransceiver.kTargetRpcTargetRpcReceiveData = -1873892515;
    NetworkBehaviour.RegisterRpcDelegate(typeof (MarkupTransceiver), MarkupTransceiver.kTargetRpcTargetRpcReceiveData, new NetworkBehaviour.CmdDelegate(MarkupTransceiver.InvokeRpcTargetRpcReceiveData));
    NetworkCRC.RegisterBehaviour(nameof (MarkupTransceiver), 0);
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
