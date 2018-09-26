// Decompiled with JetBrains decompiler
// Type: AlphaWarheadNukesitePanel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class AlphaWarheadNukesitePanel : NetworkBehaviour
{
  public Transform lever;
  public BlastDoor blastDoor;
  public Door outsideDoor;
  public Material led_blastdoors;
  public Material led_outsidedoor;
  public Material led_detonationinprogress;
  public Material led_cancel;
  public Material[] onOffMaterial;
  private float _leverStatus;
  [SyncVar(hook = "SetEnabled")]
  public bool enabled;

  private void Awake()
  {
    AlphaWarheadOutsitePanel.nukeside = this;
  }

  private void FixedUpdate()
  {
    this.UpdateLeverStatus();
  }

  public bool AllowChangeLevelState()
  {
    if ((double) this._leverStatus != 0.0)
      return (double) this._leverStatus == 1.0;
    return true;
  }

  private void UpdateLeverStatus()
  {
    if ((Object) AlphaWarheadController.host == (Object) null)
      return;
    Color color = new Color(0.2f, 0.3f, 0.5f);
    this.led_detonationinprogress.SetColor("_EmissionColor", !AlphaWarheadController.host.inProgress ? Color.black : color);
    this.led_outsidedoor.SetColor("_EmissionColor", !this.outsideDoor.isOpen ? Color.black : color);
    this.led_blastdoors.SetColor("_EmissionColor", !this.blastDoor.isClosed ? Color.black : color);
    this.led_cancel.SetColor("_EmissionColor", (double) AlphaWarheadController.host.timeToDetonation <= 10.0 || !AlphaWarheadController.host.inProgress ? Color.black : Color.red);
    this._leverStatus += !this.enabled ? -0.04f : 0.04f;
    this._leverStatus = Mathf.Clamp01(this._leverStatus);
    for (int index = 0; index < 2; ++index)
      this.onOffMaterial[index].SetColor("_EmissionColor", index != Mathf.RoundToInt(this._leverStatus) ? Color.black : new Color(1.2f, 1.2f, 1.2f, 1f));
    this.lever.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(10f, -170f, this._leverStatus), -90f, 90f));
  }

  public void SetEnabled(bool b)
  {
    this.Networkenabled = b;
  }

  private void UNetVersion()
  {
  }

  public bool Networkenabled
  {
    get
    {
      return this.enabled;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.enabled;
      int num2 = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetEnabled(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<bool>(num1 != 0, ref local, (uint) num2);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.enabled);
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
      writer.Write(this.enabled);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.enabled = reader.ReadBoolean();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetEnabled(reader.ReadBoolean());
    }
  }
}
