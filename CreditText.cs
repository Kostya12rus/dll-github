// Decompiled with JetBrains decompiler
// Type: CreditText
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class CreditText : MonoBehaviour
{
  public bool move;
  public float speed;

  private void FixedUpdate()
  {
    if (!this.move)
      return;
    this.transform.Translate(Vector3.up * this.speed);
  }
}
