﻿// Decompiled with JetBrains decompiler
// Type: SECTR_CullingCamera
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("SECTR/Vis/SECTR Culling Camera")]
[RequireComponent(typeof (Camera))]
public class SECTR_CullingCamera : MonoBehaviour
{
  private static List<SECTR_CullingCamera> allCullingCameras = new List<SECTR_CullingCamera>(4);
  private Camera myCamera;
  private SECTR_Member cullingMember;
  private Dictionary<int, SECTR_Member.Child> hiddenRenderers;
  private Dictionary<int, SECTR_Member.Child> hiddenLights;
  private Dictionary<int, SECTR_Member.Child> hiddenTerrains;
  private int renderersCulled;
  private int lightsCulled;
  private int terrainsCulled;
  private bool didCull;
  private bool runOnce;
  private List<SECTR_Sector> initialSectors;
  private Stack<SECTR_CullingCamera.VisibilityNode> nodeStack;
  private List<SECTR_CullingCamera.ClipVertex> portalVertices;
  private List<Plane> newFrustum;
  private List<Plane> cullingPlanes;
  private List<List<Plane>> occluderFrustums;
  private Dictionary<SECTR_Occluder, SECTR_Occluder> activeOccluders;
  private List<SECTR_CullingCamera.ClipVertex> occluderVerts;
  private Dictionary<SECTR_Member.Child, int> shadowLights;
  private List<SECTR_Sector> shadowSectors;
  private Dictionary<SECTR_Sector, List<SECTR_Member.Child>> shadowSectorTable;
  private Dictionary<int, SECTR_Member.Child> visibleRenderers;
  private Dictionary<int, SECTR_Member.Child> visibleLights;
  private Dictionary<int, SECTR_Member.Child> visibleTerrains;
  private Stack<List<Plane>> frustumPool;
  private Stack<List<SECTR_Member.Child>> shadowLightPool;
  private Stack<Dictionary<int, SECTR_Member.Child>> threadVisibleListPool;
  private Stack<Dictionary<SECTR_Member.Child, int>> threadShadowLightPool;
  private Stack<List<Plane>> threadFrustumPool;
  private Stack<List<List<Plane>>> threadOccluderPool;
  private List<Thread> workerThreads;
  private Queue<SECTR_CullingCamera.ThreadCullData> cullingWorkQueue;
  private int remainingThreadWork;
  [SECTR_ToolTip("Allows multiple culling cameras to be active at once, but at the cost of some performance.")]
  public bool MultiCameraCulling;
  [SECTR_ToolTip("Forces culling into a mode designed for 2D and iso games where the camera is always outside the scene.")]
  public bool SimpleCulling;
  [SECTR_ToolTip("Distance to draw clipped frustums.", 0.0f, 100f)]
  public float GizmoDistance;
  [SECTR_ToolTip("Material to use to render the debug frustum mesh.")]
  public Material GizmoMaterial;
  [SECTR_ToolTip("Makes the Editor camera display the Game view's culling while playing in editor.")]
  public bool CullInEditor;
  [SECTR_ToolTip("Set to false to disable shadow culling post pass.", true)]
  public bool CullShadows;
  [SECTR_ToolTip("Use another camera for culling properties.", true)]
  public Camera cullingProxy;
  [SECTR_ToolTip("Number of worker threads for culling. Do not set this too high or you may see hitching.", 0.0f, -1f)]
  public int NumWorkerThreads;

  public SECTR_CullingCamera()
  {
    base.\u002Ector();
  }

  public static List<SECTR_CullingCamera> All
  {
    get
    {
      return SECTR_CullingCamera.allCullingCameras;
    }
  }

  public int RenderersCulled
  {
    get
    {
      return this.renderersCulled;
    }
  }

  public int LightsCulled
  {
    get
    {
      return this.lightsCulled;
    }
  }

  public int TerrainsCulled
  {
    get
    {
      return this.terrainsCulled;
    }
  }

  public void ResetStats()
  {
    this.renderersCulled = 0;
    this.lightsCulled = 0;
    this.terrainsCulled = 0;
    this.runOnce = false;
  }

  private void OnEnable()
  {
    this.myCamera = (Camera) ((Component) this).GetComponent<Camera>();
    this.cullingMember = (SECTR_Member) ((Component) this).GetComponent<SECTR_Member>();
    SECTR_CullingCamera.allCullingCameras.Add(this);
    this.runOnce = false;
    int num = Mathf.Min(this.NumWorkerThreads, SystemInfo.get_processorCount());
    for (int index = 0; index < num; ++index)
    {
      Thread thread = new Thread(new ThreadStart(this._CullingWorker));
      thread.IsBackground = true;
      thread.Priority = ThreadPriority.Highest;
      thread.Start();
      this.workerThreads.Add(thread);
    }
  }

  private void OnDisable()
  {
    if (!this.MultiCameraCulling)
      this._UndoCulling();
    SECTR_CullingCamera.allCullingCameras.Remove(this);
    int count = this.workerThreads.Count;
    for (int index = 0; index < count; ++index)
      this.workerThreads[index].Abort();
  }

  private void OnDestroy()
  {
  }

