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

  private void OnEnable()
  {
    if (QualitySettings.vSyncCount != 0)
      this.warning.SetActive(true);
    else
      this.warning.SetActive(false);
    int num = PlayerPrefs.GetInt("MaxFramerate", 969);
    Application.targetFrameRate = num != 969 ? num : -1;
    if (Application.targetFrameRate == -1)
    {
      this.gameObject.GetComponent<Dropdown>().value = 0;
    }
    else
    {
      bool flag = false;
      for (int index = 1; index < this.gameObject.GetComponent<Dropdown>().options.Count; ++index)
      {
        int result = 0;
        if (!flag && int.TryParse(this.gameObject.GetComponent<Dropdown>().options[index].text, out result) && result == Application.targetFrameRate)
        {
          this.gameObject.GetComponent<Dropdown>().value = index;
          flag = true;
        }
      }
      if (flag)
        return;
      this.gameObject.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(Application.targetFrameRate.ToString()));
      this.gameObject.GetComponent<Dropdown>().RefreshShownValue();
      this.gameObject.GetComponent<Dropdown>().value = this.gameObject.GetComponent<Dropdown>().options.Count - 1;
    }
  }

  public void OnValueChange()
  {
    this.ChangeLimit(this.gameObject.GetComponent<Dropdown>().options[this.gameObject.GetComponent<Dropdown>().value].text);
  }

  private void ChangeLimit(string limit)
  {
    int result;
    if (int.TryParse(this.gameObject.GetComponent<Dropdown>().options[this.gameObject.GetComponent<Dropdown>().value].text, out result))
    {
      Application.targetFrameRate = Mathf.Clamp(result, 15, 999);
      PlayerPrefs.SetInt("MaxFramerate", Mathf.Clamp(result, 15, 999));
    }
    else
    {
      Application.targetFrameRate = -1;
      PlayerPrefs.SetInt("MaxFramerate", 969);
    }
  }
}
