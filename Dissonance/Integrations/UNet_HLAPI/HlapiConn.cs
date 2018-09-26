// Decompiled with JetBrains decompiler
// Type: Dissonance.Integrations.UNet_HLAPI.HlapiConn
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
  public struct HlapiConn : IEquatable<HlapiConn>
  {
    public readonly NetworkConnection Connection;

    public HlapiConn(NetworkConnection connection)
    {
      this = new HlapiConn();
      this.Connection = connection;
    }

    public override int GetHashCode()
    {
      return this.Connection.GetHashCode();
    }

    public override string ToString()
    {
      return this.Connection.ToString();
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj) || !(obj is HlapiConn))
        return false;
      return this.Equals((HlapiConn) obj);
    }

    public bool Equals(HlapiConn other)
    {
      if (this.Connection != null)
        return this.Connection.Equals((object) other.Connection);
      return other.Connection == null;
    }
  }
}
