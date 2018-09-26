// Decompiled with JetBrains decompiler
// Type: SECTR_GhostController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("SECTR/Demos/SECTR Ghost Controller")]
public class SECTR_GhostController : SECTR_FPController
{
  [SECTR_ToolTip("The speed at which to fly through the world.")]
  public float FlySpeed = 0.5f;
  [SECTR_ToolTip("The translation acceleration amount applied by keyboard input.")]
  public float AccelerationRatio = 1f;
  [SECTR_ToolTip("The amount by which holding down Ctrl slows you down.")]
  public float SlowDownRatio = 0.5f;

  protected override void Update()
  {
    base.Update();
    if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
      this.FlySpeed *= this.AccelerationRatio * Time.deltaTime;
    if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
      this.FlySpeed /= this.AccelerationRatio * Time.deltaTime;
    if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
      this.FlySpeed *= this.SlowDownRatio * Time.deltaTime;
    if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
      this.FlySpeed /= this.SlowDownRatio * Time.deltaTime;
    Vector2 vector2 = !Input.multiTouchEnabled || Application.isEditor ? new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) : this.GetScreenJoystick(false);
    this.transform.position += this.transform.forward * this.FlySpeed * Time.deltaTime * vector2.y + this.transform.right * this.FlySpeed * Time.deltaTime * vector2.x;
    if (Input.GetKey(KeyCode.E))
    {
      this.transform.position += this.transform.up * this.FlySpeed * Time.deltaTime * 0.5f;
    }
    else
    {
      if (!Input.GetKey(KeyCode.Q))
        return;
      this.transform.position -= this.transform.right * this.FlySpeed * Time.deltaTime * 0.5f;
    }
  }
}
