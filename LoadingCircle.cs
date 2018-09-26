// Decompiled with JetBrains decompiler
// Type: LoadingCircle
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
  private int i;
  public int framesToNextRotation;

  public LoadingCircle()
  {
    base.\u002Ector();
  }

  private void FixedUpdate()
  {
    ++this.i;
    if (this.i <= this.framesToNextRotation)
      return;
    this.i = 0;
    ((Component) this).get_transform().Rotate(Vector3.op_Multiply(Vector3.get_forward(), -45f), (Space) 1);
  }
}
