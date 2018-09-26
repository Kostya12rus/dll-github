// Decompiled with JetBrains decompiler
// Type: InterfaceColorAdjuster
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class InterfaceColorAdjuster : MonoBehaviour
{
  public Graphic[] graphicsToChange;

  public void ChangeColor(Color color)
  {
    foreach (Graphic graphic in this.graphicsToChange)
    {
      if ((Object) graphic != (Object) null)
      {
        Color color1 = new Color(color.r, color.g, color.b, graphic.color.a);
        graphic.color = color1;
      }
    }
  }
}
