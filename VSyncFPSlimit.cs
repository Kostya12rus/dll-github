// Decompiled with JetBrains decompiler
// Type: VSyncFPSlimit
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class VSyncFPSlimit : MonoBehaviour
{
  public VSyncFPSlimit()
  {
    base.\u002Ector();
  }

  public void Check()
  {
    if ((double) ((Slider) ((Component) this).get_gameObject().GetComponent<Slider>()).get_value() != 0.0)
      return;
    int num = PlayerPrefs.GetInt("MaxFramerate", 969);
    if (num == 969)
      Application.set_targetFrameRate(-1);
    else
      Application.set_targetFrameRate(num);
  }
}