  private void OnPreCull()
  {
    Camera camera = !Object.op_Inequality((Object) this.cullingProxy, (Object) null) ? this.myCamera : this.cullingProxy;
    Vector3 position = ((Component) camera).get_transform().get_position();
    float num1 = Mathf.Cos(Mathf.Max(camera.get_fieldOfView(), camera.get_fieldOfView() * camera.get_aspect()) * 0.5f * ((float) Math.PI / 180f));
    float distanceTolerance = (float) ((double) camera.get_nearClipPlane() / (double) num1 * 1.00100004673004);
    if (Object.op_Implicit((Object) this.cullingProxy))
    {
      SECTR_CullingCamera component = (SECTR_CullingCamera) ((Component) this.cullingProxy).GetComponent<SECTR_CullingCamera>();
      if (Object.op_Implicit((Object) component))
      {
        this.SimpleCulling = component.SimpleCulling;
        this.CullShadows = component.CullShadows;
        if (this.MultiCameraCulling != component.MultiCameraCulling)
          this.runOnce = false;
        this.MultiCameraCulling = component.MultiCameraCulling;
      }
    }
    int count1 = SECTR_LOD.All.Count;
    for (int index = 0; index < count1; ++index)
      SECTR_LOD.All[index].SelectLOD(camera);
    int num2 = 0;
    if (!this.SimpleCulling)
    {
      if (Object.op_Implicit((Object) this.cullingMember) && ((Behaviour) this.cullingMember).get_enabled())
      {
        this.initialSectors.Clear();
        this.initialSectors.AddRange((IEnumerable<SECTR_Sector>) this.cullingMember.Sectors);
      }
      else
        SECTR_Sector.GetContaining(ref this.initialSectors, new Bounds(position, new Vector3(distanceTolerance, distanceTolerance, distanceTolerance)));
      num2 = this.initialSectors.Count;
      for (int index = 0; index < num2; ++index)
      {
        if (this.initialSectors[index].IsConnectedTerrain)
        {
          this.SimpleCulling = true;
          break;
        }
      }
    }
    if (this.SimpleCulling)
    {
      this.initialSectors.Clear();
      this.initialSectors.AddRange((IEnumerable<SECTR_Sector>) SECTR_Sector.All);
      num2 = this.initialSectors.Count;
    }
    if (!((Behaviour) this).get_enabled() || !((Behaviour) camera).get_enabled() || num2 <= 0)
      return;
    int count2 = this.workerThreads.Count;
    if (!this.MultiCameraCulling)
    {
      if (!this.runOnce)
      {
        this._HideAllMembers();
        this.runOnce = true;
      }
      else
        this._ApplyCulling(false);
    }
    else
      this._HideAllMembers();
    float shadowDistance = QualitySettings.get_shadowDistance();
    int count3 = SECTR_Member.All.Count;
    for (int index1 = 0; index1 < count3; ++index1)
    {
      SECTR_Member sectrMember = SECTR_Member.All[index1];
      if (sectrMember.ShadowLight)
      {
        int count4 = sectrMember.ShadowLights.Count;
        for (int index2 = 0; index2 < count4; ++index2)
        {
          SECTR_Member.Child shadowLight = sectrMember.ShadowLights[index2];
          if (Object.op_Implicit((Object) shadowLight.light))
          {
            shadowLight.shadowLightPosition = ((Component) shadowLight.light).get_transform().get_position();
            shadowLight.shadowLightRange = shadowLight.light.get_range();
          }
          sectrMember.ShadowLights[index2] = shadowLight;
        }
      }
    }
    this.nodeStack.Clear();
    this.shadowLights.Clear();
    this.visibleRenderers.Clear();
    this.visibleLights.Clear();
    this.visibleTerrains.Clear();
    Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
    for (int index = 0; index < num2; ++index)
      this.nodeStack.Push(new SECTR_CullingCamera.VisibilityNode(this, this.initialSectors[index], (SECTR_Portal) null, frustumPlanes, true));
    while (this.nodeStack.Count > 0)
    {
      SECTR_CullingCamera.VisibilityNode visibilityNode = this.nodeStack.Pop();
      if (visibilityNode.frustumPlanes != null)
      {
        this.cullingPlanes.Clear();
        this.cullingPlanes.AddRange((IEnumerable<Plane>) visibilityNode.frustumPlanes);
        int count4 = this.cullingPlanes.Count;
        for (int index = 0; index < count4; ++index)
        {
          Plane cullingPlane1 = this.cullingPlanes[index];
          Plane cullingPlane2 = this.cullingPlanes[(index + 1) % this.cullingPlanes.Count];
          float num3 = Vector3.Dot(((Plane) ref cullingPlane1).get_normal(), ((Plane) ref cullingPlane2).get_normal());
          if ((double) num3 < -0.899999976158142 && (double) num3 > -0.990000009536743)
          {
            Vector3 vector3_1 = Vector3.op_Addition(((Plane) ref cullingPlane1).get_normal(), ((Plane) ref cullingPlane2).get_normal());
            Vector3 vector3_2 = Vector3.Cross(((Plane) ref cullingPlane1).get_normal(), ((Plane) ref cullingPlane2).get_normal());
            Vector3 vector3_3 = Vector3.op_Subtraction(vector3_1, Vector3.op_Multiply(Vector3.Dot(vector3_1, vector3_2), vector3_2));
            ((Vector3) ref vector3_3).Normalize();
            Matrix4x4 matrix4x4 = (Matrix4x4) null;
            ((Matrix4x4) ref matrix4x4).SetRow(0, new Vector4((float) ((Plane) ref cullingPlane1).get_normal().x, (float) ((Plane) ref cullingPlane1).get_normal().y, (float) ((Plane) ref cullingPlane1).get_normal().z, 0.0f));
            ((Matrix4x4) ref matrix4x4).SetRow(1, new Vector4((float) ((Plane) ref cullingPlane2).get_normal().x, (float) ((Plane) ref cullingPlane2).get_normal().y, (float) ((Plane) ref cullingPlane2).get_normal().z, 0.0f));
            ((Matrix4x4) ref matrix4x4).SetRow(2, new Vector4((float) vector3_2.x, (float) vector3_2.y, (float) vector3_2.z, 0.0f));
            ((Matrix4x4) ref matrix4x4).SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1f));
            Matrix4x4 inverse = ((Matrix4x4) ref matrix4x4).get_inverse();
            Vector3 vector3_4 = ((Matrix4x4) ref inverse).MultiplyPoint3x4(new Vector3(-((Plane) ref cullingPlane1).get_distance(), -((Plane) ref cullingPlane2).get_distance(), 0.0f));
            this.cullingPlanes.Insert(++index, new Plane(vector3_3, vector3_4));
          }
        }
        int count5 = this.cullingPlanes.Count;
        int num4 = 0;
        for (int index = 0; index < count5; ++index)
          num4 |= 1 << index;
        SECTR_Sector sector1 = visibilityNode.sector;
        if (SECTR_Occluder.All.Count > 0)
        {
          List<SECTR_Occluder> occludersInSector = SECTR_Occluder.GetOccludersInSector(sector1);
          if (occludersInSector != null)
          {
            int count6 = occludersInSector.Count;
            for (int index1 = 0; index1 < count6; ++index1)
            {
              SECTR_Occluder key = occludersInSector[index1];
              if (Object.op_Implicit((Object) key.HullMesh) && !this.activeOccluders.ContainsKey(key))
              {
                Matrix4x4 cullingMatrix = key.GetCullingMatrix(position);
                Vector3[] vertsCw = key.VertsCW;
                Vector3 vector3_1 = ((Matrix4x4) ref cullingMatrix).MultiplyVector(Vector3.op_UnaryNegation(key.MeshNormal));
                Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
                if (vertsCw != null && !SECTR_Geometry.IsPointInFrontOfPlane(position, key.Center, normalized))
                {
                  int length = vertsCw.Length;
                  this.occluderVerts.Clear();
                  Bounds bounds;
                  ((Bounds) ref bounds).\u002Ector(((Component) key).get_transform().get_position(), Vector3.get_zero());
                  for (int index2 = 0; index2 < length; ++index2)
                  {
                    Vector3 vector3_2 = ((Matrix4x4) ref cullingMatrix).MultiplyPoint3x4(vertsCw[index2]);
                    ((Bounds) ref bounds).Encapsulate(vector3_2);
                    this.occluderVerts.Add(new SECTR_CullingCamera.ClipVertex(new Vector4((float) vector3_2.x, (float) vector3_2.y, (float) vector3_2.z, 1f), 0.0f));
                  }
                  int outMask;
                  if (SECTR_Geometry.FrustumIntersectsBounds(key.BoundingBox, this.cullingPlanes, num4, out outMask))
                  {
                    List<Plane> newFrustum;
                    if (this.frustumPool.Count > 0)
                    {
                      newFrustum = this.frustumPool.Pop();
                      newFrustum.Clear();
                    }
                    else
                      newFrustum = new List<Plane>(length + 1);
                    this._BuildFrustumFromHull(camera, true, this.occluderVerts, ref newFrustum);
                    newFrustum.Add(new Plane(normalized, key.Center));
                    this.occluderFrustums.Add(newFrustum);
                    this.activeOccluders[key] = key;
                  }
                }
              }
            }
          }
        }
        if (count2 > 0)
        {
          lock ((object) this.cullingWorkQueue)
          {
            this.cullingWorkQueue.Enqueue(new SECTR_CullingCamera.ThreadCullData(sector1, this, position, this.cullingPlanes, this.occluderFrustums, num4, shadowDistance, this.SimpleCulling));
            Monitor.Pulse((object) this.cullingWorkQueue);
          }
          Interlocked.Increment(ref this.remainingThreadWork);
        }
        else
          SECTR_CullingCamera._FrustumCullSector(sector1, position, this.cullingPlanes, this.occluderFrustums, num4, shadowDistance, this.SimpleCulling, ref this.visibleRenderers, ref this.visibleLights, ref this.visibleTerrains, ref this.shadowLights);
        int num5 = !this.SimpleCulling ? visibilityNode.sector.Portals.Count : 0;
        for (int index1 = 0; index1 < num5; ++index1)
        {
          SECTR_Portal portal = visibilityNode.sector.Portals[index1];
          bool flag1 = (portal.Flags & SECTR_Portal.PortalFlags.PassThrough) != (SECTR_Portal.PortalFlags) 0;
          if ((Object.op_Implicit((Object) portal.HullMesh) || flag1) && (portal.Flags & SECTR_Portal.PortalFlags.Closed) == (SECTR_Portal.PortalFlags) 0)
          {
            bool forwardTraversal = Object.op_Equality((Object) visibilityNode.sector, (Object) portal.FrontSector);
            SECTR_Sector sector2 = !forwardTraversal ? portal.FrontSector : portal.BackSector;
            bool flag2 = !Object.op_Implicit((Object) sector2);
            if (!flag2)
              flag2 = SECTR_Geometry.IsPointInFrontOfPlane(position, portal.Center, portal.Normal) != forwardTraversal;
            if (!flag2 && Object.op_Implicit((Object) visibilityNode.portal))
            {
              Vector3 vector3 = Vector3.op_Subtraction(portal.Center, visibilityNode.portal.Center);
              flag2 = (double) Vector3.Dot(((Vector3) ref vector3).get_normalized(), !visibilityNode.forwardTraversal ? visibilityNode.portal.Normal : visibilityNode.portal.ReverseNormal) < 0.0;
            }
            if (!flag2 && !flag1)
            {
              int count6 = this.occluderFrustums.Count;
              for (int index2 = 0; index2 < count6; ++index2)
              {
                if (SECTR_Geometry.FrustumContainsBounds(portal.BoundingBox, this.occluderFrustums[index2]))
                {
                  flag2 = true;
                  break;
                }
              }
            }
            if (!flag2)
            {
              if (!flag1)
              {
                this.portalVertices.Clear();
                Matrix4x4 localToWorldMatrix = ((Component) portal).get_transform().get_localToWorldMatrix();
                Vector3[] vertsCw = portal.VertsCW;
                if (vertsCw != null)
                {
                  int length = vertsCw.Length;
                  for (int index2 = 0; index2 < length; ++index2)
                  {
                    Vector3 vector3 = ((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(vertsCw[index2]);
                    this.portalVertices.Add(new SECTR_CullingCamera.ClipVertex(new Vector4((float) vector3.x, (float) vector3.y, (float) vector3.z, 1f), 0.0f));
                  }
                }
              }
              this.newFrustum.Clear();
              if (!flag1 && !portal.IsPointInHull(position, distanceTolerance))
              {
                int count6 = visibilityNode.frustumPlanes.Count;
                for (int index2 = 0; index2 < count6; ++index2)
                {
                  Plane frustumPlane = visibilityNode.frustumPlanes[index2];
                  Vector4 vector4;
                  ((Vector4) ref vector4).\u002Ector((float) ((Plane) ref frustumPlane).get_normal().x, (float) ((Plane) ref frustumPlane).get_normal().y, (float) ((Plane) ref frustumPlane).get_normal().z, ((Plane) ref frustumPlane).get_distance());
                  bool flag3 = true;
                  bool flag4 = true;
                  for (int index3 = 0; index3 < this.portalVertices.Count; ++index3)
                  {
                    Vector4 vertex = this.portalVertices[index3].vertex;
                    float side = Vector4.Dot(vector4, vertex);
                    this.portalVertices[index3] = new SECTR_CullingCamera.ClipVertex(vertex, side);
                    flag3 = flag3 && (double) side > 0.0;
                    flag4 = flag4 && (double) side <= -1.0 / 1000.0;
                  }
                  if (flag4)
                  {
                    this.portalVertices.Clear();
                    break;
                  }
                  if (!flag3)
                  {
                    int count7 = this.portalVertices.Count;
                    for (int index3 = 0; index3 < count7; ++index3)
                    {
                      int index4 = (index3 + 1) % this.portalVertices.Count;
                      float side1 = this.portalVertices[index3].side;
                      float side2 = this.portalVertices[index4].side;
                      if ((double) side1 > 0.0 && (double) side2 <= -1.0 / 1000.0 || (double) side2 > 0.0 && (double) side1 <= -1.0 / 1000.0)
                      {
                        Vector4 vertex1 = this.portalVertices[index3].vertex;
                        Vector4 vertex2 = this.portalVertices[index4].vertex;
                        float num3 = side1 / Vector4.Dot(vector4, Vector4.op_Subtraction(vertex1, vertex2));
                        Vector4 vertex3 = Vector4.op_Addition(vertex1, Vector4.op_Multiply(num3, Vector4.op_Subtraction(vertex2, vertex1)));
                        vertex3.w = (__Null) 1.0;
                        this.portalVertices.Insert(index3 + 1, new SECTR_CullingCamera.ClipVertex(vertex3, 0.0f));
                        ++count7;
                      }
                    }
                    int index5 = 0;
                    while (index5 < count7)
                    {
                      if ((double) this.portalVertices[index5].side < -1.0 / 1000.0)
                      {
                        this.portalVertices.RemoveAt(index5);
                        --count7;
                      }
                      else
                        ++index5;
                    }
                  }
                }
                this._BuildFrustumFromHull(camera, forwardTraversal, this.portalVertices, ref this.newFrustum);
              }
              else
                this.newFrustum.AddRange((IEnumerable<Plane>) frustumPlanes);
              if (this.newFrustum.Count > 2)
                this.nodeStack.Push(new SECTR_CullingCamera.VisibilityNode(this, sector2, portal, this.newFrustum, forwardTraversal));
            }
          }
        }
      }
      if (visibilityNode.frustumPlanes != null)
      {
        visibilityNode.frustumPlanes.Clear();
        this.frustumPool.Push(visibilityNode.frustumPlanes);
      }
    }
    if (count2 > 0)
    {
      while (this.remainingThreadWork > 0)
      {
        while (this.cullingWorkQueue.Count > 0)
        {
          SECTR_CullingCamera.ThreadCullData cullData = new SECTR_CullingCamera.ThreadCullData();
          lock ((object) this.cullingWorkQueue)
          {
            if (this.cullingWorkQueue.Count > 0)
              cullData = this.cullingWorkQueue.Dequeue();
          }
          if (cullData.cullingMode == SECTR_CullingCamera.ThreadCullData.CullingModes.Graph)
          {
            this._FrustumCullSectorThread(cullData);
            Interlocked.Decrement(ref this.remainingThreadWork);
          }
        }
      }
      this.remainingThreadWork = 0;
    }
    if (this.shadowLights.Count > 0 && this.CullShadows)
    {
      this.shadowSectorTable.Clear();
      Dictionary<SECTR_Member.Child, int>.Enumerator enumerator1 = this.shadowLights.GetEnumerator();
      while (enumerator1.MoveNext())
      {
        SECTR_Member.Child key1 = enumerator1.Current.Key;
        List<SECTR_Sector> sectrSectorList;
        if (Object.op_Implicit((Object) key1.member) && key1.member.IsSector)
        {
          this.shadowSectors.Clear();
          this.shadowSectors.Add((SECTR_Sector) key1.member);
          sectrSectorList = this.shadowSectors;
        }
        else if (Object.op_Implicit((Object) key1.member) && key1.member.Sectors.Count > 0)
        {
          sectrSectorList = key1.member.Sectors;
        }
        else
        {
          SECTR_Sector.GetContaining(ref this.shadowSectors, key1.lightBounds);
          sectrSectorList = this.shadowSectors;
        }
        int count4 = sectrSectorList.Count;
        for (int index = 0; index < count4; ++index)
        {
          SECTR_Sector key2 = sectrSectorList[index];
          List<SECTR_Member.Child> childList;
          if (!this.shadowSectorTable.TryGetValue(key2, out childList))
          {
            childList = this.shadowLightPool.Count <= 0 ? new List<SECTR_Member.Child>(16) : this.shadowLightPool.Pop();
            this.shadowSectorTable[key2] = childList;
          }
          childList.Add(key1);
        }
      }
      Dictionary<SECTR_Sector, List<SECTR_Member.Child>>.Enumerator enumerator2 = this.shadowSectorTable.GetEnumerator();
      while (enumerator2.MoveNext())
      {
        SECTR_Sector key = enumerator2.Current.Key;
        List<SECTR_Member.Child> sectorShadowLights = enumerator2.Current.Value;
        if (count2 > 0)
        {
          lock ((object) this.cullingWorkQueue)
          {
            this.cullingWorkQueue.Enqueue(new SECTR_CullingCamera.ThreadCullData(key, position, sectorShadowLights));
            Monitor.Pulse((object) this.cullingWorkQueue);
          }
          Interlocked.Increment(ref this.remainingThreadWork);
        }
        else
          SECTR_CullingCamera._ShadowCullSector(key, sectorShadowLights, ref this.visibleRenderers, ref this.visibleTerrains);
      }
      if (count2 > 0)
      {
        while (this.remainingThreadWork > 0)
        {
          while (this.cullingWorkQueue.Count > 0)
          {
            SECTR_CullingCamera.ThreadCullData cullData = new SECTR_CullingCamera.ThreadCullData();
            lock ((object) this.cullingWorkQueue)
            {
              if (this.cullingWorkQueue.Count > 0)
                cullData = this.cullingWorkQueue.Dequeue();
            }
            if (cullData.cullingMode == SECTR_CullingCamera.ThreadCullData.CullingModes.Shadow)
            {
              this._ShadowCullSectorThread(cullData);
              Interlocked.Decrement(ref this.remainingThreadWork);
            }
          }
        }
        this.remainingThreadWork = 0;
      }
      enumerator2 = this.shadowSectorTable.GetEnumerator();
      while (enumerator2.MoveNext())
      {
        List<SECTR_Member.Child> childList = enumerator2.Current.Value;
        childList.Clear();
        this.shadowLightPool.Push(childList);
      }
    }
    this._ApplyCulling(true);
    int count8 = this.occluderFrustums.Count;
    for (int index = 0; index < count8; ++index)
    {
      this.occluderFrustums[index].Clear();
      this.frustumPool.Push(this.occluderFrustums[index]);
    }
    this.occluderFrustums.Clear();
    this.activeOccluders.Clear();
  }

