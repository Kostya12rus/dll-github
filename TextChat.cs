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
  private List<GameObject> msgs = new List<GameObject>();
  public int messageDuration;
  private static Transform lply;
  public GameObject textMessagePrefab;
  private Transform attachParent;
  public bool enabledChat;
  private static int kRpcRpcSendChat;

  private void Start()
  {
    if (!this.isLocalPlayer)
      return;
    TextChat.lply = this.transform;
  }

  private void Update()
  {
    if (!this.isLocalPlayer || !this.enabledChat)
      return;
    for (int index = 0; index < this.msgs.Count; ++index)
    {
      if ((Object) this.msgs[index] == (Object) null)
      {
        this.msgs.RemoveAt(index);
        break;
      }
      this.msgs[index].GetComponent<TextMessage>().position = (float) (this.msgs.Count - index - 1);
    }
    if (!Input.GetKeyDown(KeyCode.Return))
      return;
    this.SendChat("(づ｡◕‿‿◕｡)づ" + (object) Random.Range(0, 4654), this.GetComponent<NicknameSync>().myNick, this.transform.position);
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
    if ((double) Vector3.Distance(TextChat.lply.position, pos) >= 15.0)
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
    GameObject gameObject = Object.Instantiate<GameObject>(this.textMessagePrefab);
    gameObject.transform.SetParent(this.attachParent);
    this.msgs.Add(gameObject);
    gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    gameObject.transform.localScale = Vector3.one;
    gameObject.GetComponent<Text>().text = str;
    gameObject.GetComponent<TextMessage>().remainingLife = (float) this.messageDuration;
    Object.Destroy((Object) gameObject, (float) this.messageDuration);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdSendChat(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSendChat called on client.");
    else
      ((TextChat) obj).CmdSendChat(reader.ReadString(), reader.ReadString(), reader.ReadVector3());
  }

  public void CallCmdSendChat(string msg, string nick, Vector3 pos)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSendChat called on server.");
    else if (this.isServer)
    {
      this.CmdSendChat(msg, nick, pos);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) TextChat.kCmdCmdSendChat);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(msg);
      writer.Write(nick);
      writer.Write(pos);
      this.SendCommandInternal(writer, 2, "CmdSendChat");
    }
  }

  protected static void InvokeRpcRpcSendChat(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcSendChat called on server.");
    else
      ((TextChat) obj).RpcSendChat(reader.ReadString(), reader.ReadString(), reader.ReadVector3());
  }

  public void CallRpcSendChat(string msg, string nick, Vector3 pos)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcSendChat called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) TextChat.kRpcRpcSendChat);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(msg);
      writer.Write(nick);
      writer.Write(pos);
      this.SendRPCInternal(writer, 2, "RpcSendChat");
    }
  }

  static TextChat()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (TextChat), TextChat.kCmdCmdSendChat, new NetworkBehaviour.CmdDelegate(TextChat.InvokeCmdCmdSendChat));
    TextChat.kRpcRpcSendChat = -734819717;
    NetworkBehaviour.RegisterRpcDelegate(typeof (TextChat), TextChat.kRpcRpcSendChat, new NetworkBehaviour.CmdDelegate(TextChat.InvokeRpcRpcSendChat));
    NetworkCRC.RegisterBehaviour(nameof (TextChat), 0);
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
