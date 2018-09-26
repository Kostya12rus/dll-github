// Decompiled with JetBrains decompiler
// Type: GunAim
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class GunAim : MonoBehaviour
{
  public int borderLeft;
  public int borderRight;
  public int borderTop;
  public int borderBottom;
  private Camera parentCamera;
  private bool isOutOfBounds;

  private void Start()
  {
    this.parentCamera = this.GetComponentInParent<Camera>();
  }

  private void Update()
  {
    float x = Input.mousePosition.x;
    float y = Input.mousePosition.y;
    this.isOutOfBounds = (double) x <= (double) this.borderLeft || (double) x >= (double) (Screen.width - this.borderRight) || ((double) y <= (double) this.borderBottom || (double) y >= (double) (Screen.height - this.borderTop));
    if (this.isOutOfBounds)
      return;
    this.transform.LookAt(this.parentCamera.ScreenToWorldPoint(new Vector3(x, y, 5f)));
  }

  public bool GetIsOutOfBounds()
  {
    return this.isOutOfBounds;
  }
}
