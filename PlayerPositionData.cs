// Decompiled with JetBrains decompiler
// Type: PlayerPositionData
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using RemoteAdmin;
using UnityEngine;

public struct PlayerPositionData
{
  public Vector3 position;
  public float rotation;
  public int playerID;

  public PlayerPositionData(Vector3 _pos, float _rotY, int _id)
  {
    this.position = _pos;
    this.rotation = _rotY;
    this.playerID = _id;
  }

  public PlayerPositionData(GameObject _player)
  {
    this.playerID = ((QueryProcessor) _player.GetComponent<QueryProcessor>()).PlayerId;
    PlyMovementSync component = (PlyMovementSync) _player.GetComponent<PlyMovementSync>();
    this.position = component.position;
    this.rotation = component.rotation;
  }
}
