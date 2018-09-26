// Decompiled with JetBrains decompiler
// Type: SECTR_Wanderer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SECTR/Demos/SECTR Wanderer")]
public class SECTR_Wanderer : MonoBehaviour
{
  private List<SECTR_Graph.Node> path;
  private List<Vector3> waypoints;
  private int currentWaypointIndex;
  [SECTR_ToolTip("The speed at which the wanderer moves throughout the world.")]
  public float MovementSpeed;

  public SECTR_Wanderer()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (this.waypoints.Count == 0 && SECTR_Sector.All.Count > 0 && (double) this.MovementSpeed > 0.0)
    {
      SECTR_Sector sectrSector = SECTR_Sector.All[Random.Range(0, SECTR_Sector.All.Count)];
      SECTR_Graph.FindShortestPath(ref this.path, ((Component) this).get_transform().get_position(), ((Component) sectrSector).get_transform().get_position(), SECTR_Portal.PortalFlags.Locked);
      Vector3 zero = Vector3.get_zero();
      Collider component = (Collider) ((Component) this).GetComponent<Collider>();
      if (Object.op_Implicit((Object) component))
      {
        ref Vector3 local = ref zero;
        // ISSUE: variable of the null type
        __Null y1 = local.y;
        Bounds bounds = component.get_bounds();
        // ISSUE: variable of the null type
        __Null y2 = ((Bounds) ref bounds).get_extents().y;
        local.y = y1 + y2;
      }
      this.waypoints.Clear();
      int count = this.path.Count;
      for (int index = 0; index < count; ++index)
      {
        SECTR_Graph.Node node = this.path[index];
        this.waypoints.Add(Vector3.op_Addition(((Component) node.Sector).get_transform().get_position(), zero));
        if (Object.op_Implicit((Object) node.Portal))
          this.waypoints.Add(((Component) node.Portal).get_transform().get_position());
      }
      this.waypoints.Add(Vector3.op_Addition(((Component) sectrSector).get_transform().get_position(), zero));
      this.currentWaypointIndex = 0;
    }
    if (this.waypoints.Count <= 0 || (double) this.MovementSpeed <= 0.0)
      return;
    Vector3 vector3 = Vector3.op_Subtraction(this.waypoints[this.currentWaypointIndex], ((Component) this).get_transform().get_position());
    float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
    if ((double) sqrMagnitude > 1.0 / 1000.0)
    {
      float num = Mathf.Sqrt(sqrMagnitude);
      vector3 = Vector3.op_Division(vector3, num);
      vector3 = Vector3.op_Multiply(vector3, Mathf.Min(this.MovementSpeed * Time.get_deltaTime(), num));
      Transform transform = ((Component) this).get_transform();
      transform.set_position(Vector3.op_Addition(transform.get_position(), vector3));
    }
    else
    {
      ++this.currentWaypointIndex;
      if (this.currentWaypointIndex < this.waypoints.Count)
        return;
      this.waypoints.Clear();
    }
  }
}
