// Decompiled with JetBrains decompiler
// Type: SECTR_Geometry
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public static class SECTR_Geometry
{
  public const float kVERTEX_EPSILON = 0.001f;
  public const float kBOUNDS_CHEAT = 0.01f;

  public static Bounds ComputeBounds(Light light)
  {
    Bounds bounds;
    if (Object.op_Implicit((Object) light))
    {
      switch ((int) light.get_type())
      {
        case 0:
          Vector3 position = ((Component) light).get_transform().get_position();
          ((Bounds) ref bounds).\u002Ector(position, Vector3.get_zero());
          Vector3 up = ((Component) light).get_transform().get_up();
          Vector3 right = ((Component) light).get_transform().get_right();
          Vector3 vector3_1 = Vector3.op_Addition(position, Vector3.op_Multiply(((Component) light).get_transform().get_forward(), light.get_range()));
          float num1 = Mathf.Tan((float) ((double) light.get_spotAngle() * 0.5 * (Math.PI / 180.0))) * light.get_range();
          ((Bounds) ref bounds).Encapsulate(vector3_1);
          Vector3 vector3_2 = Vector3.op_Addition(vector3_1, Vector3.op_Multiply(up, num1));
          Vector3 vector3_3 = Vector3.op_Addition(vector3_1, Vector3.op_Multiply(up, -num1));
          Vector3 vector3_4 = Vector3.op_Multiply(right, num1);
          Vector3 vector3_5 = Vector3.op_Multiply(right, -num1);
          ((Bounds) ref bounds).Encapsulate(Vector3.op_Addition(vector3_2, vector3_4));
          ((Bounds) ref bounds).Encapsulate(Vector3.op_Addition(vector3_2, vector3_5));
          ((Bounds) ref bounds).Encapsulate(Vector3.op_Addition(vector3_3, vector3_4));
          ((Bounds) ref bounds).Encapsulate(Vector3.op_Addition(vector3_3, vector3_5));
          break;
        case 2:
          float num2 = light.get_range() * 2f;
          ((Bounds) ref bounds).\u002Ector(((Component) light).get_transform().get_position(), new Vector3(num2, num2, num2));
          break;
        default:
          ((Bounds) ref bounds).\u002Ector(((Component) light).get_transform().get_position(), new Vector3(0.01f, 0.01f, 0.01f));
          break;
      }
    }
    else
      ((Bounds) ref bounds).\u002Ector(((Component) light).get_transform().get_position(), new Vector3(0.01f, 0.01f, 0.01f));
    return bounds;
  }

  public static Bounds ComputeBounds(Terrain terrain)
  {
    if (!Object.op_Implicit((Object) terrain))
      return (Bounds) null;
    Vector3 vector3_1 = !Object.op_Inequality((Object) terrain.get_terrainData(), (Object) null) ? Vector3.get_zero() : terrain.get_terrainData().get_size();
    Vector3 position = ((Component) terrain).get_transform().get_position();
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector((float) (position.x + vector3_1.x * 0.5), (float) (position.y + vector3_1.y * 0.5), (float) (position.z + vector3_1.z * 0.5));
    return new Bounds(vector3_2, vector3_1);
  }

  public static bool FrustumIntersectsBounds(Bounds bounds, List<Plane> frustum, int inMask, out int outMask)
  {
    Vector3 center = ((Bounds) ref bounds).get_center();
    Vector3 extents = ((Bounds) ref bounds).get_extents();
    outMask = 0;
    int index = 0;
    int num = 1;
    while (num <= inMask)
    {
      if ((num & inMask) != 0)
      {
        Plane plane = frustum[index];
        if ((double) ((float) (center.x * ((Plane) ref plane).get_normal().x + center.y * ((Plane) ref plane).get_normal().y + center.z * ((Plane) ref plane).get_normal().z) + ((Plane) ref plane).get_distance()) + (extents.x * (double) Mathf.Abs((float) ((Plane) ref plane).get_normal().x) + extents.y * (double) Mathf.Abs((float) ((Plane) ref plane).get_normal().y) + extents.z * (double) Mathf.Abs((float) ((Plane) ref plane).get_normal().z)) < 0.0)
          return false;
        outMask |= num;
      }
      ++index;
      num += num;
    }
    return true;
  }

  public static bool FrustumContainsBounds(Bounds bounds, List<Plane> frustum)
  {
    Vector3 center = ((Bounds) ref bounds).get_center();
    Vector3 extents = ((Bounds) ref bounds).get_extents();
    int count = frustum.Count;
    for (int index = 0; index < count; ++index)
    {
      Plane plane = frustum[index];
      float num1 = (float) (center.x * ((Plane) ref plane).get_normal().x + center.y * ((Plane) ref plane).get_normal().y + center.z * ((Plane) ref plane).get_normal().z) + ((Plane) ref plane).get_distance();
      float num2 = (float) (extents.x * (double) Mathf.Abs((float) ((Plane) ref plane).get_normal().x) + extents.y * (double) Mathf.Abs((float) ((Plane) ref plane).get_normal().y) + extents.z * (double) Mathf.Abs((float) ((Plane) ref plane).get_normal().z));
      if ((double) num1 + (double) num2 < 0.0 || (double) num1 - (double) num2 < 0.0)
        return false;
    }
    return true;
  }

  public static bool BoundsContainsBounds(Bounds container, Bounds contained)
  {
    if (((Bounds) ref container).Contains(((Bounds) ref contained).get_min()))
      return ((Bounds) ref container).Contains(((Bounds) ref contained).get_max());
    return false;
  }

  public static bool BoundsIntersectsSphere(Bounds bounds, Vector3 sphereCenter, float sphereRadius)
  {
    return (double) Vector3.SqrMagnitude(Vector3.op_Subtraction(Vector3.Min(Vector3.Max(sphereCenter, ((Bounds) ref bounds).get_min()), ((Bounds) ref bounds).get_max()), sphereCenter)) <= (double) sphereRadius * (double) sphereRadius;
  }

  public static Bounds ProjectBounds(Bounds bounds, Vector3 projection)
  {
    Vector3 vector3_1 = Vector3.op_Addition(((Bounds) ref bounds).get_min(), projection);
    Vector3 vector3_2 = Vector3.op_Addition(((Bounds) ref bounds).get_max(), projection);
    ((Bounds) ref bounds).Encapsulate(vector3_1);
    ((Bounds) ref bounds).Encapsulate(vector3_2);
    return bounds;
  }

  public static bool IsPointInFrontOfPlane(Vector3 position, Vector3 center, Vector3 normal)
  {
    Vector3 vector3 = Vector3.op_Subtraction(position, center);
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(normal, normalized) > 0.0;
  }

  public static bool IsPolygonConvex(Vector3[] verts)
  {
    int length = verts.Length;
    if (length < 3)
      return false;
    float num = (float) (length - 2) * 3.141593f;
    for (int index = 0; index < length; ++index)
    {
      Vector3 vert1 = verts[index];
      Vector3 vert2 = verts[(index + 1) % length];
      Vector3 vert3 = verts[(index + 2) % length];
      Vector3 vector3_1 = Vector3.op_Subtraction(vert1, vert2);
      ((Vector3) ref vector3_1).Normalize();
      Vector3 vector3_2 = Vector3.op_Subtraction(vert3, vert2);
      ((Vector3) ref vector3_2).Normalize();
      num -= Mathf.Acos(Vector3.Dot(vector3_1, vector3_2));
    }
    return (double) Mathf.Abs(num) < 1.0 / 1000.0;
  }

  public static int CompareVectorsCW(Vector3 a, Vector3 b, Vector3 centroid, Vector3 normal)
  {
    Vector3 vector3_1 = Vector3.Cross(Vector3.op_Subtraction(a, centroid), Vector3.op_Subtraction(b, centroid));
    float magnitude = ((Vector3) ref vector3_1).get_magnitude();
    if ((double) magnitude <= 1.0 / 1000.0)
      return 0;
    Vector3 vector3_2 = Vector3.op_Division(vector3_1, magnitude);
    return (double) Vector3.Dot(normal, vector3_2) > 0.0 ? 1 : -1;
  }
}