  private void OnPostRender()
  {
    if (!this.MultiCameraCulling)
      return;
    this._UndoCulling();
  }

  private void _CullingWorker()
  {
    while (true)
    {
      SECTR_CullingCamera.ThreadCullData cullData;
      do
      {
        cullData = new SECTR_CullingCamera.ThreadCullData();
        lock ((object) this.cullingWorkQueue)
        {
          while (this.cullingWorkQueue.Count == 0)
            Monitor.Wait((object) this.cullingWorkQueue);
          cullData = this.cullingWorkQueue.Dequeue();
        }
        switch (cullData.cullingMode)
        {
          case SECTR_CullingCamera.ThreadCullData.CullingModes.Graph:
            this._FrustumCullSectorThread(cullData);
            break;
          case SECTR_CullingCamera.ThreadCullData.CullingModes.Shadow:
            this._ShadowCullSectorThread(cullData);
            break;
        }
      }
      while (cullData.cullingMode != SECTR_CullingCamera.ThreadCullData.CullingModes.Graph && cullData.cullingMode != SECTR_CullingCamera.ThreadCullData.CullingModes.Shadow);
      Interlocked.Decrement(ref this.remainingThreadWork);
    }
  }

  private void _FrustumCullSectorThread(SECTR_CullingCamera.ThreadCullData cullData)
  {
    Dictionary<int, SECTR_Member.Child> visibleRenderers = (Dictionary<int, SECTR_Member.Child>) null;
    Dictionary<int, SECTR_Member.Child> visibleLights = (Dictionary<int, SECTR_Member.Child>) null;
    Dictionary<int, SECTR_Member.Child> visibleTerrains = (Dictionary<int, SECTR_Member.Child>) null;
    Dictionary<SECTR_Member.Child, int> shadowLights = (Dictionary<SECTR_Member.Child, int>) null;
    lock ((object) this.threadVisibleListPool)
    {
      visibleRenderers = this.threadVisibleListPool.Count <= 0 ? new Dictionary<int, SECTR_Member.Child>(32) : this.threadVisibleListPool.Pop();
      visibleLights = this.threadVisibleListPool.Count <= 0 ? new Dictionary<int, SECTR_Member.Child>(32) : this.threadVisibleListPool.Pop();
      visibleTerrains = this.threadVisibleListPool.Count <= 0 ? new Dictionary<int, SECTR_Member.Child>(32) : this.threadVisibleListPool.Pop();
    }
    lock ((object) this.threadShadowLightPool)
      shadowLights = this.threadShadowLightPool.Count <= 0 ? new Dictionary<SECTR_Member.Child, int>(32) : this.threadShadowLightPool.Pop();
    SECTR_CullingCamera._FrustumCullSector(cullData.sector, cullData.cameraPos, cullData.cullingPlanes, cullData.occluderFrustums, cullData.baseMask, cullData.shadowDistance, cullData.cullingSimpleCulling, ref visibleRenderers, ref visibleLights, ref visibleTerrains, ref shadowLights);
    lock ((object) this.visibleRenderers)
    {
      Dictionary<int, SECTR_Member.Child>.Enumerator enumerator = visibleRenderers.GetEnumerator();
      while (enumerator.MoveNext())
        this.visibleRenderers[enumerator.Current.Key] = enumerator.Current.Value;
    }
    lock ((object) this.visibleLights)
    {
      Dictionary<int, SECTR_Member.Child>.Enumerator enumerator = visibleLights.GetEnumerator();
      while (enumerator.MoveNext())
        this.visibleLights[enumerator.Current.Key] = enumerator.Current.Value;
    }
    lock ((object) this.visibleTerrains)
    {
      Dictionary<int, SECTR_Member.Child>.Enumerator enumerator = visibleTerrains.GetEnumerator();
      while (enumerator.MoveNext())
        this.visibleTerrains[enumerator.Current.Key] = enumerator.Current.Value;
    }
    lock ((object) this.shadowLights)
    {
      Dictionary<SECTR_Member.Child, int>.Enumerator enumerator = shadowLights.GetEnumerator();
      while (enumerator.MoveNext())
        this.shadowLights[enumerator.Current.Key] = 0;
    }
    lock ((object) this.threadVisibleListPool)
    {
      visibleRenderers.Clear();
      this.threadVisibleListPool.Push(visibleRenderers);
      visibleLights.Clear();
      this.threadVisibleListPool.Push(visibleLights);
      visibleTerrains.Clear();
      this.threadVisibleListPool.Push(visibleTerrains);
    }
    lock ((object) this.threadShadowLightPool)
    {
      shadowLights.Clear();
      this.threadShadowLightPool.Push(shadowLights);
    }
    lock ((object) this.threadFrustumPool)
    {
      cullData.cullingPlanes.Clear();
      this.threadFrustumPool.Push(cullData.cullingPlanes);
      int count = cullData.occluderFrustums.Count;
      for (int index = 0; index < count; ++index)
      {
        cullData.occluderFrustums[index].Clear();
        this.threadFrustumPool.Push(cullData.occluderFrustums[index]);
      }
    }
    lock ((object) this.threadOccluderPool)
    {
      cullData.occluderFrustums.Clear();
      this.threadOccluderPool.Push(cullData.occluderFrustums);
    }
  }

