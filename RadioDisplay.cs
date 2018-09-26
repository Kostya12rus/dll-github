// Decompiled with JetBrains decompiler
// Type: RadioDisplay
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class RadioDisplay : MonoBehaviour
{
  public Text label_t;
  public Text power_t;
  public Text battery_t;
  public static string label;
  public static string power;
  public static string battery;
  public GameObject normal;
  public GameObject nobattery;
  public Texture batt1;
  public Texture batt2;
  public RawImage img;

  private void Start()
  {
    this.InvokeRepeating("ChangeImg", 1f, 0.5f);
  }

  private void Update()
  {
    this.normal.SetActive(RadioDisplay.battery != "0");
    this.nobattery.SetActive(RadioDisplay.battery == "0");
    this.label_t.text = RadioDisplay.label;
    this.power_t.text = RadioDisplay.power;
    this.battery_t.text = "BAT. " + RadioDisplay.battery + "%";
  }

  private void ChangeImg()
  {
    this.img.texture = !((Object) this.img.texture == (Object) this.batt1) ? this.batt1 : this.batt2;
  }
}
