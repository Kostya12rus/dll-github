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
  public int seed;
  private static int staticSeed;
  public static bool generated;

  public RandomSeedSync()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (!this.get_isLocalPlayer())
      return;
    if (NetworkServer.get_active())
    {
      foreach (WorkStation workStation1 in (WorkStation[]) Object.FindObjectsOfType<WorkStation>())
      {
        WorkStation workStation2 = workStation1;
        Offset offset = new Offset();
        offset.position = ((Component) workStation1).get_transform().get_localPosition();
        ref Offset local = ref offset;
        Quaternion localRotation = ((Component) workStation1).get_transform().get_localRotation();
        Vector3 eulerAngles = ((Quaternion) ref localRotation).get_eulerAngles();
        local.rotation = eulerAngles;
        offset.scale = Vector3.get_one();
        Offset pos = offset;
        workStation2.SetPosition(pos);
      }
    }
    RandomSeedSync.generated = false;
    this.Networkseed = ConfigFile.ServerConfig.GetInt("map_seed", -1);
    while (NetworkServer.get_active() && this.seed == -1)
      this.Networkseed = Random.Range(-999999999, 999999999);
  }

  private void Update()
  {
    if (RandomSeedSync.generated || !(((Object) this).get_name() == "Host") || this.seed == -1)
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
    Console objectOfType = (Console) Object.FindObjectOfType<Console>();
    objectOfType.AddLog("Initializing generator...", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
    ImageGenerator imageGenerator1 = (ImageGenerator) null;
    ImageGenerator imageGenerator2 = (ImageGenerator) null;
    ImageGenerator imageGenerator3 = (ImageGenerator) null;
    foreach (ImageGenerator imageGenerator4 in (ImageGenerator[]) Object.FindObjectsOfType<ImageGenerator>())
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
      foreach (Door door in (Door[]) Object.FindObjectsOfType<Door>())
        door.UpdatePos();
    }
    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("DoorButton"))
    {
      try
      {
        ((ButtonWallAdjuster) gameObject.GetComponent<ButtonWallAdjuster>()).Adjust();
        foreach (MonoBehaviour componentsInChild in (ButtonWallAdjuster[]) gameObject.GetComponentsInChildren<ButtonWallAdjuster>())
          componentsInChild.Invoke("Adjust", 4f);
      }
      catch
      {
      }
    }
    foreach (Lift lift in (Lift[]) Object.FindObjectsOfType<Lift>())
    {
      foreach (Lift.Elevator elevator in lift.elevators)
        elevator.SetPosition();
    }
    objectOfType.AddLog("Spawning items...", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
    foreach (Door door in (Door[]) Object.FindObjectsOfType<Door>())
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
    if (NetworkServer.get_active())
      ((HostItemSpawner) PlayerManager.localPlayer.GetComponent<HostItemSpawner>()).Spawn(RandomSeedSync.staticSeed);
    foreach (SECTR_Member sectrMember in (SECTR_Member[]) Object.FindObjectsOfType<SECTR_Member>())
      sectrMember.UpdateViaScript();
    foreach (Pickup pickup in (Pickup[]) Object.FindObjectsOfType<Pickup>())
    {
      ((Component) pickup).get_transform().set_position(pickup.info.position);
      ((Component) pickup).get_transform().set_rotation(pickup.info.rotation);
    }
    ((LCZ_LabelManager) Object.FindObjectOfType<LCZ_LabelManager>()).RefreshLabels();
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetSeed(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.seed);
      return true;
    }
    bool flag = false;
    if (((int) this.get_syncVarDirtyBits() & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.seed);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
