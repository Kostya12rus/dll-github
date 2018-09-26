// Decompiled with JetBrains decompiler
// Type: SECTR_Member
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("SECTR/Core/SECTR Member")]
[ExecuteInEditMode]
public class SECTR_Member : MonoBehaviour
{
  private static List<SECTR_Member> allMembers = new List<SECTR_Member>(256);
  private static Dictionary<Transform, SECTR_Member> allMemberTable = new Dictionary<Transform, SECTR_Member>(256);
  [HideInInspector]
  [SerializeField]
  private List<SECTR_Member.Child> children = new List<SECTR_Member.Child>(16);
  [HideInInspector]
  [SerializeField]
  private List<SECTR_Member.Child> renderers = new List<SECTR_Member.Child>(16);
  [SerializeField]
  [HideInInspector]
  private List<SECTR_Member.Child> lights = new List<SECTR_Member.Child>(16);
  [HideInInspector]
  [SerializeField]
  private List<SECTR_Member.Child> terrains = new List<SECTR_Member.Child>(2);
  [SerializeField]
  [HideInInspector]
  private List<SECTR_Member.Child> shadowLights = !SECTR_Modules.VIS ? (List<SECTR_Member.Child>) null : new List<SECTR_Member.Child>(16);
  [SerializeField]
  [HideInInspector]
  private List<SECTR_Member.Child> shadowCasters = !SECTR_Modules.VIS ? (List<SECTR_Member.Child>) null : new List<SECTR_Member.Child>(16);
  [HideInInspector]
  [SerializeField]
  protected List<Light> bakedOnlyLights = !SECTR_Modules.VIS ? (List<Light>) null : new List<Light>(8);
  private List<SECTR_Sector> sectors = new List<SECTR_Sector>(4);
  private List<SECTR_Sector> newSectors = new List<SECTR_Sector>(4);
  private List<SECTR_Sector> leftSectors = new List<SECTR_Sector>(4);
  private Dictionary<Transform, SECTR_Member.Child> childTable = new Dictionary<Transform, SECTR_Member.Child>(8);
  private Vector3 lastPosition = Vector3.zero;
  private Stack<SECTR_Member.Child> childPool = new Stack<SECTR_Member.Child>(32);
  [SECTR_ToolTip("Determines how often the bounds are recomputed. More frequent updates requires more CPU.")]
  public SECTR_Member.BoundsUpdateModes BoundsUpdateMode = SECTR_Member.BoundsUpdateModes.Always;
  [SECTR_ToolTip("Adds a buffer on bounding box to compensate for minor imprecisions.")]
  public float ExtraBounds = 0.01f;
  [SECTR_ToolTip("Distance by which to extend the bounds away from the shadow casting light.", "DirShadowCaster")]
  public float DirShadowDistance = 100f;
  [HideInInspector]
  [SerializeField]
  private Bounds totalBounds;
  [HideInInspector]
  [SerializeField]
  private Bounds renderBounds;
  [HideInInspector]
  [SerializeField]
  private Bounds lightBounds;
  [HideInInspector]
  [SerializeField]
  private bool hasRenderBounds;
  [SerializeField]
  [HideInInspector]
  private bool hasLightBounds;
  [SerializeField]
  [HideInInspector]
  private bool shadowCaster;
  [HideInInspector]
  [SerializeField]
  private bool shadowLight;
  [HideInInspector]
  [SerializeField]
  private bool frozen;
  [HideInInspector]
  [SerializeField]
  private bool neverJoin;
  protected bool isSector;
  protected SECTR_Member childProxy;
  protected bool hasChildProxy;
  private bool started;
  private bool usedStartSector;
  private Dictionary<Light, Light> bakedOnlyTable;
  [SECTR_ToolTip("Set to true if Sector membership should only change when crossing a portal.")]
  public bool PortalDetermined;
  [SECTR_ToolTip("If set, forces the initial Sector to be the specified Sector.", "PortalDetermined")]
  public SECTR_Sector ForceStartSector;
  [SECTR_ToolTip("Override computed bounds with the user specified bounds. Advanced users only.")]
  public bool OverrideBounds;
  [SECTR_ToolTip("User specified override bounds. Auto-populated with the current bounds when override is inactive.", "OverrideBounds")]
  public Bounds BoundsOverride;
  [SECTR_ToolTip("Optional shadow casting directional light to use in membership calculations. Bounds will be extruded away from light, if set.")]
  public Light DirShadowCaster;
  [SECTR_ToolTip("Determines if this SectorCuller should cull individual children, or cull all children based on the aggregate bounds.")]
  public SECTR_Member.ChildCullModes ChildCulling;
  [HideInInspector]
  [NonSerialized]
  public int LastVisibleFrameNumber;

