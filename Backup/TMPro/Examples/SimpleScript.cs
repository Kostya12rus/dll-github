// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.SimpleScript
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class SimpleScript : MonoBehaviour
  {
    private TextMeshPro m_textMeshPro;
    private const string label = "The <#0050FF>count is: </color>{0:2}";
    private float m_frame;

    public SimpleScript()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      this.m_textMeshPro = (TextMeshPro) ((Component) this).get_gameObject().AddComponent<TextMeshPro>();
      ((TMP_Text) this.m_textMeshPro).set_autoSizeTextContainer(true);
      ((TMP_Text) this.m_textMeshPro).set_fontSize(48f);
      ((TMP_Text) this.m_textMeshPro).set_alignment((TextAlignmentOptions) 514);
      ((TMP_Text) this.m_textMeshPro).set_enableWordWrapping(false);
    }

    private void Update()
    {
      ((TMP_Text) this.m_textMeshPro).SetText("The <#0050FF>count is: </color>{0:2}", this.m_frame % 1000f);
      this.m_frame += 1f * Time.get_deltaTime();
    }
  }
}
