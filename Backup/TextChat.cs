// Decompiled with JetBrains decompiler
// Type: TextChat
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TextChat : NetworkBehaviour
{
  private static int kCmdCmdSendChat = -683434843;
  public int messageDuration;
  private static Transform lply;
  public GameObject textMessagePrefab;
  private Transform attachParent;
  public bool enabledChat;
  private List<GameObject> msgs;
  private static int kRpcRpcSendChat;

  public TextChat()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (!this.get_isLocalPlayer())
      return;
    TextChat.lply = ((Component) this).get_transform();
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer() || !this.enabledChat)
      return;
    for (int index = 0; index < this.msgs.Count; ++index)
    {
      if (Object.op_Equality((Object) this.msgs[index], (Object) null))
      {
        this.msgs.RemoveAt(index);
        break;
      }
      ((TextMessage) this.msgs[index].GetComponent<TextMessage>()).position = (float) (this.msgs.Count - index - 1);
    }
    if (!Input.GetKeyDown((KeyCode) 13))
      return;
    this.SendChat("(づ｡◕‿‿◕｡)づ" + (object) Random.Range(0, 4654), ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick, ((Component) this).get_transform().get_position());
  }

  private void SendChat(string msg, string nick, Vector3 position)
  {
    this.CallCmdSendChat(msg, nick, position);
  }

  [Command(channel = 2)]
  private void CmdSendChat(string msg, string nick, Vector3 pos)
  {
    this.CallRpcSendChat(msg, nick, pos);
  }

  [ClientRpc(channel = 2)]
  private void RpcSendChat(string msg, string nick, Vector3 pos)
  {
    if ((double) Vector3.Distance(TextChat.lply.get_position(), pos) >= 15.0)
      return;
    this.AddMsg(msg, nick);
  }

  private void AddMsg(string msg, string nick)
  {
    while (msg.Contains("<"))
      msg = msg.Replace("<", "＜");
    while (msg.Contains(">"))
      msg = msg.Replace(">", "＞");
    string str = "<b>" + nick + "</b>: " + msg;
    GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) this.textMessagePrefab);
    gameObject.get_transform().SetParent(this.attachParent);
    this.msgs.Add(gameObject);
    gameObject.get_transform().set_localRotation(Quaternion.Euler(Vector3.get_zero()));
    gameObject.get_transform().set_localScale(Vector3.get_one());
    ((Text) gameObject.GetComponent<Text>()).set_text(str);
    ((TextMessage) gameObject.GetComponent<TextMessage>()).remainingLife = (float) this.messageDuration;
    Object.Destroy((Object) gameObject, (float) this.messageDuration);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdSendChat(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSendChat called on client.");
    else
      ((TextChat) obj).CmdSendChat(reader.ReadString(), reader.ReadString(), (Vector3) reader.ReadVector3());
  }

  public void CallCmdSendChat(string msg, string nick, Vector3 pos)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSendChat called on server.");
    else if (this.get_isServer())
    {
      this.CmdSendChat(msg, nick, pos);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) TextChat.kCmdCmdSendChat);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(msg);
      networkWriter.Write(nick);
      networkWriter.Write((Vector3) pos);
      this.SendCommandInternal(networkWriter, 2, "CmdSendChat");
    }
  }

  protected static void InvokeRpcRpcSendChat(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcSendChat called on server.");
    else
      ((TextChat) obj).RpcSendChat(reader.ReadString(), reader.ReadString(), (Vector3) reader.ReadVector3());
  }

  public void CallRpcSendChat(string msg, string nick, Vector3 pos)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcSendChat called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) TextChat.kRpcRpcSendChat);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(msg);
      networkWriter.Write(nick);
      networkWriter.Write((Vector3) pos);
      this.SendRPCInternal(networkWriter, 2, "RpcSendChat");
    }
  }

  static TextChat()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (TextChat), TextChat.kCmdCmdSendChat, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSendChat)));
    TextChat.kRpcRpcSendChat = -734819717;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (TextChat), TextChat.kRpcRpcSendChat, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcSendChat)));
    NetworkCRC.RegisterBehaviour(nameof (TextChat), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