  private void _ShadowCullSectorThread(SECTR_CullingCamera.ThreadCullData cullData)
  {
    Dictionary<int, SECTR_Member.Child> visibleRenderers = (Dictionary<int, SECTR_Member.Child>) null;
    Dictionary<int, SECTR_Member.Child> visibleTerrains = (Dictionary<int, SECTR_Member.Child>) null;
    lock ((object) this.threadVisibleListPool)
    {
      visibleRenderers = this.threadVisibleListPool.Count <= 0 ? new Dictionary<int, SECTR_Member.Child>(32) : this.threadVisibleListPool.Pop();
      visibleTerrains = this.threadVisibleListPool.Count <= 0 ? new Dictionary<int, SECTR_Member.Child>(32) : this.threadVisibleListPool.Pop();
    }
    SECTR_CullingCamera._ShadowCullSector(cullData.sector, cullData.sectorShadowLights, ref visibleRenderers, ref visibleTerrains);
    lock ((object) this.visibleRenderers)
    {
      Dictionary<int, SECTR_Member.Child>.Enumerator enumerator = visibleRenderers.GetEnumerator();
      while (enumerator.MoveNext())
        this.visibleRenderers[enumerator.Current.Key] = enumerator.Current.Value;
    }
    lock ((object) this.visibleTerrains)
    {
      Dictionary<int, SECTR_Member.Child>.Enumerator enumerator = visibleTerrains.GetEnumerator();
      while (enumerator.MoveNext())
        this.visibleTerrains[enumerator.Current.Key] = enumerator.Current.Value;
    }
    lock ((object) this.threadVisibleListPool)
    {
      visibleRenderers.Clear();
      this.threadVisibleListPool.Push(visibleRenderers);
      visibleTerrains.Clear();
      this.threadVisibleListPool.Push(visibleTerrains);
    }
  }

