// Decompiled with JetBrains decompiler
// Type: PocketDimensionTeleport
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;

public class PocketDimensionTeleport : NetworkBehaviour
{
  private PocketDimensionTeleport.PDTeleportType type;

  public PocketDimensionTeleport()
  {
    base.\u002Ector();
  }

  public void SetType(PocketDimensionTeleport.PDTeleportType t)
  {
    this.type = t;
  }

  [ServerCallback]
  private void OnTriggerEnter(Collider other)
  {
    if (!NetworkServer.get_active())
      return;
    NetworkIdentity component = (NetworkIdentity) ((Component) other).GetComponent<NetworkIdentity>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    if (this.type == PocketDimensionTeleport.PDTeleportType.Killer || ((BlastDoor) Object.FindObjectOfType<BlastDoor>()).isClosed)
    {
      ((PlayerStats) ((Component) component).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(999990f, "WORLD", "POCKET", 0), ((Component) other).get_gameObject());
    }
    else
    {
      if (this.type != PocketDimensionTeleport.PDTeleportType.Exit)
        return;
      GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("PD_EXIT");
      ((PlyMovementSync) ((Component) other).GetComponent<PlyMovementSync>()).SetPosition(gameObjectsWithTag[Random.Range(0, gameObjectsWithTag.Length)].get_transform().get_position());
      ((PlayerStats) PlayerManager.localPlayer.GetComponent<PlayerStats>()).CallTargetAchieve(component.get_connectionToClient(), "larryisyourfriend");
    }
  }

  private void UNetVersion()
  {
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }

  public enum PDTeleportType
  {
    Killer,
    Exit,
  }
}
