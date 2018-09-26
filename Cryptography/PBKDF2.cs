// Decompiled with JetBrains decompiler
// Type: Cryptography.PBKDF2
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Security.Cryptography;

namespace Cryptography
{
  public class PBKDF2
  {
    public static string Pbkdf2HashString(string password, byte[] salt, int iterations, int outputBytes)
    {
      return Convert.ToBase64String(PBKDF2.Pbkdf2HashBytes(password, salt, iterations, outputBytes));
    }

    public static byte[] Pbkdf2HashBytes(string password, byte[] salt, int iterations, int outputBytes)
    {
      return new Rfc2898DeriveBytes(password, salt) { IterationCount = iterations }.GetBytes(outputBytes);
    }
  }
}
