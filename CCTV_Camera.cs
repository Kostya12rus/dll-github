// Decompiled with JetBrains decompiler
// Type: CCTV_Camera
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class CCTV_Camera : MonoBehaviour
{
  public Transform cameraTarget;
  public Transform camera079;
  public Transform spriteHolder;
  public MeshRenderer spriteRenderer;
  public Material mat;
  public AnimationCurve transparencyOverDistance;
  public AnimationCurve scaleOverDistance;
  public string liftID;

  private void Start()
  {
    this.spriteRenderer.material = new Material(this.mat);
  }

  public void UpdateLOD()
  {
    float time = Vector3.Distance(this.camera079.position, this.transform.position);
    this.spriteRenderer.material.color = new Color(this.spriteRenderer.material.color.r, this.spriteRenderer.material.color.g, this.spriteRenderer.material.color.b, this.transparencyOverDistance.Evaluate(time));
    this.spriteHolder.transform.localScale = Vector3.one * this.scaleOverDistance.Evaluate(time);
    this.spriteHolder.transform.LookAt(this.camera079);
    this.GetComponent<SphereCollider>().radius = Mathf.Clamp(this.spriteHolder.transform.localScale.x / 2f, 0.13f, 10f);
  }
}
