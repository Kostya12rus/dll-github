// Decompiled with JetBrains decompiler
// Type: PlayerPositionManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerPositionManager : NetworkBehaviour
{
  private static int kTargetRpcTargetTransmit = -1979501602;
  public static PlayerPositionManager singleton;
  private bool isReadyToWork;
  private PlayerPositionData[] receivedData;
  private CharacterClassManager myCCM;

  private void Start()
  {
    Timing.RunCoroutine(this._Start(), Segment.Update);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new PlayerPositionManager.\u003C_Start\u003Ec__Iterator0() { \u0024this = this };
  }

  public static void StaticReceiveData(PlayerPositionData[] data)
  {
    PlayerPositionManager.singleton.ReceiveData(data);
  }

  public void ReceiveData(PlayerPositionData[] data)
  {
    this.receivedData = data;
  }

  private void FixedUpdate()
  {
    this.ReceiveData();
    if (!NetworkServer.active)
      return;
    this.TransmitData();
  }

  [ServerCallback]
  private void TransmitData()
  {
    if (!NetworkServer.active)
      return;
    List<PlayerPositionData> playerPositionDataList1 = new List<PlayerPositionData>();
    List<GameObject> list = ((IEnumerable<GameObject>) PlayerManager.singleton.players).ToList<GameObject>();
    foreach (GameObject _player in list)
      playerPositionDataList1.Add(new PlayerPositionData(_player));
    this.receivedData = playerPositionDataList1.ToArray();
    foreach (GameObject gameObject in list)
    {
      CharacterClassManager component1 = gameObject.GetComponent<CharacterClassManager>();
      if (component1.curClass >= 0 && component1.klasy[component1.curClass].fullName.Contains("939"))
      {
        List<PlayerPositionData> playerPositionDataList2 = new List<PlayerPositionData>((IEnumerable<PlayerPositionData>) playerPositionDataList1);
        for (int index = 0; index < playerPositionDataList2.Count; ++index)
        {
          CharacterClassManager component2 = list[index].GetComponent<CharacterClassManager>();
          if ((double) playerPositionDataList2[index].position.y < 800.0 && component2.klasy[component2.curClass].team != Team.SCP && (component2.klasy[component2.curClass].team != Team.RIP && !list[index].GetComponent<Scp939_VisionController>().CanSee(component1.GetComponent<Scp939PlayerScript>())))
            playerPositionDataList2[index] = new PlayerPositionData()
            {
              position = Vector3.up * 6000f,
              rotation = 0.0f,
              playerID = playerPositionDataList2[index].playerID
            };
        }
        this.CallTargetTransmit(gameObject.GetComponent<NetworkIdentity>().connectionToClient, playerPositionDataList2.ToArray());
      }
      else
        this.CallTargetTransmit(gameObject.GetComponent<NetworkIdentity>().connectionToClient, playerPositionDataList1.ToArray());
    }
  }

  [TargetRpc(channel = 5)]
  private void TargetTransmit(NetworkConnection conn, PlayerPositionData[] data)
  {
    this.receivedData = data;
  }

  private void ReceiveData()
  {
    if (!this.isReadyToWork)
      return;
    if ((Object) this.myCCM != (Object) null)
    {
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        QueryProcessor component1 = player.GetComponent<QueryProcessor>();
        foreach (PlayerPositionData playerPositionData in this.receivedData)
        {
          if (component1.PlayerId == playerPositionData.playerID)
          {
            if (!component1.isLocalPlayer)
            {
              CharacterClassManager component2 = player.GetComponent<CharacterClassManager>();
              if ((double) Vector3.Distance(player.transform.position, playerPositionData.position) < 10.0 && this.myCCM.curClass != -1 && (component2.curClass != 0 || !this.myCCM.IsHuman()))
              {
                player.transform.position = Vector3.Lerp(player.transform.position, playerPositionData.position, 0.2f);
                this.SetRotation(component2, Quaternion.Lerp(Quaternion.Euler(player.transform.rotation.eulerAngles), Quaternion.Euler(Vector3.up * playerPositionData.rotation), 0.3f));
              }
              else
              {
                player.transform.position = playerPositionData.position;
                this.SetRotation(component2, Quaternion.Euler(0.0f, playerPositionData.rotation, 0.0f));
              }
            }
            if (!NetworkServer.active)
            {
              player.GetComponent<PlyMovementSync>().SetupPosRot(playerPositionData.position, playerPositionData.rotation);
              break;
            }
            break;
          }
        }
      }
    }
    else
      this.myCCM = PlayerManager.localPlayer.GetComponent<CharacterClassManager>();
  }

  private void SetRotation(CharacterClassManager target, Quaternion quat)
  {
    if (target.curClass == 0 && this.myCCM.IsHuman() && !Scp173PlayerScript.isBlinking)
      return;
    target.transform.rotation = quat;
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeRpcTargetTransmit(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetTransmit called on server.");
    else
      ((PlayerPositionManager) obj).TargetTransmit(ClientScene.readyConnection, GeneratedNetworkCode._ReadArrayPlayerPositionData_None(reader));
  }

  public void CallTargetTransmit(NetworkConnection conn, PlayerPositionData[] data)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetTransmit called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetTransmit called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlayerPositionManager.kTargetRpcTargetTransmit);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      GeneratedNetworkCode._WriteArrayPlayerPositionData_None(writer, data);
      this.SendTargetRPCInternal(conn, writer, 5, "TargetTransmit");
    }
  }

  static PlayerPositionManager()
  {
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlayerPositionManager), PlayerPositionManager.kTargetRpcTargetTransmit, new NetworkBehaviour.CmdDelegate(PlayerPositionManager.InvokeRpcTargetTransmit));
    NetworkCRC.RegisterBehaviour(nameof (PlayerPositionManager), 0);
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
