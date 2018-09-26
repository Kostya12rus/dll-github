// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.PropertyButton
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
  public class PropertyButton : MonoBehaviour
  {
    private PropertyButton[] _otherbuttons;
    private Outline _outline;
    private Color _color;
    public int argumentId;
    public string value;

    private void Start()
    {
      this._color = this.GetComponentInParent<SubmenuSelector>().c_selected;
      this._otherbuttons = this.transform.parent.GetComponentsInChildren<PropertyButton>(true);
    }

    public void Click()
    {
      foreach (PropertyButton otherbutton in this._otherbuttons)
        otherbutton.SetStatus(false);
      this.SetStatus(true);
    }

    private void OnEnable()
    {
      this.SetStatus(false);
    }

    private void SetStatus(bool b)
    {
      if ((Object) this._outline == (Object) null)
        this._outline = this.GetComponent<Outline>();
      this._outline.effectColor = !b ? Color.white : this._color;
      if (!b)
        return;
      this.GetComponentInParent<SubmenuSelector>().SetProperty(this.argumentId, this.value);
    }
  }
}
