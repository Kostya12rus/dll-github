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
  private List<SECTR_Graph.Node> path = new List<SECTR_Graph.Node>(16);
  private List<Vector3> waypoints = new List<Vector3>(16);
  [SECTR_ToolTip("The speed at which the wanderer moves throughout the world.")]
  public float MovementSpeed = 1f;
  private int currentWaypointIndex;

  private void Update()
  {
    if (this.waypoints.Count == 0 && SECTR_Sector.All.Count > 0 && (double) this.MovementSpeed > 0.0)
    {
      SECTR_Sector sectrSector = SECTR_Sector.All[Random.Range(0, SECTR_Sector.All.Count)];
      SECTR_Graph.FindShortestPath(ref this.path, this.transform.position, sectrSector.transform.position, SECTR_Portal.PortalFlags.Locked);
      Vector3 zero = Vector3.zero;
      Collider component = this.GetComponent<Collider>();
      if ((bool) ((Object) component))
        zero.y += component.bounds.extents.y;
      this.waypoints.Clear();
      int count = this.path.Count;
      for (int index = 0; index < count; ++index)
      {
        SECTR_Graph.Node node = this.path[index];
        this.waypoints.Add(node.Sector.transform.position + zero);
        if ((bool) ((Object) node.Portal))
          this.waypoints.Add(node.Portal.transform.position);
      }
      this.waypoints.Add(sectrSector.transform.position + zero);
      this.currentWaypointIndex = 0;
    }
    if (this.waypoints.Count <= 0 || (double) this.MovementSpeed <= 0.0)
      return;
    Vector3 vector3 = this.waypoints[this.currentWaypointIndex] - this.transform.position;
    float sqrMagnitude = vector3.sqrMagnitude;
    if ((double) sqrMagnitude > 1.0 / 1000.0)
    {
      float b = Mathf.Sqrt(sqrMagnitude);
      vector3 /= b;
      vector3 *= Mathf.Min(this.MovementSpeed * Time.deltaTime, b);
      this.transform.position += vector3;
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
