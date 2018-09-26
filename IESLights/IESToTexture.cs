// Decompiled with JetBrains decompiler
// Type: IESLights.IESToTexture
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace IESLights
{
  public class IESToTexture : MonoBehaviour
  {
    public IESToTexture()
    {
      base.\u002Ector();
    }

    public static Texture2D ConvertIesData(IESData data)
    {
      Texture2D texture2D1 = new Texture2D(data.NormalizedValues.Count, data.NormalizedValues[0].Count, (TextureFormat) 20, false, true);
      ((Texture) texture2D1).set_filterMode((FilterMode) 2);
      ((Texture) texture2D1).set_wrapMode((TextureWrapMode) 1);
      Texture2D texture2D2 = texture2D1;
      Color[] colorArray = new Color[((Texture) texture2D2).get_width() * ((Texture) texture2D2).get_height()];
      for (int index1 = 0; index1 < ((Texture) texture2D2).get_width(); ++index1)
      {
        for (int index2 = 0; index2 < ((Texture) texture2D2).get_height(); ++index2)
        {
          float num = data.NormalizedValues[index1][index2];
          colorArray[index1 + index2 * ((Texture) texture2D2).get_width()] = new Color(num, num, num, num);
        }
      }
      texture2D2.SetPixels(colorArray);
      texture2D2.Apply();
      return texture2D2;
    }
  }
}
