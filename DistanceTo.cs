// Decompiled with JetBrains decompiler
// Type: DistanceTo
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

public class DistanceTo : NetworkBehaviour
{
  private PlayerManager pm;
  private static CharacterClassManager localPlayerCcm;
  public float distanceToLocalPlayer;
  public GameObject spectCamera;

  public DistanceTo()
  {
    base.\u002Ector();
  }

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DistanceTo.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public void CalculateDistanceToLocalPlayer()
  {
    this.distanceToLocalPlayer = Vector3.Distance(((Component) this).get_transform().get_position(), ((Component) DistanceTo.localPlayerCcm).get_transform().get_position());
  }

  public bool IsInRange()
  {
    if (Object.op_Inequality((Object) DistanceTo.localPlayerCcm, (Object) null) && DistanceTo.localPlayerCcm.curClass == 2)
      return true;
    if (((Component) this).get_transform().get_position().y > 800.0)
      return (double) this.distanceToLocalPlayer < 500.0;
    return (double) this.distanceToLocalPlayer < 70.0;
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
}
