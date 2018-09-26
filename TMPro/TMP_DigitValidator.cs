// Decompiled with JetBrains decompiler
// Type: TMPro.TMP_DigitValidator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace TMPro
{
  [Serializable]
  public class TMP_DigitValidator : TMP_InputValidator
  {
    public override char Validate(ref string text, ref int pos, char ch)
    {
      if (ch < '0' || ch > '9')
        return char.MinValue;
      ++pos;
      return ch;
    }
  }
}
