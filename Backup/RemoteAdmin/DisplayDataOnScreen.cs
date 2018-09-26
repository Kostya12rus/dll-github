// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.DisplayDataOnScreen
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
  public class DisplayDataOnScreen : MonoBehaviour
  {
    public static DisplayDataOnScreen singleton;

    public DisplayDataOnScreen()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      DisplayDataOnScreen.singleton = this;
    }

    public void Show(Text text, string content)
    {
      text.set_text(content);
    }

    public void Show(TextMeshProUGUI text, string content)
    {
      ((TMP_Text) text).set_text(content);
    }

    public void Show(int menuId, string content)
    {
      ((TMP_Text) ((SubmenuSelector) ((Component) this).GetComponent<SubmenuSelector>()).menus[menuId].optionalDisplay).set_text(content);
    }
  }
}
