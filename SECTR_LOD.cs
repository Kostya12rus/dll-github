// Decompiled with JetBrains decompiler
// Type: SECTR_LOD
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SECTR/Vis/SECTR LOD")]
[RequireComponent(typeof (SECTR_Member))]
[ExecuteInEditMode]
public class SECTR_LOD : MonoBehaviour
{
  private static List<SECTR_LOD> allLODs = new List<SECTR_LOD>(128);
  [HideInInspector]
  [SerializeField]
  private Vector3 boundsOffset;
  [HideInInspector]
  [SerializeField]
  private float boundsRadius;
  [HideInInspector]
  [SerializeField]
  private bool boundsUpdated;
  private int activeLOD;
  private bool siblingsDisabled;
  private SECTR_Member cachedMember;
  private List<GameObject> toHide;
  private List<SECTR_LOD.LODEntry> toShow;
  public List<SECTR_LOD.LODSet> LODs;
  [SECTR_ToolTip("Determines which sibling components are disabled when the LOD is culled.", null, typeof (SECTR_LOD.SiblinglFlags))]
  public SECTR_LOD.SiblinglFlags CullSiblings;

  public SECTR_LOD()
  {
    base.\u002Ector();
  }

  public static List<SECTR_LOD> All
  {
    get
    {
      return SECTR_LOD.allLODs;
    }
  }

