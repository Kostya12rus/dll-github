// Decompiled with JetBrains decompiler
// Type: ButtonWallAdjuster
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class ButtonWallAdjuster : MonoBehaviour
{
  public float offset = 0.1f;
  public bool onAwake;
  private bool _adjusted;

  private void Start()
  {
    if (!this.onAwake)
      return;
    this.Adjust();
  }

  public void Adjust()
  {
    if (this._adjusted && !this.onAwake)
      return;
    this._adjusted = true;
    this.transform.position += this.transform.up;
    RaycastHit hitInfo;
    if (!Physics.Raycast(new Ray(this.transform.position, -this.transform.up), out hitInfo, 2.5f))
      return;
    this.transform.position = hitInfo.point;
    this.transform.position -= this.transform.up * this.offset * 0.1f;
  }
}
