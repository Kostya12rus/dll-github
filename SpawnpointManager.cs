// Decompiled with JetBrains decompiler
// Type: SpawnpointManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class SpawnpointManager : MonoBehaviour
{
  public GameObject GetRandomPosition(int classID)
  {
    GameObject gameObject = (GameObject) null;
    Class @class = GameObject.Find("Host").GetComponent<CharacterClassManager>().klasy[classID];
    if (@class.team == Team.CDP || @class.team == Team.TUT)
    {
      GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("SP_CDP");
      int index = Random.Range(0, gameObjectsWithTag.Length);
      gameObject = gameObjectsWithTag[index];
    }
    if (classID == 10)
      return (GameObject) null;
    if (@class.team == Team.SCP)
    {
      switch (classID)
      {
        case 3:
          GameObject[] gameObjectsWithTag1 = GameObject.FindGameObjectsWithTag("SP_106");
          int index1 = Random.Range(0, gameObjectsWithTag1.Length);
          gameObject = gameObjectsWithTag1[index1];
          break;
        case 5:
          GameObject[] gameObjectsWithTag2 = GameObject.FindGameObjectsWithTag("SP_049");
          int index2 = Random.Range(0, gameObjectsWithTag2.Length);
          gameObject = gameObjectsWithTag2[index2];
          break;
        case 7:
          GameObject[] gameObjectsWithTag3 = GameObject.FindGameObjectsWithTag("SP_079");
          int index3 = Random.Range(0, gameObjectsWithTag3.Length);
          gameObject = gameObjectsWithTag3[index3];
          break;
        case 9:
          GameObject[] gameObjectsWithTag4 = GameObject.FindGameObjectsWithTag("SCP_096");
          int index4 = Random.Range(0, gameObjectsWithTag4.Length);
          gameObject = gameObjectsWithTag4[index4];
          break;
        default:
          if (@class.fullName.Contains("SCP-939"))
          {
            GameObject[] gameObjectsWithTag5 = GameObject.FindGameObjectsWithTag("SCP_939");
            int index5 = Random.Range(0, gameObjectsWithTag5.Length);
            gameObject = gameObjectsWithTag5[index5];
            break;
          }
          GameObject[] gameObjectsWithTag6 = GameObject.FindGameObjectsWithTag("SP_173");
          int index6 = Random.Range(0, gameObjectsWithTag6.Length);
          gameObject = gameObjectsWithTag6[index6];
          break;
      }
    }
    if (@class.team == Team.MTF)
    {
      GameObject[] gameObjectArray = classID != 15 ? GameObject.FindGameObjectsWithTag("SP_MTF") : GameObject.FindGameObjectsWithTag("SP_GUARD");
      int index5 = Random.Range(0, gameObjectArray.Length);
      gameObject = gameObjectArray[index5];
    }
    if (@class.team == Team.RSC)
    {
      GameObject[] gameObjectsWithTag5 = GameObject.FindGameObjectsWithTag("SP_RSC");
      int index5 = Random.Range(0, gameObjectsWithTag5.Length);
      gameObject = gameObjectsWithTag5[index5];
    }
    if (@class.team == Team.CHI)
    {
      GameObject[] gameObjectsWithTag5 = GameObject.FindGameObjectsWithTag("SP_CI");
      int index5 = Random.Range(0, gameObjectsWithTag5.Length);
      gameObject = gameObjectsWithTag5[index5];
    }
    return gameObject;
  }
}
