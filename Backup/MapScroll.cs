// Decompiled with JetBrains decompiler
// Type: MapScroll
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class MapScroll : MonoBehaviour
{
  public RectTransform map;
  public RectTransform rootTransf;
  public float minZoom;
  public float maxZoom;
  public float speed;

  public MapScroll()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.rootTransf = (RectTransform) ((Component) this).GetComponent<RectTransform>();
  }

  private void Update()
  {
    RectTransform rootTransf = this.rootTransf;
    ((Transform) rootTransf).set_localScale(Vector3.op_Addition(((Transform) rootTransf).get_localScale(), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_one(), Input.GetAxis("Mouse ScrollWheel")), 2f), this.minZoom)));
    ((Transform) this.rootTransf).set_localScale(Vector3.op_Multiply(Vector3.get_one(), Mathf.Clamp((float) ((Transform) this.rootTransf).get_localScale().x, this.minZoom, this.maxZoom)));
    if (Input.GetKey(NewInput.GetKey("Fire1")))
    {
      RectTransform map = this.map;
      ((Transform) map).set_localPosition(Vector3.op_Addition(((Transform) map).get_localPosition(), Vector3.op_Multiply(Vector3.op_Multiply(new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f), this.speed), (float) (2.0 / ((Transform) this.rootTransf).get_localScale().x))));
    }
    if (!Input.GetKey(NewInput.GetKey("Zoom")))
      return;
    ((Transform) this.rootTransf).set_localScale(Vector3.get_one());
    ((Transform) this.map).set_localPosition(Vector3.get_zero());
  }
}
