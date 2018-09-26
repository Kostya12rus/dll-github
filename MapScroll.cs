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

  private void Start()
  {
    this.rootTransf = this.GetComponent<RectTransform>();
  }

  private void Update()
  {
    RectTransform rootTransf = this.rootTransf;
    rootTransf.localScale = rootTransf.localScale + Vector3.one * Input.GetAxis("Mouse ScrollWheel") * 2f * this.minZoom;
    this.rootTransf.localScale = Vector3.one * Mathf.Clamp(this.rootTransf.localScale.x, this.minZoom, this.maxZoom);
    if (Input.GetKey(NewInput.GetKey("Fire1")))
    {
      RectTransform map = this.map;
      map.localPosition = map.localPosition + new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f) * this.speed * (2f / this.rootTransf.localScale.x);
    }
    if (!Input.GetKey(NewInput.GetKey("Zoom")))
      return;
    this.rootTransf.localScale = Vector3.one;
    this.map.localPosition = Vector3.zero;
  }
}
