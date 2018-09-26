// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TMP_TextSelector_B
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro.Examples
{
  public class TMP_TextSelector_B : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler, IEventSystemHandler
  {
    private int m_selectedWord = -1;
    private int m_selectedLink = -1;
    private int m_lastIndex = -1;
    public RectTransform TextPopup_Prefab_01;
    private RectTransform m_TextPopup_RectTransform;
    private TextMeshProUGUI m_TextPopup_TMPComponent;
    private const string k_LinkText = "You have selected link <#ffff00>";
    private const string k_WordText = "Word Index: <#ffff00>";
    private TextMeshProUGUI m_TextMeshPro;
    private Canvas m_Canvas;
    private Camera m_Camera;
    private bool isHoveringObject;
    private Matrix4x4 m_matrix;
    private TMP_MeshInfo[] m_cachedMeshInfoVertexData;

    private void Awake()
    {
      this.m_TextMeshPro = this.gameObject.GetComponent<TextMeshProUGUI>();
      this.m_Canvas = this.gameObject.GetComponentInParent<Canvas>();
      this.m_Camera = this.m_Canvas.renderMode != RenderMode.ScreenSpaceOverlay ? this.m_Canvas.worldCamera : (Camera) null;
      this.m_TextPopup_RectTransform = Object.Instantiate<RectTransform>(this.TextPopup_Prefab_01);
      this.m_TextPopup_RectTransform.SetParent(this.m_Canvas.transform, false);
      this.m_TextPopup_TMPComponent = this.m_TextPopup_RectTransform.GetComponentInChildren<TextMeshProUGUI>();
      this.m_TextPopup_RectTransform.gameObject.SetActive(false);
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
      if (!(obj == (Object) this.m_TextMeshPro))
        return;
      this.m_cachedMeshInfoVertexData = this.m_TextMeshPro.textInfo.CopyMeshInfoVertexData();
    }

    private void LateUpdate()
    {
      if (this.isHoveringObject)
      {
        int intersectingCharacter = TMP_TextUtilities.FindIntersectingCharacter((TMP_Text) this.m_TextMeshPro, Input.mousePosition, this.m_Camera, true);
        if (intersectingCharacter == -1 || intersectingCharacter != this.m_lastIndex)
        {
          this.RestoreCachedVertexAttributes(this.m_lastIndex);
          this.m_lastIndex = -1;
        }
        if (intersectingCharacter != -1 && intersectingCharacter != this.m_lastIndex && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
          this.m_lastIndex = intersectingCharacter;
          int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[intersectingCharacter].materialReferenceIndex;
          int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[intersectingCharacter].vertexIndex;
          Vector3[] vertices = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].vertices;
          Vector3 vector3 = (Vector3) (Vector2) ((vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f);
          vertices[vertexIndex] = vertices[vertexIndex] - vector3;
          vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - vector3;
          vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - vector3;
          vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - vector3;
          this.m_matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 1.5f);
          vertices[vertexIndex] = this.m_matrix.MultiplyPoint3x4(vertices[vertexIndex]);
          vertices[vertexIndex + 1] = this.m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
          vertices[vertexIndex + 2] = this.m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
          vertices[vertexIndex + 3] = this.m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
          vertices[vertexIndex] = vertices[vertexIndex] + vector3;
          vertices[vertexIndex + 1] = vertices[vertexIndex + 1] + vector3;
          vertices[vertexIndex + 2] = vertices[vertexIndex + 2] + vector3;
          vertices[vertexIndex + 3] = vertices[vertexIndex + 3] + vector3;
          Color32 color32 = new Color32(byte.MaxValue, byte.MaxValue, (byte) 192, byte.MaxValue);
          Color32[] colors32 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
          colors32[vertexIndex] = color32;
          colors32[vertexIndex + 1] = color32;
          colors32[vertexIndex + 2] = color32;
          colors32[vertexIndex + 3] = color32;
          TMP_MeshInfo tmpMeshInfo = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex];
          int dst = vertices.Length - 4;
          tmpMeshInfo.SwapVertexData(vertexIndex, dst);
          this.m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
        int intersectingWord = TMP_TextUtilities.FindIntersectingWord((TMP_Text) this.m_TextMeshPro, Input.mousePosition, this.m_Camera);
        if ((Object) this.m_TextPopup_RectTransform != (Object) null && this.m_selectedWord != -1 && (intersectingWord == -1 || intersectingWord != this.m_selectedWord))
        {
          TMP_WordInfo tmpWordInfo = this.m_TextMeshPro.textInfo.wordInfo[this.m_selectedWord];
          for (int index1 = 0; index1 < tmpWordInfo.characterCount; ++index1)
          {
            int index2 = tmpWordInfo.firstCharacterIndex + index1;
            int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[index2].materialReferenceIndex;
            int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[index2].vertexIndex;
            Color32[] colors32 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
            Color32 color32 = colors32[vertexIndex].Tint(1.33333f);
            colors32[vertexIndex] = color32;
            colors32[vertexIndex + 1] = color32;
            colors32[vertexIndex + 2] = color32;
            colors32[vertexIndex + 3] = color32;
          }
          this.m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
          this.m_selectedWord = -1;
        }
        if (intersectingWord != -1 && intersectingWord != this.m_selectedWord && (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
        {
          this.m_selectedWord = intersectingWord;
          TMP_WordInfo tmpWordInfo = this.m_TextMeshPro.textInfo.wordInfo[intersectingWord];
          for (int index1 = 0; index1 < tmpWordInfo.characterCount; ++index1)
          {
            int index2 = tmpWordInfo.firstCharacterIndex + index1;
            int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[index2].materialReferenceIndex;
            int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[index2].vertexIndex;
            Color32[] colors32 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
            Color32 color32 = colors32[vertexIndex].Tint(0.75f);
            colors32[vertexIndex] = color32;
            colors32[vertexIndex + 1] = color32;
            colors32[vertexIndex + 2] = color32;
            colors32[vertexIndex + 3] = color32;
          }
          this.m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
        int intersectingLink = TMP_TextUtilities.FindIntersectingLink((TMP_Text) this.m_TextMeshPro, Input.mousePosition, this.m_Camera);
        if (intersectingLink == -1 && this.m_selectedLink != -1 || intersectingLink != this.m_selectedLink)
        {
          this.m_TextPopup_RectTransform.gameObject.SetActive(false);
          this.m_selectedLink = -1;
        }
        if (intersectingLink == -1 || intersectingLink == this.m_selectedLink)
          return;
        this.m_selectedLink = intersectingLink;
        TMP_LinkInfo tmpLinkInfo = this.m_TextMeshPro.textInfo.linkInfo[intersectingLink];
        Vector3 worldPoint = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_TextMeshPro.rectTransform, (Vector2) Input.mousePosition, this.m_Camera, out worldPoint);
        switch (tmpLinkInfo.GetLinkID())
        {
          case "id_01":
            this.m_TextPopup_RectTransform.position = worldPoint;
            this.m_TextPopup_RectTransform.gameObject.SetActive(true);
            this.m_TextPopup_TMPComponent.text = "You have selected link <#ffff00> ID 01";
            break;
          case "id_02":
            this.m_TextPopup_RectTransform.position = worldPoint;
            this.m_TextPopup_RectTransform.gameObject.SetActive(true);
            this.m_TextPopup_TMPComponent.text = "You have selected link <#ffff00> ID 02";
            break;
        }
      }
      else
      {
        if (this.m_lastIndex == -1)
          return;
        this.RestoreCachedVertexAttributes(this.m_lastIndex);
        this.m_lastIndex = -1;
      }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      this.isHoveringObject = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      this.isHoveringObject = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    private void RestoreCachedVertexAttributes(int index)
    {
      if (index == -1 || index > this.m_TextMeshPro.textInfo.characterCount - 1)
        return;
      int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[index].materialReferenceIndex;
      int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[index].vertexIndex;
      Vector3[] vertices1 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].vertices;
      Vector3[] vertices2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].vertices;
      vertices2[vertexIndex] = vertices1[vertexIndex];
      vertices2[vertexIndex + 1] = vertices1[vertexIndex + 1];
      vertices2[vertexIndex + 2] = vertices1[vertexIndex + 2];
      vertices2[vertexIndex + 3] = vertices1[vertexIndex + 3];
      Color32[] colors32_1 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
      Color32[] colors32_2 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].colors32;
      colors32_1[vertexIndex] = colors32_2[vertexIndex];
      colors32_1[vertexIndex + 1] = colors32_2[vertexIndex + 1];
      colors32_1[vertexIndex + 2] = colors32_2[vertexIndex + 2];
      colors32_1[vertexIndex + 3] = colors32_2[vertexIndex + 3];
      Vector2[] uvs0_1 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
      Vector2[] uvs0_2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs0;
      uvs0_2[vertexIndex] = uvs0_1[vertexIndex];
      uvs0_2[vertexIndex + 1] = uvs0_1[vertexIndex + 1];
      uvs0_2[vertexIndex + 2] = uvs0_1[vertexIndex + 2];
      uvs0_2[vertexIndex + 3] = uvs0_1[vertexIndex + 3];
      Vector2[] uvs2_1 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs2;
      Vector2[] uvs2_2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs2;
      uvs2_2[vertexIndex] = uvs2_1[vertexIndex];
      uvs2_2[vertexIndex + 1] = uvs2_1[vertexIndex + 1];
      uvs2_2[vertexIndex + 2] = uvs2_1[vertexIndex + 2];
      uvs2_2[vertexIndex + 3] = uvs2_1[vertexIndex + 3];
      int index1 = (vertices1.Length / 4 - 1) * 4;
      vertices2[index1] = vertices1[index1];
      vertices2[index1 + 1] = vertices1[index1 + 1];
      vertices2[index1 + 2] = vertices1[index1 + 2];
      vertices2[index1 + 3] = vertices1[index1 + 3];
      Color32[] colors32_3 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].colors32;
      Color32[] colors32_4 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
      colors32_4[index1] = colors32_3[index1];
      colors32_4[index1 + 1] = colors32_3[index1 + 1];
      colors32_4[index1 + 2] = colors32_3[index1 + 2];
      colors32_4[index1 + 3] = colors32_3[index1 + 3];
      Vector2[] uvs0_3 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
      Vector2[] uvs0_4 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs0;
      uvs0_4[index1] = uvs0_3[index1];
      uvs0_4[index1 + 1] = uvs0_3[index1 + 1];
      uvs0_4[index1 + 2] = uvs0_3[index1 + 2];
      uvs0_4[index1 + 3] = uvs0_3[index1 + 3];
      Vector2[] uvs2_3 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs2;
      Vector2[] uvs2_4 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs2;
      uvs2_4[index1] = uvs2_3[index1];
      uvs2_4[index1 + 1] = uvs2_3[index1 + 1];
      uvs2_4[index1 + 2] = uvs2_3[index1 + 2];
      uvs2_4[index1 + 3] = uvs2_3[index1 + 3];
      this.m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
  }
}
