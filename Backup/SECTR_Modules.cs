// Decompiled with JetBrains decompiler
// Type: SECTR_Modules
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

public static class SECTR_Modules
{
  public static string VERSION = "1.3.6";
  public static bool AUDIO = Type.GetType("SECTR_AudioSystem") != null;
  public static bool VIS = Type.GetType("SECTR_CullingCamera") != null;
  public static bool STREAM = Type.GetType("SECTR_Chunk") != null;
  public static bool DEV = Type.GetType("SECTR_Tests") != null;

  public static bool HasPro()
  {
    return true;
  }

  public static bool HasComplete()
  {
    if (SECTR_Modules.AUDIO && SECTR_Modules.VIS)
      return SECTR_Modules.STREAM;
    return false;
  }
}
