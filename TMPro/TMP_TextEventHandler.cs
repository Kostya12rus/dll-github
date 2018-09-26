// Decompiled with JetBrains decompiler
// Type: TMPro.TMP_TextEventHandler
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TMPro
{
  public class TMP_TextEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
  {
    [SerializeField]
    private TMP_TextEventHandler.CharacterSelectionEvent m_OnCharacterSelection = new TMP_TextEventHandler.CharacterSelectionEvent();
    [SerializeField]
    private TMP_TextEventHandler.SpriteSelectionEvent m_OnSpriteSelection = new TMP_TextEventHandler.SpriteSelectionEvent();
    [SerializeField]
    private TMP_TextEventHandler.WordSelectionEvent m_OnWordSelection = new TMP_TextEventHandler.WordSelectionEvent();
    [SerializeField]
    private TMP_TextEventHandler.LineSelectionEvent m_OnLineSelection = new TMP_TextEventHandler.LineSelectionEvent();
    [SerializeField]
    private TMP_TextEventHandler.LinkSelectionEvent m_OnLinkSelection = new TMP_TextEventHandler.LinkSelectionEvent();
    private int m_selectedLink = -1;
    private int m_lastCharIndex = -1;
    private int m_lastWordIndex = -1;
    private int m_lastLineIndex = -1;
    private TMP_Text m_TextComponent;
    private Camera m_Camera;
    private Canvas m_Canvas;

    public TMP_TextEventHandler.CharacterSelectionEvent onCharacterSelection
    {
      get
      {
        return this.m_OnCharacterSelection;
      }
      set
      {
        this.m_OnCharacterSelection = value;
      }
    }

    public TMP_TextEventHandler.SpriteSelectionEvent onSpriteSelection
    {
      get
      {
        return this.m_OnSpriteSelection;
      }
      set
      {
        this.m_OnSpriteSelection = value;
      }
    }

    public TMP_TextEventHandler.WordSelectionEvent onWordSelection
    {
      get
      {
        return this.m_OnWordSelection;
      }
      set
      {
        this.m_OnWordSelection = value;
      }
    }

    public TMP_TextEventHandler.LineSelectionEvent onLineSelection
    {
      get
      {
        return this.m_OnLineSelection;
      }
      set
      {
        this.m_OnLineSelection = value;
      }
    }

    public TMP_TextEventHandler.LinkSelectionEvent onLinkSelection
    {
      get
      {
        return this.m_OnLinkSelection;
      }
      set
      {
        this.m_OnLinkSelection = value;
      }
    }

    private void Awake()
    {
      this.m_TextComponent = this.gameObject.GetComponent<TMP_Text>();
      if (this.m_TextComponent.GetType() == typeof (TextMeshProUGUI))
      {
        this.m_Canvas = this.gameObject.GetComponentInParent<Canvas>();
        if (!((Object) this.m_Canvas != (Object) null))
          return;
        if (this.m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
          this.m_Camera = (Camera) null;
        else
          this.m_Camera = this.m_Canvas.worldCamera;
      }
      else
        this.m_Camera = Camera.main;
    }

    private void LateUpdate()
    {
      if (!TMP_TextUtilities.IsIntersectingRectTransform(this.m_TextComponent.rectTransform, Input.mousePosition, this.m_Camera))
        return;
      int intersectingCharacter = TMP_TextUtilities.FindIntersectingCharacter(this.m_TextComponent, Input.mousePosition, this.m_Camera, true);
      if (intersectingCharacter != -1 && intersectingCharacter != this.m_lastCharIndex)
      {
        this.m_lastCharIndex = intersectingCharacter;
        switch (this.m_TextComponent.textInfo.characterInfo[intersectingCharacter].elementType)
        {
          case TMP_TextElementType.Character:
            this.SendOnCharacterSelection(this.m_TextComponent.textInfo.characterInfo[intersectingCharacter].character, intersectingCharacter);
            break;
          case TMP_TextElementType.Sprite:
            this.SendOnSpriteSelection(this.m_TextComponent.textInfo.characterInfo[intersectingCharacter].character, intersectingCharacter);
            break;
        }
      }
      int intersectingWord = TMP_TextUtilities.FindIntersectingWord(this.m_TextComponent, Input.mousePosition, this.m_Camera);
      if (intersectingWord != -1 && intersectingWord != this.m_lastWordIndex)
      {
        this.m_lastWordIndex = intersectingWord;
        TMP_WordInfo tmpWordInfo = this.m_TextComponent.textInfo.wordInfo[intersectingWord];
        this.SendOnWordSelection(tmpWordInfo.GetWord(), tmpWordInfo.firstCharacterIndex, tmpWordInfo.characterCount);
      }
      int intersectingLine = TMP_TextUtilities.FindIntersectingLine(this.m_TextComponent, Input.mousePosition, this.m_Camera);
      if (intersectingLine != -1 && intersectingLine != this.m_lastLineIndex)
      {
        this.m_lastLineIndex = intersectingLine;
        TMP_LineInfo tmpLineInfo = this.m_TextComponent.textInfo.lineInfo[intersectingLine];
        char[] chArray = new char[tmpLineInfo.characterCount];
        for (int index = 0; index < tmpLineInfo.characterCount && index < this.m_TextComponent.textInfo.characterInfo.Length; ++index)
          chArray[index] = this.m_TextComponent.textInfo.characterInfo[index + tmpLineInfo.firstCharacterIndex].character;
        this.SendOnLineSelection(new string(chArray), tmpLineInfo.firstCharacterIndex, tmpLineInfo.characterCount);
      }
      int intersectingLink = TMP_TextUtilities.FindIntersectingLink(this.m_TextComponent, Input.mousePosition, this.m_Camera);
      if (intersectingLink == -1 || intersectingLink == this.m_selectedLink)
        return;
      this.m_selectedLink = intersectingLink;
      TMP_LinkInfo tmpLinkInfo = this.m_TextComponent.textInfo.linkInfo[intersectingLink];
      this.SendOnLinkSelection(tmpLinkInfo.GetLinkID(), tmpLinkInfo.GetLinkText(), intersectingLink);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    private void SendOnCharacterSelection(char character, int characterIndex)
    {
      if (this.onCharacterSelection == null)
        return;
      this.onCharacterSelection.Invoke(character, characterIndex);
    }

    private void SendOnSpriteSelection(char character, int characterIndex)
    {
      if (this.onSpriteSelection == null)
        return;
      this.onSpriteSelection.Invoke(character, characterIndex);
    }

    private void SendOnWordSelection(string word, int charIndex, int length)
    {
      if (this.onWordSelection == null)
        return;
      this.onWordSelection.Invoke(word, charIndex, length);
    }

    private void SendOnLineSelection(string line, int charIndex, int length)
    {
      if (this.onLineSelection == null)
        return;
      this.onLineSelection.Invoke(line, charIndex, length);
    }

    private void SendOnLinkSelection(string linkID, string linkText, int linkIndex)
    {
      if (this.onLinkSelection == null)
        return;
      this.onLinkSelection.Invoke(linkID, linkText, linkIndex);
    }

    [Serializable]
    public class CharacterSelectionEvent : UnityEvent<char, int>
    {
    }

    [Serializable]
    public class SpriteSelectionEvent : UnityEvent<char, int>
    {
    }

    [Serializable]
    public class WordSelectionEvent : UnityEvent<string, int, int>
    {
    }

    [Serializable]
    public class LineSelectionEvent : UnityEvent<string, int, int>
    {
    }

    [Serializable]
    public class LinkSelectionEvent : UnityEvent<string, string, int>
    {
    }
  }
}
