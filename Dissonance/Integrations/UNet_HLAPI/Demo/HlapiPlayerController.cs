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
    private void Update()
    {
      if (!this.isLocalPlayer)
        return;
      CharacterController component = this.GetComponent<CharacterController>();
      float yAngle = (float) ((double) Input.GetAxis("Horizontal") * (double) Time.deltaTime * 150.0);
      float num = Input.GetAxis("Vertical") * 3f;
      this.transform.Rotate(0.0f, yAngle, 0.0f);
      Vector3 vector3 = this.transform.TransformDirection(Vector3.forward);
      component.SimpleMove(vector3 * num);
      if ((double) this.transform.position.y >= -3.0)
        return;
      this.transform.position = Vector3.zero;
      this.transform.rotation = Quaternion.identity;
    }

    private void UNetVersion()
    {
    }

    public override bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
      bool flag;
      return flag;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
    }
  }
}