  private static void _FrustumCullSector(SECTR_Sector sector, Vector3 cameraPos, List<Plane> cullingPlanes, List<List<Plane>> occluderFrustums, int baseMask, float shadowDistance, bool forceGroupCull, ref Dictionary<int, SECTR_Member.Child> visibleRenderers, ref Dictionary<int, SECTR_Member.Child> visibleLights, ref Dictionary<int, SECTR_Member.Child> visibleTerrains, ref Dictionary<SECTR_Member.Child, int> shadowLights)
  {
    SECTR_CullingCamera._FrustumCull((SECTR_Member) sector, cameraPos, cullingPlanes, occluderFrustums, baseMask, shadowDistance, forceGroupCull, ref visibleRenderers, ref visibleLights, ref visibleTerrains, ref shadowLights);
    int count = sector.Members.Count;
    for (int index = 0; index < count; ++index)
    {
      SECTR_Member member = sector.Members[index];
      if (member.HasRenderBounds || member.HasLightBounds)
        SECTR_CullingCamera._FrustumCull(member, cameraPos, cullingPlanes, occluderFrustums, baseMask, shadowDistance, forceGroupCull, ref visibleRenderers, ref visibleLights, ref visibleTerrains, ref shadowLights);
    }
  }

  private static void _FrustumCull(SECTR_Member member, Vector3 cameraPos, List<Plane> frustumPlanes, List<List<Plane>> occluders, int baseMask, float shadowDistance, bool forceGroupCull, ref Dictionary<int, SECTR_Member.Child> visibleRenderers, ref Dictionary<int, SECTR_Member.Child> visibleLights, ref Dictionary<int, SECTR_Member.Child> visibleTerrains, ref Dictionary<SECTR_Member.Child, int> shadowLights)
  {
    int inMask1 = baseMask;
    int inMask2 = baseMask;
    int outMask1 = 0;
    int outMask2 = 0;
    bool flag1 = member.CullEachChild && !forceGroupCull;
    bool flag2 = member.HasRenderBounds && SECTR_Geometry.FrustumIntersectsBounds(member.RenderBounds, frustumPlanes, inMask1, out outMask1);
    bool flag3 = member.HasLightBounds && SECTR_Geometry.FrustumIntersectsBounds(member.LightBounds, frustumPlanes, inMask2, out outMask2);
    int count1 = occluders.Count;
    for (int index = 0; index < count1 && (flag2 || flag3); ++index)
    {
      List<Plane> occluder = occluders[index];
      if (flag2)
        flag2 = !SECTR_Geometry.FrustumContainsBounds(member.RenderBounds, occluder);
      if (flag3)
        flag3 = !SECTR_Geometry.FrustumContainsBounds(member.LightBounds, occluder);
    }
    if (flag2)
    {
      int count2 = member.Renderers.Count;
      for (int index = 0; index < count2; ++index)
      {
        SECTR_Member.Child renderer = member.Renderers[index];
        if (renderer.renderHash != 0 && !visibleRenderers.ContainsKey(renderer.renderHash) && (!flag1 || SECTR_CullingCamera._IsVisible(renderer.rendererBounds, frustumPlanes, outMask1, occluders)))
          visibleRenderers.Add(renderer.renderHash, renderer);
      }
      int count3 = member.Terrains.Count;
      for (int index = 0; index < count3; ++index)
      {
        SECTR_Member.Child terrain = member.Terrains[index];
        if (terrain.terrainHash != 0 && !visibleTerrains.ContainsKey(terrain.terrainHash) && (!flag1 || SECTR_CullingCamera._IsVisible(terrain.terrainBounds, frustumPlanes, outMask1, occluders)))
          visibleTerrains.Add(terrain.terrainHash, terrain);
      }
    }
    if (!flag3)
      return;
    int count4 = member.Lights.Count;
    for (int index = 0; index < count4; ++index)
    {
      SECTR_Member.Child light = member.Lights[index];
      if (light.lightHash != 0 && !visibleLights.ContainsKey(light.lightHash) && (!flag1 || SECTR_CullingCamera._IsVisible(light.lightBounds, frustumPlanes, outMask1, occluders)))
      {
        visibleLights.Add(light.lightHash, light);
        if (light.shadowLight && !shadowLights.ContainsKey(light) && (double) Vector3.Distance(cameraPos, light.shadowLightPosition) - (double) light.shadowLightRange <= (double) shadowDistance)
          shadowLights.Add(light, 0);
      }
    }
  }

