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
  private int myRadio = -1;
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

  public void ResetPreset()
  {
    this.NetworkcurPreset = 1;
    this.CallCmdUpdatePreset(1);
    if (this.myRadio >= 0)
      return;
    for (int index = 0; index < (int) this.inv.items.Count; ++index)
    {
      if (this.inv.items[index].id == 12)
        this.radioUniq = this.inv.items[index].uniq;
    }
  }

  private void Start()
  {
    if ((Object) Radio.comms == (Object) null)
      Radio.comms = Object.FindObjectOfType<DissonanceComms>();
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.noiseSource = GameObject.Find("RadioNoiseSound").GetComponent<AudioSource>();
    this.inv = this.GetComponent<Inventory>();
    this.scp939 = this.GetComponent<Scp939PlayerScript>();
    if (this.isLocalPlayer)
    {
      Radio.roundEnded = false;
      Radio.localRadio = this;
    }
    if (NetworkServer.active)
      this.InvokeRepeating("UseBattery", 1f, 1f);
    this.icon = this.GetComponentInChildren<SpeakerIcon>();
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
    if ((Object) this.unityPlayback == (Object) null && !string.IsNullOrEmpty(this.GetComponent<HlapiPlayer>().PlayerId))
    {
      this.state = Radio.comms.FindPlayer(this.GetComponent<HlapiPlayer>().PlayerId);
      if (this.state != null && this.state.Playback != null)
        this.unityPlayback = (VoicePlayback) this.state.Playback;
    }
    this.UpdateClass();
    if ((Object) this.host == (Object) null)
      this.host = GameObject.Find("Host");
    if (this.inv.GetItemIndex() != -1 && this.inv.items[this.inv.GetItemIndex()].id == 12)
      this.radioUniq = this.inv.items[this.inv.GetItemIndex()].uniq;
    this.myRadio = -1;
    for (int index = 0; index < (int) this.inv.items.Count; ++index)
    {
      if (this.inv.items[index].uniq == this.radioUniq)
        this.myRadio = index;
    }
    if (!this.isLocalPlayer)
      return;
    this.noiseSource.volume = Radio.noiseIntensity * this.noiseMultiplier;
    Radio.noiseIntensity = 0.0f;
    this.GetInput();
    if (this.myRadio != -1)
    {
      RadioDisplay.battery = Mathf.Clamp(Mathf.CeilToInt(this.inv.items[this.myRadio].durability), 0, 100).ToString();
      RadioDisplay.power = this.presets[this.curPreset].powerText;
      RadioDisplay.label = this.presets[this.curPreset].label;
    }
    using (IEnumerator<Inventory.SyncItemInfo> enumerator = this.inv.items.GetEnumerator())
    {
      do
        ;
      while (enumerator.MoveNext() && enumerator.Current.id != 12);
    }
  }

  private void UseBattery()
  {
    if (!this.CheckRadio() || this.inv.items[this.myRadio].id != 12)
      return;
    float num = this.inv.items[this.myRadio].durability - (float) (1.66999995708466 * (1.0 / (double) this.presets[this.curPreset].powerTime) * (!this.isTransmitting ? 1.0 : 3.0));
    if ((double) num <= -1.0 || (double) num >= 101.0)
      return;
    this.inv.items.ModifyDuration(this.myRadio, num);
  }

  private void GetInput()
  {
    if ((double) this.timeToNextTransmition > 0.0)
      this.timeToNextTransmition -= Time.deltaTime;
    bool b = Input.GetKey(NewInput.GetKey("Voice Chat")) && this.CheckRadio();
    if (b != this.isTransmitting && (double) this.timeToNextTransmition <= 0.0)
    {
      this.NetworkisTransmitting = b;
      this.timeToNextTransmition = 0.5f;
      this.CallCmdSyncTransmitionStatus(b, this.transform.position);
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
    if (this.isLocalPlayer || (Object) this.unityPlayback == (Object) null)
      return;
    this.mySource = this.unityPlayback.AudioSource;
    this.icon.id = 0;
    bool flag1 = false;
    bool flag2 = false;
    this.mySource.outputAudioMixerGroup = this.g_voice;
    this.mySource.volume = 0.0f;
    this.mySource.spatialBlend = 1f;
    if (!Radio.roundStarted || Radio.roundEnded || this.voiceInfo.IsDead() && Radio.localRadio.voiceInfo.IsDead())
    {
      this.mySource.volume = 1f;
      this.mySource.spatialBlend = 0.0f;
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
        this.mySource.volume = 1f;
      }
      if (!flag1 && (Object) this.host != (Object) null && (Object) this.gameObject == (Object) this.host.GetComponent<Intercom>().speaker)
      {
        this.icon.id = 2;
        this.mySource.outputAudioMixerGroup = this.g_icom;
        flag1 = true;
        if ((double) this.icomNoise > (double) Radio.noiseIntensity)
          Radio.noiseIntensity = this.icomNoise;
      }
      else if (this.isTransmitting && Radio.localRadio.CheckRadio() && !flag1)
      {
        this.mySource.outputAudioMixerGroup = this.g_radio;
        flag1 = true;
        int lowerPresetId = this.GetLowerPresetID();
        float time = Vector3.Distance(Radio.localRadio.transform.position, this.transform.position);
        this.mySource.volume = this.presets[lowerPresetId].volume.Evaluate(time);
        float num = this.presets[lowerPresetId].nosie.Evaluate(time);
        if ((double) num > (double) Radio.noiseIntensity && !this.isLocalPlayer)
          Radio.noiseIntensity = num;
      }
      if (this.isTransmitting)
        this.icon.id = 2;
      if (!flag1)
        return;
      this.mySource.spatialBlend = 0.0f;
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
    if (this.myRadio != -1 && ((double) this.inv.items[this.myRadio].durability > 0.0 && this.voiceInfo.isAliveHuman))
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
    if (!((Object) Radio.localRadio != (Object) null) || !Radio.localRadio.CheckRadio() || (double) this.presets[this.GetLowerPresetID()].beepRange <= (double) this.Distance(myPos, Radio.localRadio.transform.position))
      return;
    this.beepSource.PlayOneShot(!b ? this.b_off : this.b_on);
  }

  private float Distance(Vector3 a, Vector3 b)
  {
    return Vector3.Distance(new Vector3(a.x, a.y / 4f, a.z), new Vector3(b.x, b.y / 4f, b.z));
  }

  public bool ShouldBeVisible(GameObject localplayer)
  {
    if (this.isTransmitting)
      return (double) this.presets[this.GetLowerPresetID()].beepRange > (double) this.Distance(this.transform.position, localplayer.transform.position);
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
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetPreset(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<int>(num1, ref local, (uint) num2);
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
      this.SetSyncVar<bool>(value, ref this.isTransmitting, 2U);
    }
  }

  protected static void InvokeCmdCmdSyncTransmitionStatus(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSyncTransmitionStatus called on client.");
    else
      ((Radio) obj).CmdSyncTransmitionStatus(reader.ReadBoolean(), reader.ReadVector3());
  }

  protected static void InvokeCmdCmdUpdatePreset(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUpdatePreset called on client.");
    else
      ((Radio) obj).CmdUpdatePreset((int) reader.ReadPackedUInt32());
  }

  public void CallCmdSyncTransmitionStatus(bool b, Vector3 myPos)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSyncTransmitionStatus called on server.");
    else if (this.isServer)
    {
      this.CmdSyncTransmitionStatus(b, myPos);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Radio.kCmdCmdSyncTransmitionStatus);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(b);
      writer.Write(myPos);
      this.SendCommandInternal(writer, 6, "CmdSyncTransmitionStatus");
    }
  }

  public void CallCmdUpdatePreset(int preset)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUpdatePreset called on server.");
    else if (this.isServer)
    {
      this.CmdUpdatePreset(preset);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Radio.kCmdCmdUpdatePreset);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) preset);
      this.SendCommandInternal(writer, 6, "CmdUpdatePreset");
    }
  }

  protected static void InvokeRpcRpcPlaySound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcPlaySound called on server.");
    else
      ((Radio) obj).RpcPlaySound(reader.ReadBoolean(), reader.ReadVector3());
  }

  public void CallRpcPlaySound(bool b, Vector3 myPos)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcPlaySound called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Radio.kRpcRpcPlaySound);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(b);
      writer.Write(myPos);
      this.SendRPCInternal(writer, 0, "RpcPlaySound");
    }
  }

  static Radio()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Radio), Radio.kCmdCmdSyncTransmitionStatus, new NetworkBehaviour.CmdDelegate(Radio.InvokeCmdCmdSyncTransmitionStatus));
    Radio.kCmdCmdUpdatePreset = -1209260349;
    NetworkBehaviour.RegisterCommandDelegate(typeof (Radio), Radio.kCmdCmdUpdatePreset, new NetworkBehaviour.CmdDelegate(Radio.InvokeCmdCmdUpdatePreset));
    Radio.kRpcRpcPlaySound = 1107833674;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Radio), Radio.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate(Radio.InvokeRpcRpcPlaySound));
    NetworkCRC.RegisterBehaviour(nameof (Radio), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.curPreset);
      writer.Write(this.isTransmitting);
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
      writer.WritePackedUInt32((uint) this.curPreset);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.isTransmitting);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
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
