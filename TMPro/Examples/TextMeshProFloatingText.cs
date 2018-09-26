// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TextMeshProFloatingText
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro.Examples
{
  public class TextMeshProFloatingText : MonoBehaviour
  {
    public Font TheFont;
    private GameObject m_floatingText;
    private TextMeshPro m_textMeshPro;
    private TextMesh m_textMesh;
    private Transform m_transform;
    private Transform m_floatingText_Transform;
    private Transform m_cameraTransform;
    private Vector3 lastPOS;
    private Quaternion lastRotation;
    public int SpawnType;

    public TextMeshProFloatingText()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this.m_transform = ((Component) this).get_transform();
      this.m_floatingText = new GameObject(((Object) this).get_name() + " floating text");
      this.m_cameraTransform = ((Component) Camera.get_main()).get_transform();
    }

    private void Start()
    {
      if (this.SpawnType == 0)
      {
        this.m_textMeshPro = (TextMeshPro) this.m_floatingText.AddComponent<TextMeshPro>();
        ((TMP_Text) this.m_textMeshPro).get_rectTransform().set_sizeDelta(new Vector2(3f, 3f));
        this.m_floatingText_Transform = this.m_floatingText.get_transform();
        this.m_floatingText_Transform.set_position(Vector3.op_Addition(this.m_transform.get_position(), new Vector3(0.0f, 15f, 0.0f)));
        ((TMP_Text) this.m_textMeshPro).set_alignment((TextAlignmentOptions) 514);
        ((Graphic) this.m_textMeshPro).set_color(Color32.op_Implicit(new Color32((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), byte.MaxValue)));
        ((TMP_Text) this.m_textMeshPro).set_fontSize(24f);
        ((TMP_Text) this.m_textMeshPro).set_enableKerning(false);
        ((TMP_Text) this.m_textMeshPro).set_text(string.Empty);
        this.StartCoroutine(this.DisplayTextMeshProFloatingText());
      }
      else if (this.SpawnType == 1)
      {
        this.m_floatingText_Transform = this.m_floatingText.get_transform();
        this.m_floatingText_Transform.set_position(Vector3.op_Addition(this.m_transform.get_position(), new Vector3(0.0f, 15f, 0.0f)));
        this.m_textMesh = (TextMesh) this.m_floatingText.AddComponent<TextMesh>();
        this.m_textMesh.set_font((Font) Resources.Load<Font>("Fonts/ARIAL"));
        ((Renderer) ((Component) this.m_textMesh).GetComponent<Renderer>()).set_sharedMaterial(this.m_textMesh.get_font().get_material());
        this.m_textMesh.set_color(Color32.op_Implicit(new Color32((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), byte.MaxValue)));
        this.m_textMesh.set_anchor((TextAnchor) 7);
        this.m_textMesh.set_fontSize(24);
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
