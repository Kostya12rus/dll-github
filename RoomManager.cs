// Decompiled with JetBrains decompiler
// Type: RoomManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
  public bool isGenerated;
  public int useSimulator;
  public List<RoomManager.Room> rooms;
  public List<RoomManager.RoomPosition> positions;

  public RoomManager()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (this.useSimulator == -1)
      return;
    this.GenerateMap(this.useSimulator);
  }

  public void GenerateMap(int seed)
  {
    GameConsole.Console objectOfType = (GameConsole.Console) Object.FindObjectOfType<GameConsole.Console>();
    if (!TutorialManager.status)
    {
      ((PocketDimensionGenerator) ((Component) this).GetComponent<PocketDimensionGenerator>()).GenerateMap(seed);
      for (int index = 0; index < this.positions.Count; ++index)
      {
        ((Object) this.positions[index].point).set_name("POINT" + (object) index);
        if (Object.op_Equality((Object) ((Component) this.positions[index].point).GetComponent<Point>(), (Object) null))
        {
          Debug.LogError((object) "RoomManager: Missing 'Point' script at current position.");
          return;
        }
      }
      Random.InitState(seed);
      objectOfType.AddLog("[MG REPLY]: Successfully recieved map seed!", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), true);
      List<RoomManager.RoomPosition> positions = this.positions;
      objectOfType.AddLog("[MG TASK]: Setting rooms positions...", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
      foreach (RoomManager.Room room in this.rooms)
      {
        objectOfType.AddLog("\t\t[MG INFO]: " + room.label + " is about to set!", new Color32((byte) 120, (byte) 120, (byte) 120, byte.MaxValue), true);
        List<int> intList1 = new List<int>();
        for (int index = 0; index < positions.Count; ++index)
        {
          if (this.positions[index].type.Equals(room.type))
          {
            bool flag = true;
            foreach (Point componentsInChild in (Point[]) room.roomPrefab.GetComponentsInChildren<Point>())
            {
              if (((Object) this.positions[index].point).get_name() == ((Object) ((Component) componentsInChild).get_gameObject()).get_name())
                flag = false;
            }
            if (flag)
              intList1.Add(index);
          }
        }
        List<int> intList2 = intList1;
        for (int index = 0; index < intList2.Count; ++index)
        {
          foreach (Point componentsInChild in (Point[]) room.roomPrefab.GetComponentsInChildren<Point>())
          {
            if (((Object) this.positions[intList2[index]].point).get_name() == ((Object) ((Component) componentsInChild).get_gameObject()).get_name())
              intList1.Remove(intList2[index]);
          }
        }
        int index1 = intList1[Random.Range(0, intList1.Count)];
        RoomManager.RoomPosition position = this.positions[index1];
        GameObject roomPrefab = room.roomPrefab;
        RawImage icon = room.icon;
        room.readonlyPoint = position.point;
        roomPrefab.get_transform().SetParent(position.point);
        roomPrefab.get_transform().set_localPosition(room.roomOffset.position);
        roomPrefab.get_transform().set_localRotation(Quaternion.Euler(room.roomOffset.rotation));
        roomPrefab.get_transform().set_localScale(room.roomOffset.scale);
        if (Object.op_Inequality((Object) icon, (Object) null))
        {
          ((Transform) ((Graphic) icon).get_rectTransform()).SetParent((Transform) position.ui_point);
          ((Component) icon).get_transform().set_localPosition(room.iconoffset.position);
          ((Transform) ((Graphic) icon).get_rectTransform()).set_localRotation(Quaternion.Euler(room.iconoffset.rotation));
          ((Component) icon).get_transform().set_localScale(room.iconoffset.scale);
        }
        roomPrefab.SetActive(true);
        this.positions.RemoveAt(index1);
      }
    }
    objectOfType.AddLog("--Map successfully generated--", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
    this.isGenerated = true;
  }

  [Serializable]
  public class Room
  {
    public string label;
    public Offset roomOffset;
    public GameObject roomPrefab;
    public string type;
    public Transform readonlyPoint;
    public RawImage icon;
    public Offset iconoffset;
  }

  [Serializable]
  public struct RoomPosition
  {
    public string type;
    public Transform point;
    public RectTransform ui_point;
  }
}
