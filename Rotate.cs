// Decompiled with JetBrains decompiler
// Type: Rotate
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class Rotate : MonoBehaviour
{
  private Vector3 speed;

  public Rotate()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    ((Component) this).get_transform().Rotate(Vector3.op_Multiply(this.speed, Time.get_deltaTime()));
  }
}
