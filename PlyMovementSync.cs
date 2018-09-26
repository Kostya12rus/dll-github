// Decompiled with JetBrains decompiler
// Type: PlyMovementSync
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using AntiFaker;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class PlyMovementSync : NetworkBehaviour
{
  private static int kCmdCmdSyncData = -1186400596;
  public float rotation;
  public Vector3 position;
  [SyncVar]
  public float rotX;
  private bool allowInput;
  private float myRotation;
  private CharacterClassManager ccm;
  private AntiFakeCommands speedhack;
  private Transform plyCam;
  private Vector3 teleportPosition;
  private static int kTargetRpcTargetSetPosition;
  private static int kTargetRpcTargetSetRotation;

  public void SetupPosRot(Vector3 _p, float _r)
  {
    this.position = _p;
    this.rotation = _r;
  }

  private void FixedUpdate()
  {
    if (this.isLocalPlayer)
      this.myRotation = this.transform.rotation.eulerAngles.y;
    this.TransmitData();
  }

  [ClientCallback]
  private void TransmitData()
  {
    if (!NetworkClient.active || !this.isLocalPlayer)
      return;
    this.CallCmdSyncData(this.myRotation, this.transform.position, this.GetComponent<PlayerInteract>().playerCamera.transform.localRotation.eulerAngles.x);
  }

  private void Start()
  {
    this.plyCam = this.GetComponent<Scp049PlayerScript>().plyCam.transform;
    this.speedhack = this.GetComponent<AntiFakeCommands>();
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.teleportPosition = Vector3.zero;
    this.allowInput = true;
  }

  [Command(channel = 5)]
  private void CmdSyncData(float rot, Vector3 pos, float x)
  {
    this.rotation = rot;
    if (this.teleportPosition != Vector3.zero)
    {
      this.position = this.teleportPosition;
      this.speedhack.SetPosition(this.teleportPosition);
      this.transform.position = this.teleportPosition;
      this.teleportPosition = Vector3.zero;
    }
    else if (this.allowInput && this.speedhack.CheckMovement(pos))
    {
      if (this.ccm.curClass == 2)
        pos = new Vector3(0.0f, 2048f, 0.0f);
      this.position = pos;
    }
    else
      this.CallTargetSetPosition(this.connectionToClient, this.position);
    this.NetworkrotX = x;
    this.plyCam.transform.localRotation = Quaternion.Euler(x, 0.0f, 0.0f);
  }

  [TargetRpc]
  private void TargetSetPosition(NetworkConnection target, Vector3 pos)
  {
    this.transform.position = pos;
    this.position = pos;
  }

  [TargetRpc]
  private void TargetSetRotation(NetworkConnection target, float rot)
  {
    this.myRotation = rot;
    this.rotation = rot;
    this.transform.rotation = Quaternion.Euler(0.0f, rot, 0.0f);
    try
    {
      FirstPersonController component = this.GetComponent<FirstPersonController>();
      if (!((Object) component != (Object) null))
        return;
      component.m_MouseLook.SetRotation(rot);
    }
    catch
    {
    }
  }

  [Client]
  public void ClientSetRotation(float rot)
  {
    if (!NetworkClient.active)
      Debug.LogWarning((object) "[Client] function 'System.Void PlyMovementSync::ClientSetRotation(System.Single)' called on server");
    else
      this.myRotation = rot;
  }

  [Server]
  public void SetPosition(Vector3 pos)
  {
    if (!NetworkServer.active)
    {
      Debug.LogWarning((object) "[Server] function 'System.Void PlyMovementSync::SetPosition(UnityEngine.Vector3)' called on client");
    }
    else
    {
      this.teleportPosition = pos;
      this.position = pos;
      this.transform.position = pos;
      this.speedhack.SetPosition(pos);
      this.CallTargetSetPosition(this.connectionToClient, pos);
    }
  }

  [Server]
  public void SetRotation(float rot)
  {
    if (!NetworkServer.active)
    {
      Debug.LogWarning((object) "[Server] function 'System.Void PlyMovementSync::SetRotation(System.Single)' called on client");
    }
    else
    {
      this.rotation = rot;
      this.myRotation = rot;
      this.CallTargetSetRotation(this.connectionToClient, rot);
    }
  }

  [Server]
  public void SetAllowInput(bool b)
  {
    if (!NetworkServer.active)
      Debug.LogWarning((object) "[Server] function 'System.Void PlyMovementSync::SetAllowInput(System.Boolean)' called on client");
    else
      this.allowInput = b;
  }

  private void UNetVersion()
  {
  }

  public float NetworkrotX
  {
    get
    {
      return this.rotX;
    }
    [param: In] set
    {
      this.SetSyncVar<float>(value, ref this.rotX, 1U);
    }
  }

  protected static void InvokeCmdCmdSyncData(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSyncData called on client.");
    else
      ((PlyMovementSync) obj).CmdSyncData(reader.ReadSingle(), reader.ReadVector3(), reader.ReadSingle());
  }

  public void CallCmdSyncData(float rot, Vector3 pos, float x)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSyncData called on server.");
    else if (this.isServer)
    {
      this.CmdSyncData(rot, pos, x);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) PlyMovementSync.kCmdCmdSyncData);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(rot);
      writer.Write(pos);
      writer.Write(x);
      this.SendCommandInternal(writer, 5, "CmdSyncData");
    }
  }

  protected static void InvokeRpcTargetSetPosition(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetSetPosition called on server.");
    else
      ((PlyMovementSync) obj).TargetSetPosition(ClientScene.readyConnection, reader.ReadVector3());
  }

  protected static void InvokeRpcTargetSetRotation(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetSetRotation called on server.");
    else
      ((PlyMovementSync) obj).TargetSetRotation(ClientScene.readyConnection, reader.ReadSingle());
  }

  public void CallTargetSetPosition(NetworkConnection target, Vector3 pos)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetSetPosition called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetPosition called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlyMovementSync.kTargetRpcTargetSetPosition);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(pos);
      this.SendTargetRPCInternal(target, writer, 0, "TargetSetPosition");
    }
  }

  public void CallTargetSetRotation(NetworkConnection target, float rot)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetSetRotation called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetRotation called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) PlyMovementSync.kTargetRpcTargetSetRotation);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(rot);
      this.SendTargetRPCInternal(target, writer, 0, "TargetSetRotation");
    }
  }

  static PlyMovementSync()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlyMovementSync), PlyMovementSync.kCmdCmdSyncData, new NetworkBehaviour.CmdDelegate(PlyMovementSync.InvokeCmdCmdSyncData));
    PlyMovementSync.kTargetRpcTargetSetPosition = 1295245089;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlyMovementSync), PlyMovementSync.kTargetRpcTargetSetPosition, new NetworkBehaviour.CmdDelegate(PlyMovementSync.InvokeRpcTargetSetPosition));
    PlyMovementSync.kTargetRpcTargetSetRotation = 507139446;
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlyMovementSync), PlyMovementSync.kTargetRpcTargetSetRotation, new NetworkBehaviour.CmdDelegate(PlyMovementSync.InvokeRpcTargetSetRotation));
    NetworkCRC.RegisterBehaviour(nameof (PlyMovementSync), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.rotX);
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
      writer.Write(this.rotX);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.rotX = reader.ReadSingle();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.rotX = reader.ReadSingle();
    }
  }

  private void OnGUI()
  {
    Camera main = Camera.main;
    GameObject localPlayer = this.FindLocalPlayer();
    GameObject.FindGameObjectsWithTag("Player");
    int curClass1 = localPlayer.GetComponent<CharacterClassManager>().curClass;
    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
    {
      if ((bool) ((Object) gameObject.GetComponent<NetworkIdentity>()))
      {
        NicknameSync component1 = gameObject.transform.GetComponent<NicknameSync>();
        if (Random.Range(0, 3) == 1)
        {
          string nick = component1.myNick;
        }
        CharacterClassManager component2 = component1.GetComponent<CharacterClassManager>();
        if ((Object) component1 != (Object) null)
        {
          Vector3 position = gameObject.GetComponent<NetworkIdentity>().transform.position;
          int curClass2 = component2.curClass;
          if (curClass2 >= 0 && !((Object) localPlayer == (Object) gameObject))
          {
            int num = (int) Vector3.Distance(main.transform.position, position);
            Vector3 vector3;
            vector3.x = main.WorldToScreenPoint(position).x;
            vector3.y = main.WorldToScreenPoint(position).y;
            vector3.z = main.WorldToScreenPoint(position).z;
            GUI.color = this.getColorById(curClass2);
            string teamNameById = this.GetTeamNameById(curClass2);
            if ((double) main.WorldToScreenPoint(position).z > 0.0)
            {
              GUI.Label(new Rect(vector3.x - 50f, (float) Screen.height - vector3.y, 100f, 50f), teamNameById + " [" + num.ToString() + "]");
              GUI.color = this.getTeamById(curClass1) != this.getTeamById(curClass2) ? Color.red : Color.green;
              GUI.Label(new Rect(vector3.x - 50f, (float) ((double) Screen.height - (double) vector3.y - 60.0), 100f, 50f), component1.myNick + "[" + gameObject.GetComponent<PlayerStats>().health.ToString() + " HP]");
            }
          }
        }
      }
    }
    foreach (GameObject gameObject1 in GameObject.FindGameObjectsWithTag("PD_EXIT"))
    {
      if ((bool) ((Object) gameObject1.GetComponent<NetworkIdentity>()) && (Object) gameObject1 != (Object) null)
      {
        Vector3 position = gameObject1.GetComponent<NetworkIdentity>().transform.position;
        float num1 = Vector3.Distance(main.transform.position, position);
        if ((double) num1 < 450.100006103516)
        {
          int itemId = gameObject1.GetComponent<Pickup>().info.itemId;
          int num2 = (int) num1;
          GUI.color = Color.white;
          Vector3 vector3 = new Vector3(main.WorldToScreenPoint(position).x, main.WorldToScreenPoint(position).y, main.WorldToScreenPoint(position).z);
          GUI.Label(new Rect(vector3.x - 50f, (float) Screen.height - vector3.y, 100f, 50f), gameObject1.GetType().ToString() + " #" + (object) itemId + " [" + num2.ToString() + "]");
        }
      }
      foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("Pickup"))
      {
        if ((bool) ((Object) gameObject2.GetComponent<NetworkIdentity>()) && (Object) gameObject2 != (Object) null)
        {
          Vector3 position = gameObject2.GetComponent<NetworkIdentity>().transform.position;
          float num1 = Vector3.Distance(main.transform.position, position);
          int itemId = gameObject2.GetComponent<Pickup>().info.itemId;
          this.name = "";
          switch (itemId)
          {
            case 0:
              this.name = "Janitor Card";
              break;
            case 1:
              this.name = "Scientist Card";
              break;
            case 2:
              this.name = "Major Scientist Card";
              break;
            case 3:
              this.name = "Zone Manager";
              break;
            case 4:
              this.name = "Guard Card";
              break;
            case 5:
              this.name = "Senior Guard Card";
              break;
            case 6:
              this.name = "Containment Engineer Card";
              break;
            case 7:
              this.name = "Lieutenant Card";
              break;
            case 8:
              this.name = "Commander Card";
              break;
            case 9:
              this.name = "Facility Manager Card";
              break;
            case 10:
              this.name = "Chaos Insurgency Device";
              break;
            case 11:
              this.name = "O5 Command Card";
              break;
          }
          if ((double) main.WorldToScreenPoint(position).z > 0.0)
          {
            int num2 = (int) num1;
            GUI.color = Color.white;
            Vector3 vector3_1 = new Vector3(main.WorldToScreenPoint(position).x, main.WorldToScreenPoint(position).y, main.WorldToScreenPoint(position).z);
            GUI.Label(new Rect(vector3_1.x - 50f, (float) Screen.height - vector3_1.y, 100f, 50f), this.name + " [" + num2.ToString() + "]");
            if ((double) main.WorldToScreenPoint(position).z >= (double) localPlayer.transform.position.y)
            {
              switch (itemId)
              {
                case 19:
                  this.name = "Ammo Box";
                  break;
                case 22:
                  this.name = "SFA ammo";
                  break;
                case 28:
                  this.name = "RAT ammo";
                  break;
                case 29:
                  this.name = "PAT ammo";
                  break;
              }
              if ((double) main.WorldToScreenPoint(position).z > 0.0)
              {
                int num3 = (int) num1;
                GUI.color = Color.white;
                Vector3 vector3_2 = new Vector3(main.WorldToScreenPoint(position).x, main.WorldToScreenPoint(position).y, main.WorldToScreenPoint(position).z);
                GUI.Label(new Rect(vector3_2.x - 50f, (float) Screen.height - vector3_2.y, 100f, 50f), this.name + " [" + num3.ToString() + "]");
                if ((double) main.WorldToScreenPoint(position).z >= (double) localPlayer.transform.position.y)
                {
                  switch (itemId)
                  {
                    case 12:
                      this.name = "Radio";
                      break;
                    case 13:
                      this.name = "Pistol";
                      break;
                    case 16:
                      this.name = "Micro HID";
                      break;
                    case 20:
                      this.name = "MTF Rifle";
                      break;
                    case 21:
                      this.name = "MTF Pistol";
                      break;
                    case 23:
                      this.name = "SMG";
                      break;
                    case 24:
                      this.name = "C.I. MG";
                      break;
                    case 25:
                      this.name = "EMP Grenade";
                      break;
                    case 26:
                      this.name = "Smoke Grenade";
                      break;
                    case 27:
                      this.name = "Disarmer";
                      break;
                  }
                  if ((double) main.WorldToScreenPoint(position).z > 0.0)
                  {
                    int num4 = (int) num1;
                    GUI.color = Color.white;
                    Vector3 vector3_3 = new Vector3(main.WorldToScreenPoint(position).x, main.WorldToScreenPoint(position).y, main.WorldToScreenPoint(position).z);
                    GUI.Label(new Rect(vector3_3.x - 50f, (float) Screen.height - vector3_3.y, 100f, 50f), this.name + " [" + num4.ToString() + "]");
                    int num5 = (int) num1;
                    GUI.color = Color.white;
                    Vector3 vector3_4 = new Vector3(main.WorldToScreenPoint(position).x, main.WorldToScreenPoint(position).y, main.WorldToScreenPoint(position).z);
                    if ((double) vector3_4.x < (double) Screen.width && (double) vector3_4.x > 0.0 && ((double) vector3_4.y > 0.0 && (double) vector3_4.y < (double) Screen.height))
                      GUI.Label(new Rect(vector3_4.x - 50f, (float) Screen.height - vector3_4.y, 100f, 50f), this.name + " [" + num5.ToString() + "]");
                  }
                }
              }
            }
          }
        }
      }
    }
  }

  private GameObject FindLocalPlayer()
  {
    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
    {
      if (this.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        return this.gameObject;
    }
    return (GameObject) null;
  }

  private Color getColorById(int curTeam)
  {
    switch (curTeam)
    {
      case 0:
      case 3:
      case 5:
      case 9:
      case 10:
        return Color.red;
      case 1:
        return new Color(1f, 0.7f, 0.0f, 1f);
      case 4:
      case 11:
      case 12:
      case 13:
        return Color.blue;
      case 6:
        return Color.white;
      case 8:
        return Color.green;
      default:
        return new Color(1f, 0.7f, 0.3f, 0.5f);
    }
  }

  private int getTeamById(int iId)
  {
    switch (iId)
    {
      case 1:
      case 8:
        return 0;
      case 4:
      case 6:
      case 11:
      case 12:
      case 13:
      case 15:
      case 16:
      case 17:
        return 1;
      default:
        return 2;
    }
  }

  private string GetTeamNameById(int tid)
  {
    switch (tid)
    {
      case -1:
        return "Server Admin";
      case 0:
        return "SCP-173";
      case 1:
        return "Class D";
      case 2:
        return "Spectator";
      case 3:
        return "SCP-106";
      case 4:
        return "MTF Scientist";
      case 5:
        return "SCP-049";
      case 6:
        return "Scientist";
      case 8:
        return "Chaos Insurgency";
      case 9:
        return "SCP-096";
      case 10:
        return "SCP-049-2";
      case 11:
        return "MTF Lieutenant";
      case 12:
        return "MTF Commander";
      case 13:
        return "MTF Guard";
      case 15:
        return "facility Guard";
      case 16:
        return "scp-939-53";
      case 17:
        return "scp-939-89";
      default:
        return "undefined (#" + tid.ToString() + ")";
    }
  }
}
