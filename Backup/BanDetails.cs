// Decompiled with JetBrains decompiler
// Type: BanDetails
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

public class BanDetails
{
  public string OriginalName;
  public string Id;
  public long Expires;
  public string Reason;
  public string Issuer;
  public long IssuanceTime;

  public override string ToString()
  {
    return string.Format("{0};{1};{2};{3};{4};{5}", (object) this.OriginalName.Replace(";", ":"), (object) this.Id.Replace(";", ":"), (object) Convert.ToString(this.Expires), (object) this.Reason.Replace(";", ":"), (object) this.Issuer.Replace(";", ":"), (object) Convert.ToString(this.IssuanceTime));
  }
}
