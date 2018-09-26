// Decompiled with JetBrains decompiler
// Type: UserGroup
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

[Serializable]
public class UserGroup
{
  public string BadgeColor;
  public string BadgeText;
  public ulong Permissions;

  public UserGroup Clone()
  {
    return new UserGroup() { BadgeColor = this.BadgeColor, BadgeText = this.BadgeText, Permissions = this.Permissions };
  }
}
