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

  public Decal()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.renderer = (MeshRenderer) ((Component) this).GetComponent<MeshRenderer>();
    this.filter = (MeshFilter) ((Component) this).GetComponent<MeshFilter>();
    Mesh sharedMesh = this.filter.get_sharedMesh();
    Vector3[] vertices = sharedMesh.get_vertices();
    for (int index = 0; index < vertices.Length; ++index)
    {
      MonoBehaviour.print((object) index);
      Debug.DrawRay(((Component) this).get_transform().TransformPoint(vertices[index]), Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()), Color.get_red(), 10f);
      RaycastHit raycastHit;
      vertices[index] = !Physics.Raycast(((Component) this).get_transform().TransformPoint(vertices[index]), Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()), ref raycastHit) ? Vector3.get_zero() : ((Component) this).get_transform().InverseTransformPoint(((RaycastHit) ref raycastHit).get_point());
    }
    sharedMesh.set_vertices(vertices);
    sharedMesh.RecalculateNormals();
  }
}
