// Decompiled with JetBrains decompiler
// Type: PlayerPermissions
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

public enum PlayerPermissions : ulong
{
  KickingAndShortTermBanning = 1,
  BanningUpToDay = 2,
  LongTermBanning = 4,
  ForceclassSelf = 8,
  ForceclassToSpectator = 16, // 0x0000000000000010
  ForceclassWithoutRestrictions = 32, // 0x0000000000000020
  GivingItems = 64, // 0x0000000000000040
  WarheadEvents = 128, // 0x0000000000000080
  RespawnEvents = 256, // 0x0000000000000100
  RoundEvents = 512, // 0x0000000000000200
  SetGroup = 1024, // 0x0000000000000400
  GameplayData = 2048, // 0x0000000000000800
  Overwatch = 4096, // 0x0000000000001000
  FacilityManagement = 8192, // 0x0000000000002000
  PlayersManagement = 16384, // 0x0000000000004000
}
