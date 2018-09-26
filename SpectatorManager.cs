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

  public SpectatorManager()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.actrl = (AnimationController) ((Component) this).GetComponent<AnimationController>();
    this.cam = (SpectatorCamera) Object.FindObjectOfType<SpectatorCamera>();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.inf = SpectatorInterface.singleton;
    this.pmng = PlayerManager.singleton;
    this.stats = (PlayerStats) ((Component) this).GetComponent<PlayerStats>();
    this.pms = (PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>();
    this.rls = (ServerRoles) ((Component) this).GetComponent<ServerRoles>();
    this.qrpr = (QueryProcessor) ((Component) this).GetComponent<QueryProcessor>();
    if (Object.op_Equality((Object) SpectatorManager.rlsMy, (Object) null))
    {
      foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
      {
        if (((NetworkIdentity) gameObject.GetComponent<NetworkIdentity>()).get_isLocalPlayer())
          SpectatorManager.rlsMy = (ServerRoles) gameObject.GetComponent<ServerRoles>();
      }
    }
    this.myCamera = ((Scp049PlayerScript) ((Component) this).GetComponent<Scp049PlayerScript>()).plyCam.get_transform();
    if (this.get_isLocalPlayer())
      Timing.RunCoroutine(this._PeriodicRefresher(), (Segment) 1);
    else
      ((Behaviour) this).set_enabled(false);
  }

  [DebuggerHidden]
  private IEnumerator<float> _PeriodicRefresher()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new SpectatorManager.\u003C_PeriodicRefresher\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public void RefreshList()
  {
    if (!this.get_isLocalPlayer() || Object.op_Equality((Object) this.inf, (Object) null) || Object.op_Equality((Object) this.cam, (Object) null))
      return;
    TextMeshProUGUI playerList = this.inf.playerList;
    ((TMP_Text) playerList).set_text(string.Empty);
    if (this.ccm.curClass == 2)
    {
      if (!((Behaviour) this.cam.freeCam).get_enabled())
        ((Behaviour) this.cam.cam).set_enabled(true);
      ((Behaviour) this.mainCam).set_enabled(false);
      this.weaponCams.SetActive(false);
      this.inf.rootPanel.SetActive(true);
      if (Object.op_Equality((Object) this.curPlayer, (Object) null) || Object.op_Equality((Object) this.curPlayer, (Object) this))
        ((TMP_Text) this.inf.playerInfo).set_text(string.Empty);
      foreach (GameObject player in this.pmng.players)
      {
        CharacterClassManager component = (CharacterClassManager) player.GetComponent<CharacterClassManager>();
        string nick = ((NicknameSync) player.GetComponent<NicknameSync>()).myNick;
        if (component.curClass >= 0 && component.curClass != 2)
        {
          if (Object.op_Inequality((Object) this.curPlayer, (Object) null) && Object.op_Equality((Object) ((Component) this.curPlayer).get_gameObject(), (Object) player))
          {
            TextMeshProUGUI textMeshProUgui = playerList;
            ((TMP_Text) textMeshProUgui).set_text(((TMP_Text) textMeshProUgui).get_text() + string.Format("<u>{1}{0}</color></u>\n", (object) nick, (object) this.ColorToHex(component.klasy[component.curClass].classColor)));
          }
          else
          {
            TextMeshProUGUI textMeshProUgui = playerList;
            ((TMP_Text) textMeshProUgui).set_text(((TMP_Text) textMeshProUgui).get_text() + string.Format("{1}{0}</color>\n", (object) nick, (object) this.ColorToHex(component.klasy[component.curClass].classColor)));
          }
        }
      }
    }
    else
    {
      if (Object.op_Inequality((Object) this.curPlayer, (Object) null) && !this.curPlayer.get_isLocalPlayer() && Object.op_Inequality((Object) this.curPlayer.actrl.headAnimator, (Object) null))
        ((Component) this.curPlayer.actrl.headAnimator).get_transform().set_localScale(Vector3.get_one());
      this.curPlayer = this;
      ((Behaviour) this.mainCam).set_enabled(true);
      ((Behaviour) this.cam.cam).set_enabled(false);
      ((Behaviour) this.cam.freeCam).set_enabled(false);
      this.weaponCams.SetActive(true);
      this.inf.rootPanel.SetActive(false);
    }
  }

  private void NextPlayer()
  {
    ((Behaviour) this.cam.cam).set_enabled(true);
    ((Behaviour) this.cam.freeCam).set_enabled(false);
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (GameObject player in this.pmng.players)
      gameObjectList.Add(player);
    if (Object.op_Equality((Object) this.curPlayer, (Object) null))
      this.curPlayer = (SpectatorManager) gameObjectList[0].GetComponent<SpectatorManager>();
    if (Object.op_Inequality((Object) this.curPlayer, (Object) null) && !this.curPlayer.get_isLocalPlayer() && Object.op_Inequality((Object) this.curPlayer.actrl.headAnimator, (Object) null))
      ((Component) this.curPlayer.actrl.headAnimator).get_transform().set_localScale(Vector3.get_one());
    int num = gameObjectList.IndexOf(((Component) this.curPlayer).get_gameObject());
    for (int index1 = 1; index1 <= gameObjectList.Count; ++index1)
    {
      int index2 = index1 + num;
      if (index2 >= gameObjectList.Count)
        index2 -= gameObjectList.Count;
      int curClass = ((CharacterClassManager) gameObjectList[index2].GetComponent<CharacterClassManager>()).curClass;
      if (curClass >= 0 && curClass != 2)
      {
        this.curPlayer = (SpectatorManager) gameObjectList[index2].GetComponent<SpectatorManager>();
        this.RefreshList();
        return;
      }
    }
    if (Object.op_Inequality((Object) this.curPlayer, (Object) null) && !this.curPlayer.get_isLocalPlayer() && Object.op_Inequality((Object) this.curPlayer.actrl.headAnimator, (Object) null))
      ((Component) this.curPlayer.actrl.headAnimator).get_transform().set_localScale(Vector3.get_zero());
    ((Behaviour) this.cam.cam).set_enabled(false);
    ((Behaviour) this.cam.freeCam).set_enabled(true);
    this.RefreshList();
  }

  private void PreviousPlayer()
  {
    ((Behaviour) this.cam.cam).set_enabled(true);
    ((Behaviour) this.cam.freeCam).set_enabled(false);
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (GameObject player in this.pmng.players)
      gameObjectList.Add(player);
    if (Object.op_Equality((Object) this.curPlayer, (Object) null))
      this.curPlayer = (SpectatorManager) gameObjectList[0].GetComponent<SpectatorManager>();
    if (Object.op_Inequality((Object) this.curPlayer, (Object) null) && !this.curPlayer.get_isLocalPlayer() && Object.op_Inequality((Object) this.curPlayer.actrl.headAnimator, (Object) null))
      ((Component) this.curPlayer.actrl.headAnimator).get_transform().set_localScale(Vector3.get_one());
    for (int index1 = gameObjectList.IndexOf(((Component) this.curPlayer).get_gameObject()) - 1; index1 >= -gameObjectList.Count; --index1)
    {
      int index2 = index1;
      if (index2 < 0)
        index2 += gameObjectList.Count;
      int curClass = ((CharacterClassManager) gameObjectList[index2].GetComponent<CharacterClassManager>()).curClass;
      if (curClass >= 0 && curClass != 2)
      {
        this.curPlayer = (SpectatorManager) gameObjectList[index2].GetComponent<SpectatorManager>();
        this.RefreshList();
        return;
      }
    }
    if (Object.op_Inequality((Object) this.curPlayer, (Object) null) && !this.curPlayer.get_isLocalPlayer() && Object.op_Inequality((Object) this.curPlayer.actrl.headAnimator, (Object) null))
      ((Component) this.curPlayer.actrl.headAnimator).get_transform().set_localScale(Vector3.get_zero());
    ((Behaviour) this.cam.cam).set_enabled(false);
    ((Behaviour) this.cam.freeCam).set_enabled(true);
    this.RefreshList();
  }

  private void LateUpdate()
  {
    if (!this.get_isLocalPlayer())
      return;
    if (Object.op_Inequality((Object) this.curPlayer, (Object) null))
      this.curPlayer.TrackPlayer();
    if (this.ccm.curClass == 2 && !Cursor.get_visible() && Radio.roundStarted)
    {
      if (Input.GetKeyDown((KeyCode) 323))
        this.NextPlayer();
      if (Input.GetKeyDown((KeyCode) 324))
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
    ((Component) this.cam.cam).get_transform().set_position(this.cameraPosition.get_position());
    ((Component) this.cam.cam).get_transform().set_rotation(Quaternion.Lerp(((Component) this.cam.cam).get_transform().get_rotation(), Quaternion.Euler(this.pms.rotX, (float) this.cameraPosition.get_eulerAngles().y, 0.0f), Time.get_deltaTime() * 23f));
    ((TMP_Text) this.inf.playerInfo).set_text(string.Format("{0}\n{1} HP{2}", (object) ((!SpectatorManager.rlsMy.AmIInOverwatch ? string.Empty : "<color=#008080>OVERWATCH MODE</color>\nPlayer ID: " + (object) this.qrpr.PlayerId + "\n") + (this.ccm.curClass >= 0 ? this.ccm.klasy[this.ccm.curClass].fullName : string.Empty)), (object) this.stats.health, (object) this.rls.GetColoredRoleString(true)));
  }

  private string ColorToHex(Color c)
  {
    Color32 color32 = Color32.op_Implicit(c);
    return "<color=#" + (color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2")) + ">";
  }

  private void UNetVersion()
  {
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
