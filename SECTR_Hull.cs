// Decompiled with JetBrains decompiler
// Type: SECTR_Hull
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

public abstract class SECTR_Hull : MonoBehaviour
{
  private Mesh previousMesh;
  private Vector3[] vertsCW;
  private Vector3 meshCentroid;
  protected Vector3 meshNormal;
  [SECTR_ToolTip("Convex, planar mesh that defines the portal shape.")]
  public Mesh HullMesh;

  protected SECTR_Hull()
  {
    base.\u002Ector();
  }

  public Vector3[] VertsCW
  {
    get
    {
      this.ComputeVerts();
      return this.vertsCW;
    }
  }

  public Vector3 Normal
  {
    get
    {
      this.ComputeVerts();
      return Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), this.meshNormal);
    }
  }

  public Vector3 ReverseNormal
  {
    get
    {
      this.ComputeVerts();
      return Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), Vector3.op_UnaryNegation(this.meshNormal));
    }
  }

  public Vector3 Center
  {
    get
    {
      this.ComputeVerts();
      Matrix4x4 localToWorldMatrix = ((Component) this).get_transform().get_localToWorldMatrix();
      return ((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(this.meshCentroid);
    }
  }

  public Plane HullPlane
  {
    get
    {
      this.ComputeVerts();
      return new Plane(this.Normal, this.Center);
    }
  }

  public Plane ReverseHullPlane
  {
    get
    {
      this.ComputeVerts();
      return new Plane(this.ReverseNormal, this.Center);
    }
  }

  public Bounds BoundingBox
  {
    get
    {
      Bounds bounds;
      ((Bounds) ref bounds).\u002Ector(((Component) this).get_transform().get_position(), Vector3.get_zero());
      if (Object.op_Implicit((Object) this.HullMesh))
      {
        this.ComputeVerts();
        if (this.vertsCW != null)
        {
          Matrix4x4 localToWorldMatrix = ((Component) this).get_transform().get_localToWorldMatrix();
          int length = this.vertsCW.Length;
          for (int index = 0; index < length; ++index)
            ((Bounds) ref bounds).Encapsulate(((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(this.vertsCW[index]));
        }
      }
      return bounds;
    }
  }

  public bool IsPointInHull(Vector3 p, float distanceTolerance)
  {
    this.ComputeVerts();
    Matrix4x4 worldToLocalMatrix = ((Component) this).get_transform().get_worldToLocalMatrix();
    Vector3 vector3_1 = ((Matrix4x4) ref worldToLocalMatrix).MultiplyPoint3x4(p);
    Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, Vector3.op_Multiply(Vector3.Dot(Vector3.op_Subtraction(vector3_1, this.meshCentroid), this.meshNormal), this.meshNormal));
    if (this.vertsCW == null || (double) Vector3.SqrMagnitude(Vector3.op_Subtraction(vector3_1, vector3_2)) >= (double) distanceTolerance * (double) distanceTolerance)
      return false;
    float num1 = 6.283185f;
    int length = this.vertsCW.Length;
    for (int index = 0; index < length; ++index)
    {
      Vector3 vector3_3 = Vector3.op_Subtraction(this.vertsCW[index], vector3_2);
      Vector3 vector3_4 = Vector3.op_Subtraction(this.vertsCW[(index + 1) % length], vector3_2);
      float num2 = ((Vector3) ref vector3_3).get_magnitude() * ((Vector3) ref vector3_4).get_magnitude();
      if ((double) num2 < 1.0 / 1000.0)
        return true;
      float num3 = Vector3.Dot(vector3_3, vector3_4) / num2;
      num1 -= Mathf.Acos(num3);
    }
    return (double) Mathf.Abs(num1) < 1.0 / 1000.0;
  }

  protected void ComputeVerts()
  {
    if (!Object.op_Inequality((Object) this.HullMesh, (Object) this.previousMesh))
      return;
    if (Object.op_Implicit((Object) this.HullMesh))
    {
      int vertexCount = this.HullMesh.get_vertexCount();
      this.vertsCW = new Vector3[vertexCount];
      this.meshCentroid = Vector3.get_zero();
      for (int index = 0; index < vertexCount; ++index)
      {
        Vector3 vertex = this.HullMesh.get_vertices()[index];
        this.vertsCW[index] = vertex;
        SECTR_Hull sectrHull = this;
        sectrHull.meshCentroid = Vector3.op_Addition(sectrHull.meshCentroid, vertex);
      }
      SECTR_Hull sectrHull1 = this;
      sectrHull1.meshCentroid = Vector3.op_Division(sectrHull1.meshCentroid, (float) this.HullMesh.get_vertexCount());
      this.meshNormal = Vector3.get_zero();
      int length = this.HullMesh.get_normals().Length;
      for (int index = 0; index < length; ++index)
      {
        SECTR_Hull sectrHull2 = this;
        sectrHull2.meshNormal = Vector3.op_Addition(sectrHull2.meshNormal, this.HullMesh.get_normals()[index]);
      }
      SECTR_Hull sectrHull3 = this;
      sectrHull3.meshNormal = Vector3.op_Division(sectrHull3.meshNormal, (float) this.HullMesh.get_normals().Length);
      ((Vector3) ref this.meshNormal).Normalize();
      bool flag = true;
      for (int index = 0; index < vertexCount; ++index)
      {
        Vector3 vector3_1 = this.vertsCW[index];
        Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, Vector3.op_Multiply(Vector3.Dot(Vector3.op_Subtraction(vector3_1, this.meshCentroid), this.meshNormal), this.meshNormal));
        flag = flag && (double) Vector3.SqrMagnitude(Vector3.op_Subtraction(vector3_1, vector3_2)) < 1.0 / 1000.0;
        this.vertsCW[index] = vector3_2;
      }
      if (!flag)
        Debug.LogWarning((object) ("Occluder mesh of " + ((Object) this).get_name() + " is not planar!"));
      Array.Sort<Vector3>(this.vertsCW, (Comparison<Vector3>) ((a, b) => SECTR_Geometry.CompareVectorsCW(a, b, this.meshCentroid, this.meshNormal) * -1));
      if (!SECTR_Geometry.IsPolygonConvex(this.vertsCW))
        Debug.LogWarning((object) ("Occluder mesh of " + ((Object) this).get_name() + " is not convex!"));
    }
    else
    {
      this.meshNormal = Vector3.get_zero();
      this.meshCentroid = Vector3.get_zero();
      this.vertsCW = (Vector3[]) null;
    }
    this.previousMesh = this.HullMesh;
  }
}
