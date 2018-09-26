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
    private int m_selectedLink = -1;
    private int m_lastCharIndex = -1;
    private int m_lastWordIndex = -1;
    private TextMeshPro m_TextMeshPro;
    private Camera m_Camera;
    private bool m_isHoveringObject;

    private void Awake()
    {
      this.m_TextMeshPro = this.gameObject.GetComponent<TextMeshPro>();
      this.m_Camera = Camera.main;
      this.m_TextMeshPro.ForceMeshUpdate();
    }

    private void LateUpdate()
    {
      this.m_isHoveringObject = false;
      if (TMP_TextUtilities.IsIntersectingRectTransform(this.m_TextMeshPro.rectTransform, Input.mousePosition, Camera.main))
        this.m_isHoveringObject = true;
      if (!this.m_isHoveringObject)
        return;
      int intersectingCharacter = TMP_TextUtilities.FindIntersectingCharacter((TMP_Text) this.m_TextMeshPro, Input.mousePosition, Camera.main, true);
      if (intersectingCharacter != -1 && intersectingCharacter != this.m_lastCharIndex && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
      {
        this.m_lastCharIndex = intersectingCharacter;
        int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[intersectingCharacter].materialReferenceIndex;
        int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[intersectingCharacter].vertexIndex;
        Color32 color32 = new Color32((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), byte.MaxValue);
        Color32[] colors32 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
        colors32[vertexIndex] = color32;
        colors32[vertexIndex + 1] = color32;
        colors32[vertexIndex + 2] = color32;
        colors32[vertexIndex + 3] = color32;
        this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].mesh.colors32 = colors32;
      }
      int intersectingLink = TMP_TextUtilities.FindIntersectingLink((TMP_Text) this.m_TextMeshPro, Input.mousePosition, this.m_Camera);
      if (intersectingLink == -1 && this.m_selectedLink != -1 || intersectingLink != this.m_selectedLink)
        this.m_selectedLink = -1;
      if (intersectingLink != -1 && intersectingLink != this.m_selectedLink)
      {
        this.m_selectedLink = intersectingLink;
        TMP_LinkInfo tmpLinkInfo = this.m_TextMeshPro.textInfo.linkInfo[intersectingLink];
        Debug.Log((object) ("Link ID: \"" + tmpLinkInfo.GetLinkID() + "\"   Link Text: \"" + tmpLinkInfo.GetLinkText() + "\""));
        Vector3 worldPoint = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_TextMeshPro.rectTransform, (Vector2) Input.mousePosition, this.m_Camera, out worldPoint);
        switch (tmpLinkInfo.GetLinkID())
        {
        }
      }
      int intersectingWord = TMP_TextUtilities.FindIntersectingWord((TMP_Text) this.m_TextMeshPro, Input.mousePosition, Camera.main);
      if (intersectingWord == -1 || intersectingWord == this.m_lastWordIndex)
        return;
      this.m_lastWordIndex = intersectingWord;
      TMP_WordInfo tmpWordInfo = this.m_TextMeshPro.textInfo.wordInfo[intersectingWord];
      Camera.main.WorldToScreenPoint(this.m_TextMeshPro.transform.TransformPoint(this.m_TextMeshPro.textInfo.characterInfo[tmpWordInfo.firstCharacterIndex].bottomLeft));
      Color32[] colors32_1 = this.m_TextMeshPro.textInfo.meshInfo[0].colors32;
      Color32 color32_1 = new Color32((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), byte.MaxValue);
      for (int index = 0; index < tmpWordInfo.characterCount; ++index)
      {
        int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[tmpWordInfo.firstCharacterIndex + index].vertexIndex;
        colors32_1[vertexIndex] = color32_1;
        colors32_1[vertexIndex + 1] = color32_1;
        colors32_1[vertexIndex + 2] = color32_1;
        colors32_1[vertexIndex + 3] = color32_1;
      }
      this.m_TextMeshPro.mesh.colors32 = colors32_1;
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
