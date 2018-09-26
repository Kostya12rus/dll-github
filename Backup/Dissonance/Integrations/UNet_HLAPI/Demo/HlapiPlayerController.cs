// Decompiled with JetBrains decompiler
// Type: Dissonance.Integrations.UNet_HLAPI.Demo.HlapiPlayerController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI.Demo
{
  public class HlapiPlayerController : NetworkBehaviour
  {
    public HlapiPlayerController()
    {
      base.\u002Ector();
    }

    private void Update()
    {
      if (!this.get_isLocalPlayer())
        return;
      CharacterController component = (CharacterController) ((Component) this).GetComponent<CharacterController>();
      float num1 = (float) ((double) Input.GetAxis("Horizontal") * (double) Time.get_deltaTime() * 150.0);
      float num2 = Input.GetAxis("Vertical") * 3f;
      ((Component) this).get_transform().Rotate(0.0f, num1, 0.0f);
      Vector3 vector3 = ((Component) this).get_transform().TransformDirection(Vector3.get_forward());
      component.SimpleMove(Vector3.op_Multiply(vector3, num2));
      if (((Component) this).get_transform().get_position().y >= -3.0)
        return;
      ((Component) this).get_transform().set_position(Vector3.get_zero());
      ((Component) this).get_transform().set_rotation(Quaternion.get_identity());
    }

    private void UNetVersion()
    {
    }

    public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
      bool flag;
      return flag;
    }

    public virtual void OnDeserialize(NetworkReader reader, bool initialState)
    {
    }
  }
}
