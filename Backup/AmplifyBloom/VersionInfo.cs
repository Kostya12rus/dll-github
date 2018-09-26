// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.VersionInfo
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class VersionInfo
  {
    private static string StageSuffix = "_dev001";
    public const byte Major = 1;
    public const byte Minor = 1;
    public const byte Release = 2;
    [SerializeField]
    private int m_major;
    [SerializeField]
    private int m_minor;
    [SerializeField]
    private int m_release;

    private VersionInfo()
    {
      this.m_major = 1;
      this.m_minor = 1;
      this.m_release = 2;
    }

    private VersionInfo(byte major, byte minor, byte release)
    {
      this.m_major = (int) major;
      this.m_minor = (int) minor;
      this.m_release = (int) release;
    }

    public static string StaticToString()
    {
      return string.Format("{0}.{1}.{2}", (object) (byte) 1, (object) (byte) 1, (object) (byte) 2) + VersionInfo.StageSuffix;
    }

    public override string ToString()
    {
      return string.Format("{0}.{1}.{2}", (object) this.m_major, (object) this.m_minor, (object) this.m_release) + VersionInfo.StageSuffix;
    }

    public int Number
    {
      get
      {
        return this.m_major * 100 + this.m_minor * 10 + this.m_release;
      }
    }

    public static VersionInfo Current()
    {
      return new VersionInfo((byte) 1, (byte) 1, (byte) 2);
    }

    public static bool Matches(VersionInfo version)
    {
      if (version.m_major == 1 && version.m_minor == 1)
        return 2 == version.m_release;
      return false;
    }
  }
}
