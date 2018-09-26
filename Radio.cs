// Decompiled with JetBrains decompiler
// Type: Radio
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance;
using Dissonance.Audio.Playback;
using Dissonance.Integrations.UNet_HLAPI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class Radio : NetworkBehaviour
{
  private static int kCmdCmdSyncTransmitionStatus = 860412084;
  public static Radio localRadio;
  public AudioMixerGroup g_voice;
  public AudioMixerGroup g_radio;
  public AudioMixerGroup g_icom;
  public AudioClip b_on;
  public AudioClip b_off;
  public AudioClip b_battery;
  public AudioSource beepSource;
  [Space]
  public AudioSource mySource;
  [Space]
  public Radio.VoiceInfo voiceInfo;
  public Radio.RadioPreset[] presets;
  [SyncVar(hook = "SetPreset")]
  public int curPreset;
  [SyncVar]
  public bool isTransmitting;
  private int myRadio;
  private float timeToNextTransmition;
  private AudioSource noiseSource;
  private int lastPreset;
  private SpeakerIcon icon;
  private static float noiseIntensity;
  public static bool roundStarted;
  public static bool roundEnded;
  private GameObject host;
  public float icomNoise;
  private Inventory inv;
  public float noiseMultiplier;
  private CharacterClassManager ccm;
  private Scp939PlayerScript scp939;
  private static DissonanceComms comms;
  private VoicePlayerState state;
  private VoicePlayback unityPlayback;
  private int radioUniq;
  private static int kRpcRpcPlaySound;
  private static int kCmdCmdUpdatePreset;

  public Radio()
  {
    base.\u002Ector();
  }

  public void ResetPreset()
  {
    this.NetworkcurPreset = 1;
    this.CallCmdUpdatePreset(1);
    if (this.myRadio >= 0)
      return;
    for (int index = 0; index < (int) this.inv.items.get_Count(); ++index)
    {
      if (((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(index).id == 12)
        this.radioUniq = ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(index).uniq;
    }
  }

  private void Start()
  {
    if (Object.op_Equality((Object) Radio.comms, (Object) null))
      Radio.comms = (DissonanceComms) Object.FindObjectOfType<DissonanceComms>();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.noiseSource = (AudioSource) GameObject.Find("RadioNoiseSound").GetComponent<AudioSource>();
    this.inv = (Inventory) ((Component) this).GetComponent<Inventory>();
    this.scp939 = (Scp939PlayerScript) ((Component) this).GetComponent<Scp939PlayerScript>();
    if (this.get_isLocalPlayer())
    {
      Radio.roundEnded = false;
      Radio.localRadio = this;
    }
    if (NetworkServer.get_active())
      ((MonoBehaviour) this).InvokeRepeating("UseBattery", 1f, 1f);
    this.icon = (SpeakerIcon) ((Component) this).GetComponentInChildren<SpeakerIcon>();
  }

  public void UpdateClass()
  {
    bool flag1 = false;
    bool flag2 = false;
    if (this.ccm.curClass != -1 && this.ccm.curClass != 2)
    {
      if (this.ccm.klasy[this.ccm.curClass].team == Team.SCP)
        flag1 = true;
      else
        flag2 = true;
    }
    this.voiceInfo.isAliveHuman = flag2;
    this.voiceInfo.isSCP = flag1;
  }

  private void Update()
  {
    if (Object.op_Equality((Object) this.unityPlayback, (Object) null) && !string.IsNullOrEmpty(((HlapiPlayer) ((Component) this).GetComponent<HlapiPlayer>()).PlayerId))
    {
      this.state = Radio.comms.FindPlayer(((HlapiPlayer) ((Component) this).GetComponent<HlapiPlayer>()).PlayerId);
      if (this.state != null && this.state.get_Playback() != null)
        this.unityPlayback = (VoicePlayback) this.state.get_Playback();
    }
    this.UpdateClass();
    if (Object.op_Equality((Object) this.host, (Object) null))
      this.host = GameObject.Find("Host");
    if (this.inv.GetItemIndex() != -1 && ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.inv.GetItemIndex()).id == 12)
      this.radioUniq = ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.inv.GetItemIndex()).uniq;
    this.myRadio = -1;
    for (int index = 0; index < (int) this.inv.items.get_Count(); ++index)
    {
      if (((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(index).uniq == this.radioUniq)
        this.myRadio = index;
    }
    if (!this.get_isLocalPlayer())
      return;
    this.noiseSource.set_volume(Radio.noiseIntensity * this.noiseMultiplier);
    Radio.noiseIntensity = 0.0f;
    this.GetInput();
    if (this.myRadio != -1)
    {
      RadioDisplay.battery = Mathf.Clamp(Mathf.CeilToInt(((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.myRadio).durability), 0, 100).ToString();
      RadioDisplay.power = this.presets[this.curPreset].powerText;
      RadioDisplay.label = this.presets[this.curPreset].label;
    }
    using (IEnumerator<Inventory.SyncItemInfo> enumerator = ((SyncList<Inventory.SyncItemInfo>) this.inv.items).GetEnumerator())
    {
      do
        ;
      while (enumerator.MoveNext() && enumerator.Current.id != 12);
    }
  }

  private void UseBattery()
  {
    if (!this.CheckRadio() || ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.myRadio).id != 12)
      return;
    float num = ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.myRadio).durability - (float) (1.66999995708466 * (1.0 / (double) this.presets[this.curPreset].powerTime) * (!this.isTransmitting ? 1.0 : 3.0));
    if ((double) num <= -1.0 || (double) num >= 101.0)
      return;
    this.inv.items.ModifyDuration(this.myRadio, num);
  }

  private void GetInput()
  {
    if ((double) this.timeToNextTransmition > 0.0)
      this.timeToNextTransmition -= Time.get_deltaTime();
    bool b = Input.GetKey(NewInput.GetKey("Voice Chat")) && this.CheckRadio();
    if (b != this.isTransmitting && (double) this.timeToNextTransmition <= 0.0)
    {
      this.NetworkisTransmitting = b;
      this.timeToNextTransmition = 0.5f;
      this.CallCmdSyncTransmitionStatus(b, ((Component) this).get_transform().get_position());
    }
    if (this.inv.curItem != 12 || (double) Inventory.inventoryCooldown > 0.0)
      return;
    if (Input.GetKeyDown(NewInput.GetKey("Shoot")) && this.curPreset != 0)
    {
      Radio radio = this;
      radio.NetworkcurPreset = radio.curPreset + 1;
      if (this.curPreset >= this.presets.Length)
        this.NetworkcurPreset = 1;
      this.lastPreset = this.curPreset;
      this.CallCmdUpdatePreset(this.curPreset);
    }
    if (!Input.GetKeyDown(NewInput.GetKey("Zoom")))
      return;
    this.lastPreset = Mathf.Clamp(this.lastPreset, 1, this.presets.Length - 1);
    this.NetworkcurPreset = this.curPreset != 0 ? 0 : this.lastPreset;
    this.CallCmdUpdatePreset(this.curPreset);
  }

  public void SetRelationship()
  {
    if (this.get_isLocalPlayer() || Object.op_Equality((Object) this.unityPlayback, (Object) null))
      return;
    this.mySource = this.unityPlayback.get_AudioSource();
    this.icon.id = 0;
    bool flag1 = false;
    bool flag2 = false;
    this.mySource.set_outputAudioMixerGroup(this.g_voice);
    this.mySource.set_volume(0.0f);
    this.mySource.set_spatialBlend(1f);
    if (!Radio.roundStarted || Radio.roundEnded || this.voiceInfo.IsDead() && Radio.localRadio.voiceInfo.IsDead())
    {
      this.mySource.set_volume(1f);
      this.mySource.set_spatialBlend(0.0f);
    }
    else
    {
      if (this.voiceInfo.isAliveHuman || this.scp939.iAm939 && this.scp939.usingHumanChat)
        flag2 = true;
      if (this.voiceInfo.isSCP && Radio.localRadio.voiceInfo.isSCP)
      {
        flag2 = true;
        flag1 = true;
      }
      if (flag2)
      {
        this.icon.id = 1;
        this.mySource.set_volume(1f);
      }
      if (!flag1 && Object.op_Inequality((Object) this.host, (Object) null) && Object.op_Equality((Object) ((Component) this).get_gameObject(), (Object) ((Intercom) this.host.GetComponent<Intercom>()).speaker))
      {
        this.icon.id = 2;
        this.mySource.set_outputAudioMixerGroup(this.g_icom);
        flag1 = true;
        if ((double) this.icomNoise > (double) Radio.noiseIntensity)
          Radio.noiseIntensity = this.icomNoise;
      }
      else if (this.isTransmitting && Radio.localRadio.CheckRadio() && !flag1)
      {
        this.mySource.set_outputAudioMixerGroup(this.g_radio);
        flag1 = true;
        int lowerPresetId = this.GetLowerPresetID();
        float num1 = Vector3.Distance(((Component) Radio.localRadio).get_transform().get_position(), ((Component) this).get_transform().get_position());
        this.mySource.set_volume(this.presets[lowerPresetId].volume.Evaluate(num1));
        float num2 = this.presets[lowerPresetId].nosie.Evaluate(num1);
        if ((double) num2 > (double) Radio.noiseIntensity && !this.get_isLocalPlayer())
          Radio.noiseIntensity = num2;
      }
      if (this.isTransmitting)
        this.icon.id = 2;
      if (!flag1)
        return;
      this.mySource.set_spatialBlend(0.0f);
    }
  }

  public int GetLowerPresetID()
  {
    if (this.curPreset < Radio.localRadio.curPreset)
      return this.curPreset;
    return Radio.localRadio.curPreset;
  }

  public bool CheckRadio()
  {
    if (this.myRadio != -1 && ((double) ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.myRadio).durability > 0.0 && this.voiceInfo.isAliveHuman))
      return this.curPreset > 0;
    return false;
  }

  [Command(channel = 6)]
  private void CmdSyncTransmitionStatus(bool b, Vector3 myPos)
  {
    this.NetworkisTransmitting = b;
    this.CallRpcPlaySound(b, myPos);
  }

  [ClientRpc]
  private void RpcPlaySound(bool b, Vector3 myPos)
  {
    if (!Object.op_Inequality((Object) Radio.localRadio, (Object) null) || !Radio.localRadio.CheckRadio() || (double) this.presets[this.GetLowerPresetID()].beepRange <= (double) this.Distance(myPos, ((Component) Radio.localRadio).get_transform().get_position()))
      return;
    this.beepSource.PlayOneShot(!b ? this.b_off : this.b_on);
  }

  private float Distance(Vector3 a, Vector3 b)
  {
    return Vector3.Distance(new Vector3((float) a.x, (float) (a.y / 4.0), (float) a.z), new Vector3((float) b.x, (float) (b.y / 4.0), (float) b.z));
  }

  public bool ShouldBeVisible(GameObject localplayer)
  {
    if (this.isTransmitting)
      return (double) this.presets[this.GetLowerPresetID()].beepRange > (double) this.Distance(((Component) this).get_transform().get_position(), localplayer.get_transform().get_position());
    return true;
  }

  private void SetPreset(int preset)
  {
    this.NetworkcurPreset = preset;
  }

  [Command(channel = 6)]
  public void CmdUpdatePreset(int preset)
  {
    this.SetPreset(preset);
  }

  private void UNetVersion()
  {
  }

  public int NetworkcurPreset
  {
    get
    {
      return this.curPreset;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.curPreset;
      int num2 = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetPreset(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public bool NetworkisTransmitting
  {
    get
    {
      return this.isTransmitting;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this.isTransmitting, 2U);
    }
  }

  protected static void InvokeCmdCmdSyncTransmitionStatus(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSyncTransmitionStatus called on client.");
    else
      ((Radio) obj).CmdSyncTransmitionStatus(reader.ReadBoolean(), (Vector3) reader.ReadVector3());
  }

  protected static void InvokeCmdCmdUpdatePreset(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUpdatePreset called on client.");
    else
      ((Radio) obj).CmdUpdatePreset((int) reader.ReadPackedUInt32());
  }

  public void CallCmdSyncTransmitionStatus(bool b, Vector3 myPos)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSyncTransmitionStatus called on server.");
    else if (this.get_isServer())
    {
      this.CmdSyncTransmitionStatus(b, myPos);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Radio.kCmdCmdSyncTransmitionStatus);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(b);
      networkWriter.Write((Vector3) myPos);
      this.SendCommandInternal(networkWriter, 6, "CmdSyncTransmitionStatus");
    }
  }

  public void CallCmdUpdatePreset(int preset)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUpdatePreset called on server.");
    else if (this.get_isServer())
    {
      this.CmdUpdatePreset(preset);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Radio.kCmdCmdUpdatePreset);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) preset);
      this.SendCommandInternal(networkWriter, 6, "CmdUpdatePreset");
    }
  }

  protected static void InvokeRpcRpcPlaySound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcPlaySound called on server.");
    else
      ((Radio) obj).RpcPlaySound(reader.ReadBoolean(), (Vector3) reader.ReadVector3());
  }

  public void CallRpcPlaySound(bool b, Vector3 myPos)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcPlaySound called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Radio.kRpcRpcPlaySound);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(b);
      networkWriter.Write((Vector3) myPos);
      this.SendRPCInternal(networkWriter, 0, "RpcPlaySound");
    }
  }

  static Radio()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Radio), Radio.kCmdCmdSyncTransmitionStatus, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSyncTransmitionStatus)));
    Radio.kCmdCmdUpdatePreset = -1209260349;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Radio), Radio.kCmdCmdUpdatePreset, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUpdatePreset)));
    Radio.kRpcRpcPlaySound = 1107833674;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Radio), Radio.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcPlaySound)));
    NetworkCRC.RegisterBehaviour(nameof (Radio), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.curPreset);
      writer.Write(this.isTransmitting);
      return true;
    }
    bool flag = false;
    if (((int) this.get_syncVarDirtyBits() & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.curPreset);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.isTransmitting);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.curPreset = (int) reader.ReadPackedUInt32();
      this.isTransmitting = reader.ReadBoolean();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.SetPreset((int) reader.ReadPackedUInt32());
      if ((num & 2) == 0)
        return;
      this.isTransmitting = reader.ReadBoolean();
    }
  }

  [Serializable]
  public class VoiceInfo
  {
    public bool isAliveHuman;
    public bool isSCP;

    public bool IsDead()
    {
      return (this.isSCP ? 1 : (this.isAliveHuman ? 1 : 0)) == 0;
    }
  }

  [Serializable]
  public class RadioPreset
  {
    public string label;
    public string powerText;
    public float powerTime;
    public AnimationCurve nosie;
    public AnimationCurve volume;
    public float beepRange;
  }
}
