// Decompiled with JetBrains decompiler
// Type: Utf8
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Text;

public class Utf8
{
  public static byte[] GetBytes(string data)
  {
    return Encoding.UTF8.GetBytes(data);
  }

  public static string GetString(byte[] data)
  {
    return Encoding.UTF8.GetString(data);
  }
}
