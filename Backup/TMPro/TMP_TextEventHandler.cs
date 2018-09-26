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
    private TMP_TextEventHandler.CharacterSelectionEvent m_OnCharacterSelection;
    [SerializeField]
    private TMP_TextEventHandler.SpriteSelectionEvent m_OnSpriteSelection;
    [SerializeField]
    private TMP_TextEventHandler.WordSelectionEvent m_OnWordSelection;
    [SerializeField]
    private TMP_TextEventHandler.LineSelectionEvent m_OnLineSelection;
    [SerializeField]
    private TMP_TextEventHandler.LinkSelectionEvent m_OnLinkSelection;
    private TMP_Text m_TextComponent;
    private Camera m_Camera;
    private Canvas m_Canvas;
    private int m_selectedLink;
    private int m_lastCharIndex;
    private int m_lastWordIndex;
    private int m_lastLineIndex;

    public TMP_TextEventHandler()
    {
      base.\u002Ector();
    }

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
      this.m_TextComponent = (TMP_Text) ((Component) this).get_gameObject().GetComponent<TMP_Text>();
      if (((object) this.m_TextComponent).GetType() == typeof (TextMeshProUGUI))
      {
        this.m_Canvas = (Canvas) ((Component) this).get_gameObject().GetComponentInParent<Canvas>();
        if (!Object.op_Inequality((Object) this.m_Canvas, (Object) null))
          return;
        if (this.m_Canvas.get_renderMode() == null)
          this.m_Camera = (Camera) null;
        else
          this.m_Camera = this.m_Canvas.get_worldCamera();
      }
      else
        this.m_Camera = Camera.get_main();
    }

    private void LateUpdate()
    {
      if (!TMP_TextUtilities.IsIntersectingRectTransform(this.m_TextComponent.get_rectTransform(), Input.get_mousePosition(), this.m_Camera))
        return;
      int intersectingCharacter = TMP_TextUtilities.FindIntersectingCharacter(this.m_TextComponent, Input.get_mousePosition(), this.m_Camera, true);
      if (intersectingCharacter != -1 && intersectingCharacter != this.m_lastCharIndex)
      {
        this.m_lastCharIndex = intersectingCharacter;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        TMP_TextElementType elementType = (TMP_TextElementType) (^(TMP_CharacterInfo&) ref this.m_TextComponent.get_textInfo().characterInfo[intersectingCharacter]).elementType;
        if (elementType == null)
        {
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          this.SendOnCharacterSelection((char) (^(TMP_CharacterInfo&) ref this.m_TextComponent.get_textInfo().characterInfo[intersectingCharacter]).character, intersectingCharacter);
        }
        else if (elementType == 1)
        {
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          this.SendOnSpriteSelection((char) (^(TMP_CharacterInfo&) ref this.m_TextComponent.get_textInfo().characterInfo[intersectingCharacter]).character, intersectingCharacter);
        }
      }
      int intersectingWord = TMP_TextUtilities.FindIntersectingWord(this.m_TextComponent, Input.get_mousePosition(), this.m_Camera);
      if (intersectingWord != -1 && intersectingWord != this.m_lastWordIndex)
      {
        this.m_lastWordIndex = intersectingWord;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        TMP_WordInfo tmpWordInfo = ^(TMP_WordInfo&) ref this.m_TextComponent.get_textInfo().wordInfo[intersectingWord];
        this.SendOnWordSelection(((TMP_WordInfo) ref tmpWordInfo).GetWord(), (int) tmpWordInfo.firstCharacterIndex, (int) tmpWordInfo.characterCount);
      }
      int intersectingLine = TMP_TextUtilities.FindIntersectingLine(this.m_TextComponent, Input.get_mousePosition(), this.m_Camera);
      if (intersectingLine != -1 && intersectingLine != this.m_lastLineIndex)
      {
        this.m_lastLineIndex = intersectingLine;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        TMP_LineInfo tmpLineInfo = ^(TMP_LineInfo&) ref this.m_TextComponent.get_textInfo().lineInfo[intersectingLine];
        char[] chArray = new char[tmpLineInfo.characterCount];
        for (int index = 0; index < tmpLineInfo.characterCount && index < this.m_TextComponent.get_textInfo().characterInfo.Length; ++index)
        {
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          chArray[index] = (char) (^(TMP_CharacterInfo&) ref this.m_TextComponent.get_textInfo().characterInfo[index + tmpLineInfo.firstCharacterIndex]).character;
        }
        this.SendOnLineSelection(new string(chArray), (int) tmpLineInfo.firstCharacterIndex, (int) tmpLineInfo.characterCount);
      }
      int intersectingLink = TMP_TextUtilities.FindIntersectingLink(this.m_TextComponent, Input.get_mousePosition(), this.m_Camera);
      if (intersectingLink == -1 || intersectingLink == this.m_selectedLink)
        return;
      this.m_selectedLink = intersectingLink;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      TMP_LinkInfo tmpLinkInfo = ^(TMP_LinkInfo&) ref this.m_TextComponent.get_textInfo().linkInfo[intersectingLink];
      this.SendOnLinkSelection(((TMP_LinkInfo) ref tmpLinkInfo).GetLinkID(), ((TMP_LinkInfo) ref tmpLinkInfo).GetLinkText(), intersectingLink);
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
      public CharacterSelectionEvent()
      {
        base.\u002Ector();
      }
    }

    [Serializable]
    public class SpriteSelectionEvent : UnityEvent<char, int>
    {
      public SpriteSelectionEvent()
      {
        base.\u002Ector();
      }
    }

    [Serializable]
    public class WordSelectionEvent : UnityEvent<string, int, int>
    {
      public WordSelectionEvent()
      {
        base.\u002Ector();
      }
    }

    [Serializable]
    public class LineSelectionEvent : UnityEvent<string, int, int>
    {
      public LineSelectionEvent()
      {
        base.\u002Ector();
      }
    }

    [Serializable]
    public class LinkSelectionEvent : UnityEvent<string, string, int>
    {
      public LinkSelectionEvent()
      {
        base.\u002Ector();
      }
    }
  }
}
