// Decompiled with JetBrains decompiler
// Type: AlphaWarheadController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class AlphaWarheadController : NetworkBehaviour
{
  private static int kRpcRpcShake = -737840022;
  public AlphaWarheadController.DetonationScenario[] scenarios_start;
  public AlphaWarheadController.DetonationScenario[] scenarios_resume;
  public AudioClip sound_canceled;
  internal BlastDoor[] blastDoors;
  public bool doorsClosed;
  public bool doorsOpen;
  public bool detonated;
  public int cooldown;
  public int warheadKills;
  private static int _startScenario;
  private static int _resumeScenario;
  private float _shake;
  public static AudioSource alarmSource;
  public static AlphaWarheadController host;
  [SyncVar(hook = "SetTime")]
  public float timeToDetonation;
  [SyncVar(hook = "SetStartScenario")]
  public int sync_startScenario;
  [SyncVar(hook = "SetResumeScenario")]
  public int sync_resumeScenario;
  [SyncVar(hook = "SetProgress")]
  public bool inProgress;

  public AlphaWarheadController()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (!this.get_isLocalPlayer() || TutorialManager.status)
      return;
    Timing.RunCoroutine(this._ReadCustomTranslations(), (Segment) 1);
    AlphaWarheadController.alarmSource = (AudioSource) GameObject.Find("GameManager").GetComponent<AudioSource>();
    this.blastDoors = (BlastDoor[]) Object.FindObjectsOfType<BlastDoor>();
    if (!this.get_isServer())
      return;
    int num = Mathf.RoundToInt((float) (Mathf.Clamp(ConfigFile.ServerConfig.GetInt("warhead_tminus_start_duration", 90), 80, 120) / 10)) * 10;
    this.Networksync_startScenario = 3;
    for (int index = 0; index < this.scenarios_start.Length; ++index)
    {
      if (this.scenarios_start[index].tMinusTime == num)
        this.Networksync_startScenario = index;
    }
  }

  private void SetTime(float f)
  {
    this.NetworktimeToDetonation = f;
  }

  private void SetStartScenario(int i)
  {
    this.Networksync_startScenario = i;
  }

  private void SetResumeScenario(int i)
  {
    this.Networksync_resumeScenario = i;
  }

  private void SetProgress(bool b)
  {
    this.NetworkinProgress = b;
  }

  public void StartDetonation()
  {
    this.doorsOpen = false;
    ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent);
    if ((AlphaWarheadController._resumeScenario != -1 || (double) this.scenarios_start[AlphaWarheadController._startScenario].SumTime() != (double) this.timeToDetonation) && (AlphaWarheadController._resumeScenario == -1 || (double) this.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime() != (double) this.timeToDetonation))
      return;
    this.SetProgress(true);
  }

  public void InstantPrepare()
  {
    this.NetworktimeToDetonation = AlphaWarheadController._resumeScenario != -1 ? this.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime() : this.scenarios_start[AlphaWarheadController._startScenario].SumTime();
  }

  [DebuggerHidden]
  private IEnumerator<float> _ReadCustomTranslations()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new AlphaWarheadController.\u003C_ReadCustomTranslations\u003Ec__Iterator0() { \u0024this = this };
  }

  public void CancelDetonation(GameObject disabler)
  {
    ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Detonation cancelled.", ServerLogs.ServerLogType.GameEvent);
    if (!this.inProgress || (double) this.timeToDetonation <= 10.0)
      return;
    if ((double) this.timeToDetonation <= 15.0 && Object.op_Inequality((Object) disabler, (Object) null))
      ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).CallTargetAchieve(((NetworkIdentity) disabler.GetComponent<NetworkIdentity>()).get_connectionToClient(), "thatwasclose");
    for (int index = 0; index < this.scenarios_resume.Length; ++index)
    {
      if ((double) this.scenarios_resume[index].SumTime() > (double) this.timeToDetonation && (double) this.scenarios_resume[index].SumTime() < (double) this.scenarios_start[AlphaWarheadController._startScenario].SumTime())
        this.Networksync_resumeScenario = index;
    }
    this.SetTime((AlphaWarheadController._resumeScenario >= 0 ? this.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime() : this.scenarios_start[AlphaWarheadController._startScenario].SumTime()) + (float) this.cooldown);
    this.SetProgress(false);
    foreach (Door door in (Door[]) Object.FindObjectsOfType<Door>())
    {
      door.warheadlock = false;
      door.UpdateLock();
    }
  }

  internal void Detonate()
  {
    ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Warhead detonated.", ServerLogs.ServerLogType.GameEvent);
    this.detonated = true;
    this.CallRpcShake();
    GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("LiftTarget");
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      foreach (GameObject gameObject in gameObjectsWithTag)
      {
        if (((PlayerStats) player.GetComponent<PlayerStats>()).Explode((double) Vector3.Distance(gameObject.get_transform().get_position(), player.get_transform().get_position()) < 3.5))
          ++this.warheadKills;
      }
    }
    foreach (Door door in (Door[]) Object.FindObjectsOfType<Door>())
    {
      if (door.blockAfterDetonation)
        door.OpenWarhead(true, true);
    }
  }

  [ClientRpc]
  private void RpcShake()
  {
    ExplosionCameraShake.singleton.Shake(1f);
    if (PlayerManager.localPlayer.get_transform().get_position().y <= 900.0)
      return;
    AchievementManager.Achieve("tminus");
  }

  private void FixedUpdate()
  {
    if (((Object) this).get_name() == "Host")
    {
      AlphaWarheadController.host = this;
      AlphaWarheadController._startScenario = this.sync_startScenario;
      AlphaWarheadController._resumeScenario = this.sync_resumeScenario;
    }
    if (Object.op_Equality((Object) AlphaWarheadController.host, (Object) null) || !this.get_isLocalPlayer())
      return;
    this.UpdateSourceState();
    if (!this.get_isServer())
      return;
    this.ServerCountdown();
  }

  private void UpdateSourceState()
  {
    if (TutorialManager.status)
      return;
    if (AlphaWarheadController.host.inProgress)
    {
      if ((double) AlphaWarheadController.host.timeToDetonation != 0.0)
      {
        if (!AlphaWarheadController.alarmSource.get_isPlaying())
        {
          AlphaWarheadController.alarmSource.set_volume(1f);
          AlphaWarheadController.alarmSource.set_clip(AlphaWarheadController._resumeScenario >= 0 ? this.scenarios_resume[AlphaWarheadController._resumeScenario].clip : this.scenarios_start[AlphaWarheadController._startScenario].clip);
          AlphaWarheadController.alarmSource.Play();
          return;
        }
        float num1 = this.RealDetonationTime();
        float num2 = num1 - AlphaWarheadController.host.timeToDetonation;
        if ((double) Mathf.Abs(AlphaWarheadController.alarmSource.get_time() - num2) > 0.5)
          AlphaWarheadController.alarmSource.set_time(Mathf.Clamp(num2, 0.0f, num1));
      }
      if ((double) AlphaWarheadController.host.timeToDetonation >= 5.0 || (double) AlphaWarheadController.host.timeToDetonation == 0.0)
        return;
      this._shake += Time.get_fixedDeltaTime() / 20f;
      this._shake = Mathf.Clamp(this._shake, 0.0f, 0.5f);
      if ((double) Vector3.Distance(((Component) this).get_transform().get_position(), ((Component) AlphaWarheadOutsitePanel.nukeside).get_transform().get_position()) >= 100.0)
        return;
      ExplosionCameraShake.singleton.Shake(this._shake);
    }
    else
    {
      if (!AlphaWarheadController.alarmSource.get_isPlaying() || !Object.op_Inequality((Object) AlphaWarheadController.alarmSource.get_clip(), (Object) null))
        return;
      AlphaWarheadController.alarmSource.Stop();
      AlphaWarheadController.alarmSource.set_clip((AudioClip) null);
      AlphaWarheadController.alarmSource.PlayOneShot(this.sound_canceled);
    }
  }

  public float RealDetonationTime()
  {
    if (AlphaWarheadController._resumeScenario >= 0)
      return this.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime();
    return this.scenarios_start[AlphaWarheadController._startScenario].SumTime();
  }

  [ServerCallback]
  private void ServerCountdown()
  {
    if (!NetworkServer.get_active())
      return;
    float num1 = this.RealDetonationTime();
    float f = this.timeToDetonation;
    if ((double) this.timeToDetonation != 0.0)
    {
      if (this.inProgress)
      {
        float num2 = f - Time.get_fixedDeltaTime();
        if ((double) num2 < 2.0 && !this.doorsClosed)
        {
          this.doorsClosed = true;
          foreach (BlastDoor blastDoor in this.blastDoors)
            blastDoor.SetClosed(true);
        }
        if (!this.doorsOpen && (double) num2 < (double) num1 - (AlphaWarheadController._resumeScenario < 0 ? (double) this.scenarios_start[AlphaWarheadController._startScenario].additionalTime : (double) this.scenarios_resume[AlphaWarheadController._resumeScenario].additionalTime))
        {
          this.doorsOpen = true;
          bool flag = ConfigFile.ServerConfig.GetBool("lock_gates_on_countdown", true);
          foreach (Door door in (Door[]) Object.FindObjectsOfType<Door>())
            door.OpenWarhead(false, flag || !door.DoorName.Contains("GATE"));
        }
        if ((double) num2 <= 0.0)
          this.Detonate();
        f = Mathf.Clamp(num2, 0.0f, num1);
      }
      else
      {
        if ((double) f > (double) num1)
          f -= Time.get_fixedDeltaTime();
        f = Mathf.Clamp(f, num1, (float) this.cooldown + num1);
      }
    }
    if ((double) f == (double) this.timeToDetonation)
      return;
    this.SetTime(f);
  }

  private void UNetVersion()
  {
  }

  public float NetworktimeToDetonation
  {
    get
    {
      return this.timeToDetonation;
    }
    [param: In] set
    {
      double num1 = (double) value;
      ref float local = ref this.timeToDetonation;
      int num2 = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetTime(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<float>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public int Networksync_startScenario
  {
    get
    {
      return this.sync_startScenario;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.sync_startScenario;
      int num2 = 2;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetStartScenario(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public int Networksync_resumeScenario
  {
    get
    {
      return this.sync_resumeScenario;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.sync_resumeScenario;
      int num2 = 4;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetResumeScenario(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public bool NetworkinProgress
  {
    get
    {
      return this.inProgress;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.inProgress;
      int num2 = 8;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetProgress(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  protected static void InvokeRpcRpcShake(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcShake called on server.");
    else
      ((AlphaWarheadController) obj).RpcShake();
  }

  public void CallRpcShake()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcShake called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) AlphaWarheadController.kRpcRpcShake);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 0, "RpcShake");
    }
  }

  static AlphaWarheadController()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (AlphaWarheadController), AlphaWarheadController.kRpcRpcShake, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcShake)));
    NetworkCRC.RegisterBehaviour(nameof (AlphaWarheadController), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.timeToDetonation);
      writer.WritePackedUInt32((uint) this.sync_startScenario);
      writer.WritePackedUInt32((uint) this.sync_resumeScenario);
      writer.Write(this.inProgress);
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
      writer.Write(this.timeToDetonation);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.sync_startScenario);
    }
    if (((int) this.get_syncVarDirtyBits() & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.sync_resumeScenario);
    }
    if (((int) this.get_syncVarDirtyBits() & 8) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.inProgress);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.timeToDetonation = reader.ReadSingle();
      this.sync_startScenario = (int) reader.ReadPackedUInt32();
      this.sync_resumeScenario = (int) reader.ReadPackedUInt32();
      this.inProgress = reader.ReadBoolean();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.SetTime(reader.ReadSingle());
      if ((num & 2) != 0)
        this.SetStartScenario((int) reader.ReadPackedUInt32());
      if ((num & 4) != 0)
        this.SetResumeScenario((int) reader.ReadPackedUInt32());
      if ((num & 8) == 0)
        return;
      this.SetProgress(reader.ReadBoolean());
    }
  }

  [Serializable]
  public class DetonationScenario
  {
    public AudioClip clip;
    public int tMinusTime;
    public float additionalTime;

    public float SumTime()
    {
      return (float) this.tMinusTime + this.additionalTime;
    }
  }
}