  private static void _ShadowCullSector(SECTR_Sector sector, List<SECTR_Member.Child> sectorShadowLights, ref Dictionary<int, SECTR_Member.Child> visibleRenderers, ref Dictionary<int, SECTR_Member.Child> visibleTerrains)
  {
    if (sector.ShadowCaster)
      SECTR_CullingCamera._ShadowCull((SECTR_Member) sector, sectorShadowLights, ref visibleRenderers, ref visibleTerrains);
    int count = sector.Members.Count;
    for (int index = 0; index < count; ++index)
    {
      SECTR_Member member = sector.Members[index];
      if (member.ShadowCaster)
        SECTR_CullingCamera._ShadowCull(member, sectorShadowLights, ref visibleRenderers, ref visibleTerrains);
    }
  }

  private static void _ShadowCull(SECTR_Member member, List<SECTR_Member.Child> shadowLights, ref Dictionary<int, SECTR_Member.Child> visibleRenderers, ref Dictionary<int, SECTR_Member.Child> visibleTerrains)
  {
    int count1 = shadowLights.Count;
    int count2 = member.ShadowCasters.Count;
    if (member.CullEachChild)
    {
      for (int index1 = 0; index1 < count2; ++index1)
      {
        SECTR_Member.Child shadowCaster = member.ShadowCasters[index1];
        if (shadowCaster.renderHash != 0 && !visibleRenderers.ContainsKey(shadowCaster.renderHash))
        {
          for (int index2 = 0; index2 < count1; ++index2)
          {
            SECTR_Member.Child shadowLight = shadowLights[index2];
            if ((shadowLight.shadowCullingMask & 1 << LayerMask.op_Implicit(shadowCaster.layer)) != 0 && (shadowLight.shadowLightType == null && ((Bounds) ref shadowCaster.rendererBounds).Intersects(shadowLight.lightBounds) || shadowLight.shadowLightType == 2 && SECTR_Geometry.BoundsIntersectsSphere(shadowCaster.rendererBounds, shadowLight.shadowLightPosition, shadowLight.shadowLightRange)))
            {
              visibleRenderers.Add(shadowCaster.renderHash, shadowCaster);
              break;
            }
          }
        }
        if (shadowCaster.terrainHash != 0 && !visibleTerrains.ContainsKey(shadowCaster.terrainHash))
        {
          for (int index2 = 0; index2 < count1; ++index2)
          {
            SECTR_Member.Child shadowLight = shadowLights[index2];
            if ((shadowLight.shadowCullingMask & 1 << LayerMask.op_Implicit(shadowCaster.layer)) != 0 && (shadowLight.shadowLightType == null && ((Bounds) ref shadowCaster.terrainBounds).Intersects(shadowLight.lightBounds) || shadowLight.shadowLightType == 2 && SECTR_Geometry.BoundsIntersectsSphere(shadowCaster.terrainBounds, shadowLight.shadowLightPosition, shadowLight.shadowLightRange)))
            {
              visibleTerrains.Add(shadowCaster.terrainHash, shadowCaster);
              break;
            }
          }
        }
      }
    }
    else
    {
      for (int index1 = 0; index1 < count1; ++index1)
      {
        SECTR_Member.Child shadowLight = shadowLights[index1];
        int num;
        if (shadowLight.shadowLightType == null)
        {
          Bounds renderBounds = member.RenderBounds;
          num = ((Bounds) ref renderBounds).Intersects(shadowLight.lightBounds) ? 1 : 0;
        }
        else
          num = SECTR_Geometry.BoundsIntersectsSphere(member.RenderBounds, shadowLight.shadowLightPosition, shadowLight.shadowLightRange) ? 1 : 0;
        if (num != 0)
        {
          int shadowCullingMask = shadowLight.shadowCullingMask;
          for (int index2 = 0; index2 < count2; ++index2)
          {
            SECTR_Member.Child shadowCaster = member.ShadowCasters[index2];
            if (shadowCaster.renderHash != 0 && shadowCaster.terrainHash != 0)
            {
              if ((shadowCullingMask & 1 << LayerMask.op_Implicit(shadowCaster.layer)) != 0)
              {
                if (!visibleRenderers.ContainsKey(shadowCaster.renderHash))
                  visibleRenderers.Add(shadowCaster.renderHash, shadowCaster);
                if (!visibleTerrains.ContainsKey(shadowCaster.terrainHash))
                  visibleTerrains.Add(shadowCaster.terrainHash, shadowCaster);
              }
            }
            else if (shadowCaster.renderHash != 0 && !visibleRenderers.ContainsKey(shadowCaster.renderHash) && (shadowCullingMask & 1 << LayerMask.op_Implicit(shadowCaster.layer)) != 0)
              visibleRenderers.Add(shadowCaster.renderHash, shadowCaster);
            else if (shadowCaster.terrainHash != 0 && !visibleTerrains.ContainsKey(shadowCaster.terrainHash) && (shadowCullingMask & 1 << LayerMask.op_Implicit(shadowCaster.layer)) != 0)
              visibleTerrains.Add(shadowCaster.terrainHash, shadowCaster);
          }
        }
      }
    }
  }

