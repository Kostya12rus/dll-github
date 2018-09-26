// Decompiled with JetBrains decompiler
// Type: ButtonWallAdjuster
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class ButtonWallAdjuster : MonoBehaviour
{
  public bool onAwake;
  private bool _adjusted;
  public float offset;

  public ButtonWallAdjuster()
  {
    base.\u002Ector();
  }

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
    Transform transform1 = ((Component) this).get_transform();
    transform1.set_position(Vector3.op_Addition(transform1.get_position(), ((Component) this).get_transform().get_up()));
    RaycastHit raycastHit;
    if (!Physics.Raycast(new Ray(((Component) this).get_transform().get_position(), Vector3.op_UnaryNegation(((Component) this).get_transform().get_up())), ref raycastHit, 2.5f))
      return;
    ((Component) this).get_transform().set_position(((RaycastHit) ref raycastHit).get_point());
    Transform transform2 = ((Component) this).get_transform();
    transform2.set_position(Vector3.op_Subtraction(transform2.get_position(), Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_up(), this.offset), 0.1f)));
  }
}
