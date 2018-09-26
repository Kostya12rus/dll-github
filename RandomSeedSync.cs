// Decompiled with JetBrains decompiler
// Type: RandomSeedSync
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using GameConsole;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class RandomSeedSync : NetworkBehaviour
{
  [SyncVar(hook = "SetSeed")]
  public int seed = -1;
  private static int staticSeed;
  public static bool generated;

  private void Start()
  {
    if (!this.isLocalPlayer)
      return;
    if (NetworkServer.active)
    {
      foreach (WorkStation workStation in Object.FindObjectsOfType<WorkStation>())
        workStation.SetPosition(new Offset()
        {
          position = workStation.transform.localPosition,
          rotation = workStation.transform.localRotation.eulerAngles,
          scale = Vector3.one
        });
    }
    RandomSeedSync.generated = false;
    this.Networkseed = ConfigFile.ServerConfig.GetInt("map_seed", -1);
    while (NetworkServer.active && this.seed == -1)
      this.Networkseed = Random.Range(-999999999, 999999999);
  }

  private void Update()
  {
    if (RandomSeedSync.generated || !(this.name == "Host") || this.seed == -1)
      return;
    RandomSeedSync.staticSeed = this.seed;
    RandomSeedSync.generated = true;
    RandomSeedSync.GenerateLevel();
  }

  private void SetSeed(int i)
  {
    this.Networkseed = i;
  }

  public static void GenerateLevel()
  {
    Console objectOfType = Object.FindObjectOfType<Console>();
    objectOfType.AddLog("Initializing generator...", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
    ImageGenerator imageGenerator1 = (ImageGenerator) null;
    ImageGenerator imageGenerator2 = (ImageGenerator) null;
    ImageGenerator imageGenerator3 = (ImageGenerator) null;
    foreach (ImageGenerator imageGenerator4 in Object.FindObjectsOfType<ImageGenerator>())
    {
      if (imageGenerator4.height == 0)
        imageGenerator1 = imageGenerator4;
      if (imageGenerator4.height == -1000)
        imageGenerator2 = imageGenerator4;
      if (imageGenerator4.height == -1001)
        imageGenerator3 = imageGenerator4;
    }
    if (!TutorialManager.status)
    {
      imageGenerator1.GenerateMap(RandomSeedSync.staticSeed);
      imageGenerator2.GenerateMap(RandomSeedSync.staticSeed + 1);
      imageGenerator3.GenerateMap(RandomSeedSync.staticSeed + 2);
      foreach (Door door in Object.FindObjectsOfType<Door>())
        door.UpdatePos();
    }
    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("DoorButton"))
    {
      try
      {
        gameObject.GetComponent<ButtonWallAdjuster>().Adjust();
        foreach (MonoBehaviour componentsInChild in gameObject.GetComponentsInChildren<ButtonWallAdjuster>())
          componentsInChild.Invoke("Adjust", 4f);
      }
      catch
      {
      }
    }
    foreach (Lift lift in Object.FindObjectsOfType<Lift>())
    {
      foreach (Lift.Elevator elevator in lift.elevators)
        elevator.SetPosition();
    }
    objectOfType.AddLog("Spawning items...", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
    foreach (Door door in Object.FindObjectsOfType<Door>())
    {
      if (door.destroyed)
      {
        door.DestroyDoor(true);
      }
      else
      {
        door.SetActiveStatus(1);
        door.SetActiveStatus(0);
      }
      door.SetState(door.isOpen);
    }
    if (NetworkServer.active)
      PlayerManager.localPlayer.GetComponent<HostItemSpawner>().Spawn(RandomSeedSync.staticSeed);
    foreach (SECTR_Member sectrMember in Object.FindObjectsOfType<SECTR_Member>())
      sectrMember.UpdateViaScript();
    foreach (Pickup pickup in Object.FindObjectsOfType<Pickup>())
    {
      pickup.transform.position = pickup.info.position;
      pickup.transform.rotation = pickup.info.rotation;
    }
    Object.FindObjectOfType<LCZ_LabelManager>().RefreshLabels();
    objectOfType.AddLog("The scene is ready! Good luck!", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
  }

  private void UNetVersion()
  {
  }

  public int Networkseed
  {
    get
    {
      return this.seed;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.seed;
      int num2 = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetSeed(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<int>(num1, ref local, (uint) num2);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.seed);
      return true;
    }
    bool flag = false;
    if (((int) this.syncVarDirtyBits & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.seed);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.seed = (int) reader.ReadPackedUInt32();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetSeed((int) reader.ReadPackedUInt32());
    }
  }
}