  private static bool _IsVisible(Bounds childBounds, List<Plane> frustumPlanes, int parentMask, List<List<Plane>> occluders)
  {
    int outMask;
    if (!SECTR_Geometry.FrustumIntersectsBounds(childBounds, frustumPlanes, parentMask, out outMask))
      return false;
    int count = occluders.Count;
    for (int index = 0; index < count; ++index)
    {
      if (SECTR_Geometry.FrustumContainsBounds(childBounds, occluders[index]))
        return false;
    }
    return true;
  }

  private void _HideAllMembers()
  {
    int count1 = SECTR_Member.All.Count;
    for (int index1 = 0; index1 < count1; ++index1)
    {
      SECTR_Member sectrMember = SECTR_Member.All[index1];
      int count2 = sectrMember.Renderers.Count;
      for (int index2 = 0; index2 < count2; ++index2)
      {
        SECTR_Member.Child renderer = sectrMember.Renderers[index2];
        renderer.renderCulled = true;
        if (Object.op_Implicit((Object) renderer.renderer))
          renderer.renderer.set_enabled(false);
        this.hiddenRenderers[renderer.renderHash] = renderer;
      }
      int count3 = sectrMember.Lights.Count;
      for (int index2 = 0; index2 < count3; ++index2)
      {
        SECTR_Member.Child light = sectrMember.Lights[index2];
        light.lightCulled = true;
        if (Object.op_Implicit((Object) light.light))
          ((Behaviour) light.light).set_enabled(false);
        this.hiddenLights[light.lightHash] = light;
      }
      int count4 = sectrMember.Terrains.Count;
      for (int index2 = 0; index2 < count4; ++index2)
      {
        SECTR_Member.Child terrain = sectrMember.Terrains[index2];
        terrain.terrainCulled = true;
        if (Object.op_Implicit((Object) terrain.terrain))
        {
          terrain.terrain.set_drawHeightmap(false);
          terrain.terrain.set_drawTreesAndFoliage(false);
        }
        this.hiddenTerrains[terrain.terrainHash] = terrain;
      }
    }
  }

  private void _ApplyCulling(bool visible)
  {
    Dictionary<int, SECTR_Member.Child>.Enumerator enumerator1 = this.visibleRenderers.GetEnumerator();
    while (enumerator1.MoveNext())
    {
      SECTR_Member.Child child = enumerator1.Current.Value;
      if (Object.op_Implicit((Object) child.renderer))
        child.renderer.set_enabled(visible);
      child.renderCulled = !visible;
      if (visible)
        this.hiddenRenderers.Remove(enumerator1.Current.Key);
      else
        this.hiddenRenderers[enumerator1.Current.Key] = child;
    }
    if (visible)
      this.renderersCulled = this.hiddenRenderers.Count;
    Dictionary<int, SECTR_Member.Child>.Enumerator enumerator2 = this.visibleLights.GetEnumerator();
    while (enumerator2.MoveNext())
    {
      SECTR_Member.Child child = enumerator2.Current.Value;
      if (Object.op_Implicit((Object) child.light))
        ((Behaviour) child.light).set_enabled(visible);
      child.lightCulled = !visible;
      if (visible)
        this.hiddenLights.Remove(enumerator2.Current.Key);
      else
        this.hiddenLights[enumerator2.Current.Key] = child;
    }
    if (visible)
      this.lightsCulled = this.hiddenLights.Count;
    Dictionary<int, SECTR_Member.Child>.Enumerator enumerator3 = this.visibleTerrains.GetEnumerator();
    while (enumerator3.MoveNext())
    {
      SECTR_Member.Child child = enumerator3.Current.Value;
      if (Object.op_Implicit((Object) child.terrain))
      {
        child.terrain.set_drawHeightmap(visible);
        child.terrain.set_drawTreesAndFoliage(visible);
      }
      child.terrainCulled = !visible;
      if (visible)
        this.hiddenTerrains.Remove(enumerator3.Current.Key);
      else
        this.hiddenTerrains[enumerator3.Current.Key] = child;
    }
    if (visible)
      this.terrainsCulled = this.hiddenTerrains.Count;
    this.didCull = true;
  }

  private void _UndoCulling()
  {
    if (!this.didCull)
      return;
    Dictionary<int, SECTR_Member.Child>.Enumerator enumerator1 = this.hiddenRenderers.GetEnumerator();
    while (enumerator1.MoveNext())
    {
      SECTR_Member.Child child = enumerator1.Current.Value;
      if (Object.op_Implicit((Object) child.renderer))
        child.renderer.set_enabled(true);
      child.renderCulled = false;
    }
    this.hiddenRenderers.Clear();
    Dictionary<int, SECTR_Member.Child>.Enumerator enumerator2 = this.hiddenLights.GetEnumerator();
    while (enumerator2.MoveNext())
    {
      SECTR_Member.Child child = enumerator2.Current.Value;
      if (Object.op_Implicit((Object) child.light))
        ((Behaviour) child.light).set_enabled(true);
      child.lightCulled = false;
    }
    this.hiddenLights.Clear();
    Dictionary<int, SECTR_Member.Child>.Enumerator enumerator3 = this.hiddenTerrains.GetEnumerator();
    while (enumerator3.MoveNext())
    {
      SECTR_Member.Child child = enumerator3.Current.Value;
      Terrain terrain = child.terrain;
      if (Object.op_Implicit((Object) child.terrain))
      {
        terrain.set_drawHeightmap(true);
        terrain.set_drawTreesAndFoliage(true);
      }
      child.terrainCulled = false;
    }
    this.hiddenTerrains.Clear();
    this.didCull = false;
  }

