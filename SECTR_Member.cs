// Decompiled with JetBrains decompiler
// Type: SECTR_Member
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SECTR/Core/SECTR Member")]
[ExecuteInEditMode]
public class SECTR_Member : MonoBehaviour
{
  private static List<SECTR_Member> allMembers = new List<SECTR_Member>(256);
  private static Dictionary<Transform, SECTR_Member> allMemberTable = new Dictionary<Transform, SECTR_Member>(256);
  [HideInInspector]
  [SerializeField]
  private List<SECTR_Member.Child> children;
  [HideInInspector]
  [SerializeField]
  private List<SECTR_Member.Child> renderers;
  [SerializeField]
  [HideInInspector]
  private List<SECTR_Member.Child> lights;
  [HideInInspector]
  [SerializeField]
  private List<SECTR_Member.Child> terrains;
  [SerializeField]
  [HideInInspector]
  private List<SECTR_Member.Child> shadowLights;
  [SerializeField]
  [HideInInspector]
  private List<SECTR_Member.Child> shadowCasters;
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
  [HideInInspector]
  [SerializeField]
  protected List<Light> bakedOnlyLights;
  protected bool isSector;
  protected SECTR_Member childProxy;
  protected bool hasChildProxy;
  private bool started;
  private bool usedStartSector;
  private List<SECTR_Sector> sectors;
  private List<SECTR_Sector> newSectors;
  private List<SECTR_Sector> leftSectors;
  private Dictionary<Transform, SECTR_Member.Child> childTable;
  private Dictionary<Light, Light> bakedOnlyTable;
  private Vector3 lastPosition;
  private Stack<SECTR_Member.Child> childPool;
  [SECTR_ToolTip("Set to true if Sector membership should only change when crossing a portal.")]
  public bool PortalDetermined;
  [SECTR_ToolTip("If set, forces the initial Sector to be the specified Sector.", "PortalDetermined")]
  public SECTR_Sector ForceStartSector;
  [SECTR_ToolTip("Determines how often the bounds are recomputed. More frequent updates requires more CPU.")]
  public SECTR_Member.BoundsUpdateModes BoundsUpdateMode;
  [SECTR_ToolTip("Adds a buffer on bounding box to compensate for minor imprecisions.")]
  public float ExtraBounds;
  [SECTR_ToolTip("Override computed bounds with the user specified bounds. Advanced users only.")]
  public bool OverrideBounds;
  [SECTR_ToolTip("User specified override bounds. Auto-populated with the current bounds when override is inactive.", "OverrideBounds")]
  public Bounds BoundsOverride;
  [SECTR_ToolTip("Optional shadow casting directional light to use in membership calculations. Bounds will be extruded away from light, if set.")]
  public Light DirShadowCaster;
  [SECTR_ToolTip("Distance by which to extend the bounds away from the shadow casting light.", "DirShadowCaster")]
  public float DirShadowDistance;
  [SECTR_ToolTip("Determines if this SectorCuller should cull individual children, or cull all children based on the aggregate bounds.")]
  public SECTR_Member.ChildCullModes ChildCulling;
  [HideInInspector]
  [NonSerialized]
  public int LastVisibleFrameNumber;

