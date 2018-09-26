// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoUIToggle
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
  public sealed class DemoUIToggle : DemoUIElement
  {
    private Toggle m_toggle;

    private void Start()
    {
      this.m_toggle = (Toggle) ((Component) this).GetComponent<Toggle>();
    }

    public override void DoAction(DemoUIElementAction action, object[] vars)
    {
      if (!((Selectable) this.m_toggle).IsInteractable() || action != DemoUIElementAction.Press)
        return;
      this.m_toggle.set_isOn(!this.m_toggle.get_isOn());
    }
  }
}
