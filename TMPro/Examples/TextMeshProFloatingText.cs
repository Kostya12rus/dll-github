// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TextMeshProFloatingText
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class TextMeshProFloatingText : MonoBehaviour
  {
    private Vector3 lastPOS = Vector3.zero;
    private Quaternion lastRotation = Quaternion.identity;
    public Font TheFont;
    private GameObject m_floatingText;
    private TextMeshPro m_textMeshPro;
    private TextMesh m_textMesh;
    private Transform m_transform;
    private Transform m_floatingText_Transform;
    private Transform m_cameraTransform;
    public int SpawnType;

    private void Awake()
    {
      this.m_transform = this.transform;
      this.m_floatingText = new GameObject(this.name + " floating text");
      this.m_cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
      if (this.SpawnType == 0)
      {
        this.m_textMeshPro = this.m_floatingText.AddComponent<TextMeshPro>();
        this.m_textMeshPro.rectTransform.sizeDelta = new Vector2(3f, 3f);
        this.m_floatingText_Transform = this.m_floatingText.transform;
        this.m_floatingText_Transform.position = this.m_transform.position + new Vector3(0.0f, 15f, 0.0f);
        this.m_textMeshPro.alignment = TextAlignmentOptions.Center;
        this.m_textMeshPro.color = (Color) new Color32((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), byte.MaxValue);
        this.m_textMeshPro.fontSize = 24f;
        this.m_textMeshPro.enableKerning = false;
        this.m_textMeshPro.text = string.Empty;
        this.StartCoroutine(this.DisplayTextMeshProFloatingText());
      }
      else if (this.SpawnType == 1)
      {
        this.m_floatingText_Transform = this.m_floatingText.transform;
        this.m_floatingText_Transform.position = this.m_transform.position + new Vector3(0.0f, 15f, 0.0f);
        this.m_textMesh = this.m_floatingText.AddComponent<TextMesh>();
        this.m_textMesh.font = Resources.Load<Font>("Fonts/ARIAL");
        this.m_textMesh.GetComponent<Renderer>().sharedMaterial = this.m_textMesh.font.material;
        this.m_textMesh.color = (Color) new Color32((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), byte.MaxValue);
        this.m_textMesh.anchor = TextAnchor.LowerCenter;
        this.m_textMesh.fontSize = 24;
        this.StartCoroutine(this.DisplayTextMeshFloatingText());
      }
      else if (this.SpawnType == 2)
        ;
    }

    [DebuggerHidden]
    public IEnumerator DisplayTextMeshProFloatingText()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TextMeshProFloatingText.\u003CDisplayTextMeshProFloatingText\u003Ec__Iterator0() { \u0024this = this };
    }

    [DebuggerHidden]
    public IEnumerator DisplayTextMeshFloatingText()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TextMeshProFloatingText.\u003CDisplayTextMeshFloatingText\u003Ec__Iterator1() { \u0024this = this };
    }
  }
}