  public static List<SECTR_Member> All
  {
    get
    {
      return SECTR_Member.allMembers;
    }
  }

  public bool CullEachChild
  {
    get
    {
      if (this.ChildCulling == SECTR_Member.ChildCullModes.Individual)
        return true;
      if (this.ChildCulling == SECTR_Member.ChildCullModes.Default)
        return this.isSector;
      return false;
    }
  }

  public List<SECTR_Sector> Sectors
  {
    get
    {
      return this.sectors;
    }
  }

  public List<SECTR_Member.Child> Children
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.children;
      return this.children;
    }
  }

  public List<SECTR_Member.Child> Renderers
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.renderers;
      return this.renderers;
    }
  }

  public bool ShadowCaster
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.shadowCaster;
      return this.shadowCaster;
    }
  }

  public List<SECTR_Member.Child> ShadowCasters
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.shadowCasters;
      return this.shadowCasters;
    }
  }

  public List<SECTR_Member.Child> Lights
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.lights;
      return this.lights;
    }
  }

  public bool ShadowLight
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.shadowLight;
      return this.shadowLight;
    }
  }

  public List<SECTR_Member.Child> ShadowLights
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.shadowLights;
      return this.shadowLights;
    }
  }

  public List<SECTR_Member.Child> Terrains
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.terrains;
      return this.terrains;
    }
  }

  public Bounds TotalBounds
  {
    get
    {
      return this.totalBounds;
    }
  }

  public Bounds RenderBounds
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.renderBounds;
      return this.renderBounds;
    }
  }

  public bool HasRenderBounds
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.hasRenderBounds;
      return this.hasRenderBounds;
    }
  }

  public Bounds LightBounds
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.lightBounds;
      return this.lightBounds;
    }
  }

  public bool HasLightBounds
  {
    get
    {
      if (this.hasChildProxy)
        return this.childProxy.hasLightBounds;
      return this.hasLightBounds;
    }
  }

  public bool IsVisibleThisFrame()
  {
    return this.LastVisibleFrameNumber == Time.frameCount;
  }

  public bool WasVisibleLastFrame()
  {
    return this.LastVisibleFrameNumber == Time.frameCount - 1;
  }

  public bool Frozen
  {
    set
    {
      if (!this.isSector)
        return;
      this.frozen = value;
    }
    get
    {
      return this.frozen;
    }
  }

  public SECTR_Member ChildProxy
  {
    set
    {
      this.childProxy = value;
      this.hasChildProxy = (Object) this.childProxy != (Object) null;
    }
  }

  public bool NeverJoin
  {
    set
    {
      this.neverJoin = true;
    }
  }

  public bool IsSector
  {
    get
    {
      return this.isSector;
    }
  }

  public void ForceUpdate(bool updateChildren)
  {
    if (updateChildren)
      this._UpdateChildren();
    this.lastPosition = this.transform.position;
    if (this.isSector || this.neverJoin)
      return;
    this._UpdateSectorMembership();
  }

  public void SectorDisabled(SECTR_Sector sector)
  {
    if (!(bool) ((Object) sector))
      return;
    this.sectors.Remove(sector);
    // ISSUE: reference to a compiler-generated field
    if (this.Changed == null)
      return;
    this.leftSectors.Clear();
    this.leftSectors.Add(sector);
    // ISSUE: reference to a compiler-generated field
    this.Changed(this.leftSectors, (List<SECTR_Sector>) null);
  }

  public event SECTR_Member.MembershipChanged Changed;

  private void Start()
  {
    this.started = true;
    this.ForceUpdate(this.children.Count == 0);
  }

  protected virtual void OnEnable()
  {
    SECTR_Member.allMembers.Add(this);
    SECTR_Member.allMemberTable.Add(this.transform, this);
    if (this.bakedOnlyLights != null)
    {
      int count = this.bakedOnlyLights.Count;
      this.bakedOnlyTable = new Dictionary<Light, Light>(count);
      for (int index = 0; index < count; ++index)
      {
        Light bakedOnlyLight = this.bakedOnlyLights[index];
        if ((bool) ((Object) bakedOnlyLight))
          this.bakedOnlyTable[bakedOnlyLight] = bakedOnlyLight;
      }
    }
    if (!this.started)
      return;
    this.ForceUpdate(true);
  }

  protected virtual void OnDisable()
  {
    // ISSUE: reference to a compiler-generated field
    if (this.Changed != null && this.sectors.Count > 0)
    {
      // ISSUE: reference to a compiler-generated field
      this.Changed(this.sectors, (List<SECTR_Sector>) null);
    }
    if (!this.isSector && !this.neverJoin)
    {
      int count = this.sectors.Count;
      for (int index = 0; index < count; ++index)
      {
        SECTR_Sector sector = this.sectors[index];
        if ((bool) ((Object) sector))
          sector.Deregister(this);
      }
      this.sectors.Clear();
    }
    int count1 = this.children.Count;
    for (int index = 0; index < count1; ++index)
    {
      SECTR_Member.Child child = this.children[index];
      child.processed = false;
      this.childPool.Push(child);
    }
    this.children.Clear();
    this.childTable.Clear();
    this.renderers.Clear();
    this.lights.Clear();
    this.terrains.Clear();
    if (SECTR_Modules.VIS)
    {
      this.shadowLights.Clear();
      this.shadowCasters.Clear();
    }
    this.bakedOnlyTable = (Dictionary<Light, Light>) null;
    SECTR_Member.allMembers.Remove(this);
    SECTR_Member.allMemberTable.Remove(this.transform);
  }

  private void LateUpdate()
  {
    if (this.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Static || this.BoundsUpdateMode != SECTR_Member.BoundsUpdateModes.Always && !this.transform.hasChanged)
      return;
    this._UpdateChildren();
    if (!this.isSector && !this.neverJoin)
      this._UpdateSectorMembership();
    this.lastPosition = this.transform.position;
    this.transform.hasChanged = false;
  }

  public void UpdateViaScript()
  {
    this._UpdateChildren();
    if (!this.isSector && !this.neverJoin)
      this._UpdateSectorMembership();
    this.lastPosition = this.transform.position;
  }

  private void _UpdateChildren()
  {
    if (this.frozen || (bool) ((Object) this.childProxy))
      return;
    bool dirShadowCaster = SECTR_Modules.VIS && (bool) ((Object) this.DirShadowCaster) && this.DirShadowCaster.type == LightType.Directional && this.DirShadowCaster.shadows != LightShadows.None;
    Vector3 shadowVec = !dirShadowCaster ? Vector3.zero : this.DirShadowCaster.transform.forward * this.DirShadowDistance;
    int count1 = this.children.Count;
    int index1 = 0;
    this.hasLightBounds = false;
    this.hasRenderBounds = false;
    this.shadowCaster = false;
    this.shadowLight = false;
    this.renderers.Clear();
    this.lights.Clear();
    this.terrains.Clear();
    if (SECTR_Modules.VIS)
    {
      this.shadowCasters.Clear();
      this.shadowLights.Clear();
    }
    if ((this.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Start || this.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.SelfOnly) && count1 > 0)
    {
      while (index1 < count1)
      {
        SECTR_Member.Child child = this.children[index1];
        if (!(bool) ((Object) child.gameObject))
        {
          this.children.RemoveAt(index1);
          --count1;
        }
        else
        {
          child.Init(child.gameObject, child.renderer, child.light, child.terrain, child.member, dirShadowCaster, shadowVec);
          if ((bool) ((Object) child.renderer))
          {
            if (!this.hasRenderBounds)
            {
              this.renderBounds = child.rendererBounds;
              this.hasRenderBounds = true;
            }
            else
              this.renderBounds.Encapsulate(child.rendererBounds);
            this.renderers.Add(child);
          }
          if ((bool) ((Object) child.terrain))
          {
            if (!this.hasRenderBounds)
            {
              this.renderBounds = child.terrainBounds;
              this.hasRenderBounds = true;
            }
            else
              this.renderBounds.Encapsulate(child.terrainBounds);
            this.terrains.Add(child);
          }
          if ((bool) ((Object) child.light))
          {
            if (SECTR_Modules.VIS && child.shadowLight)
            {
              this.shadowLights.Add(child);
              this.shadowLight = true;
            }
            if (!this.hasLightBounds)
            {
              this.lightBounds = child.lightBounds;
              this.hasLightBounds = true;
            }
            else
              this.lightBounds.Encapsulate(child.lightBounds);
            this.lights.Add(child);
          }
          if (SECTR_Modules.VIS && (child.terrainCastsShadows || child.rendererCastsShadows))
          {
            this.shadowCasters.Add(child);
            this.shadowCaster = true;
          }
          ++index1;
        }
      }
    }
    else
    {
      for (int index2 = 0; index2 < count1; ++index2)
        this.children[index2].processed = false;
      this._AddChildren(this.transform, dirShadowCaster, shadowVec);
      int index3 = 0;
      int count2 = this.children.Count;
      while (index3 < count2)
      {
        SECTR_Member.Child child = this.children[index3];
        if (!child.processed)
        {
          this.childPool.Push(child);
          this.children.RemoveAt(index3);
          --count2;
        }
        else
          ++index3;
      }
    }
    Bounds bounds = new Bounds(this.transform.position, Vector3.zero);
    if (this.hasRenderBounds && (this.isSector || this.neverJoin))
      this.totalBounds = this.renderBounds;
    else if (this.hasRenderBounds && this.hasLightBounds)
    {
      this.totalBounds = this.renderBounds;
      this.totalBounds.Encapsulate(this.lightBounds);
    }
    else if (this.hasRenderBounds)
    {
      this.totalBounds = this.renderBounds;
      this.lightBounds = bounds;
    }
    else if (this.hasLightBounds)
    {
      this.totalBounds = this.lightBounds;
      this.renderBounds = bounds;
    }
    else
    {
      this.totalBounds = bounds;
      this.lightBounds = bounds;
      this.renderBounds = bounds;
    }
    this.totalBounds.Expand(this.ExtraBounds);
    if (!this.OverrideBounds)
      return;
    this.totalBounds = this.BoundsOverride;
  }

  private void _AddChildren(Transform childTransform, bool dirShadowCaster, Vector3 shadowVec)
  {
    if (!childTransform.gameObject.activeSelf || !((Object) childTransform == (Object) this.transform) && SECTR_Member.allMemberTable.ContainsKey(childTransform))
      return;
    SECTR_Member.Child child1 = (SECTR_Member.Child) null;
    this.childTable.TryGetValue(childTransform, out child1);
    Light light = !(child1 != (SECTR_Member.Child) null) ? childTransform.GetComponent<Light>() : child1.light;
    Renderer renderer = !(child1 != (SECTR_Member.Child) null) ? childTransform.GetComponent<Renderer>() : child1.renderer;
    Terrain terrain = (Terrain) null;
    if (this.isSector || this.neverJoin)
      terrain = !(child1 != (SECTR_Member.Child) null) ? childTransform.GetComponent<Terrain>() : child1.terrain;
    if (this.bakedOnlyLights != null && (bool) ((Object) light) && (light.bakingOutput.isBaked && LightmapSettings.lightmaps.Length > 0 && (this.bakedOnlyTable != null && this.bakedOnlyTable.ContainsKey(light))))
      light = (Light) null;
    SECTR_Member.Child child2 = child1;
    if (child2 == (SECTR_Member.Child) null)
    {
      child2 = this.childPool.Count <= 0 ? new SECTR_Member.Child() : this.childPool.Pop();
      this.childTable[childTransform] = child2;
      this.children.Add(child2);
    }
    child2.Init(childTransform.gameObject, renderer, light, terrain, this, dirShadowCaster, shadowVec);
    if ((bool) ((Object) child2.renderer))
    {
      bool flag = true;
      if (this.isSector && renderer.GetType() == typeof (ParticleSystemRenderer))
        flag = false;
      if (flag)
      {
        if (!this.hasRenderBounds)
        {
          this.renderBounds = child2.rendererBounds;
          this.hasRenderBounds = true;
        }
        else
          this.renderBounds.Encapsulate(child2.rendererBounds);
      }
      this.renderers.Add(child2);
    }
    if ((bool) ((Object) child2.light))
    {
      if (SECTR_Modules.VIS && child2.shadowLight)
      {
        this.shadowLights.Add(child2);
        this.shadowLight = true;
      }
      if (!this.hasLightBounds)
      {
        this.lightBounds = child2.lightBounds;
        this.hasLightBounds = true;
      }
      else
        this.lightBounds.Encapsulate(child2.lightBounds);
      this.lights.Add(child2);
    }
    if ((bool) ((Object) child2.terrain))
    {
      if (!this.hasRenderBounds)
      {
        this.renderBounds = child2.terrainBounds;
        this.hasRenderBounds = true;
      }
      else
        this.renderBounds.Encapsulate(child2.terrainBounds);
      this.terrains.Add(child2);
    }
    if (SECTR_Modules.VIS && (child2.terrainCastsShadows || child2.rendererCastsShadows))
    {
      this.shadowCasters.Add(child2);
      this.shadowCaster = true;
    }
    if (this.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.SelfOnly)
      return;
    int childCount = childTransform.transform.childCount;
    for (int index = 0; index < childCount; ++index)
      this._AddChildren(childTransform.GetChild(index), dirShadowCaster, shadowVec);
  }

  private void _UpdateSectorMembership()
  {
    if (this.frozen || this.isSector || this.neverJoin)
      return;
    this.newSectors.Clear();
    this.leftSectors.Clear();
    if (this.PortalDetermined && this.sectors.Count > 0)
    {
      int count1 = this.sectors.Count;
      for (int index = 0; index < count1; ++index)
      {
        SECTR_Sector sector = this.sectors[index];
        SECTR_Portal sectrPortal = this._CrossedPortal(sector);
        if ((bool) ((Object) sectrPortal))
        {
          SECTR_Sector sectrSector = !((Object) sectrPortal.FrontSector == (Object) sector) ? sectrPortal.FrontSector : sectrPortal.BackSector;
          if (!this.newSectors.Contains(sectrSector))
            this.newSectors.Add(sectrSector);
          this.leftSectors.Add(sector);
        }
      }
      int count2 = this.newSectors.Count;
      for (int index = 0; index < count2; ++index)
      {
        SECTR_Sector newSector = this.newSectors[index];
        newSector.Register(this);
        this.sectors.Add(newSector);
      }
      int count3 = this.leftSectors.Count;
      for (int index = 0; index < count3; ++index)
      {
        SECTR_Sector leftSector = this.leftSectors[index];
        leftSector.Deregister(this);
        this.sectors.Remove(leftSector);
      }
    }
    else if (this.PortalDetermined && (bool) ((Object) this.ForceStartSector) && !this.usedStartSector)
    {
      this.ForceStartSector.Register(this);
      this.sectors.Add(this.ForceStartSector);
      this.newSectors.Add(this.ForceStartSector);
      this.usedStartSector = true;
    }
    else
    {
      SECTR_Sector.GetContaining(ref this.newSectors, this.TotalBounds);
      int index1 = 0;
      int count1 = this.sectors.Count;
      while (index1 < count1)
      {
        SECTR_Sector sector = this.sectors[index1];
        if (this.newSectors.Contains(sector))
        {
          this.newSectors.Remove(sector);
          ++index1;
        }
        else
        {
          sector.Deregister(this);
          this.leftSectors.Add(sector);
          this.sectors.RemoveAt(index1);
          --count1;
        }
      }
      int count2 = this.newSectors.Count;
      if (count2 > 0)
      {
        for (int index2 = 0; index2 < count2; ++index2)
        {
          SECTR_Sector newSector = this.newSectors[index2];
          newSector.Register(this);
          this.sectors.Add(newSector);
        }
      }
    }
    // ISSUE: reference to a compiler-generated field
    if (this.Changed == null || this.leftSectors.Count <= 0 && this.newSectors.Count <= 0)
      return;
    // ISSUE: reference to a compiler-generated field
    this.Changed(this.leftSectors, this.newSectors);
  }

  private SECTR_Portal _CrossedPortal(SECTR_Sector sector)
  {
    if ((bool) ((Object) sector))
    {
      Vector3 lhs = this.transform.position - this.lastPosition;
      int count = sector.Portals.Count;
      for (int index = 0; index < count; ++index)
      {
        SECTR_Portal portal = sector.Portals[index];
        if ((bool) ((Object) portal))
        {
          bool flag = (Object) portal.FrontSector == (Object) sector;
          Plane plane = !flag ? portal.ReverseHullPlane : portal.HullPlane;
          if ((bool) (!flag ? (Object) portal.FrontSector : (Object) portal.BackSector) && (double) Vector3.Dot(lhs, plane.normal) < 0.0 && (plane.GetSide(this.transform.position) != plane.GetSide(this.lastPosition) && portal.IsPointInHull(this.transform.position, lhs.magnitude)))
            return portal;
        }
      }
    }
    return (SECTR_Portal) null;
  }

  [Serializable]
  public class Child
  {
    public GameObject gameObject;
    public int gameObjectHash;
    public SECTR_Member member;
    public Renderer renderer;
    public int renderHash;
    public Light light;
    public int lightHash;
    public Terrain terrain;
    public int terrainHash;
    public Bounds rendererBounds;
    public Bounds lightBounds;
    public Bounds terrainBounds;
    public bool shadowLight;
    public bool rendererCastsShadows;
    public bool terrainCastsShadows;
    public LayerMask layer;
    public Vector3 shadowLightPosition;
    public float shadowLightRange;
    public LightType shadowLightType;
    public int shadowCullingMask;
    public bool processed;
    public bool renderCulled;
    public bool lightCulled;
    public bool terrainCulled;

    public void Init(GameObject gameObject, Renderer renderer, Light light, Terrain terrain, SECTR_Member member, bool dirShadowCaster, Vector3 shadowVec)
    {
      this.gameObject = gameObject;
      this.gameObjectHash = this.gameObject.GetInstanceID();
      this.member = member;
      this.renderer = !(bool) ((Object) renderer) || !this.renderCulled && !renderer.enabled ? (Renderer) null : renderer;
      this.light = !(bool) ((Object) light) || !this.lightCulled && !light.enabled || light.type != LightType.Point && light.type != LightType.Spot ? (Light) null : light;
      this.terrain = !(bool) ((Object) terrain) || !this.terrainCulled && !terrain.enabled ? (Terrain) null : terrain;
      this.rendererBounds = !(bool) ((Object) this.renderer) ? new Bounds() : this.renderer.bounds;
      this.lightBounds = !(bool) ((Object) this.light) ? new Bounds() : SECTR_Geometry.ComputeBounds(this.light);
      this.terrainBounds = !(bool) ((Object) this.terrain) ? new Bounds() : SECTR_Geometry.ComputeBounds(this.terrain);
      this.layer = (LayerMask) gameObject.layer;
      if (SECTR_Modules.VIS)
      {
        this.renderHash = !(bool) ((Object) this.renderer) ? 0 : this.renderer.GetInstanceID();
        this.lightHash = !(bool) ((Object) this.light) ? 0 : this.light.GetInstanceID();
        this.terrainHash = !(bool) ((Object) this.terrain) ? 0 : this.terrain.GetInstanceID();
        bool flag = true;
        this.shadowLight = (bool) ((Object) this.light) && light.shadows != LightShadows.None && (!light.bakingOutput.isBaked || flag);
        this.rendererCastsShadows = (bool) ((Object) this.renderer) && renderer.shadowCastingMode != ShadowCastingMode.Off && (renderer.lightmapIndex == -1 || flag);
        this.terrainCastsShadows = (bool) ((Object) this.terrain) && terrain.castShadows && (terrain.lightmapIndex == -1 || flag);
        if (dirShadowCaster)
        {
          if (this.rendererCastsShadows)
            this.rendererBounds = SECTR_Geometry.ProjectBounds(this.rendererBounds, shadowVec);
          if (this.terrainCastsShadows)
            this.terrainBounds = SECTR_Geometry.ProjectBounds(this.terrainBounds, shadowVec);
        }
        if (this.shadowLight)
        {
          this.shadowLightPosition = light.transform.position;
          this.shadowLightRange = light.range;
          this.shadowLightType = light.type;
          this.shadowCullingMask = light.cullingMask;
        }
        else
        {
          this.shadowLightPosition = Vector3.zero;
          this.shadowLightRange = 0.0f;
          this.shadowLightType = LightType.Area;
          this.shadowCullingMask = 0;
        }
      }
      else
      {
        this.renderHash = 0;
        this.lightHash = 0;
        this.terrainHash = 0;
        this.shadowLight = false;
        this.rendererCastsShadows = false;
        this.terrainCastsShadows = false;
        this.shadowLightPosition = Vector3.zero;
        this.shadowLightRange = 0.0f;
        this.shadowLightType = LightType.Area;
        this.shadowCullingMask = 0;
      }
      this.processed = true;
    }

    public override bool Equals(object obj)
    {
      if ((object) (obj as SECTR_Member.Child) != null)
        return this == (SECTR_Member.Child) obj;
      return false;
    }

    public override int GetHashCode()
    {
      return this.gameObjectHash;
    }

    public static bool operator ==(SECTR_Member.Child x, SECTR_Member.Child y)
    {
      if ((object) x == null && (object) y == null)
        return true;
      if ((object) x == null || (object) y == null)
        return false;
      return x.gameObjectHash == y.gameObjectHash;
    }

    public static bool operator !=(SECTR_Member.Child x, SECTR_Member.Child y)
    {
      return !(x == y);
    }
  }

  public enum BoundsUpdateModes
  {
    Start,
    Movement,
    Always,
    Static,
    SelfOnly,
  }

  public enum ChildCullModes
  {
    Default,
    Group,
    Individual,
  }

  public delegate void MembershipChanged(List<SECTR_Sector> left, List<SECTR_Sector> joined);
}