  private void _BuildFrustumFromHull(Camera cullingCamera, bool forwardTraversal, List<SECTR_CullingCamera.ClipVertex> portalVertices, ref List<Plane> newFrustum)
  {
    int count = portalVertices.Count;
    if (count <= 2)
      return;
    for (int index = 0; index < count; ++index)
    {
      Vector3 vector3_1 = Vector4.op_Implicit(portalVertices[index].vertex);
      Vector3 vector3_2 = Vector3.op_Subtraction(Vector4.op_Implicit(portalVertices[(index + 1) % count].vertex), vector3_1);
      if ((double) Vector3.SqrMagnitude(vector3_2) > 1.0 / 1000.0)
      {
        Vector3 vector3_3 = Vector3.op_Subtraction(vector3_1, ((Component) cullingCamera).get_transform().get_position());
        Vector3 vector3_4 = !forwardTraversal ? Vector3.Cross(vector3_3, vector3_2) : Vector3.Cross(vector3_2, vector3_3);
        ((Vector3) ref vector3_4).Normalize();
        newFrustum.Add(new Plane(vector3_4, vector3_1));
      }
    }
  }

  private struct VisibilityNode
  {
    public SECTR_Sector sector;
    public SECTR_Portal portal;
    public List<Plane> frustumPlanes;
    public bool forwardTraversal;

    public VisibilityNode(SECTR_CullingCamera cullingCamera, SECTR_Sector sector, SECTR_Portal portal, Plane[] frustumPlanes, bool forwardTraversal)
    {
      this.sector = sector;
      this.portal = portal;
      if (frustumPlanes == null)
        this.frustumPlanes = (List<Plane>) null;
      else if (cullingCamera.frustumPool.Count > 0)
      {
        this.frustumPlanes = cullingCamera.frustumPool.Pop();
        this.frustumPlanes.AddRange((IEnumerable<Plane>) frustumPlanes);
      }
      else
        this.frustumPlanes = new List<Plane>((IEnumerable<Plane>) frustumPlanes);
      this.forwardTraversal = forwardTraversal;
    }

    public VisibilityNode(SECTR_CullingCamera cullingCamera, SECTR_Sector sector, SECTR_Portal portal, List<Plane> frustumPlanes, bool forwardTraversal)
    {
      this.sector = sector;
      this.portal = portal;
      if (frustumPlanes == null)
        this.frustumPlanes = (List<Plane>) null;
      else if (cullingCamera.frustumPool.Count > 0)
      {
        this.frustumPlanes = cullingCamera.frustumPool.Pop();
        this.frustumPlanes.AddRange((IEnumerable<Plane>) frustumPlanes);
      }
      else
        this.frustumPlanes = new List<Plane>((IEnumerable<Plane>) frustumPlanes);
      this.forwardTraversal = forwardTraversal;
    }
  }

  private struct ClipVertex
  {
    public Vector4 vertex;
    public float side;

    public ClipVertex(Vector4 vertex, float side)
    {
      this.vertex = vertex;
      this.side = side;
    }
  }

  private struct ThreadCullData
  {
    public SECTR_Sector sector;
    public Vector3 cameraPos;
    public List<Plane> cullingPlanes;
    public List<List<Plane>> occluderFrustums;
    public int baseMask;
    public float shadowDistance;
    public bool cullingSimpleCulling;
    public List<SECTR_Member.Child> sectorShadowLights;
    public SECTR_CullingCamera.ThreadCullData.CullingModes cullingMode;

    public ThreadCullData(SECTR_Sector sector, SECTR_CullingCamera cullingCamera, Vector3 cameraPos, List<Plane> cullingPlanes, List<List<Plane>> occluderFrustums, int baseMask, float shadowDistance, bool cullingSimpleCulling)
    {
      this.sector = sector;
      this.cameraPos = cameraPos;
      this.baseMask = baseMask;
      this.shadowDistance = shadowDistance;
      this.cullingSimpleCulling = cullingSimpleCulling;
      this.sectorShadowLights = (List<SECTR_Member.Child>) null;
      lock ((object) cullingCamera.threadOccluderPool)
        this.occluderFrustums = cullingCamera.threadOccluderPool.Count <= 0 ? new List<List<Plane>>(occluderFrustums.Count) : cullingCamera.threadOccluderPool.Pop();
      lock ((object) cullingCamera.threadFrustumPool)
      {
        if (cullingCamera.threadFrustumPool.Count > 0)
        {
          this.cullingPlanes = cullingCamera.threadFrustumPool.Pop();
          this.cullingPlanes.AddRange((IEnumerable<Plane>) cullingPlanes);
        }
        else
          this.cullingPlanes = new List<Plane>((IEnumerable<Plane>) cullingPlanes);
        int count = occluderFrustums.Count;
        for (int index = 0; index < count; ++index)
        {
          List<Plane> planeList;
          if (cullingCamera.threadFrustumPool.Count > 0)
          {
            planeList = cullingCamera.threadFrustumPool.Pop();
            planeList.AddRange((IEnumerable<Plane>) occluderFrustums[index]);
          }
          else
            planeList = new List<Plane>((IEnumerable<Plane>) occluderFrustums[index]);
          this.occluderFrustums.Add(planeList);
        }
      }
      this.cullingMode = SECTR_CullingCamera.ThreadCullData.CullingModes.Graph;
    }

    public ThreadCullData(SECTR_Sector sector, Vector3 cameraPos, List<SECTR_Member.Child> sectorShadowLights)
    {
      this.sector = sector;
      this.cameraPos = cameraPos;
      this.cullingPlanes = (List<Plane>) null;
      this.occluderFrustums = (List<List<Plane>>) null;
      this.baseMask = 0;
      this.shadowDistance = 0.0f;
      this.cullingSimpleCulling = false;
      this.sectorShadowLights = sectorShadowLights;
      this.cullingMode = SECTR_CullingCamera.ThreadCullData.CullingModes.Shadow;
    }

    public enum CullingModes
    {
      None,
      Graph,
      Shadow,
    }
  }
}
