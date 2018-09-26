// Decompiled with JetBrains decompiler
// Type: ChatController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
  public TMP_InputField TMP_ChatInput;
  public TMP_Text TMP_ChatOutput;
  public Scrollbar ChatScrollbar;

  public ChatController()
  {
    base.\u002Ector();
  }

  private void OnEnable()
  {
    // ISSUE: method pointer
    ((UnityEvent<string>) this.TMP_ChatInput.get_onSubmit()).AddListener(new UnityAction<string>((object) this, __methodptr(AddToChatOutput)));
  }

  private void OnDisable()
  {
    // ISSUE: method pointer
    ((UnityEvent<string>) this.TMP_ChatInput.get_onSubmit()).RemoveListener(new UnityAction<string>((object) this, __methodptr(AddToChatOutput)));
  }

  private void AddToChatOutput(string newText)
  {
    this.TMP_ChatInput.set_text(string.Empty);
    DateTime now = DateTime.Now;
    TMP_Text tmpChatOutput = this.TMP_ChatOutput;
    tmpChatOutput.set_text(tmpChatOutput.get_text() + "[<#FFFF80>" + now.Hour.ToString("d2") + ":" + now.Minute.ToString("d2") + ":" + now.Second.ToString("d2") + "</color>] " + newText + "\n");
    this.TMP_ChatInput.ActivateInputField();
    this.ChatScrollbar.set_value(0.0f);
  }
}
