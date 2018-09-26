// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoUIElement
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
  public class DemoUIElement : MonoBehaviour
  {
    private bool m_isSelected;
    private Text m_text;
    private Color m_selectedColor;
    private Color m_unselectedColor;

    public DemoUIElement()
    {
      base.\u002Ector();
    }

    public void Init()
    {
      this.m_text = (Text) ((Component) ((Component) this).get_transform()).GetComponentInChildren<Text>();
      this.m_unselectedColor = ((Graphic) this.m_text).get_color();
    }

    public virtual void DoAction(DemoUIElementAction action, params object[] vars)
    {
    }

    public virtual void Idle()
    {
    }

    public bool Select
    {
      get
      {
        return this.m_isSelected;
      }
      set
      {
        this.m_isSelected = value;
        ((Graphic) this.m_text).set_color(!value ? this.m_unselectedColor : this.m_selectedColor);
      }
    }
  }
}
