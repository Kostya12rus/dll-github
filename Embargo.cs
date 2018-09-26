// Decompiled with JetBrains decompiler
// Type: Embargo
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class Embargo : MonoBehaviour
{
  private Text txt;
  public bool showEmbargo;

  public Embargo()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.txt = (Text) ((Component) this).GetComponent<Text>();
    this.InvokeRepeating("ChangePosition", 3f, 3f);
  }

  private void ChangePosition()
  {
    ((Transform) ((Component) this).GetComponent<RectTransform>()).set_localPosition(new Vector3((float) Random.Range(-500, 500), (float) Random.Range(-250, 280), 0.0f));
  }

  private void Update()
  {
    if (this.showEmbargo)
    {
      DateTime now = DateTime.Now;
      this.txt.set_text("<size=30><color=#a11>EMBARGO</color></size>\n\n" + (object) now.Day + "." + (object) now.Month + "." + (object) now.Year + " " + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00") + "\n" + SystemInfo.get_operatingSystem() + "\n" + SystemInfo.get_deviceName() + "\n<size=18><color=#a11>DO NOT SHARE</color></size>");
    }
    else
      this.txt.set_text(string.Empty);
  }
}
