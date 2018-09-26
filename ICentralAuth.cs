// Decompiled with JetBrains decompiler
// Type: ICentralAuth
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

public interface ICentralAuth
{
  void TokenGenerated(string token);

  void RequestBadge(string token);

  void Fail();

  CharacterClassManager GetCcm();

  void Ok(string steamId, string nickname, string ban, string steamban, string server, bool bypass);

  void FailToken(string reason);
}
