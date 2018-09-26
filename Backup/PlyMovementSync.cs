// Decompiled with JetBrains decompiler
// Type: PlyMovementSync
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using AntiFaker;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

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

  public PlyMovementSync()
  {
    base.\u002Ector();
  }

  public void SetupPosRot(Vector3 _p, float _r)
  {
    this.position = _p;
    this.rotation = _r;
  }

  private void FixedUpdate()
  {
    if (this.get_isLocalPlayer())
    {
      Quaternion rotation = ((Component) this).get_transform().get_rotation();
      this.myRotation = (float) ((Quaternion) ref rotation).get_eulerAngles().y;
    }
    this.TransmitData();
  }

  [ClientCallback]
  private void TransmitData()
  {
    if (!NetworkClient.get_active() || !this.get_isLocalPlayer())
      return;
    double rotation = (double) this.myRotation;
    Vector3 position = ((Component) this).get_transform().get_position();
    Quaternion localRotation = ((PlayerInteract) ((Component) this).GetComponent<PlayerInteract>()).playerCamera.get_transform().get_localRotation();
    // ISSUE: variable of the null type
    __Null x = ((Quaternion) ref localRotation).get_eulerAngles().x;
    this.CallCmdSyncData((float) rotation, position, (float) x);
  }

  private void Start()
  {
    this.plyCam = ((Scp049PlayerScript) ((Component) this).GetComponent<Scp049PlayerScript>()).plyCam.get_transform();
    this.speedhack = (AntiFakeCommands) ((Component) this).GetComponent<AntiFakeCommands>();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.teleportPosition = Vector3.get_zero();
    this.allowInput = true;
  }

  [Command(channel = 5)]
  private void CmdSyncData(float rot, Vector3 pos, float x)
  {
    this.rotation = rot;
    if (Vector3.op_Inequality(this.teleportPosition, Vector3.get_zero()))
    {
      this.position = this.teleportPosition;
      this.speedhack.SetPosition(this.teleportPosition);
      ((Component) this).get_transform().set_position(this.teleportPosition);
      this.teleportPosition = Vector3.get_zero();
    }
    else if (this.allowInput && this.speedhack.CheckMovement(pos))
    {
      if (this.ccm.curClass == 2)
        ((Vector3) ref pos).\u002Ector(0.0f, 2048f, 0.0f);
      this.position = pos;
    }
    else
      this.CallTargetSetPosition(this.get_connectionToClient(), this.position);
    this.NetworkrotX = x;
    ((Component) this.plyCam).get_transform().set_localRotation(Quaternion.Euler(x, 0.0f, 0.0f));
  }

  [TargetRpc]
  private void TargetSetPosition(NetworkConnection target, Vector3 pos)
  {
    ((Component) this).get_transform().set_position(pos);
    this.position = pos;
  }

  [TargetRpc]
  private void TargetSetRotation(NetworkConnection target, float rot)
  {
    this.myRotation = rot;
    this.rotation = rot;
    ((Component) this).get_transform().set_rotation(Quaternion.Euler(0.0f, rot, 0.0f));
    try
    {
      FirstPersonController component = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      ((MouseLook) component.m_MouseLook).SetRotation(rot);
    }
    catch
    {
    }
  }

  [Client]
  public void ClientSetRotation(float rot)
  {
    if (!NetworkClient.get_active())
      Debug.LogWarning((object) "[Client] function 'System.Void PlyMovementSync::ClientSetRotation(System.Single)' called on server");
    else
      this.myRotation = rot;
  }

  [Server]
  public void SetPosition(Vector3 pos)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogWarning((object) "[Server] function 'System.Void PlyMovementSync::SetPosition(UnityEngine.Vector3)' called on client");
    }
    else
    {
      this.teleportPosition = pos;
      this.position = pos;
      ((Component) this).get_transform().set_position(pos);
      this.speedhack.SetPosition(pos);
      this.CallTargetSetPosition(this.get_connectionToClient(), pos);
    }
  }

  [Server]
  public void SetRotation(float rot)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogWarning((object) "[Server] function 'System.Void PlyMovementSync::SetRotation(System.Single)' called on client");
    }
    else
    {
      this.rotation = rot;
      this.myRotation = rot;
      this.CallTargetSetRotation(this.get_connectionToClient(), rot);
    }
  }

  [Server]
  public void SetAllowInput(bool b)
  {
    if (!NetworkServer.get_active())
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
      this.SetSyncVar<float>((M0) (double) value, (M0&) ref this.rotX, 1U);
    }
  }

  protected static void InvokeCmdCmdSyncData(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSyncData called on client.");
    else
      ((PlyMovementSync) obj).CmdSyncData(reader.ReadSingle(), (Vector3) reader.ReadVector3(), reader.ReadSingle());
  }

  public void CallCmdSyncData(float rot, Vector3 pos, float x)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSyncData called on server.");
    else if (this.get_isServer())
    {
      this.CmdSyncData(rot, pos, x);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) PlyMovementSync.kCmdCmdSyncData);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(rot);
      networkWriter.Write((Vector3) pos);
      networkWriter.Write(x);
      this.SendCommandInternal(networkWriter, 5, "CmdSyncData");
    }
  }

  protected static void InvokeRpcTargetSetPosition(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetSetPosition called on server.");
    else
      ((PlyMovementSync) obj).TargetSetPosition(ClientScene.get_readyConnection(), (Vector3) reader.ReadVector3());
  }

  protected static void InvokeRpcTargetSetRotation(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetSetRotation called on server.");
    else
      ((PlyMovementSync) obj).TargetSetRotation(ClientScene.get_readyConnection(), reader.ReadSingle());
  }

  public void CallTargetSetPosition(NetworkConnection target, Vector3 pos)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetSetPosition called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetPosition called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlyMovementSync.kTargetRpcTargetSetPosition);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((Vector3) pos);
      this.SendTargetRPCInternal(target, networkWriter, 0, "TargetSetPosition");
    }
  }

  public void CallTargetSetRotation(NetworkConnection target, float rot)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetSetRotation called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetRotation called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) PlyMovementSync.kTargetRpcTargetSetRotation);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(rot);
      this.SendTargetRPCInternal(target, networkWriter, 0, "TargetSetRotation");
    }
  }

  static PlyMovementSync()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (PlyMovementSync), PlyMovementSync.kCmdCmdSyncData, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSyncData)));
    PlyMovementSync.kTargetRpcTargetSetPosition = 1295245089;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlyMovementSync), PlyMovementSync.kTargetRpcTargetSetPosition, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetSetPosition)));
    PlyMovementSync.kTargetRpcTargetSetRotation = 507139446;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (PlyMovementSync), PlyMovementSync.kTargetRpcTargetSetRotation, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetSetRotation)));
    NetworkCRC.RegisterBehaviour(nameof (PlyMovementSync), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.rotX);
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
      writer.Write(this.rotX);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
    Camera main = Camera.get_main();
    GameObject localPlayer = this.FindLocalPlayer();
    GameObject.FindGameObjectsWithTag("Player");
    int curClass1 = ((CharacterClassManager) localPlayer.GetComponent<CharacterClassManager>()).curClass;
    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
    {
      if (Object.op_Implicit((Object) gameObject.GetComponent<NetworkIdentity>()))
      {
        NicknameSync component1 = (NicknameSync) ((Component) gameObject.get_transform()).GetComponent<NicknameSync>();
        if (Random.Range(0, 3) == 1)
        {
          string nick = component1.myNick;
        }
        CharacterClassManager component2 = (CharacterClassManager) ((Component) component1).GetComponent<CharacterClassManager>();
        if (Object.op_Inequality((Object) component1, (Object) null))
        {
          Vector3 position = ((Component) gameObject.GetComponent<NetworkIdentity>()).get_transform().get_position();
          int curClass2 = component2.curClass;
          if (curClass2 >= 0 && !Object.op_Equality((Object) localPlayer, (Object) gameObject))
          {
            int num = (int) Vector3.Distance(((Component) main).get_transform().get_position(), position);
            Vector3 vector3;
            vector3.x = main.WorldToScreenPoint(position).x;
            vector3.y = main.WorldToScreenPoint(position).y;
            vector3.z = main.WorldToScreenPoint(position).z;
            GUI.set_color(this.getColorById(curClass2));
            string teamNameById = this.GetTeamNameById(curClass2);
            if (main.WorldToScreenPoint(position).z > 0.0)
            {
              GUI.Label(new Rect((float) (vector3.x - 50.0), (float) Screen.get_height() - (float) vector3.y, 100f, 50f), teamNameById + " [" + num.ToString() + "]");
              GUI.set_color(this.getTeamById(curClass1) != this.getTeamById(curClass2) ? Color.get_red() : Color.get_green());
              GUI.Label(new Rect((float) (vector3.x - 50.0), (float) ((double) Screen.get_height() - vector3.y - 60.0), 100f, 50f), component1.myNick + "[" + ((PlayerStats) gameObject.GetComponent<PlayerStats>()).health.ToString() + " HP]");
            }
          }
        }
      }
    }
    foreach (GameObject gameObject1 in GameObject.FindGameObjectsWithTag("PD_EXIT"))
    {
      if (Object.op_Implicit((Object) gameObject1.GetComponent<NetworkIdentity>()) && Object.op_Inequality((Object) gameObject1, (Object) null))
      {
        Vector3 position = ((Component) gameObject1.GetComponent<NetworkIdentity>()).get_transform().get_position();
        float num1 = Vector3.Distance(((Component) main).get_transform().get_position(), position);
        if ((double) num1 < 450.100006103516)
        {
          int itemId = ((Pickup) gameObject1.GetComponent<Pickup>()).info.itemId;
          int num2 = (int) num1;
          GUI.set_color(Color.get_white());
          Vector3 vector3;
          ((Vector3) ref vector3).\u002Ector((float) main.WorldToScreenPoint(position).x, (float) main.WorldToScreenPoint(position).y, (float) main.WorldToScreenPoint(position).z);
          GUI.Label(new Rect((float) (vector3.x - 50.0), (float) Screen.get_height() - (float) vector3.y, 100f, 50f), ((object) gameObject1).GetType().ToString() + " #" + (object) itemId + " [" + num2.ToString() + "]");
        }
      }
      foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("Pickup"))
      {
        if (Object.op_Implicit((Object) gameObject2.GetComponent<NetworkIdentity>()) && Object.op_Inequality((Object) gameObject2, (Object) null))
        {
          Vector3 position = ((Component) gameObject2.GetComponent<NetworkIdentity>()).get_transform().get_position();
          float num1 = Vector3.Distance(((Component) main).get_transform().get_position(), position);
          int itemId = ((Pickup) gameObject2.GetComponent<Pickup>()).info.itemId;
          ((Object) this).set_name("");
          switch (itemId)
          {
            case 0:
              ((Object) this).set_name("Janitor Card");
              break;
            case 1:
              ((Object) this).set_name("Scientist Card");
              break;
            case 2:
              ((Object) this).set_name("Major Scientist Card");
              break;
            case 3:
              ((Object) this).set_name("Zone Manager");
              break;
            case 4:
              ((Object) this).set_name("Guard Card");
              break;
            case 5:
              ((Object) this).set_name("Senior Guard Card");
              break;
            case 6:
              ((Object) this).set_name("Containment Engineer Card");
              break;
            case 7:
              ((Object) this).set_name("Lieutenant Card");
              break;
            case 8:
              ((Object) this).set_name("Commander Card");
              break;
            case 9:
              ((Object) this).set_name("Facility Manager Card");
              break;
            case 10:
              ((Object) this).set_name("Chaos Insurgency Device");
              break;
            case 11:
              ((Object) this).set_name("O5 Command Card");
              break;
          }
          if (main.WorldToScreenPoint(position).z > 0.0)
          {
            int num2 = (int) num1;
            GUI.set_color(Color.get_white());
            Vector3 vector3_1;
            ((Vector3) ref vector3_1).\u002Ector((float) main.WorldToScreenPoint(position).x, (float) main.WorldToScreenPoint(position).y, (float) main.WorldToScreenPoint(position).z);
            GUI.Label(new Rect((float) (vector3_1.x - 50.0), (float) Screen.get_height() - (float) vector3_1.y, 100f, 50f), ((Object) this).get_name() + " [" + num2.ToString() + "]");
            if (main.WorldToScreenPoint(position).z >= localPlayer.get_transform().get_position().y)
            {
              switch (itemId)
              {
                case 19:
                  ((Object) this).set_name("Ammo Box");
                  break;
                case 22:
                  ((Object) this).set_name("SFA ammo");
                  break;
                case 28:
                  ((Object) this).set_name("RAT ammo");
                  break;
                case 29:
                  ((Object) this).set_name("PAT ammo");
                  break;
              }
              if (main.WorldToScreenPoint(position).z > 0.0)
              {
                int num3 = (int) num1;
                GUI.set_color(Color.get_white());
                Vector3 vector3_2;
                ((Vector3) ref vector3_2).\u002Ector((float) main.WorldToScreenPoint(position).x, (float) main.WorldToScreenPoint(position).y, (float) main.WorldToScreenPoint(position).z);
                GUI.Label(new Rect((float) (vector3_2.x - 50.0), (float) Screen.get_height() - (float) vector3_2.y, 100f, 50f), ((Object) this).get_name() + " [" + num3.ToString() + "]");
                if (main.WorldToScreenPoint(position).z >= localPlayer.get_transform().get_position().y)
                {
                  switch (itemId)
                  {
                    case 12:
                      ((Object) this).set_name("Radio");
                      break;
                    case 13:
                      ((Object) this).set_name("Pistol");
                      break;
                    case 16:
                      ((Object) this).set_name("Micro HID");
                      break;
                    case 20:
                      ((Object) this).set_name("MTF Rifle");
                      break;
                    case 21:
                      ((Object) this).set_name("MTF Pistol");
                      break;
                    case 23:
                      ((Object) this).set_name("SMG");
                      break;
                    case 24:
                      ((Object) this).set_name("C.I. MG");
                      break;
                    case 25:
                      ((Object) this).set_name("EMP Grenade");
                      break;
                    case 26:
                      ((Object) this).set_name("Smoke Grenade");
                      break;
                    case 27:
                      ((Object) this).set_name("Disarmer");
                      break;
                  }
                  if (main.WorldToScreenPoint(position).z > 0.0)
                  {
                    int num4 = (int) num1;
                    GUI.set_color(Color.get_white());
                    Vector3 vector3_3;
                    ((Vector3) ref vector3_3).\u002Ector((float) main.WorldToScreenPoint(position).x, (float) main.WorldToScreenPoint(position).y, (float) main.WorldToScreenPoint(position).z);
                    GUI.Label(new Rect((float) (vector3_3.x - 50.0), (float) Screen.get_height() - (float) vector3_3.y, 100f, 50f), ((Object) this).get_name() + " [" + num4.ToString() + "]");
                    int num5 = (int) num1;
                    GUI.set_color(Color.get_white());
                    Vector3 vector3_4;
                    ((Vector3) ref vector3_4).\u002Ector((float) main.WorldToScreenPoint(position).x, (float) main.WorldToScreenPoint(position).y, (float) main.WorldToScreenPoint(position).z);
                    if (vector3_4.x < (double) Screen.get_width() && vector3_4.x > 0.0 && (vector3_4.y > 0.0 && vector3_4.y < (double) Screen.get_height()))
                      GUI.Label(new Rect((float) (vector3_4.x - 50.0), (float) Screen.get_height() - (float) vector3_4.y, 100f, 50f), ((Object) this).get_name() + " [" + num5.ToString() + "]");
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
      if (((NetworkIdentity) ((Component) this).get_gameObject().GetComponent<NetworkIdentity>()).get_isLocalPlayer())
        return ((Component) this).get_gameObject();
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
        return Color.get_red();
      case 1:
        return new Color(1f, 0.7f, 0.0f, 1f);
      case 4:
      case 11:
      case 12:
      case 13:
        return Color.get_blue();
      case 6:
        return Color.get_white();
      case 8:
        return Color.get_green();
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
