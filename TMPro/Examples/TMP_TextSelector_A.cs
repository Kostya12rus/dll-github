// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TMP_TextSelector_A
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro.Examples
{
  public class TMP_TextSelector_A : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
  {
    private TextMeshPro m_TextMeshPro;
    private Camera m_Camera;
    private bool m_isHoveringObject;
    private int m_selectedLink;
    private int m_lastCharIndex;
    private int m_lastWordIndex;

    public TMP_TextSelector_A()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this.m_TextMeshPro = (TextMeshPro) ((Component) this).get_gameObject().GetComponent<TextMeshPro>();
      this.m_Camera = Camera.get_main();
      ((TMP_Text) this.m_TextMeshPro).ForceMeshUpdate();
    }

    private void LateUpdate()
    {
      this.m_isHoveringObject = false;
      if (TMP_TextUtilities.IsIntersectingRectTransform(((TMP_Text) this.m_TextMeshPro).get_rectTransform(), Input.get_mousePosition(), Camera.get_main()))
        this.m_isHoveringObject = true;
      if (!this.m_isHoveringObject)
        return;
      int intersectingCharacter = TMP_TextUtilities.FindIntersectingCharacter((TMP_Text) this.m_TextMeshPro, Input.get_mousePosition(), Camera.get_main(), true);
      if (intersectingCharacter != -1 && intersectingCharacter != this.m_lastCharIndex && (Input.GetKey((KeyCode) 304) || Input.GetKey((KeyCode) 303)))
      {
        this.m_lastCharIndex = intersectingCharacter;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        int materialReferenceIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[intersectingCharacter]).materialReferenceIndex;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        int vertexIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[intersectingCharacter]).vertexIndex;
        Color32 color32;
        ((Color32) ref color32).\u002Ector((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), byte.MaxValue);
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        Color32[] colors32 = (Color32[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).colors32;
        colors32[vertexIndex] = color32;
        colors32[vertexIndex + 1] = color32;
        colors32[vertexIndex + 2] = color32;
        colors32[vertexIndex + 3] = color32;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ((Mesh) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).mesh).set_colors32(colors32);
      }
      int intersectingLink = TMP_TextUtilities.FindIntersectingLink((TMP_Text) this.m_TextMeshPro, Input.get_mousePosition(), this.m_Camera);
      if (intersectingLink == -1 && this.m_selectedLink != -1 || intersectingLink != this.m_selectedLink)
        this.m_selectedLink = -1;
      if (intersectingLink != -1 && intersectingLink != this.m_selectedLink)
      {
        this.m_selectedLink = intersectingLink;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        TMP_LinkInfo tmpLinkInfo = ^(TMP_LinkInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().linkInfo[intersectingLink];
        Debug.Log((object) ("Link ID: \"" + ((TMP_LinkInfo) ref tmpLinkInfo).GetLinkID() + "\"   Link Text: \"" + ((TMP_LinkInfo) ref tmpLinkInfo).GetLinkText() + "\""));
        Vector3 zero = Vector3.get_zero();
        RectTransformUtility.ScreenPointToWorldPointInRectangle(((TMP_Text) this.m_TextMeshPro).get_rectTransform(), Vector2.op_Implicit(Input.get_mousePosition()), this.m_Camera, ref zero);
        switch (((TMP_LinkInfo) ref tmpLinkInfo).GetLinkID())
        {
        }
      }
      int intersectingWord = TMP_TextUtilities.FindIntersectingWord((TMP_Text) this.m_TextMeshPro, Input.get_mousePosition(), Camera.get_main());
      if (intersectingWord == -1 || intersectingWord == this.m_lastWordIndex)
        return;
      this.m_lastWordIndex = intersectingWord;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      TMP_WordInfo tmpWordInfo = ^(TMP_WordInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().wordInfo[intersectingWord];
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Camera.get_main().WorldToScreenPoint(this.m_TextMeshPro.get_transform().TransformPoint((Vector3) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[tmpWordInfo.firstCharacterIndex]).bottomLeft));
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Color32[] colors32_1 = (Color32[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[0]).colors32;
      Color32 color32_1;
      ((Color32) ref color32_1).\u002Ector((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), byte.MaxValue);
      for (int index = 0; index < tmpWordInfo.characterCount; ++index)
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        int vertexIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[tmpWordInfo.firstCharacterIndex + index]).vertexIndex;
        colors32_1[vertexIndex] = color32_1;
        colors32_1[vertexIndex + 1] = color32_1;
        colors32_1[vertexIndex + 2] = color32_1;
        colors32_1[vertexIndex + 3] = color32_1;
      }
      ((TMP_Text) this.m_TextMeshPro).get_mesh().set_colors32(colors32_1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      Debug.Log((object) "OnPointerEnter()");
      this.m_isHoveringObject = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      Debug.Log((object) "OnPointerExit()");
      this.m_isHoveringObject = false;
    }
  }
}
