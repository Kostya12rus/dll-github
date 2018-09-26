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
  public int useSimulator = -1;
  public List<RoomManager.Room> rooms = new List<RoomManager.Room>();
  public List<RoomManager.RoomPosition> positions = new List<RoomManager.RoomPosition>();
  public bool isGenerated;

  private void Start()
  {
    if (this.useSimulator == -1)
      return;
    this.GenerateMap(this.useSimulator);
  }

  public void GenerateMap(int seed)
  {
    GameConsole.Console objectOfType = Object.FindObjectOfType<GameConsole.Console>();
    if (!TutorialManager.status)
    {
      this.GetComponent<PocketDimensionGenerator>().GenerateMap(seed);
      for (int index = 0; index < this.positions.Count; ++index)
      {
        this.positions[index].point.name = "POINT" + (object) index;
        if ((Object) this.positions[index].point.GetComponent<Point>() == (Object) null)
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
            foreach (Point componentsInChild in room.roomPrefab.GetComponentsInChildren<Point>())
            {
              if (this.positions[index].point.name == componentsInChild.gameObject.name)
                flag = false;
            }
            if (flag)
              intList1.Add(index);
          }
        }
        List<int> intList2 = intList1;
        for (int index = 0; index < intList2.Count; ++index)
        {
          foreach (Point componentsInChild in room.roomPrefab.GetComponentsInChildren<Point>())
          {
            if (this.positions[intList2[index]].point.name == componentsInChild.gameObject.name)
              intList1.Remove(intList2[index]);
          }
        }
        int index1 = intList1[Random.Range(0, intList1.Count)];
        RoomManager.RoomPosition position = this.positions[index1];
        GameObject roomPrefab = room.roomPrefab;
        RawImage icon = room.icon;
        room.readonlyPoint = position.point;
        roomPrefab.transform.SetParent(position.point);
        roomPrefab.transform.localPosition = room.roomOffset.position;
        roomPrefab.transform.localRotation = Quaternion.Euler(room.roomOffset.rotation);
        roomPrefab.transform.localScale = room.roomOffset.scale;
        if ((Object) icon != (Object) null)
        {
          icon.rectTransform.SetParent((Transform) position.ui_point);
          icon.transform.localPosition = room.iconoffset.position;
          icon.rectTransform.localRotation = Quaternion.Euler(room.iconoffset.rotation);
          icon.transform.localScale = room.iconoffset.scale;
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
