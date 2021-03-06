﻿// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TMP_TextEventCheck
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

namespace TMPro.Examples
{
  public class TMP_TextEventCheck : MonoBehaviour
  {
    public TMP_TextEventHandler TextEventHandler;

    public TMP_TextEventCheck()
    {
      base.\u002Ector();
    }

    private void OnEnable()
    {
      if (!Object.op_Inequality((Object) this.TextEventHandler, (Object) null))
        return;
      // ISSUE: method pointer
      this.TextEventHandler.onCharacterSelection.AddListener(new UnityAction<char, int>((object) this, __methodptr(OnCharacterSelection)));
      // ISSUE: method pointer
      this.TextEventHandler.onSpriteSelection.AddListener(new UnityAction<char, int>((object) this, __methodptr(OnSpriteSelection)));
      // ISSUE: method pointer
      this.TextEventHandler.onWordSelection.AddListener(new UnityAction<string, int, int>((object) this, __methodptr(OnWordSelection)));
      // ISSUE: method pointer
      this.TextEventHandler.onLineSelection.AddListener(new UnityAction<string, int, int>((object) this, __methodptr(OnLineSelection)));
      // ISSUE: method pointer
      this.TextEventHandler.onLinkSelection.AddListener(new UnityAction<string, string, int>((object) this, __methodptr(OnLinkSelection)));
    }

    private void OnDisable()
    {
      if (!Object.op_Inequality((Object) this.TextEventHandler, (Object) null))
        return;
      // ISSUE: method pointer
      this.TextEventHandler.onCharacterSelection.RemoveListener(new UnityAction<char, int>((object) this, __methodptr(OnCharacterSelection)));
      // ISSUE: method pointer
      this.TextEventHandler.onSpriteSelection.RemoveListener(new UnityAction<char, int>((object) this, __methodptr(OnSpriteSelection)));
      // ISSUE: method pointer
      this.TextEventHandler.onWordSelection.RemoveListener(new UnityAction<string, int, int>((object) this, __methodptr(OnWordSelection)));
      // ISSUE: method pointer
      this.TextEventHandler.onLineSelection.RemoveListener(new UnityAction<string, int, int>((object) this, __methodptr(OnLineSelection)));
      // ISSUE: method pointer
      this.TextEventHandler.onLinkSelection.RemoveListener(new UnityAction<string, string, int>((object) this, __methodptr(OnLinkSelection)));
    }

    private void OnCharacterSelection(char c, int index)
    {
      Debug.Log((object) ("Character [" + (object) c + "] at Index: " + (object) index + " has been selected."));
    }

    private void OnSpriteSelection(char c, int index)
    {
      Debug.Log((object) ("Sprite [" + (object) c + "] at Index: " + (object) index + " has been selected."));
    }

    private void OnWordSelection(string word, int firstCharacterIndex, int length)
    {
      Debug.Log((object) ("Word [" + word + "] with first character index of " + (object) firstCharacterIndex + " and length of " + (object) length + " has been selected."));
    }

    private void OnLineSelection(string lineText, int firstCharacterIndex, int length)
    {
      Debug.Log((object) ("Line [" + lineText + "] with first character index of " + (object) firstCharacterIndex + " and length of " + (object) length + " has been selected."));
    }

    private void OnLinkSelection(string linkID, string linkText, int linkIndex)
    {
      Debug.Log((object) ("Link Index: " + (object) linkIndex + " with ID [" + linkID + "] and Text \"" + linkText + "\" has been selected."));
    }
  }
}
