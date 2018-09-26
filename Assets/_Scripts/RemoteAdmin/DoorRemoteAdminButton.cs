// Decompiled with JetBrains decompiler
// Type: Assets._Scripts.RemoteAdmin.DoorRemoteAdminButton
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using RemoteAdmin;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.RemoteAdmin
{
  internal class DoorRemoteAdminButton : MonoBehaviour
  {
    public Door Door;
    public string OvrValue;
    private Outline _outline;
    public static Color Color;
    public static DoorRemoteAdminButton[] Buttons;

    public void Click()
    {
      foreach (DoorRemoteAdminButton button in DoorRemoteAdminButton.Buttons)
        button.SetStatus(false);
      this.SetStatus(true);
    }

    private void OnEnable()
    {
      this.SetStatus(false);
    }

    public void SetStatus(bool b)
    {
      if ((Object) this._outline == (Object) null)
        this._outline = this.GetComponent<Outline>();
      this._outline.effectColor = !b ? Color.white : DoorRemoteAdminButton.Color;
      if (!b)
        return;
      DoorPrinter.SelectedDoors = !((Object) this.Door != (Object) null) ? this.OvrValue : this.Door.DoorName;
    }
  }
}
