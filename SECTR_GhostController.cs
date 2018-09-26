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
    if (Input.GetKeyDown((KeyCode) 304) || Input.GetKeyDown((KeyCode) 303))
      this.FlySpeed *= this.AccelerationRatio * Time.get_deltaTime();
    if (Input.GetKeyUp((KeyCode) 304) || Input.GetKeyUp((KeyCode) 303))
      this.FlySpeed /= this.AccelerationRatio * Time.get_deltaTime();
    if (Input.GetKeyDown((KeyCode) 306) || Input.GetKeyDown((KeyCode) 305))
      this.FlySpeed *= this.SlowDownRatio * Time.get_deltaTime();
    if (Input.GetKeyUp((KeyCode) 306) || Input.GetKeyUp((KeyCode) 305))
      this.FlySpeed /= this.SlowDownRatio * Time.get_deltaTime();
    Vector2 screenJoystick;
    if (Input.get_multiTouchEnabled() && !Application.get_isEditor())
      screenJoystick = this.GetScreenJoystick(false);
    else
      ((Vector2) ref screenJoystick).\u002Ector(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    Transform transform1 = ((Component) this).get_transform();
    transform1.set_position(Vector3.op_Addition(transform1.get_position(), Vector3.op_Addition(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_forward(), this.FlySpeed), Time.get_deltaTime()), (float) screenJoystick.y), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_right(), this.FlySpeed), Time.get_deltaTime()), (float) screenJoystick.x))));
    if (Input.GetKey((KeyCode) 101))
    {
      Transform transform2 = ((Component) this).get_transform();
      transform2.set_position(Vector3.op_Addition(transform2.get_position(), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_up(), this.FlySpeed), Time.get_deltaTime()), 0.5f)));
    }
    else
    {
      if (!Input.GetKey((KeyCode) 113))
        return;
      Transform transform2 = ((Component) this).get_transform();
      transform2.set_position(Vector3.op_Subtraction(transform2.get_position(), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_right(), this.FlySpeed), Time.get_deltaTime()), 0.5f)));
    }
  }
}
