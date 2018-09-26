// Decompiled with JetBrains decompiler
// Type: PocketDimensionGenerator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PocketDimensionGenerator : MonoBehaviour
{
  private List<PocketDimensionTeleport> pdtps;

  public PocketDimensionGenerator()
  {
    base.\u002Ector();
  }

  public void GenerateMap(int seed)
  {
    Random.InitState(seed);
    foreach (PocketDimensionTeleport dimensionTeleport in (PocketDimensionTeleport[]) Object.FindObjectsOfType<PocketDimensionTeleport>())
      this.pdtps.Add(dimensionTeleport);
    foreach (PocketDimensionTeleport pdtp in this.pdtps)
      pdtp.SetType(PocketDimensionTeleport.PDTeleportType.Killer);
    for (int index = 0; index < 2; ++index)
      this.SetRandomTeleport(PocketDimensionTeleport.PDTeleportType.Exit);
  }

  private void SetRandomTeleport(PocketDimensionTeleport.PDTeleportType type)
  {
    int index = Random.Range(0, this.pdtps.Count);
    this.pdtps[index].SetType(type);
    this.pdtps.RemoveAt(index);
  }
}
