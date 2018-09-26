// Decompiled with JetBrains decompiler
// Type: TMPro.TMP_PhoneNumberValidator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace TMPro
{
  [Serializable]
  public class TMP_PhoneNumberValidator : TMP_InputValidator
  {
    public TMP_PhoneNumberValidator()
    {
      base.\u002Ector();
    }

    public virtual char Validate(ref string text, ref int pos, char ch)
    {
      Debug.Log((object) "Trying to validate...");
      if (ch < '0' && ch > '9')
        return char.MinValue;
      int length = text.Length;
      for (int index = 0; index < length + 1; ++index)
      {
        switch (index)
        {
          case 0:
            if (index == length)
              text = "(" + (object) ch;
            pos = 2;
            break;
          case 1:
            if (index == length)
              text += (string) (object) ch;
            pos = 2;
            break;
          case 2:
            if (index == length)
              text += (string) (object) ch;
            pos = 3;
            break;
          case 3:
            if (index == length)
              text = text + (object) ch + ") ";
            pos = 6;
            break;
          case 4:
            if (index == length)
              text = text + ") " + (object) ch;
            pos = 7;
            break;
          case 5:
            if (index == length)
              text = text + " " + (object) ch;
            pos = 7;
            break;
          case 6:
            if (index == length)
              text += (string) (object) ch;
            pos = 7;
            break;
          case 7:
            if (index == length)
              text += (string) (object) ch;
            pos = 8;
            break;
          case 8:
            if (index == length)
              text = text + (object) ch + "-";
            pos = 10;
            break;
          case 9:
            if (index == length)
              text = text + "-" + (object) ch;
            pos = 11;
            break;
          case 10:
            if (index == length)
              text += (string) (object) ch;
            pos = 11;
            break;
          case 11:
            if (index == length)
              text += (string) (object) ch;
            pos = 12;
            break;
          case 12:
            if (index == length)
              text += (string) (object) ch;
            pos = 13;
            break;
          case 13:
            if (index == length)
              text += (string) (object) ch;
            pos = 14;
            break;
        }
      }
      return ch;
    }
  }
}