  public SECTR_Member()
  {
    base.\u002Ector();
  }

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
    return this.LastVisibleFrameNumber == Time.get_frameCount();
  }

  public bool WasVisibleLastFrame()
  {
    return this.LastVisibleFrameNumber == Time.get_frameCount() - 1;
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
      this.hasChildProxy = Object.op_Inequality((Object) this.childProxy, (Object) null);
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
    this.lastPosition = ((Component) this).get_transform().get_position();
    if (this.isSector || this.neverJoin)
      return;
    this._UpdateSectorMembership();
  }

  public void SectorDisabled(SECTR_Sector sector)
  {
    if (!Object.op_Implicit((Object) sector))
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
    SECTR_Member.allMemberTable.Add(((Component) this).get_transform(), this);
    if (this.bakedOnlyLights != null)
    {
      int count = this.bakedOnlyLights.Count;
      this.bakedOnlyTable = new Dictionary<Light, Light>(count);
      for (int index = 0; index < count; ++index)
      {
        Light bakedOnlyLight = this.bakedOnlyLights[index];
        if (Object.op_Implicit((Object) bakedOnlyLight))
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
        if (Object.op_Implicit((Object) sector))
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
    SECTR_Member.allMemberTable.Remove(((Component) this).get_transform());
  }

  private void LateUpdate()
  {
    if (this.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Static || this.BoundsUpdateMode != SECTR_Member.BoundsUpdateModes.Always && !((Component) this).get_transform().get_hasChanged())
      return;
    this._UpdateChildren();
    if (!this.isSector && !this.neverJoin)
      this._UpdateSectorMembership();
    this.lastPosition = ((Component) this).get_transform().get_position();
    ((Component) this).get_transform().set_hasChanged(false);
  }

  public void UpdateViaScript()
  {
    this._UpdateChildren();
    if (!this.isSector && !this.neverJoin)
      this._UpdateSectorMembership();
    this.lastPosition = ((Component) this).get_transform().get_position();
  }

  private void _UpdateChildren()
  {
    if (this.frozen || Object.op_Implicit((Object) this.childProxy))
      return;
    bool dirShadowCaster = SECTR_Modules.VIS && Object.op_Implicit((Object) this.DirShadowCaster) && this.DirShadowCaster.get_type() == 1 && this.DirShadowCaster.get_shadows() != 0;
    Vector3 shadowVec = !dirShadowCaster ? Vector3.get_zero() : Vector3.op_Multiply(((Component) this.DirShadowCaster).get_transform().get_forward(), this.DirShadowDistance);
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
        if (!Object.op_Implicit((Object) child.gameObject))
        {
          this.children.RemoveAt(index1);
          --count1;
        }
        else
        {
          child.Init(child.gameObject, child.renderer, child.light, child.terrain, child.member, dirShadowCaster, shadowVec);
          if (Object.op_Implicit((Object) child.renderer))
          {
            if (!this.hasRenderBounds)
            {
              this.renderBounds = child.rendererBounds;
              this.hasRenderBounds = true;
            }
            else
              ((Bounds) ref this.renderBounds).Encapsulate(child.rendererBounds);
            this.renderers.Add(child);
          }
          if (Object.op_Implicit((Object) child.terrain))
          {
            if (!this.hasRenderBounds)
            {
              this.renderBounds = child.terrainBounds;
              this.hasRenderBounds = true;
            }
            else
              ((Bounds) ref this.renderBounds).Encapsulate(child.terrainBounds);
            this.terrains.Add(child);
          }
          if (Object.op_Implicit((Object) child.light))
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
              ((Bounds) ref this.lightBounds).Encapsulate(child.lightBounds);
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
      this._AddChildren(((Component) this).get_transform(), dirShadowCaster, shadowVec);
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
    Bounds bounds;
    ((Bounds) ref bounds).\u002Ector(((Component) this).get_transform().get_position(), Vector3.get_zero());
    if (this.hasRenderBounds && (this.isSector || this.neverJoin))
      this.totalBounds = this.renderBounds;
    else if (this.hasRenderBounds && this.hasLightBounds)
    {
      this.totalBounds = this.renderBounds;
      ((Bounds) ref this.totalBounds).Encapsulate(this.lightBounds);
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
    ((Bounds) ref this.totalBounds).Expand(this.ExtraBounds);
    if (!this.OverrideBounds)
      return;
    this.totalBounds = this.BoundsOverride;
  }

  private void _AddChildren(Transform childTransform, bool dirShadowCaster, Vector3 shadowVec)
  {
    if (!((Component) childTransform).get_gameObject().get_activeSelf() || !Object.op_Equality((Object) childTransform, (Object) ((Component) this).get_transform()) && SECTR_Member.allMemberTable.ContainsKey(childTransform))
      return;
    SECTR_Member.Child child1 = (SECTR_Member.Child) null;
    this.childTable.TryGetValue(childTransform, out child1);
    Light light = !(child1 != (SECTR_Member.Child) null) ? (Light) ((Component) childTransform).GetComponent<Light>() : child1.light;
    Renderer renderer = !(child1 != (SECTR_Member.Child) null) ? (Renderer) ((Component) childTransform).GetComponent<Renderer>() : child1.renderer;
    Terrain terrain = (Terrain) null;
    if (this.isSector || this.neverJoin)
      terrain = !(child1 != (SECTR_Member.Child) null) ? (Terrain) ((Component) childTransform).GetComponent<Terrain>() : child1.terrain;
    if (this.bakedOnlyLights != null && Object.op_Implicit((Object) light) && (light.get_bakingOutput().isBaked != null && LightmapSettings.get_lightmaps().Length > 0 && (this.bakedOnlyTable != null && this.bakedOnlyTable.ContainsKey(light))))
      light = (Light) null;
    SECTR_Member.Child child2 = child1;
    if (child2 == (SECTR_Member.Child) null)
    {
      child2 = this.childPool.Count <= 0 ? new SECTR_Member.Child() : this.childPool.Pop();
      this.childTable[childTransform] = child2;
      this.children.Add(child2);
    }
    child2.Init(((Component) childTransform).get_gameObject(), renderer, light, terrain, this, dirShadowCaster, shadowVec);
    if (Object.op_Implicit((Object) child2.renderer))
    {
      bool flag = true;
      if (this.isSector && ((object) renderer).GetType() == typeof (ParticleSystemRenderer))
        flag = false;
      if (flag)
      {
        if (!this.hasRenderBounds)
        {
          this.renderBounds = child2.rendererBounds;
          this.hasRenderBounds = true;
        }
        else
          ((Bounds) ref this.renderBounds).Encapsulate(child2.rendererBounds);
      }
      this.renderers.Add(child2);
    }
    if (Object.op_Implicit((Object) child2.light))
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
        ((Bounds) ref this.lightBounds).Encapsulate(child2.lightBounds);
      this.lights.Add(child2);
    }
    if (Object.op_Implicit((Object) child2.terrain))
    {
      if (!this.hasRenderBounds)
      {
        this.renderBounds = child2.terrainBounds;
        this.hasRenderBounds = true;
      }
      else
        ((Bounds) ref this.renderBounds).Encapsulate(child2.terrainBounds);
      this.terrains.Add(child2);
    }
    if (SECTR_Modules.VIS && (child2.terrainCastsShadows || child2.rendererCastsShadows))
    {
      this.shadowCasters.Add(child2);
      this.shadowCaster = true;
    }
    if (this.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.SelfOnly)
      return;
    int childCount = ((Component) childTransform).get_transform().get_childCount();
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
        if (Object.op_Implicit((Object) sectrPortal))
        {
          SECTR_Sector sectrSector = !Object.op_Equality((Object) sectrPortal.FrontSector, (Object) sector) ? sectrPortal.FrontSector : sectrPortal.BackSector;
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
    else if (this.PortalDetermined && Object.op_Implicit((Object) this.ForceStartSector) && !this.usedStartSector)
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
    if (Object.op_Implicit((Object) sector))
    {
      Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), this.lastPosition);
      int count = sector.Portals.Count;
      for (int index = 0; index < count; ++index)
      {
        SECTR_Portal portal = sector.Portals[index];
        if (Object.op_Implicit((Object) portal))
        {
          bool flag = Object.op_Equality((Object) portal.FrontSector, (Object) sector);
          Plane plane = !flag ? portal.ReverseHullPlane : portal.HullPlane;
          if (Object.op_Implicit(!flag ? (Object) portal.FrontSector : (Object) portal.BackSector) && (double) Vector3.Dot(vector3, ((Plane) ref plane).get_normal()) < 0.0 && (((Plane) ref plane).GetSide(((Component) this).get_transform().get_position()) != ((Plane) ref plane).GetSide(this.lastPosition) && portal.IsPointInHull(((Component) this).get_transform().get_position(), ((Vector3) ref vector3).get_magnitude())))
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
      this.gameObjectHash = ((Object) this.gameObject).GetInstanceID();
      this.member = member;
      this.renderer = !Object.op_Implicit((Object) renderer) || !this.renderCulled && !renderer.get_enabled() ? (Renderer) null : renderer;
      this.light = !Object.op_Implicit((Object) light) || !this.lightCulled && !((Behaviour) light).get_enabled() || light.get_type() != 2 && light.get_type() != null ? (Light) null : light;
      this.terrain = !Object.op_Implicit((Object) terrain) || !this.terrainCulled && !((Behaviour) terrain).get_enabled() ? (Terrain) null : terrain;
      this.rendererBounds = !Object.op_Implicit((Object) this.renderer) ? (Bounds) null : this.renderer.get_bounds();
      this.lightBounds = !Object.op_Implicit((Object) this.light) ? (Bounds) null : SECTR_Geometry.ComputeBounds(this.light);
      this.terrainBounds = !Object.op_Implicit((Object) this.terrain) ? (Bounds) null : SECTR_Geometry.ComputeBounds(this.terrain);
      this.layer = LayerMask.op_Implicit(gameObject.get_layer());
      if (SECTR_Modules.VIS)
      {
        this.renderHash = !Object.op_Implicit((Object) this.renderer) ? 0 : ((Object) this.renderer).GetInstanceID();
        this.lightHash = !Object.op_Implicit((Object) this.light) ? 0 : ((Object) this.light).GetInstanceID();
        this.terrainHash = !Object.op_Implicit((Object) this.terrain) ? 0 : ((Object) this.terrain).GetInstanceID();
        bool flag = true;
        this.shadowLight = Object.op_Implicit((Object) this.light) && light.get_shadows() != null && (light.get_bakingOutput().isBaked == null || flag);
        this.rendererCastsShadows = Object.op_Implicit((Object) this.renderer) && renderer.get_shadowCastingMode() != null && (renderer.get_lightmapIndex() == -1 || flag);
        this.terrainCastsShadows = Object.op_Implicit((Object) this.terrain) && terrain.get_castShadows() && (terrain.get_lightmapIndex() == -1 || flag);
        if (dirShadowCaster)
        {
          if (this.rendererCastsShadows)
            this.rendererBounds = SECTR_Geometry.ProjectBounds(this.rendererBounds, shadowVec);
          if (this.terrainCastsShadows)
            this.terrainBounds = SECTR_Geometry.ProjectBounds(this.terrainBounds, shadowVec);
        }
        if (this.shadowLight)
        {
          this.shadowLightPosition = ((Component) light).get_transform().get_position();
          this.shadowLightRange = light.get_range();
          this.shadowLightType = light.get_type();
          this.shadowCullingMask = light.get_cullingMask();
        }
        else
        {
          this.shadowLightPosition = Vector3.get_zero();
          this.shadowLightRange = 0.0f;
          this.shadowLightType = (LightType) 3;
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
        this.shadowLightPosition = Vector3.get_zero();
        this.shadowLightRange = 0.0f;
        this.shadowLightType = (LightType) 3;
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
