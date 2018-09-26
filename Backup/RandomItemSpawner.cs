// Decompiled with JetBrains decompiler
// Type: RandomItemSpawner
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
  public RandomItemSpawner.PickupPositionRelation[] pickups;
  public RandomItemSpawner.PositionPosIdRelation[] posIds;

  public RandomItemSpawner()
  {
    base.\u002Ector();
  }

  public void RefreshIndexes()
  {
    for (int index = 0; index < this.posIds.Length; ++index)
      this.posIds[index].index = index;
  }

  [Serializable]
  public class PickupPositionRelation
  {
    public Pickup pickup;
    public int itemID;
    public string posID;
  }

  [Serializable]
  public class PositionPosIdRelation
  {
    public string posID;
    public Transform position;
    public int index;
  }
}
