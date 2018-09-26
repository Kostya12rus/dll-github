// Decompiled with JetBrains decompiler
// Type: SkyboxFollower
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class SkyboxFollower : MonoBehaviour
{
  public Transform camera;
  public static bool iAm939;

  public SkyboxFollower()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (SkyboxFollower.iAm939 || this.camera.get_position().y < 800.0)
      ((Component) this).get_transform().set_position(Vector3.op_Multiply(Vector3.get_down(), 12345f));
    else
      ((Component) this).get_transform().set_position(this.camera.get_position());
  }
}
