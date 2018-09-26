// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoUISlider
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
  public sealed class DemoUISlider : DemoUIElement
  {
    public bool SingleStep;
    private Slider m_slider;
    private bool m_lastStep;

    private void Start()
    {
      this.m_slider = (Slider) ((Component) this).GetComponent<Slider>();
    }

    public override void DoAction(DemoUIElementAction action, object[] vars)
    {
      if (!((Selectable) this.m_slider).IsInteractable() || action != DemoUIElementAction.Slide)
        return;
      float var = (float) vars[0];
      if (this.SingleStep)
      {
        if (this.m_lastStep)
          return;
        this.m_lastStep = true;
      }
      if (this.m_slider.get_wholeNumbers())
      {
        if ((double) var > 0.0)
        {
          Slider slider = this.m_slider;
          slider.set_value(slider.get_value() + 1f);
        }
        else
        {
          if ((double) var >= 0.0)
            return;
          Slider slider = this.m_slider;
          slider.set_value(slider.get_value() - 1f);
        }
      }
      else
      {
        Slider slider = this.m_slider;
        slider.set_value(slider.get_value() + var);
      }
    }

    public override void Idle()
    {
      this.m_lastStep = false;
    }
  }
}
