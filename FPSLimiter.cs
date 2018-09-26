// Decompiled with JetBrains decompiler
// Type: FPSLimiter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class FPSLimiter : MonoBehaviour
{
  public GameObject warning;

  public FPSLimiter()
  {
    base.\u002Ector();
  }

  private void OnEnable()
  {
    if (QualitySettings.get_vSyncCount() != 0)
      this.warning.SetActive(true);
    else
      this.warning.SetActive(false);
    int num = PlayerPrefs.GetInt("MaxFramerate", 969);
    if (num == 969)
      Application.set_targetFrameRate(-1);
    else
      Application.set_targetFrameRate(num);
    if (Application.get_targetFrameRate() == -1)
    {
      ((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).set_value(0);
    }
    else
    {
      bool flag = false;
      for (int index = 1; index < ((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).get_options().Count; ++index)
      {
        int result = 0;
        if (!flag && int.TryParse(((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).get_options()[index].get_text(), out result) && result == Application.get_targetFrameRate())
        {
          ((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).set_value(index);
          flag = true;
        }
      }
      if (flag)
        return;
      ((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).get_options().Add(new Dropdown.OptionData(Application.get_targetFrameRate().ToString()));
      ((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).RefreshShownValue();
      ((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).set_value(((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).get_options().Count - 1);
    }
  }

  public void OnValueChange()
  {
    this.ChangeLimit(((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).get_options()[((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).get_value()].get_text());
  }

  private void ChangeLimit(string limit)
  {
    int result;
    if (int.TryParse(((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).get_options()[((Dropdown) ((Component) this).get_gameObject().GetComponent<Dropdown>()).get_value()].get_text(), out result))
    {
      Application.set_targetFrameRate(Mathf.Clamp(result, 15, 999));
      PlayerPrefs.SetInt("MaxFramerate", Mathf.Clamp(result, 15, 999));
    }
    else
    {
      Application.set_targetFrameRate(-1);
      PlayerPrefs.SetInt("MaxFramerate", 969);
    }
  }
}
