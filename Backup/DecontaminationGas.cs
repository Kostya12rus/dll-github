// Decompiled with JetBrains decompiler
// Type: DecontaminationGas
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class DecontaminationGas : MonoBehaviour
{
  public static List<DecontaminationGas> gases = new List<DecontaminationGas>();

  public DecontaminationGas()
  {
    base.\u002Ector();
  }

  public static void TurnOn()
  {
    if (ServerStatic.IsDedicated)
      return;
    foreach (DecontaminationGas gase in DecontaminationGas.gases)
    {
      if (Object.op_Inequality((Object) gase, (Object) null))
        ((Component) gase).get_gameObject().SetActive(true);
    }
  }

  private void Start()
  {
    DecontaminationGas.gases.Add(this);
    ((Component) this).get_gameObject().SetActive(false);
  }
}
