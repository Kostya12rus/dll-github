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
  public int height;
  public Texture2D[] maps;
  private Texture2D map;
  private Color[] copy;
  public float gridSize;
  public List<ImageGenerator.ColorMap> colorMap;
  public List<ImageGenerator.Room> availableRooms;
  public List<GameObject> doors;
  private Vector3 offset;
  public float y_offset;
  private Transform entrRooms;
  public ImageGenerator.RoomsOfType[] roomsOfType;

  public ImageGenerator()
  {
    base.\u002Ector();
  }

  public bool GenerateMap(int seed)
  {
    foreach (ImageGenerator.Room availableRoom in this.availableRooms)
    {
      using (List<GameObject>.Enumerator enumerator = availableRoom.room.GetEnumerator())
      {
        while (enumerator.MoveNext())
          enumerator.Current.SetActive(false);
      }
    }
    ((PocketDimensionGenerator) ((Component) this).GetComponent<PocketDimensionGenerator>()).GenerateMap(seed);
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
    if (Object.op_Inequality((Object) this.entrRooms, (Object) null))
      this.entrRooms.set_parent((Transform) null);
    return true;
  }

  private void InitEntrance()
  {
    if (this.height != -1001)
      return;
    GameObject.Find("Root_Checkpoint").get_transform();
    this.entrRooms = GameObject.Find("EntranceRooms").get_transform();
    for (int index1 = 0; index1 < ((Texture) this.map).get_height(); ++index1)
    {
      for (int index2 = 0; index2 < ((Texture) this.map).get_width(); ++index2)
      {
        if (Color.op_Equality(this.map.GetPixel(index2, index1), Color.get_white()))
          this.offset = Vector3.op_Division(Vector3.op_UnaryNegation(new Vector3((float) index2 * this.gridSize, 0.0f, (float) index1 * this.gridSize)), 3f);
      }
    }
    ImageGenerator imageGenerator = this;
    imageGenerator.offset = Vector3.op_Addition(imageGenerator.offset, Vector3.get_up());
  }

  private void GeneratorTask_Cleanup()
  {
    foreach (ImageGenerator.RoomsOfType roomsOfType in this.roomsOfType)
    {
      foreach (ImageGenerator.Room room in roomsOfType.roomsOfType)
      {
        using (List<GameObject>.Enumerator enumerator = room.room.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            GameObject current = enumerator.Current;
            if (room.type != ImageGenerator.RoomType.Prison)
              current.SetActive(false);
          }
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
    using (List<GameObject>.Enumerator enumerator1 = gameObjectList.GetEnumerator())
    {
      while (enumerator1.MoveNext())
      {
        GameObject current1 = enumerator1.Current;
        using (List<GameObject>.Enumerator enumerator2 = gameObjectList.GetEnumerator())
        {
          while (enumerator2.MoveNext())
          {
            GameObject current2 = enumerator2.Current;
            if ((double) Vector3.Distance(current1.get_transform().get_position(), current2.get_transform().get_position()) < 2.0 && Object.op_Inequality((Object) current1, (Object) current2))
            {
              Object.DestroyImmediate((Object) current2);
              this.GeneratorTask_RemoveDoubledDoorPoints();
              return;
            }
          }
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
          this.doors[index].get_transform().set_position(gameObjectList[index].get_transform().get_position());
          this.doors[index].get_transform().set_rotation(gameObjectList[index].get_transform().get_rotation());
          SECTR_Portal component = (SECTR_Portal) gameObjectList[index].GetComponent<SECTR_Portal>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            sectrPortalList.Add(component);
            if (this.height % 2 == 0)
              ((Door) this.doors[index].GetComponent<Door>()).SetPortal(component);
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
    for (int index1 = 0; index1 < ((Texture) this.map).get_height(); ++index1)
    {
      for (int index2 = 0; index2 < ((Texture) this.map).get_width(); ++index2)
      {
        Color pixel = this.map.GetPixel(index2, index1);
        foreach (ImageGenerator.ColorMap color in this.colorMap)
        {
          if (Color.op_Equality(color.color, pixel))
            this.PlaceRoom(Vector2.op_Addition(new Vector2((float) index2, (float) index1), color.centerOffset), color);
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
    for (int index1 = 0; index1 < ((Texture) this.map).get_height(); ++index1)
    {
      for (int index2 = 0; index2 < ((Texture) this.map).get_width(); ++index2)
      {
        Color pixel = this.map.GetPixel(index2, index1);
        foreach (ImageGenerator.ColorMap color in this.colorMap)
        {
          if (Color.op_Equality(color.color, pixel))
          {
            this.BlankSquare(Vector2.op_Addition(new Vector2((float) index2, (float) index1), color.centerOffset));
            ++this.roomsOfType[(int) color.type].amount;
            List<ImageGenerator.Room> roomList = new List<ImageGenerator.Room>();
            bool flag1 = false;
            for (int index3 = 0; index3 < this.availableRooms.Count; ++index3)
            {
              if (this.availableRooms[index3].type == color.type && this.availableRooms[index3].room.Count > 0 && this.availableRooms[index3].required)
                flag1 = true;
            }
            bool flag2;
            do
            {
              flag2 = false;
              for (int index3 = 0; index3 < this.availableRooms.Count; ++index3)
              {
                if (this.availableRooms[index3].type == color.type && this.availableRooms[index3].room.Count > 0 && (this.availableRooms[index3].required || !flag1))
                {
                  roomList.Add(new ImageGenerator.Room(this.availableRooms[index3]));
                  this.availableRooms.RemoveAt(index3);
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
      room.room[0].get_transform().set_localPosition(Vector3.op_Addition(new Vector3((float) (pos.x * (double) this.gridSize / 3.0), (float) this.height, (float) (pos.y * (double) this.gridSize / 3.0)), this.offset));
      str = "rot";
      room.room[0].get_transform().set_localRotation(Quaternion.Euler(Vector3.op_Multiply(Vector3.get_up(), type.rotationY + this.y_offset)));
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
    ((Vector2) ref centerPoint).\u002Ector((float) (centerPoint.x - 1.0), (float) (centerPoint.y - 1.0));
    for (int index1 = 0; index1 < 3; ++index1)
    {
      for (int index2 = 0; index2 < 3; ++index2)
        this.map.SetPixel((int) centerPoint.x + index1, (int) centerPoint.y + index2, new Color(0.3921f, 0.3921f, 0.3921f, 1f));
    }
    this.map.Apply();
  }

  private void Awake()
  {
    using (List<GameObject>.Enumerator enumerator = this.doors.GetEnumerator())
    {
      while (enumerator.MoveNext())
        ((Door) enumerator.Current.GetComponent<Door>()).SetZero();
    }
  }

  [Serializable]
  public class ColorMap
  {
    public Color color = Color.get_white();
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
