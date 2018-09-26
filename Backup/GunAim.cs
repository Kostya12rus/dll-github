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

  public GunAim()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.parentCamera = (Camera) ((Component) this).GetComponentInParent<Camera>();
  }

  private void Update()
  {
    float x = (float) Input.get_mousePosition().x;
    float y = (float) Input.get_mousePosition().y;
    this.isOutOfBounds = (double) x <= (double) this.borderLeft || (double) x >= (double) (Screen.get_width() - this.borderRight) || ((double) y <= (double) this.borderBottom || (double) y >= (double) (Screen.get_height() - this.borderTop));
    if (this.isOutOfBounds)
      return;
    ((Component) this).get_transform().LookAt(this.parentCamera.ScreenToWorldPoint(new Vector3(x, y, 5f)));
  }

  public bool GetIsOutOfBounds()
  {
    return this.isOutOfBounds;
  }
}
