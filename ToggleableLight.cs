// Decompiled with JetBrains decompiler
// Type: ToggleableLight
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class ToggleableLight : MonoBehaviour
{
  public GameObject[] allLights;
  public bool isAlarm;

  public ToggleableLight()
  {
    base.\u002Ector();
  }

  public void SetLights(bool b)
  {
    foreach (GameObject allLight in this.allLights)
      allLight.SetActive(!this.isAlarm ? !b : b);
  }
}
