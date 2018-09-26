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

  private void Update()
  {
    if (SkyboxFollower.iAm939 || (double) this.camera.position.y < 800.0)
      this.transform.position = Vector3.down * 12345f;
    else
      this.transform.position = this.camera.position;
  }
}
