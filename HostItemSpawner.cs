// Decompiled with JetBrains decompiler
// Type: HostItemSpawner
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostItemSpawner : NetworkBehaviour
{
  private RandomItemSpawner ris;
  private Item[] avItems;

  private void Start()
  {
    this.avItems = Object.FindObjectOfType<Inventory>().availableItems;
  }

  public void Spawn(int seed)
  {
    Random.InitState(seed);
    string str = string.Empty;
    try
    {
      this.ris = Object.FindObjectOfType<RandomItemSpawner>();
      RandomItemSpawner.PickupPositionRelation[] pickups = this.ris.pickups;
      List<RandomItemSpawner.PositionPosIdRelation> positionPosIdRelationList1 = new List<RandomItemSpawner.PositionPosIdRelation>();
      str = "Starting";
      foreach (RandomItemSpawner.PositionPosIdRelation posId in this.ris.posIds)
        positionPosIdRelationList1.Add(posId);
      int num = 0;
      foreach (RandomItemSpawner.PickupPositionRelation positionRelation in pickups)
      {
        for (int index = 0; index < positionPosIdRelationList1.Count; ++index)
          positionPosIdRelationList1[index].index = index;
        List<RandomItemSpawner.PositionPosIdRelation> positionPosIdRelationList2 = new List<RandomItemSpawner.PositionPosIdRelation>();
        foreach (RandomItemSpawner.PositionPosIdRelation positionPosIdRelation in positionPosIdRelationList1)
        {
          if (positionPosIdRelation.posID == positionRelation.posID)
            positionPosIdRelationList2.Add(positionPosIdRelation);
        }
        str = "Setting items: " + (object) num;
        int index1 = Random.Range(0, positionPosIdRelationList2.Count);
        RandomItemSpawner.PositionPosIdRelation positionPosIdRelation1 = positionPosIdRelationList2[index1];
        int index2 = positionPosIdRelation1.index;
        this.SetPos(positionRelation.pickup.gameObject, positionPosIdRelation1.position.position, positionRelation.itemID, positionPosIdRelation1.position.rotation.eulerAngles);
        positionRelation.pickup.RefreshDurability(true);
        positionPosIdRelationList1.RemoveAt(index2);
        ++num;
      }
    }
    catch
    {
      Debug.LogError((object) ("Something is wrong, lol: " + str));
    }
  }

  [ServerCallback]
  private void SetPos(GameObject obj, Vector3 pos, int item, Vector3 rot)
  {
    if (!NetworkServer.active)
      return;
    obj.GetComponent<Pickup>().SetupPickup(new Pickup.PickupInfo()
    {
      position = pos,
      rotation = Quaternion.Euler(rot),
      itemId = item,
      durability = 0.0f,
      ownerPlayerID = 0
    });
  }

  private void UNetVersion()
  {
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
