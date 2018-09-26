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
    public RectTransform TextPopup_Prefab_01;
    private RectTransform m_TextPopup_RectTransform;
    private TextMeshProUGUI m_TextPopup_TMPComponent;
    private const string k_LinkText = "You have selected link <#ffff00>";
    private const string k_WordText = "Word Index: <#ffff00>";
    private TextMeshProUGUI m_TextMeshPro;
    private Canvas m_Canvas;
    private Camera m_Camera;
    private bool isHoveringObject;
    private int m_selectedWord;
    private int m_selectedLink;
    private int m_lastIndex;
    private Matrix4x4 m_matrix;
    private TMP_MeshInfo[] m_cachedMeshInfoVertexData;

    public TMP_TextSelector_B()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this.m_TextMeshPro = (TextMeshProUGUI) ((Component) this).get_gameObject().GetComponent<TextMeshProUGUI>();
      this.m_Canvas = (Canvas) ((Component) this).get_gameObject().GetComponentInParent<Canvas>();
      this.m_Camera = this.m_Canvas.get_renderMode() != null ? this.m_Canvas.get_worldCamera() : (Camera) null;
      this.m_TextPopup_RectTransform = (RectTransform) Object.Instantiate<RectTransform>((M0) this.TextPopup_Prefab_01);
      ((Transform) this.m_TextPopup_RectTransform).SetParent(((Component) this.m_Canvas).get_transform(), false);
      this.m_TextPopup_TMPComponent = (TextMeshProUGUI) ((Component) this.m_TextPopup_RectTransform).GetComponentInChildren<TextMeshProUGUI>();
      ((Component) this.m_TextPopup_RectTransform).get_gameObject().SetActive(false);
    }

    private void OnEnable()
    {
      ((FastAction<Object>) TMPro_EventManager.TEXT_CHANGED_EVENT).Add(new Action<Object>(this.ON_TEXT_CHANGED));
    }

    private void OnDisable()
    {
      ((FastAction<Object>) TMPro_EventManager.TEXT_CHANGED_EVENT).Remove(new Action<Object>(this.ON_TEXT_CHANGED));
    }

    private void ON_TEXT_CHANGED(Object obj)
    {
      if (!Object.op_Equality(obj, (Object) this.m_TextMeshPro))
        return;
      this.m_cachedMeshInfoVertexData = ((TMP_Text) this.m_TextMeshPro).get_textInfo().CopyMeshInfoVertexData();
    }

    private void LateUpdate()
    {
      if (this.isHoveringObject)
      {
        int intersectingCharacter = TMP_TextUtilities.FindIntersectingCharacter((TMP_Text) this.m_TextMeshPro, Input.get_mousePosition(), this.m_Camera, true);
        if (intersectingCharacter == -1 || intersectingCharacter != this.m_lastIndex)
        {
          this.RestoreCachedVertexAttributes(this.m_lastIndex);
          this.m_lastIndex = -1;
        }
        if (intersectingCharacter != -1 && intersectingCharacter != this.m_lastIndex && (Input.GetKey((KeyCode) 304) || Input.GetKey((KeyCode) 303)))
        {
          this.m_lastIndex = intersectingCharacter;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          int materialReferenceIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[intersectingCharacter]).materialReferenceIndex;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          int vertexIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[intersectingCharacter]).vertexIndex;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          Vector3[] vertices = (Vector3[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).vertices;
          Vector3 vector3 = Vector2.op_Implicit(Vector2.op_Implicit(Vector3.op_Division(Vector3.op_Addition(vertices[vertexIndex], vertices[vertexIndex + 2]), 2f)));
          vertices[vertexIndex] = Vector3.op_Subtraction(vertices[vertexIndex], vector3);
          vertices[vertexIndex + 1] = Vector3.op_Subtraction(vertices[vertexIndex + 1], vector3);
          vertices[vertexIndex + 2] = Vector3.op_Subtraction(vertices[vertexIndex + 2], vector3);
          vertices[vertexIndex + 3] = Vector3.op_Subtraction(vertices[vertexIndex + 3], vector3);
          this.m_matrix = Matrix4x4.TRS(Vector3.get_zero(), Quaternion.get_identity(), Vector3.op_Multiply(Vector3.get_one(), 1.5f));
          vertices[vertexIndex] = ((Matrix4x4) ref this.m_matrix).MultiplyPoint3x4(vertices[vertexIndex]);
          vertices[vertexIndex + 1] = ((Matrix4x4) ref this.m_matrix).MultiplyPoint3x4(vertices[vertexIndex + 1]);
          vertices[vertexIndex + 2] = ((Matrix4x4) ref this.m_matrix).MultiplyPoint3x4(vertices[vertexIndex + 2]);
          vertices[vertexIndex + 3] = ((Matrix4x4) ref this.m_matrix).MultiplyPoint3x4(vertices[vertexIndex + 3]);
          vertices[vertexIndex] = Vector3.op_Addition(vertices[vertexIndex], vector3);
          vertices[vertexIndex + 1] = Vector3.op_Addition(vertices[vertexIndex + 1], vector3);
          vertices[vertexIndex + 2] = Vector3.op_Addition(vertices[vertexIndex + 2], vector3);
          vertices[vertexIndex + 3] = Vector3.op_Addition(vertices[vertexIndex + 3], vector3);
          Color32 color32;
          ((Color32) ref color32).\u002Ector(byte.MaxValue, byte.MaxValue, (byte) 192, byte.MaxValue);
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          Color32[] colors32 = (Color32[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).colors32;
          colors32[vertexIndex] = color32;
          colors32[vertexIndex + 1] = color32;
          colors32[vertexIndex + 2] = color32;
          colors32[vertexIndex + 3] = color32;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          TMP_MeshInfo tmpMeshInfo = ^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex];
          int num = vertices.Length - 4;
          ((TMP_MeshInfo) ref tmpMeshInfo).SwapVertexData(vertexIndex, num);
          ((TMP_Text) this.m_TextMeshPro).UpdateVertexData((TMP_VertexDataUpdateFlags) (int) byte.MaxValue);
        }
        int intersectingWord = TMP_TextUtilities.FindIntersectingWord((TMP_Text) this.m_TextMeshPro, Input.get_mousePosition(), this.m_Camera);
        if (Object.op_Inequality((Object) this.m_TextPopup_RectTransform, (Object) null) && this.m_selectedWord != -1 && (intersectingWord == -1 || intersectingWord != this.m_selectedWord))
        {
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          TMP_WordInfo tmpWordInfo = ^(TMP_WordInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().wordInfo[this.m_selectedWord];
          for (int index1 = 0; index1 < tmpWordInfo.characterCount; ++index1)
          {
            int index2 = tmpWordInfo.firstCharacterIndex + index1;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            int materialReferenceIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[index2]).materialReferenceIndex;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            int vertexIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[index2]).vertexIndex;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            Color32[] colors32 = (Color32[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).colors32;
            Color32 color32 = TMPro_ExtensionMethods.Tint(colors32[vertexIndex], 1.33333f);
            colors32[vertexIndex] = color32;
            colors32[vertexIndex + 1] = color32;
            colors32[vertexIndex + 2] = color32;
            colors32[vertexIndex + 3] = color32;
          }
          ((TMP_Text) this.m_TextMeshPro).UpdateVertexData((TMP_VertexDataUpdateFlags) (int) byte.MaxValue);
          this.m_selectedWord = -1;
        }
        if (intersectingWord != -1 && intersectingWord != this.m_selectedWord && (!Input.GetKey((KeyCode) 304) && !Input.GetKey((KeyCode) 303)))
        {
          this.m_selectedWord = intersectingWord;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          TMP_WordInfo tmpWordInfo = ^(TMP_WordInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().wordInfo[intersectingWord];
          for (int index1 = 0; index1 < tmpWordInfo.characterCount; ++index1)
          {
            int index2 = tmpWordInfo.firstCharacterIndex + index1;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            int materialReferenceIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[index2]).materialReferenceIndex;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            int vertexIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[index2]).vertexIndex;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            Color32[] colors32 = (Color32[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).colors32;
            Color32 color32 = TMPro_ExtensionMethods.Tint(colors32[vertexIndex], 0.75f);
            colors32[vertexIndex] = color32;
            colors32[vertexIndex + 1] = color32;
            colors32[vertexIndex + 2] = color32;
            colors32[vertexIndex + 3] = color32;
          }
          ((TMP_Text) this.m_TextMeshPro).UpdateVertexData((TMP_VertexDataUpdateFlags) (int) byte.MaxValue);
        }
        int intersectingLink = TMP_TextUtilities.FindIntersectingLink((TMP_Text) this.m_TextMeshPro, Input.get_mousePosition(), this.m_Camera);
        if (intersectingLink == -1 && this.m_selectedLink != -1 || intersectingLink != this.m_selectedLink)
        {
          ((Component) this.m_TextPopup_RectTransform).get_gameObject().SetActive(false);
          this.m_selectedLink = -1;
        }
        if (intersectingLink == -1 || intersectingLink == this.m_selectedLink)
          return;
        this.m_selectedLink = intersectingLink;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        TMP_LinkInfo tmpLinkInfo = ^(TMP_LinkInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().linkInfo[intersectingLink];
        Vector3 zero = Vector3.get_zero();
        RectTransformUtility.ScreenPointToWorldPointInRectangle(((TMP_Text) this.m_TextMeshPro).get_rectTransform(), Vector2.op_Implicit(Input.get_mousePosition()), this.m_Camera, ref zero);
        switch (((TMP_LinkInfo) ref tmpLinkInfo).GetLinkID())
        {
          case "id_01":
            ((Transform) this.m_TextPopup_RectTransform).set_position(zero);
            ((Component) this.m_TextPopup_RectTransform).get_gameObject().SetActive(true);
            ((TMP_Text) this.m_TextPopup_TMPComponent).set_text("You have selected link <#ffff00> ID 01");
            break;
          case "id_02":
            ((Transform) this.m_TextPopup_RectTransform).set_position(zero);
            ((Component) this.m_TextPopup_RectTransform).get_gameObject().SetActive(true);
            ((TMP_Text) this.m_TextPopup_TMPComponent).set_text("You have selected link <#ffff00> ID 02");
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
      if (index == -1 || index > ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterCount - 1)
        return;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      int materialReferenceIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[index]).materialReferenceIndex;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      int vertexIndex = (int) (^(TMP_CharacterInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().characterInfo[index]).vertexIndex;
      Vector3[] vertices1 = (Vector3[]) this.m_cachedMeshInfoVertexData[materialReferenceIndex].vertices;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Vector3[] vertices2 = (Vector3[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).vertices;
      vertices2[vertexIndex] = vertices1[vertexIndex];
      vertices2[vertexIndex + 1] = vertices1[vertexIndex + 1];
      vertices2[vertexIndex + 2] = vertices1[vertexIndex + 2];
      vertices2[vertexIndex + 3] = vertices1[vertexIndex + 3];
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Color32[] colors32_1 = (Color32[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).colors32;
      Color32[] colors32_2 = (Color32[]) this.m_cachedMeshInfoVertexData[materialReferenceIndex].colors32;
      colors32_1[vertexIndex] = colors32_2[vertexIndex];
      colors32_1[vertexIndex + 1] = colors32_2[vertexIndex + 1];
      colors32_1[vertexIndex + 2] = colors32_2[vertexIndex + 2];
      colors32_1[vertexIndex + 3] = colors32_2[vertexIndex + 3];
      Vector2[] uvs0_1 = (Vector2[]) this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Vector2[] uvs0_2 = (Vector2[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).uvs0;
      uvs0_2[vertexIndex] = uvs0_1[vertexIndex];
      uvs0_2[vertexIndex + 1] = uvs0_1[vertexIndex + 1];
      uvs0_2[vertexIndex + 2] = uvs0_1[vertexIndex + 2];
      uvs0_2[vertexIndex + 3] = uvs0_1[vertexIndex + 3];
      Vector2[] uvs2_1 = (Vector2[]) this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs2;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Vector2[] uvs2_2 = (Vector2[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).uvs2;
      uvs2_2[vertexIndex] = uvs2_1[vertexIndex];
      uvs2_2[vertexIndex + 1] = uvs2_1[vertexIndex + 1];
      uvs2_2[vertexIndex + 2] = uvs2_1[vertexIndex + 2];
      uvs2_2[vertexIndex + 3] = uvs2_1[vertexIndex + 3];
      int index1 = (vertices1.Length / 4 - 1) * 4;
      vertices2[index1] = vertices1[index1];
      vertices2[index1 + 1] = vertices1[index1 + 1];
      vertices2[index1 + 2] = vertices1[index1 + 2];
      vertices2[index1 + 3] = vertices1[index1 + 3];
      Color32[] colors32_3 = (Color32[]) this.m_cachedMeshInfoVertexData[materialReferenceIndex].colors32;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Color32[] colors32_4 = (Color32[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).colors32;
      colors32_4[index1] = colors32_3[index1];
      colors32_4[index1 + 1] = colors32_3[index1 + 1];
      colors32_4[index1 + 2] = colors32_3[index1 + 2];
      colors32_4[index1 + 3] = colors32_3[index1 + 3];
      Vector2[] uvs0_3 = (Vector2[]) this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Vector2[] uvs0_4 = (Vector2[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).uvs0;
      uvs0_4[index1] = uvs0_3[index1];
      uvs0_4[index1 + 1] = uvs0_3[index1 + 1];
      uvs0_4[index1 + 2] = uvs0_3[index1 + 2];
      uvs0_4[index1 + 3] = uvs0_3[index1 + 3];
      Vector2[] uvs2_3 = (Vector2[]) this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs2;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      Vector2[] uvs2_4 = (Vector2[]) (^(TMP_MeshInfo&) ref ((TMP_Text) this.m_TextMeshPro).get_textInfo().meshInfo[materialReferenceIndex]).uvs2;
      uvs2_4[index1] = uvs2_3[index1];
      uvs2_4[index1 + 1] = uvs2_3[index1 + 1];
      uvs2_4[index1 + 2] = uvs2_3[index1 + 2];
      uvs2_4[index1 + 3] = uvs2_3[index1 + 3];
      ((TMP_Text) this.m_TextMeshPro).UpdateVertexData((TMP_VertexDataUpdateFlags) (int) byte.MaxValue);
    }
  }
}
