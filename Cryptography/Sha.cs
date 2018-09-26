// Decompiled with JetBrains decompiler
// Type: Cryptography.Sha
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Security.Cryptography;
using System.Text;

namespace Cryptography
{
  public class Sha
  {
    public static byte[] Sha256(byte[] message)
    {
      return SHA256.Create().ComputeHash(message);
    }

    public static byte[] Sha256(string message)
    {
      return Sha.Sha256(Utf8.GetBytes(message));
    }

    public static byte[] Sha256Hmac(byte[] key, byte[] message)
    {
      return new HMACSHA256(key).ComputeHash(message);
    }

    public static byte[] Sha512(string message)
    {
      return Sha.Sha512(Utf8.GetBytes(message));
    }

    public static byte[] Sha512(byte[] message)
    {
      return SHA512.Create().ComputeHash(message);
    }

    public static byte[] Sha512Hmac(byte[] key, byte[] message)
    {
      return new HMACSHA512(key).ComputeHash(message);
    }

    public static string HashToString(byte[] hash)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in hash)
        stringBuilder.Append(num.ToString("X2"));
      return stringBuilder.ToString();
    }
  }
}
