// Decompiled with JetBrains decompiler
// Type: ExampleWheelController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class ExampleWheelController : MonoBehaviour
{
  public float acceleration;
  public Renderer motionVectorRenderer;
  private Rigidbody m_Rigidbody;

  public ExampleWheelController()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.m_Rigidbody = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    this.m_Rigidbody.set_maxAngularVelocity(100f);
  }

  private void Update()
  {
    if (Input.GetKey((KeyCode) 273))
      this.m_Rigidbody.AddRelativeTorque(new Vector3(-1f * this.acceleration, 0.0f, 0.0f), (ForceMode) 5);
    else if (Input.GetKey((KeyCode) 274))
      this.m_Rigidbody.AddRelativeTorque(new Vector3(1f * this.acceleration, 0.0f, 0.0f), (ForceMode) 5);
    float num = (float) (-this.m_Rigidbody.get_angularVelocity().x / 100.0);
    if (!Object.op_Implicit((Object) this.motionVectorRenderer))
      return;
    this.motionVectorRenderer.get_material().SetFloat(ExampleWheelController.Uniforms._MotionAmount, Mathf.Clamp(num, -0.25f, 0.25f));
  }

  private static class Uniforms
  {
    internal static readonly int _MotionAmount = Shader.PropertyToID(nameof (_MotionAmount));
  }
}
