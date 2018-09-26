// Decompiled with JetBrains decompiler
// Type: SpectatorManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SpectatorManager : NetworkBehaviour
{
  private SpectatorManager curPlayer;
  private PlayerManager pmng;
  private SpectatorInterface inf;
  private CharacterClassManager ccm;
  private SpectatorCamera cam;
  private Transform myCamera;
  private PlayerStats stats;
  private PlyMovementSync pms;
  private AnimationController actrl;
  private ServerRoles rls;
  private QueryProcessor qrpr;
  private static ServerRoles rlsMy;
  public Transform cameraPosition;
  public GameObject weaponCams;
  public Camera mainCam;
  private int prevClass;

  private void Start()
  {
    this.actrl = this.GetComponent<AnimationController>();
    this.cam = Object.FindObjectOfType<SpectatorCamera>();
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.inf = SpectatorInterface.singleton;
    this.pmng = PlayerManager.singleton;
    this.stats = this.GetComponent<PlayerStats>();
    this.pms = this.GetComponent<PlyMovementSync>();
    this.rls = this.GetComponent<ServerRoles>();
    this.qrpr = this.GetComponent<QueryProcessor>();
    if ((Object) SpectatorManager.rlsMy == (Object) null)
    {
      foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
      {
        if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          SpectatorManager.rlsMy = gameObject.GetComponent<ServerRoles>();
      }
    }
    this.myCamera = this.GetComponent<Scp049PlayerScript>().plyCam.transform;
    if (this.isLocalPlayer)
      Timing.RunCoroutine(this._PeriodicRefresher(), Segment.FixedUpdate);
    else
      this.enabled = false;
  }

  [DebuggerHidden]
  private IEnumerator<float> _PeriodicRefresher()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new SpectatorManager.\u003C_PeriodicRefresher\u003Ec__Iterator0() { \u0024this = this };
  }

  public void RefreshList()
  {
    if (!this.isLocalPlayer || (Object) this.inf == (Object) null || (Object) this.cam == (Object) null)
      return;
    TextMeshProUGUI playerList = this.inf.playerList;
    playerList.text = string.Empty;
    if (this.ccm.curClass == 2)
    {
      if (!this.cam.freeCam.enabled)
        this.cam.cam.enabled = true;
      this.mainCam.enabled = false;
      this.weaponCams.SetActive(false);
      this.inf.rootPanel.SetActive(true);
      if ((Object) this.curPlayer == (Object) null || (Object) this.curPlayer == (Object) this)
        this.inf.playerInfo.text = string.Empty;
      foreach (GameObject player in this.pmng.players)
      {
        CharacterClassManager component = player.GetComponent<CharacterClassManager>();
        string nick = player.GetComponent<NicknameSync>().myNick;
        if (component.curClass >= 0 && component.curClass != 2)
        {
          if ((Object) this.curPlayer != (Object) null && (Object) this.curPlayer.gameObject == (Object) player)
            playerList.text += string.Format("<u>{1}{0}</color></u>\n", (object) nick, (object) this.ColorToHex(component.klasy[component.curClass].classColor));
          else
            playerList.text += string.Format("{1}{0}</color>\n", (object) nick, (object) this.ColorToHex(component.klasy[component.curClass].classColor));
        }
      }
    }
    else
    {
      if ((Object) this.curPlayer != (Object) null && !this.curPlayer.isLocalPlayer && (Object) this.curPlayer.actrl.headAnimator != (Object) null)
        this.curPlayer.actrl.headAnimator.transform.localScale = Vector3.one;
      this.curPlayer = this;
      this.mainCam.enabled = true;
      this.cam.cam.enabled = false;
      this.cam.freeCam.enabled = false;
      this.weaponCams.SetActive(true);
      this.inf.rootPanel.SetActive(false);
    }
  }

  private void NextPlayer()
  {
    this.cam.cam.enabled = true;
    this.cam.freeCam.enabled = false;
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (GameObject player in this.pmng.players)
      gameObjectList.Add(player);
    if ((Object) this.curPlayer == (Object) null)
      this.curPlayer = gameObjectList[0].GetComponent<SpectatorManager>();
    if ((Object) this.curPlayer != (Object) null && !this.curPlayer.isLocalPlayer && (Object) this.curPlayer.actrl.headAnimator != (Object) null)
      this.curPlayer.actrl.headAnimator.transform.localScale = Vector3.one;
    int num = gameObjectList.IndexOf(this.curPlayer.gameObject);
    for (int index1 = 1; index1 <= gameObjectList.Count; ++index1)
    {
      int index2 = index1 + num;
      if (index2 >= gameObjectList.Count)
        index2 -= gameObjectList.Count;
      int curClass = gameObjectList[index2].GetComponent<CharacterClassManager>().curClass;
      if (curClass >= 0 && curClass != 2)
      {
        this.curPlayer = gameObjectList[index2].GetComponent<SpectatorManager>();
        this.RefreshList();
        return;
      }
    }
    if ((Object) this.curPlayer != (Object) null && !this.curPlayer.isLocalPlayer && (Object) this.curPlayer.actrl.headAnimator != (Object) null)
      this.curPlayer.actrl.headAnimator.transform.localScale = Vector3.zero;
    this.cam.cam.enabled = false;
    this.cam.freeCam.enabled = true;
    this.RefreshList();
  }

  private void PreviousPlayer()
  {
    this.cam.cam.enabled = true;
    this.cam.freeCam.enabled = false;
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (GameObject player in this.pmng.players)
      gameObjectList.Add(player);
    if ((Object) this.curPlayer == (Object) null)
      this.curPlayer = gameObjectList[0].GetComponent<SpectatorManager>();
    if ((Object) this.curPlayer != (Object) null && !this.curPlayer.isLocalPlayer && (Object) this.curPlayer.actrl.headAnimator != (Object) null)
      this.curPlayer.actrl.headAnimator.transform.localScale = Vector3.one;
    for (int index1 = gameObjectList.IndexOf(this.curPlayer.gameObject) - 1; index1 >= -gameObjectList.Count; --index1)
    {
      int index2 = index1;
      if (index2 < 0)
        index2 += gameObjectList.Count;
      int curClass = gameObjectList[index2].GetComponent<CharacterClassManager>().curClass;
      if (curClass >= 0 && curClass != 2)
      {
        this.curPlayer = gameObjectList[index2].GetComponent<SpectatorManager>();
        this.RefreshList();
        return;
      }
    }
    if ((Object) this.curPlayer != (Object) null && !this.curPlayer.isLocalPlayer && (Object) this.curPlayer.actrl.headAnimator != (Object) null)
      this.curPlayer.actrl.headAnimator.transform.localScale = Vector3.zero;
    this.cam.cam.enabled = false;
    this.cam.freeCam.enabled = true;
    this.RefreshList();
  }

  private void LateUpdate()
  {
    if (!this.isLocalPlayer)
      return;
    if ((Object) this.curPlayer != (Object) null)
      this.curPlayer.TrackPlayer();
    if (this.ccm.curClass == 2 && !Cursor.visible && Radio.roundStarted)
    {
      if (Input.GetKeyDown(KeyCode.Mouse0))
        this.NextPlayer();
      if (Input.GetKeyDown(KeyCode.Mouse1))
        this.PreviousPlayer();
    }
    if (this.ccm.curClass == this.prevClass)
      return;
    this.prevClass = this.ccm.curClass;
    this.RefreshList();
  }

  public void TrackPlayer()
  {
    if (this.ccm.curClass == 2)
      return;
    this.cam.cam.transform.position = this.cameraPosition.position;
    this.cam.cam.transform.rotation = Quaternion.Lerp(this.cam.cam.transform.rotation, Quaternion.Euler(this.pms.rotX, this.cameraPosition.eulerAngles.y, 0.0f), Time.deltaTime * 23f);
    this.inf.playerInfo.text = string.Format("{0}\n{1} HP{2}", (object) ((!SpectatorManager.rlsMy.AmIInOverwatch ? string.Empty : "<color=#008080>OVERWATCH MODE</color>\nPlayer ID: " + (object) this.qrpr.PlayerId + "\n") + (this.ccm.curClass >= 0 ? this.ccm.klasy[this.ccm.curClass].fullName : string.Empty)), (object) this.stats.health, (object) this.rls.GetColoredRoleString(true));
  }

  private string ColorToHex(Color c)
  {
    Color32 color32 = (Color32) c;
    return "<color=#" + (color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2")) + ">";
  }

  private void UNetVersion()
  {
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
