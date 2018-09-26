// Decompiled with JetBrains decompiler
// Type: Unity.GeneratedNetworkCode
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity
{
  [StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
  public class GeneratedNetworkCode
  {
    public static void _ReadStructSyncListItemInfo_Inventory(NetworkReader reader, Inventory.SyncListItemInfo instance)
    {
      ushort num = reader.ReadUInt16();
      ((SyncList<Inventory.SyncItemInfo>) instance).Clear();
      for (ushort index = 0; (int) index < (int) num; ++index)
        instance.AddInternal(instance.DeserializeItem(reader));
    }

    public static void _WriteStructSyncListItemInfo_Inventory(NetworkWriter writer, Inventory.SyncListItemInfo value)
    {
      ushort count = value.get_Count();
      writer.Write(count);
      for (ushort index = 0; (int) index < (int) count; ++index)
        value.SerializeItem(writer, value.GetItem((int) index));
    }

    public static PlayerStats.HitInfo _ReadHitInfo_PlayerStats(NetworkReader reader)
    {
      return new PlayerStats.HitInfo() { amount = reader.ReadSingle(), tool = reader.ReadString(), time = (int) reader.ReadPackedUInt32(), attacker = reader.ReadString(), plyID = (int) reader.ReadPackedUInt32() };
    }

    public static void _WriteHitInfo_PlayerStats(NetworkWriter writer, PlayerStats.HitInfo value)
    {
      writer.Write(value.amount);
      writer.Write(value.tool);
      writer.WritePackedUInt32((uint) value.time);
      writer.Write(value.attacker);
      writer.WritePackedUInt32((uint) value.plyID);
    }

    public static void _WritePickupInfo_Pickup(NetworkWriter writer, Pickup.PickupInfo value)
    {
      writer.Write((Vector3) value.position);
      writer.Write((Quaternion) value.rotation);
      writer.WritePackedUInt32((uint) value.itemId);
      writer.Write(value.durability);
      writer.WritePackedUInt32((uint) value.ownerPlayerID);
    }

    public static Pickup.PickupInfo _ReadPickupInfo_Pickup(NetworkReader reader)
    {
      return new Pickup.PickupInfo() { position = (Vector3) reader.ReadVector3(), rotation = (Quaternion) reader.ReadQuaternion(), itemId = (int) reader.ReadPackedUInt32(), durability = reader.ReadSingle(), ownerPlayerID = (int) reader.ReadPackedUInt32() };
    }

    public static PlayerPositionData _ReadPlayerPositionData_None(NetworkReader reader)
    {
      return new PlayerPositionData() { position = (Vector3) reader.ReadVector3(), rotation = reader.ReadSingle(), playerID = (int) reader.ReadPackedUInt32() };
    }

    public static PlayerPositionData[] _ReadArrayPlayerPositionData_None(NetworkReader reader)
    {
      int length = (int) reader.ReadUInt16();
      if (length == 0)
        return new PlayerPositionData[0];
      PlayerPositionData[] playerPositionDataArray = new PlayerPositionData[length];
      for (int index = 0; index < length; ++index)
        playerPositionDataArray[index] = GeneratedNetworkCode._ReadPlayerPositionData_None(reader);
      return playerPositionDataArray;
    }

    public static void _WritePlayerPositionData_None(NetworkWriter writer, PlayerPositionData value)
    {
      writer.Write((Vector3) value.position);
      writer.Write(value.rotation);
      writer.WritePackedUInt32((uint) value.playerID);
    }

    public static void _WriteArrayPlayerPositionData_None(NetworkWriter writer, PlayerPositionData[] value)
    {
      if (value == null)
      {
        writer.Write((ushort) 0);
      }
      else
      {
        ushort length = (ushort) value.Length;
        writer.Write(length);
        for (ushort index = 0; (int) index < value.Length; ++index)
          GeneratedNetworkCode._WritePlayerPositionData_None(writer, value[(int) index]);
      }
    }

    public static void _WriteInfo_Ragdoll(NetworkWriter writer, Ragdoll.Info value)
    {
      writer.Write(value.ownerHLAPI_id);
      writer.Write(value.steamClientName);
      GeneratedNetworkCode._WriteHitInfo_PlayerStats(writer, value.deathCause);
      writer.WritePackedUInt32((uint) value.charclass);
    }

    public static Ragdoll.Info _ReadInfo_Ragdoll(NetworkReader reader)
    {
      return new Ragdoll.Info() { ownerHLAPI_id = reader.ReadString(), steamClientName = reader.ReadString(), deathCause = GeneratedNetworkCode._ReadHitInfo_PlayerStats(reader), charclass = (int) reader.ReadPackedUInt32() };
    }

    public static RoundSummary.SumInfo_ClassList _ReadSumInfo_ClassList_RoundSummary(NetworkReader reader)
    {
      return new RoundSummary.SumInfo_ClassList() { class_ds = (int) reader.ReadPackedUInt32(), scientists = (int) reader.ReadPackedUInt32(), chaos_insurgents = (int) reader.ReadPackedUInt32(), mtf_and_guards = (int) reader.ReadPackedUInt32(), scps_except_zombies = (int) reader.ReadPackedUInt32(), zombies = (int) reader.ReadPackedUInt32(), warhead_kills = (int) reader.ReadPackedUInt32(), time = (int) reader.ReadPackedUInt32() };
    }

    public static void _WriteSumInfo_ClassList_RoundSummary(NetworkWriter writer, RoundSummary.SumInfo_ClassList value)
    {
      writer.WritePackedUInt32((uint) value.class_ds);
      writer.WritePackedUInt32((uint) value.scientists);
      writer.WritePackedUInt32((uint) value.chaos_insurgents);
      writer.WritePackedUInt32((uint) value.mtf_and_guards);
      writer.WritePackedUInt32((uint) value.scps_except_zombies);
      writer.WritePackedUInt32((uint) value.zombies);
      writer.WritePackedUInt32((uint) value.warhead_kills);
      writer.WritePackedUInt32((uint) value.time);
    }

    public static void _WriteOffset_None(NetworkWriter writer, Offset value)
    {
      writer.Write((Vector3) value.position);
      writer.Write((Vector3) value.rotation);
      writer.Write((Vector3) value.scale);
    }

    public static Offset _ReadOffset_None(NetworkReader reader)
    {
      return new Offset() { position = (Vector3) reader.ReadVector3(), rotation = (Vector3) reader.ReadVector3(), scale = (Vector3) reader.ReadVector3() };
    }

    public static void _WriteBreakableWindowStatus_BreakableWindow(NetworkWriter writer, BreakableWindow.BreakableWindowStatus value)
    {
      writer.Write((Vector3) value.position);
      writer.Write((Quaternion) value.rotation);
      writer.Write(value.broken);
    }

    public static BreakableWindow.BreakableWindowStatus _ReadBreakableWindowStatus_BreakableWindow(NetworkReader reader)
    {
      return new BreakableWindow.BreakableWindowStatus() { position = (Vector3) reader.ReadVector3(), rotation = (Quaternion) reader.ReadQuaternion(), broken = reader.ReadBoolean() };
    }
  }
}
