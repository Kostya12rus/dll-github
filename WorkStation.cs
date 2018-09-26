// Decompiled with JetBrains decompiler
// Type: WorkStation
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorkStation : NetworkBehaviour
{
  private string currentGroup = "unconnected";
  private bool prevConn;
  [SyncVar]
  public bool isTabletConnected;
  [SyncVar]
  private GameObject playerConnected;
  [SyncVar(hook = "SetPosition")]
  private Offset position;
  private static WorkStation updateRoot;
  private float animationCooldown;
  private MeshRenderer[] meshRenderers;
  public GameObject ui_place;
  public GameObject ui_take;
  public GameObject ui_using;
  public GameObject ui_notablet;
  public Animator animator;
  public WorkStation.WorkStationScreenGroup screenGroup;
  private Button[] buttons;
  public AudioClip beepClip;
  public AudioClip powerOnClip;
  public AudioClip powerOffClip;
  private NetworkInstanceId ___playerConnectedNetId;

  private void Start()
  {
    WorkStation.updateRoot = this;
    Timing.RunCoroutine(this._Update(), Segment.Update);
    this.Invoke("UnmuteSource", 10f);
    this.meshRenderers = this.GetComponentsInChildren<MeshRenderer>(true);
  }

  private void UnmuteSource()
  {
    this.GetComponent<AudioSource>().mute = false;
  }

  public void SetPosition(Offset pos)
  {
    this.Networkposition = pos;
  }

  private void Update()
  {
    if (this.transform.localPosition != this.position.position)
    {
      this.transform.localPosition = this.position.position;
      this.transform.localRotation = Quaternion.Euler(this.position.rotation);
    }
    this.CheckConnectionChange();
    this.screenGroup.SetScreenByName(this.currentGroup);
    if ((double) this.animationCooldown < 0.0)
      return;
    this.animationCooldown -= Time.deltaTime;
  }

  public void ChangeScreen(string scene)
  {
    this.currentGroup = scene;
  }

  public void UseButton(Button button)
  {
    if (!PlayerManager.localPlayer.GetComponent<CharacterClassManager>().IsHuman())
      return;
    foreach (Button button1 in this.buttons)
    {
      if ((Object) button1 == (Object) button)
      {
        this.GetComponent<AudioSource>().PlayOneShot(this.beepClip);
        button1.onClick.Invoke();
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator<float> _Update()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WorkStation.\u003C_Update\u003Ec__Iterator0() { \u0024this = this };
  }

  private void CheckConnectionChange()
  {
    foreach (Renderer meshRenderer in this.meshRenderers)
      meshRenderer.enabled = true;
    if (this.prevConn == this.isTabletConnected)
      return;
    this.prevConn = this.isTabletConnected;
    if (this.prevConn)
      Timing.RunCoroutine(this._OnTabletConnected(), Segment.FixedUpdate);
    else
      Timing.RunCoroutine(this._OnTabletDisconnected(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _OnTabletConnected()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WorkStation.\u003C_OnTabletConnected\u003Ec__Iterator1() { \u0024this = this };
  }

  [DebuggerHidden]
  private IEnumerator<float> _OnTabletDisconnected()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WorkStation.\u003C_OnTabletDisconnected\u003Ec__Iterator2() { \u0024this = this };
  }

  public bool CanPlace(GameObject tabletOwner)
  {
    if ((Object) this.playerConnected != (Object) null || this.isTabletConnected)
      return false;
    return this.HasInInventory(tabletOwner);
  }

  private bool HasInInventory(GameObject tabletOwner)
  {
    foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) tabletOwner.GetComponent<Inventory>().items)
    {
      if (syncItemInfo.id == 19)
        return true;
    }
    return false;
  }

  public bool CanTake(GameObject taker)
  {
    if ((Object) taker != (Object) this.playerConnected && (Object) this.playerConnected != (Object) null && (double) Vector3.Distance(this.playerConnected.transform.position, this.transform.position) < 10.0)
      return false;
    return taker.GetComponent<Inventory>().items.Count < (ushort) 8;
  }

  public void UnconnectTablet(GameObject taker)
  {
    if (!this.CanTake(taker) || (double) this.animationCooldown > 0.0)
      return;
    taker.GetComponent<Inventory>().AddNewItem(19, -4.656647E+11f);
    this.NetworkplayerConnected = (GameObject) null;
    this.NetworkisTabletConnected = false;
    this.animationCooldown = 3.5f;
  }

  public void ConnectTablet(GameObject tabletOwner)
  {
    if (!this.CanPlace(tabletOwner) || (double) this.animationCooldown > 0.0)
      return;
    Inventory component = tabletOwner.GetComponent<Inventory>();
    foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) component.items)
    {
      if (syncItemInfo.id == 19)
      {
        component.items.Remove(syncItemInfo);
        this.NetworkisTabletConnected = true;
        this.animationCooldown = 6.5f;
        this.NetworkplayerConnected = tabletOwner;
        break;
      }
    }
  }

  private void UNetVersion()
  {
  }

  public bool NetworkisTabletConnected
  {
    get
    {
      return this.isTabletConnected;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>(value, ref this.isTabletConnected, 1U);
    }
  }

  public GameObject NetworkplayerConnected
  {
    get
    {
      return this.playerConnected;
    }
    [param: In] set
    {
      this.SetSyncVarGameObject(value, ref this.playerConnected, 2U, ref this.___playerConnectedNetId);
    }
  }

  public Offset Networkposition
  {
    get
    {
      return this.position;
    }
    [param: In] set
    {
      Offset offset = value;
      ref Offset local = ref this.position;
      int num = 4;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetPosition(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<Offset>(offset, ref local, (uint) num);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.isTabletConnected);
      writer.Write(this.playerConnected);
      GeneratedNetworkCode._WriteOffset_None(writer, this.position);
      return true;
    }
    bool flag = false;
    if (((int) this.syncVarDirtyBits & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.isTabletConnected);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.playerConnected);
    }
    if (((int) this.syncVarDirtyBits & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      GeneratedNetworkCode._WriteOffset_None(writer, this.position);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.isTabletConnected = reader.ReadBoolean();
      this.___playerConnectedNetId = reader.ReadNetworkId();
      this.position = GeneratedNetworkCode._ReadOffset_None(reader);
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.isTabletConnected = reader.ReadBoolean();
      if ((num & 2) != 0)
        this.playerConnected = reader.ReadGameObject();
      if ((num & 4) == 0)
        return;
      this.SetPosition(GeneratedNetworkCode._ReadOffset_None(reader));
    }
  }

  public override void PreStartClient()
  {
    if (this.___playerConnectedNetId.IsEmpty())
      return;
    this.NetworkplayerConnected = ClientScene.FindLocalObject(this.___playerConnectedNetId);
  }

  [Serializable]
  public class WorkStationScreenGroup
  {
    public WorkStation.WorkStationScreenGroup.WorkStationScreen[] screens;
    private string curScreen;
    private WorkStation station;

    public void SetWorkstation(WorkStation s)
    {
      this.station = s;
    }

    public void SetScreenByName(string _label)
    {
      if (this.curScreen == _label)
        return;
      this.curScreen = _label;
      if (!this.station.GetComponent<AudioSource>().isPlaying)
        this.station.GetComponent<AudioSource>().PlayOneShot(this.station.beepClip);
      foreach (WorkStation.WorkStationScreenGroup.WorkStationScreen screen in this.screens)
        screen.screenObject.SetActive(screen.label == _label);
    }

    [Serializable]
    public class WorkStationScreen
    {
      public string label;
      public GameObject screenObject;
    }
  }
}
