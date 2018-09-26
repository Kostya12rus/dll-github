// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.ObjectSpin
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class ObjectSpin : MonoBehaviour
  {
    public float SpinSpeed;
    public int RotationRange;
    private Transform m_transform;
    private float m_time;
    private Vector3 m_prevPOS;
    private Vector3 m_initial_Rotation;
    private Vector3 m_initial_Position;
    private Color32 m_lightColor;
    private int frames;
    public ObjectSpin.MotionType Motion;

    public ObjectSpin()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this.m_transform = ((Component) this).get_transform();
      Quaternion rotation = this.m_transform.get_rotation();
      this.m_initial_Rotation = ((Quaternion) ref rotation).get_eulerAngles();
      this.m_initial_Position = this.m_transform.get_position();
      Light component = (Light) ((Component) this).GetComponent<Light>();
      this.m_lightColor = Color32.op_Implicit(!Object.op_Inequality((Object) component, (Object) null) ? Color.get_black() : component.get_color());
    }

    private void Update()
    {
      if (this.Motion == ObjectSpin.MotionType.Rotation)
        this.m_transform.Rotate(0.0f, this.SpinSpeed * Time.get_deltaTime(), 0.0f);
      else if (this.Motion == ObjectSpin.MotionType.BackAndForth)
      {
        this.m_time += this.SpinSpeed * Time.get_deltaTime();
        this.m_transform.set_rotation(Quaternion.Euler((float) this.m_initial_Rotation.x, (float) ((double) Mathf.Sin(this.m_time) * (double) this.RotationRange + this.m_initial_Rotation.y), (float) this.m_initial_Rotation.z));
      }
      else
      {
        this.m_time += this.SpinSpeed * Time.get_deltaTime();
        float num1 = 15f * Mathf.Cos(this.m_time * 0.95f);
        float num2 = 10f;
        float num3 = 0.0f;
        this.m_transform.set_position(Vector3.op_Addition(this.m_initial_Position, new Vector3(num1, num3, num2)));
        this.m_prevPOS = this.m_transform.get_position();
        ++this.frames;
      }
    }

    public enum MotionType
    {
      Rotation,
      BackAndForth,
      Translation,
    }
  }
}
