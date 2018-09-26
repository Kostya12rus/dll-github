// Decompiled with JetBrains decompiler
// Type: ImageGenerator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class ImageGenerator : MonoBehaviour
{
  public List<ImageGenerator.ColorMap> colorMap = new List<ImageGenerator.ColorMap>();
  public List<ImageGenerator.Room> availableRooms = new List<ImageGenerator.Room>();
  public List<GameObject> doors = new List<GameObject>();
  public int height;
  public Texture2D[] maps;
  private Texture2D map;
  private Color[] copy;
  public float gridSize;
  private Vector3 offset;
  public float y_offset;
  private Transform entrRooms;
  public ImageGenerator.RoomsOfType[] roomsOfType;

  public bool GenerateMap(int seed)
  {
    foreach (ImageGenerator.Room availableRoom in this.availableRooms)
    {
      foreach (GameObject gameObject in availableRoom.room)
        gameObject.SetActive(false);
    }
    this.GetComponent<PocketDimensionGenerator>().GenerateMap(seed);
    Random.InitState(seed);
    this.map = this.maps[Random.Range(0, this.maps.Length)];
    this.InitEntrance();
    this.copy = this.map.GetPixels();
    this.GeneratorTask_CheckRooms();
    this.GeneratorTask_RemoveNotRequired();
    this.GeneratorTask_SetRooms();
    this.GeneratorTask_Cleanup();
    this.GeneratorTask_RemoveDoubledDoorPoints();
    this.map.SetPixels(this.copy);
    this.map.Apply();
    if ((Object) this.entrRooms != (Object) null)
      this.entrRooms.parent = (Transform) null;
    return true;
  }

  private void InitEntrance()
  {
    if (this.height != -1001)
      return;
    Transform transform = GameObject.Find("Root_Checkpoint").transform;
    this.entrRooms = GameObject.Find("EntranceRooms").transform;
    for (int y = 0; y < this.map.height; ++y)
    {
      for (int x = 0; x < this.map.width; ++x)
      {
        if (this.map.GetPixel(x, y) == Color.white)
          this.offset = -new Vector3((float) x * this.gridSize, 0.0f, (float) y * this.gridSize) / 3f;
      }
    }
    this.offset += Vector3.up;
  }

  private void GeneratorTask_Cleanup()
  {
    foreach (ImageGenerator.RoomsOfType roomsOfType in this.roomsOfType)
    {
      foreach (ImageGenerator.Room room in roomsOfType.roomsOfType)
      {
        foreach (GameObject gameObject in room.room)
        {
          if (room.type != ImageGenerator.RoomType.Prison)
            gameObject.SetActive(false);
        }
      }
    }
  }

  private void GeneratorTask_RemoveDoubledDoorPoints()
  {
    if (this.doors.Count == 0)
      return;
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("DoorPoint" + (object) this.height))
      gameObjectList.Add(gameObject);
    foreach (GameObject gameObject1 in gameObjectList)
    {
      foreach (GameObject gameObject2 in gameObjectList)
      {
        if ((double) Vector3.Distance(gameObject1.transform.position, gameObject2.transform.position) < 2.0 && (Object) gameObject1 != (Object) gameObject2)
        {
          Object.DestroyImmediate((Object) gameObject2);
          this.GeneratorTask_RemoveDoubledDoorPoints();
          return;
        }
      }
    }
    List<SECTR_Portal> sectrPortalList = new List<SECTR_Portal>();
    for (int index = 0; index < this.doors.Count; ++index)
    {
      try
      {
        if (index < gameObjectList.Count)
        {
          this.doors[index].transform.position = gameObjectList[index].transform.position;
          this.doors[index].transform.rotation = gameObjectList[index].transform.rotation;
          SECTR_Portal component = gameObjectList[index].GetComponent<SECTR_Portal>();
          if ((Object) component != (Object) null)
          {
            sectrPortalList.Add(component);
            if (this.height % 2 == 0)
              this.doors[index].GetComponent<Door>().SetPortal(component);
          }
        }
        else
          this.doors[index].SetActive(false);
      }
      catch
      {
        Debug.LogError((object) "Not enough doors!");
      }
    }
    foreach (SECTR_Portal sectrPortal in sectrPortalList)
      sectrPortal.Setup();
  }

  private void GeneratorTask_SetRooms()
  {
    for (int y = 0; y < this.map.height; ++y)
    {
      for (int x = 0; x < this.map.width; ++x)
      {
        Color pixel = this.map.GetPixel(x, y);
        foreach (ImageGenerator.ColorMap color in this.colorMap)
        {
          if (color.color == pixel)
            this.PlaceRoom(new Vector2((float) x, (float) y) + color.centerOffset, color);
        }
      }
    }
  }

  private void GeneratorTask_RemoveNotRequired()
  {
    foreach (ImageGenerator.ColorMap color in this.colorMap)
    {
      bool flag = false;
      while (!flag)
      {
        int num = 0;
        foreach (ImageGenerator.Room room in this.roomsOfType[(int) color.type].roomsOfType)
          num += room.room.Count;
        if (num > this.roomsOfType[(int) color.type].amount)
        {
          flag = true;
          for (int index = 0; index < this.roomsOfType[(int) color.type].roomsOfType.Count; ++index)
          {
            if (!this.roomsOfType[(int) color.type].roomsOfType[index].required && this.roomsOfType[(int) color.type].roomsOfType[index].room.Count > 0)
            {
              this.roomsOfType[(int) color.type].roomsOfType[index].room[0].SetActive(false);
              this.roomsOfType[(int) color.type].roomsOfType[index].room.RemoveAt(0);
              flag = false;
              break;
            }
          }
        }
        else
          break;
      }
    }
  }

  private void GeneratorTask_CheckRooms()
  {
    for (int y = 0; y < this.map.height; ++y)
    {
      for (int x = 0; x < this.map.width; ++x)
      {
        Color pixel = this.map.GetPixel(x, y);
        foreach (ImageGenerator.ColorMap color in this.colorMap)
        {
          if (color.color == pixel)
          {
            this.BlankSquare(new Vector2((float) x, (float) y) + color.centerOffset);
            ++this.roomsOfType[(int) color.type].amount;
            List<ImageGenerator.Room> roomList = new List<ImageGenerator.Room>();
            bool flag1 = false;
            for (int index = 0; index < this.availableRooms.Count; ++index)
            {
              if (this.availableRooms[index].type == color.type && this.availableRooms[index].room.Count > 0 && this.availableRooms[index].required)
                flag1 = true;
            }
            bool flag2;
            do
            {
              flag2 = false;
              for (int index = 0; index < this.availableRooms.Count; ++index)
              {
                if (this.availableRooms[index].type == color.type && this.availableRooms[index].room.Count > 0 && (this.availableRooms[index].required || !flag1))
                {
                  roomList.Add(new ImageGenerator.Room(this.availableRooms[index]));
                  this.availableRooms.RemoveAt(index);
                  flag2 = true;
                  break;
                }
              }
            }
            while (flag2);
            foreach (ImageGenerator.Room r in roomList)
              this.roomsOfType[(int) color.type].roomsOfType.Add(new ImageGenerator.Room(r));
          }
        }
      }
    }
    this.map.SetPixels(this.copy);
    this.map.Apply();
  }

  private void PlaceRoom(Vector2 pos, ImageGenerator.ColorMap type)
  {
    string str = string.Empty;
    try
    {
      str = "blanking";
      this.BlankSquare(pos);
      str = "do";
      ImageGenerator.Room room;
      do
      {
        str = "rand";
        int index = Random.Range(0, this.roomsOfType[(int) type.type].roomsOfType.Count);
        str = "rset " + (object) type.type + "/" + (object) this.roomsOfType.Length + (object) index;
        room = this.roomsOfType[(int) type.type].roomsOfType[index];
        if (room.room.Count == 0)
        {
          str = "remove";
          this.roomsOfType[(int) type.type].roomsOfType.RemoveAt(index);
        }
      }
      while (room.room.Count == 0);
      str = nameof (pos);
      room.room[0].transform.localPosition = new Vector3((float) ((double) pos.x * (double) this.gridSize / 3.0), (float) this.height, (float) ((double) pos.y * (double) this.gridSize / 3.0)) + this.offset;
      str = "rot";
      room.room[0].transform.localRotation = Quaternion.Euler(Vector3.up * (type.rotationY + this.y_offset));
      str = "rev";
      room.room[0].SetActive(true);
      room.room.RemoveAt(0);
    }
    catch
    {
      MonoBehaviour.print((object) str);
    }
  }

  private void BlankSquare(Vector2 centerPoint)
  {
    centerPoint = new Vector2(centerPoint.x - 1f, centerPoint.y - 1f);
    for (int index1 = 0; index1 < 3; ++index1)
    {
      for (int index2 = 0; index2 < 3; ++index2)
        this.map.SetPixel((int) centerPoint.x + index1, (int) centerPoint.y + index2, new Color(0.3921f, 0.3921f, 0.3921f, 1f));
    }
    this.map.Apply();
  }

  private void Awake()
  {
    foreach (GameObject door in this.doors)
      door.GetComponent<Door>().SetZero();
  }

  [Serializable]
  public class ColorMap
  {
    public Color color = Color.white;
    public ImageGenerator.RoomType type;
    public float rotationY;
    public Vector2 centerOffset;
  }

  [Serializable]
  public class RoomsOfType
  {
    public List<ImageGenerator.Room> roomsOfType = new List<ImageGenerator.Room>();
    public int amount;
  }

  [Serializable]
  public class Room
  {
    public List<GameObject> room = new List<GameObject>();
    public ImageGenerator.RoomType type;
    public bool required;

    public Room(ImageGenerator.Room r)
    {
      this.room = r.room;
      this.type = r.type;
      this.required = r.required;
    }
  }

  public enum RoomType
  {
    Straight,
    Curve,
    RoomT,
    Cross,
    Endoff,
    Prison,
  }
}
