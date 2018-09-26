// Decompiled with JetBrains decompiler
// Type: Decal
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class Decal : MonoBehaviour
{
  private MeshFilter filter;
  private MeshRenderer renderer;

  private void Start()
  {
    this.renderer = this.GetComponent<MeshRenderer>();
    this.filter = this.GetComponent<MeshFilter>();
    Mesh sharedMesh = this.filter.sharedMesh;
    Vector3[] vertices = sharedMesh.vertices;
    for (int index = 0; index < vertices.Length; ++index)
    {
      MonoBehaviour.print((object) index);
      Debug.DrawRay(this.transform.TransformPoint(vertices[index]), -this.transform.forward, Color.red, 10f);
      RaycastHit hitInfo;
      vertices[index] = !Physics.Raycast(this.transform.TransformPoint(vertices[index]), -this.transform.forward, out hitInfo) ? Vector3.zero : this.transform.InverseTransformPoint(hitInfo.point);
    }
    sharedMesh.vertices = vertices;
    sharedMesh.RecalculateNormals();
  }
}
