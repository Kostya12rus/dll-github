// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.TextBasedRemoteAdmin
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
  public class TextBasedRemoteAdmin : MonoBehaviour
  {
    private readonly List<string> _logs = new List<string>();
    public static TextBasedRemoteAdmin singleton;
    public TextMeshProUGUI consoleWindow;
    public InputField commandField;
    private UIController _ui;

    private void Start()
    {
      FirstPersonController.usingRemoteAdmin = false;
      this._ui = this.GetComponent<UIController>();
      this._logs.Add("(SYSTEM) Text Based Remote Admin system started at " + DateTime.Now.ToLongTimeString());
      this.RefreshConsole();
    }

    private void Awake()
    {
      TextBasedRemoteAdmin.singleton = this;
    }

    public static void AddLog(string log)
    {
      TextBasedRemoteAdmin.singleton._logs.Add(log);
      TextBasedRemoteAdmin.singleton.RefreshConsole();
    }

    private void RefreshConsole()
    {
      string str = string.Empty;
      foreach (string log in this._logs)
        str = str + log + "\n\n";
      this.consoleWindow.text = str;
    }

    private void Update()
    {
      if (!Input.GetKeyDown(KeyCode.Return) || !this._ui.loggedIn || (!this._ui.opened || string.IsNullOrEmpty(this.commandField.text)))
        return;
      this.SendCommand();
    }

    public void SendCommand()
    {
      if (!string.IsNullOrEmpty(this.commandField.text))
      {
        PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery(this.commandField.text);
        this.commandField.text = string.Empty;
      }
      this.commandField.ActivateInputField();
    }
  }
}
