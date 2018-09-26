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
    public float SpinSpeed = 5f;
    public int RotationRange = 15;
    private Transform m_transform;
    private float m_time;
    private Vector3 m_prevPOS;
    private Vector3 m_initial_Rotation;
    private Vector3 m_initial_Position;
    private Color32 m_lightColor;
    private int frames;
    public ObjectSpin.MotionType Motion;

    private void Awake()
    {
      this.m_transform = this.transform;
      this.m_initial_Rotation = this.m_transform.rotation.eulerAngles;
      this.m_initial_Position = this.m_transform.position;
      Light component = this.GetComponent<Light>();
      this.m_lightColor = (Color32) (!((Object) component != (Object) null) ? Color.black : component.color);
    }

    private void Update()
    {
      if (this.Motion == ObjectSpin.MotionType.Rotation)
        this.m_transform.Rotate(0.0f, this.SpinSpeed * Time.deltaTime, 0.0f);
      else if (this.Motion == ObjectSpin.MotionType.BackAndForth)
      {
        this.m_time += this.SpinSpeed * Time.deltaTime;
        this.m_transform.rotation = Quaternion.Euler(this.m_initial_Rotation.x, Mathf.Sin(this.m_time) * (float) this.RotationRange + this.m_initial_Rotation.y, this.m_initial_Rotation.z);
      }
      else
      {
        this.m_time += this.SpinSpeed * Time.deltaTime;
        float x = 15f * Mathf.Cos(this.m_time * 0.95f);
        float z = 10f;
        float y = 0.0f;
        this.m_transform.position = this.m_initial_Position + new Vector3(x, y, z);
        this.m_prevPOS = this.m_transform.position;
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
