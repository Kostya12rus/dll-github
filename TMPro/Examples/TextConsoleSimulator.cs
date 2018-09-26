// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TextConsoleSimulator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class TextConsoleSimulator : MonoBehaviour
  {
    private TMP_Text m_TextComponent;
    private bool hasTextChanged;

    private void Awake()
    {
      this.m_TextComponent = this.gameObject.GetComponent<TMP_Text>();
    }

    private void Start()
    {
      this.StartCoroutine(this.RevealCharacters(this.m_TextComponent));
    }

    private void OnEnable()
    {
      TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<Object>(this.ON_TEXT_CHANGED));
    }

    private void OnDisable()
    {
      TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<Object>(this.ON_TEXT_CHANGED));
    }

    private void ON_TEXT_CHANGED(Object obj)
    {
      this.hasTextChanged = true;
    }

    [DebuggerHidden]
    private IEnumerator RevealCharacters(TMP_Text textComponent)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TextConsoleSimulator.\u003CRevealCharacters\u003Ec__Iterator0() { textComponent = textComponent, \u0024this = this };
    }

    [DebuggerHidden]
    private IEnumerator RevealWords(TMP_Text textComponent)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TextConsoleSimulator.\u003CRevealWords\u003Ec__Iterator1() { textComponent = textComponent };
    }
  }
}
