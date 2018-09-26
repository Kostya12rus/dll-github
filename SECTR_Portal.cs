// Decompiled with JetBrains decompiler
// Type: SECTR_Portal
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("SECTR/Core/SECTR Portal")]
public class SECTR_Portal : SECTR_Hull
{
  private static List<SECTR_Portal> allPortals = new List<SECTR_Portal>(128);
  [SerializeField]
  [HideInInspector]
  private SECTR_Sector frontSector;
  [HideInInspector]
  [SerializeField]
  private SECTR_Sector backSector;
  private bool visited;
  [SECTR_ToolTip("Flags for this Portal. Used in graph traversals and the like.", null, typeof (SECTR_Portal.PortalFlags))]
  public SECTR_Portal.PortalFlags Flags;

  public static List<SECTR_Portal> All
  {
    get
    {
      return SECTR_Portal.allPortals;
    }
  }

  public void Setup()
  {
    bool flag = Mathf.RoundToInt(Vector3.Angle(this.transform.forward, Vector3.forward)) % 180 == 0;
    this.transform.position += Vector3.up / 2f;
    RaycastHit hitInfo;
    if (Physics.Raycast(this.transform.position - this.transform.forward, Vector3.down, out hitInfo))
      this.FrontSector = hitInfo.collider.GetComponentInParent<SECTR_Sector>();
    if (!Physics.Raycast(this.transform.position + this.transform.forward, Vector3.down, out hitInfo))
      return;
    this.BackSector = hitInfo.collider.GetComponentInParent<SECTR_Sector>();
  }

  public Vector3 GetRandomSectorPos()
  {
    if (Random.Range(0, 100) < 50)
      return this.frontSector.transform.position;
    return this.backSector.transform.position;
  }

  public SECTR_Sector FrontSector
  {
    set
    {
      if (!((Object) this.frontSector != (Object) value))
        return;
      if ((bool) ((Object) this.frontSector))
        this.frontSector.Deregister(this);
      this.frontSector = value;
      if (!(bool) ((Object) this.frontSector))
        return;
      this.frontSector.Register(this);
    }
    get
    {
      if ((bool) ((Object) this.frontSector) && this.frontSector.enabled)
        return this.frontSector;
      return (SECTR_Sector) null;
    }
  }

  public SECTR_Sector BackSector
  {
    set
    {
      if (!((Object) this.backSector != (Object) value))
        return;
      if ((bool) ((Object) this.backSector))
        this.backSector.Deregister(this);
      this.backSector = value;
      if (!(bool) ((Object) this.backSector))
        return;
      this.backSector.Register(this);
    }
    get
    {
      if ((bool) ((Object) this.backSector) && this.backSector.enabled)
        return this.backSector;
      return (SECTR_Sector) null;
    }
  }

  public bool Visited
  {
    get
    {
      return this.visited;
    }
    set
    {
      this.visited = value;
    }
  }

  [DebuggerHidden]
  public IEnumerable<SECTR_Sector> GetSectors()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    SECTR_Portal.\u003CGetSectors\u003Ec__Iterator0 sectorsCIterator0 = new SECTR_Portal.\u003CGetSectors\u003Ec__Iterator0() { \u0024this = this };
    // ISSUE: reference to a compiler-generated field
    sectorsCIterator0.\u0024PC = -2;
    return (IEnumerable<SECTR_Sector>) sectorsCIterator0;
  }

  public void SetFlag(SECTR_Portal.PortalFlags flag, bool on)
  {
    if (on)
      this.Flags |= flag;
    else
      this.Flags &= ~flag;
  }

  private void OnEnable()
  {
    SECTR_Portal.allPortals.Add(this);
    if ((bool) ((Object) this.frontSector))
      this.frontSector.Register(this);
    if (!(bool) ((Object) this.backSector))
      return;
    this.backSector.Register(this);
  }

  private void OnDisable()
  {
    SECTR_Portal.allPortals.Remove(this);
    if ((bool) ((Object) this.frontSector))
      this.frontSector.Deregister(this);
    if (!(bool) ((Object) this.backSector))
      return;
    this.backSector.Deregister(this);
  }

  [System.Flags]
  public enum PortalFlags
  {
    Closed = 1,
    Locked = 2,
    PassThrough = 4,
  }
}