  public void SelectLOD(Camera renderCamera)
  {
    if (!Object.op_Implicit((Object) renderCamera))
      return;
    if (!this.boundsUpdated)
      this._CalculateBounds();
    Matrix4x4 localToWorldMatrix = ((Component) this).get_transform().get_localToWorldMatrix();
    Vector3 vector3 = ((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(this.boundsOffset);
    float num1 = Vector3.Distance(((Component) renderCamera).get_transform().get_position(), vector3);
    float num2 = (float) ((double) this.boundsRadius / ((double) Mathf.Tan((float) ((double) renderCamera.get_fieldOfView() * 0.5 * (Math.PI / 180.0))) * (double) num1) * 2.0);
    int lodIndex = -1;
    int count = this.LODs.Count;
    for (int index = 0; index < count; ++index)
    {
      float threshold = this.LODs[index].Threshold;
      if (index == this.activeLOD)
        threshold -= threshold * 0.1f;
      if ((double) num2 >= (double) threshold)
      {
        lodIndex = index;
        break;
      }
    }
    if (lodIndex == this.activeLOD)
      return;
    this._ActivateLOD(lodIndex);
  }

  private void OnEnable()
  {
    SECTR_LOD.allLODs.Add(this);
    this.cachedMember = (SECTR_Member) ((Component) this).GetComponent<SECTR_Member>();
    SECTR_CullingCamera sectrCullingCamera = SECTR_CullingCamera.All.Count <= 0 ? (SECTR_CullingCamera) null : SECTR_CullingCamera.All[0];
    if (Object.op_Implicit((Object) sectrCullingCamera))
      this.SelectLOD((Camera) ((Component) sectrCullingCamera).GetComponent<Camera>());
    else
      this._ActivateLOD(0);
  }

  private void OnDisable()
  {
    SECTR_LOD.allLODs.Remove(this);
    this.cachedMember = (SECTR_Member) null;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(Matrix4x4.get_identity());
    Gizmos.set_color(Color.get_yellow());
    Matrix4x4 localToWorldMatrix = ((Component) this).get_transform().get_localToWorldMatrix();
    Gizmos.DrawWireSphere(((Matrix4x4) ref localToWorldMatrix).MultiplyPoint(this.boundsOffset), this.boundsRadius);
  }

  private void _ActivateLOD(int lodIndex)
  {
    this.toHide.Clear();
    this.toShow.Clear();
    if (this.activeLOD >= 0 && this.activeLOD < this.LODs.Count)
    {
      SECTR_LOD.LODSet loD = this.LODs[this.activeLOD];
      int count = loD.LODEntries.Count;
      for (int index = 0; index < count; ++index)
      {
        SECTR_LOD.LODEntry lodEntry = loD.LODEntries[index];
        if (Object.op_Implicit((Object) lodEntry.gameObject))
          this.toHide.Add(lodEntry.gameObject);
      }
    }
    if (lodIndex >= 0 && lodIndex < this.LODs.Count)
    {
      SECTR_LOD.LODSet loD = this.LODs[lodIndex];
      int count = loD.LODEntries.Count;
      for (int index = 0; index < count; ++index)
      {
        SECTR_LOD.LODEntry lodEntry = loD.LODEntries[index];
        if (Object.op_Implicit((Object) lodEntry.gameObject))
        {
          this.toHide.Remove(lodEntry.gameObject);
          this.toShow.Add(lodEntry);
        }
      }
    }
    int count1 = this.toHide.Count;
    for (int index = 0; index < count1; ++index)
      this.toHide[index].SetActive(false);
    int count2 = this.toShow.Count;
    for (int index = 0; index < count2; ++index)
    {
      SECTR_LOD.LODEntry lodEntry = this.toShow[index];
      lodEntry.gameObject.SetActive(true);
      if (Object.op_Implicit((Object) lodEntry.lightmapSource))
      {
        Renderer component = (Renderer) lodEntry.gameObject.GetComponent<Renderer>();
        if (Object.op_Implicit((Object) component))
        {
          component.set_lightmapIndex(lodEntry.lightmapSource.get_lightmapIndex());
          component.set_lightmapScaleOffset(lodEntry.lightmapSource.get_lightmapScaleOffset());
        }
      }
    }
    this.activeLOD = lodIndex;
    if (this.CullSiblings != (SECTR_LOD.SiblinglFlags) 0 && (this.activeLOD == -1 && !this.siblingsDisabled || this.activeLOD != -1 && this.siblingsDisabled))
    {
      this.siblingsDisabled = this.activeLOD == -1;
      if ((this.CullSiblings & SECTR_LOD.SiblinglFlags.Behaviors) != (SECTR_LOD.SiblinglFlags) 0)
      {
        MonoBehaviour[] components = (MonoBehaviour[]) ((Component) this).get_gameObject().GetComponents<MonoBehaviour>();
        int length = components.Length;
        for (int index = 0; index < length; ++index)
        {
          MonoBehaviour monoBehaviour = components[index];
          if (Object.op_Inequality((Object) monoBehaviour, (Object) this) && Object.op_Inequality((Object) monoBehaviour, (Object) this.cachedMember))
            ((Behaviour) monoBehaviour).set_enabled(!this.siblingsDisabled);
        }
      }
      if ((this.CullSiblings & SECTR_LOD.SiblinglFlags.Renderers) != (SECTR_LOD.SiblinglFlags) 0)
      {
        Renderer[] components = (Renderer[]) ((Component) this).get_gameObject().GetComponents<Renderer>();
        int length = components.Length;
        for (int index = 0; index < length; ++index)
          components[index].set_enabled(!this.siblingsDisabled);
      }
      if ((this.CullSiblings & SECTR_LOD.SiblinglFlags.Lights) != (SECTR_LOD.SiblinglFlags) 0)
      {
        Light[] components = (Light[]) ((Component) this).get_gameObject().GetComponents<Light>();
        int length = components.Length;
        for (int index = 0; index < length; ++index)
          ((Behaviour) components[index]).set_enabled(!this.siblingsDisabled);
      }
      if ((this.CullSiblings & SECTR_LOD.SiblinglFlags.Colliders) != (SECTR_LOD.SiblinglFlags) 0)
      {
        Collider[] components = (Collider[]) ((Component) this).get_gameObject().GetComponents<Collider>();
        int length = components.Length;
        for (int index = 0; index < length; ++index)
          components[index].set_enabled(!this.siblingsDisabled);
      }
      if ((this.CullSiblings & SECTR_LOD.SiblinglFlags.RigidBodies) != (SECTR_LOD.SiblinglFlags) 0)
      {
        Rigidbody[] components = (Rigidbody[]) ((Component) this).get_gameObject().GetComponents<Rigidbody>();
        int length = components.Length;
        for (int index = 0; index < length; ++index)
        {
          if (this.siblingsDisabled)
            components[index].Sleep();
          else
            components[index].WakeUp();
        }
      }
    }
    this.cachedMember.ForceUpdate(true);
  }

  private void _CalculateBounds()
  {
    Bounds bounds1 = (Bounds) null;
    int count1 = this.LODs.Count;
    bool flag = false;
    for (int index1 = 0; index1 < count1; ++index1)
    {
      SECTR_LOD.LODSet loD = this.LODs[index1];
      int count2 = loD.LODEntries.Count;
      for (int index2 = 0; index2 < count2; ++index2)
      {
        GameObject gameObject = loD.LODEntries[index2].gameObject;
        Renderer renderer = !Object.op_Implicit((Object) gameObject) ? (Renderer) null : (Renderer) gameObject.GetComponent<Renderer>();
        if (Object.op_Implicit((Object) renderer))
        {
          Bounds bounds2 = renderer.get_bounds();
          if (Vector3.op_Inequality(((Bounds) ref bounds2).get_extents(), Vector3.get_zero()))
          {
            if (!flag)
            {
              bounds1 = renderer.get_bounds();
              flag = true;
            }
            else
              ((Bounds) ref bounds1).Encapsulate(renderer.get_bounds());
          }
        }
      }
    }
    Matrix4x4 worldToLocalMatrix = ((Component) this).get_transform().get_worldToLocalMatrix();
    this.boundsOffset = ((Matrix4x4) ref worldToLocalMatrix).MultiplyPoint(((Bounds) ref bounds1).get_center());
    Vector3 extents = ((Bounds) ref bounds1).get_extents();
    this.boundsRadius = ((Vector3) ref extents).get_magnitude();
    this.boundsUpdated = true;
  }

  [Serializable]
  public class LODEntry
  {
    public GameObject gameObject;
    public Renderer lightmapSource;
  }

  [Serializable]
  public class LODSet
  {
    [SerializeField]
    private List<SECTR_LOD.LODEntry> lodEntries = new List<SECTR_LOD.LODEntry>(16);
    [SerializeField]
    private float threshold;

    public List<SECTR_LOD.LODEntry> LODEntries
    {
      get
      {
        return this.lodEntries;
      }
    }

    public float Threshold
    {
      get
      {
        return this.threshold;
      }
      set
      {
        this.threshold = value;
      }
    }

    public SECTR_LOD.LODEntry Add(GameObject gameObject, Renderer lightmapSource)
    {
      if (this.GetEntry(gameObject) != null)
        return (SECTR_LOD.LODEntry) null;
      SECTR_LOD.LODEntry lodEntry = new SECTR_LOD.LODEntry();
      lodEntry.gameObject = gameObject;
      lodEntry.lightmapSource = lightmapSource;
      this.lodEntries.Add(lodEntry);
      return lodEntry;
    }

    public void Remove(GameObject gameObject)
    {
      int index = 0;
      while (index < this.lodEntries.Count)
      {
        if (Object.op_Equality((Object) this.lodEntries[index].gameObject, (Object) gameObject))
          this.lodEntries.RemoveAt(index);
        else
          ++index;
      }
    }

    public SECTR_LOD.LODEntry GetEntry(GameObject gameObject)
    {
      int count = this.lodEntries.Count;
      for (int index = 0; index < count; ++index)
      {
        SECTR_LOD.LODEntry lodEntry = this.lodEntries[index];
        if (Object.op_Equality((Object) lodEntry.gameObject, (Object) gameObject))
          return lodEntry;
      }
      return (SECTR_LOD.LODEntry) null;
    }
  }

  [Flags]
  public enum SiblinglFlags
  {
    Behaviors = 1,
    Renderers = 2,
    Lights = 4,
    Colliders = 8,
    RigidBodies = 16, // 0x00000010
  }
}
