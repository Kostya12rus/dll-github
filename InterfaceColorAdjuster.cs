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

  public InterfaceColorAdjuster()
  {
    base.\u002Ector();
  }

  public void ChangeColor(Color color)
  {
    foreach (Graphic graphic in this.graphicsToChange)
    {
      if (Object.op_Inequality((Object) graphic, (Object) null))
      {
        Color color1;
        ((Color) ref color1).\u002Ector((float) color.r, (float) color.g, (float) color.b, (float) graphic.get_color().a);
        graphic.set_color(color1);
      }
    }
  }
}
