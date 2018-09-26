// Decompiled with JetBrains decompiler
// Type: RoundSummary
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoundSummary : NetworkBehaviour
{
  private static int kRpcRpcShowRoundSummary = -1028913821;
  private bool roundEnded;
  private RoundSummary.SumInfo_ClassList classlistStart;
  public Image fadeOutImage;
  public GameObject ui_root;
  public TextMeshProUGUI ui_text_who_won;
  public TextMeshProUGUI ui_text_info;
  public static RoundSummary singleton;
  public static int roundTime;
  public static int escaped_ds;
  public static int escaped_scientists;
  public static int kills_by_scp;
  public static int changed_into_zombies;
  private static int kRpcRpcDimScreen;

  private void Start()
  {
    if (!NetworkServer.active)
      return;
    RoundSummary.roundTime = 0;
    RoundSummary.singleton = this;
    Timing.RunCoroutine(this._ProcessServerSideCode(), Segment.Update);
    RoundSummary.kills_by_scp = 0;
    RoundSummary.escaped_ds = 0;
    RoundSummary.escaped_scientists = 0;
    RoundSummary.changed_into_zombies = 0;
  }

  public void SetStartClassList(RoundSummary.SumInfo_ClassList info)
  {
    this.classlistStart = info;
  }

  [DebuggerHidden]
  private IEnumerator<float> _ProcessServerSideCode()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new RoundSummary.\u003C_ProcessServerSideCode\u003Ec__Iterator0() { \u0024this = this };
  }

  [ClientRpc]
  private void RpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
  {
    Timing.RunCoroutine(this._ShowRoundSummary(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd, RoundSummary.changed_into_zombies), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _ShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd, int changedIntoZombies)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new RoundSummary.\u003C_ShowRoundSummary\u003Ec__Iterator1() { leadingTeam = leadingTeam, scp_kills = scp_kills, list_finish = list_finish, list_start = list_start, changedIntoZombies = changedIntoZombies, e_ds = e_ds, e_sc = e_sc, round_cd = round_cd, \u0024this = this };
  }

  [ClientRpc]
  private void RpcDimScreen()
  {
    Timing.RunCoroutine(this._FadeScreenOut(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _FadeScreenOut()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new RoundSummary.\u003C_FadeScreenOut\u003Ec__Iterator2() { \u0024this = this };
  }

  public static bool RoundInProgress()
  {
    if ((Object) PlayerManager.localPlayer != (Object) null && PlayerManager.localPlayer.GetComponent<CharacterClassManager>().roundStarted && !RoundSummary.singleton.roundEnded)
      return (Object) AlphaWarheadController.host != (Object) null;
    return false;
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeRpcRpcShowRoundSummary(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcShowRoundSummary called on server.");
    else
      ((RoundSummary) obj).RpcShowRoundSummary(GeneratedNetworkCode._ReadSumInfo_ClassList_RoundSummary(reader), GeneratedNetworkCode._ReadSumInfo_ClassList_RoundSummary(reader), (RoundSummary.LeadingTeam) reader.ReadInt32(), (int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcRpcDimScreen(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcDimScreen called on server.");
    else
      ((RoundSummary) obj).RpcDimScreen();
  }

  public void CallRpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcShowRoundSummary called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) RoundSummary.kRpcRpcShowRoundSummary);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      GeneratedNetworkCode._WriteSumInfo_ClassList_RoundSummary(writer, list_start);
      GeneratedNetworkCode._WriteSumInfo_ClassList_RoundSummary(writer, list_finish);
      writer.Write((int) leadingTeam);
      writer.WritePackedUInt32((uint) e_ds);
      writer.WritePackedUInt32((uint) e_sc);
      writer.WritePackedUInt32((uint) scp_kills);
      writer.WritePackedUInt32((uint) round_cd);
      this.SendRPCInternal(writer, 0, "RpcShowRoundSummary");
    }
  }

  public void CallRpcDimScreen()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcDimScreen called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) RoundSummary.kRpcRpcDimScreen);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 0, "RpcDimScreen");
    }
  }

  static RoundSummary()
  {
    NetworkBehaviour.RegisterRpcDelegate(typeof (RoundSummary), RoundSummary.kRpcRpcShowRoundSummary, new NetworkBehaviour.CmdDelegate(RoundSummary.InvokeRpcRpcShowRoundSummary));
    RoundSummary.kRpcRpcDimScreen = 784848710;
    NetworkBehaviour.RegisterRpcDelegate(typeof (RoundSummary), RoundSummary.kRpcRpcDimScreen, new NetworkBehaviour.CmdDelegate(RoundSummary.InvokeRpcRpcDimScreen));
    NetworkCRC.RegisterBehaviour(nameof (RoundSummary), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }

  public enum LeadingTeam
  {
    FacilityForces,
    ChaosInsurgency,
    Anomalies,
    Draw,
  }

  [Serializable]
  public struct SumInfo_ClassList
  {
    public int class_ds;
    public int scientists;
    public int chaos_insurgents;
    public int mtf_and_guards;
    public int scps_except_zombies;
    public int zombies;
    public int warhead_kills;
    public int time;
  }
}
