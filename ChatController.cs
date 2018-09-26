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

  private void OnEnable()
  {
    this.TMP_ChatInput.onSubmit.AddListener(new UnityAction<string>(this.AddToChatOutput));
  }

  private void OnDisable()
  {
    this.TMP_ChatInput.onSubmit.RemoveListener(new UnityAction<string>(this.AddToChatOutput));
  }

  private void AddToChatOutput(string newText)
  {
    this.TMP_ChatInput.text = string.Empty;
    DateTime now = DateTime.Now;
    TMP_Text tmpChatOutput = this.TMP_ChatOutput;
    tmpChatOutput.text = tmpChatOutput.text + "[<#FFFF80>" + now.Hour.ToString("d2") + ":" + now.Minute.ToString("d2") + ":" + now.Second.ToString("d2") + "</color>] " + newText + "\n";
    this.TMP_ChatInput.ActivateInputField();
    this.ChatScrollbar.value = 0.0f;
  }
}
