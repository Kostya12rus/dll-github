// Decompiled with JetBrains decompiler
// Type: shooter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class shooter : MonoBehaviour
{
  public int mtpl = 5;

  private void Start()
  {
  }

  private void Update()
  {
    if (!Input.GetKeyDown(KeyCode.Return))
      return;
    ScreenCapture.CaptureScreenshot("Taken" + (object) Random.Range(0, 1000) + ".png", this.mtpl);
  }
}
