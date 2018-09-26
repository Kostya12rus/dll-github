// Decompiled with JetBrains decompiler
// Type: MessageExpiredException
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

public class MessageExpiredException : Exception
{
  public MessageExpiredException()
  {
  }

  public MessageExpiredException(string message)
    : base(message)
  {
  }

  public MessageExpiredException(string message, Exception inner)
    : base(message, inner)
  {
  }
}
