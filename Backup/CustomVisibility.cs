// Decompiled with JetBrains decompiler
// Type: CustomVisibility
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

public class CustomVisibility : NetworkBehaviour
{
  public float visRange;

  public CustomVisibility()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    Timing.RunCoroutine(this._Start(), (Segment) 0);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CustomVisibility.\u003C_Start\u003Ec__Iterator0() { \u0024this = this };
  }

  public virtual bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
  {
    foreach (Component component1 in Physics.OverlapSphere(((Component) this).get_transform().get_position(), this.visRange))
    {
      NetworkIdentity component2 = (NetworkIdentity) component1.GetComponent<NetworkIdentity>();
      if (Object.op_Inequality((Object) component2, (Object) null) && component2.get_connectionToClient() != null)
        observers.Add(component2.get_connectionToClient());
    }
    return true;
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
