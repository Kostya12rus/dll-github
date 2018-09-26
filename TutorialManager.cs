// Decompiled with JetBrains decompiler
// Type: TutorialManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
  public static int curlog = -1;
  public static bool status;
  public static int levelID;
  private FirstPersonController fpc;
  private TextMeshProUGUI txt;
  public TutorialManager.TutorialScene[] tutorials;
  private List<TutorialManager.Log> logs;
  private AudioSource src;
  private float timeToNext;
  private int npcKills;
  private int reloads;
  private int burns;

  public TutorialManager()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    Scene activeScene = SceneManager.GetActiveScene();
    string name = ((Scene) ref activeScene).get_name();
    TutorialManager.status = name.Contains("Tutorial");
    if (!TutorialManager.status)
      return;
    TutorialManager.levelID = int.Parse(name.Remove(0, name.IndexOf("0")));
    this.logs = this.tutorials[TutorialManager.levelID - 1].logs;
  }

  private void Start()
  {
    if (!TutorialManager.status)
      return;
    TutorialManager.curlog = -1;
    this.fpc = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
    this.src = (AudioSource) GameObject.Find("Lector").GetComponent<AudioSource>();
    this.txt = (TextMeshProUGUI) GameObject.FindGameObjectWithTag("Respawn").GetComponent<TextMeshProUGUI>();
  }

  private void LateUpdate()
  {
    if (!TutorialManager.status)
      return;
    this.fpc.tutstop = (__Null) 0;
    if (TutorialManager.curlog >= 0 && this.logs[TutorialManager.curlog].stopPlayer)
      this.fpc.tutstop = (__Null) 1;
    if ((double) this.timeToNext > 0.0)
    {
      if (Input.GetKeyDown((KeyCode) 13))
      {
        this.timeToNext = 0.0f;
        this.src.Stop();
      }
      this.timeToNext -= Time.get_deltaTime();
      if ((double) this.timeToNext <= 0.0 && TutorialManager.curlog != -1)
      {
        ((TMP_Text) this.txt).set_text(string.Empty);
        if (this.logs[TutorialManager.curlog].jumpforward)
          this.Trigger(TutorialManager.curlog + 1);
        else
          TutorialManager.curlog = -1;
      }
    }
    if (TutorialManager.curlog == -1 || (double) this.timeToNext > 0.0)
      return;
    this.timeToNext = this.logs[TutorialManager.curlog].duration_en;
    if (Object.op_Inequality((Object) this.logs[TutorialManager.curlog].clip_en, (Object) null))
      this.src.PlayOneShot(this.logs[TutorialManager.curlog].clip_en);
    if ((double) this.logs[TutorialManager.curlog].duration_en <= 0.0)
      return;
    ((TMP_Text) this.txt).set_text(TranslationReader.Get("Tutorial_" + TutorialManager.levelID.ToString("00"), TutorialManager.curlog));
  }

  public void Trigger(int id)
  {
    TutorialManager.curlog = id;
    if ((double) this.logs[id].duration_en == -100.0)
    {
      PlayerPrefs.SetInt("TutorialProgress", Mathf.Max(TutorialManager.levelID + 1, PlayerPrefs.GetInt("TutorialProgress", 1)));
      ((NetworkManager) NetworkManager.singleton).StopHost();
    }
    if ((double) this.logs[id].duration_en == -200.0)
    {
      ((Component) this).SendMessage(this.logs[id].content_en);
      if (!this.logs[id].jumpforward)
        return;
      this.Trigger(id + 1);
    }
    else
    {
      this.src.Stop();
      ((TMP_Text) this.txt).set_text(string.Empty);
      this.timeToNext = 0.0f;
    }
  }

  public void Trigger(string alias)
  {
    for (int id = 0; id < this.logs.Count; ++id)
    {
      if (this.logs[id].alias == alias)
      {
        this.Trigger(id);
        break;
      }
    }
  }

  public void KillNPC()
  {
    ++this.npcKills;
    KillTrigger[] objectsOfType = (KillTrigger[]) Object.FindObjectsOfType<KillTrigger>();
    KillTrigger killTrigger1 = (KillTrigger) null;
    foreach (KillTrigger killTrigger2 in objectsOfType)
    {
      if (Object.op_Equality((Object) killTrigger1, (Object) null) || killTrigger2.prioirty < killTrigger1.prioirty)
        killTrigger1 = killTrigger2;
    }
    if (!Object.op_Inequality((Object) killTrigger1, (Object) null))
      return;
    killTrigger1.Trigger(this.npcKills);
  }

  public void Reload()
  {
    ++this.reloads;
  }

  private void Tutorial2_GiveNTFRifle()
  {
    Object.Destroy((Object) ((Component) Object.FindObjectOfType<NoammoTrigger>()).get_gameObject());
    ((Inventory) GameObject.Find("Host").GetComponent<Inventory>()).SetPickup(20, 0.0f, GameObject.Find("ItemPos").get_transform().get_position(), Quaternion.Euler(-90f, 0.0f, 0.0f));
    this.Invoke("Tutorial2_GiveSFA", 1f);
  }

  private void Tutorial2_GiveAmmo()
  {
    foreach (Pickup pickup in (Pickup[]) Object.FindObjectsOfType<Pickup>())
    {
      if (pickup.info.itemId == 29)
        return;
    }
    ((Inventory) GameObject.Find("Host").GetComponent<Inventory>()).SetPickup(29, 12f, GameObject.Find("ItemPos").get_transform().get_position(), (Quaternion) null);
  }

  private void Tutorial2_MoreAmmo()
  {
    foreach (Pickup pickup in (Pickup[]) Object.FindObjectsOfType<Pickup>())
    {
      if (pickup.info.itemId == 29)
        return;
    }
    ((Inventory) GameObject.Find("Host").GetComponent<Inventory>()).SetPickup(29, 12f, GameObject.Find("ItemPos").get_transform().get_position(), (Quaternion) null);
    this.Trigger(5);
  }

  private void Tutorial2_Jumpin()
  {
    this.Trigger("epsilon");
  }

  private void Tutorial2_Curtain()
  {
    ((AudioSource) GameObject.Find("Curtain").GetComponent<AudioSource>()).Play();
    ((Animator) GameObject.Find("Curtain").GetComponent<Animator>()).SetBool("Open", !((Animator) GameObject.Find("Curtain").GetComponent<Animator>()).GetBool("Open"));
  }

  private void Tutorial2_GiveSFA()
  {
    ((Inventory) GameObject.Find("Host").GetComponent<Inventory>()).SetPickup(22, 9999f, GameObject.Find("ItemPos").get_transform().get_position(), (Quaternion) null);
  }

  private void Tutorial2_ResultText()
  {
    ((Text) GameObject.Find("ResultText").GetComponent<Text>()).set_text((this.npcKills - 9).ToString("00"));
  }

  public void Tutorial2_Preset()
  {
    ((MainMenuScript) Object.FindObjectOfType<MainMenuScript>()).ChangeMenu(((MainMenuScript) Object.FindObjectOfType<MainMenuScript>()).CurMenu + 1);
  }

  public void Tutorial2_Result()
  {
    this.Tutorial2_Curtain();
    if (this.reloads == 1)
      this.Trigger("result_good");
    else if (this.reloads == 2)
      this.Trigger("result_ok");
    else
      this.Trigger("result_bad");
  }

  private void Tutorial3_GiveKeycard()
  {
    ((Inventory) GameObject.Find("Host").GetComponent<Inventory>()).SetPickup(0, 0.0f, GameObject.Find("ItemPos").get_transform().get_position(), (Quaternion) null);
  }

  public void Tutorial3_KeycardBurnt()
  {
    ++this.burns;
    if (this.burns == 1)
    {
      this.Trigger("bc1");
      this.Invoke("Tutorial3_GiveKeycard", 3f);
    }
    if (this.burns == 2)
    {
      this.Trigger("bc2");
      this.Invoke("Tutorial3_GiveKeycard", 5f);
    }
    if (this.burns == 3)
    {
      this.Trigger("bc3");
      for (int index = 1; index <= 5; ++index)
        this.Invoke("Tutorial3_GiveKeycard", (float) (1 + index / 5));
    }
    if (this.burns != 8)
      return;
    this.Trigger("bc4");
  }

  private void Tutorial3_Quit()
  {
    Application.Quit();
  }

  [Serializable]
  public class TutorialScene
  {
    public List<TutorialManager.Log> logs = new List<TutorialManager.Log>();
  }

  [Serializable]
  public class Log
  {
    [Multiline]
    public string content_en;
    public AudioClip clip_en;
    public float duration_en;
    public bool jumpforward;
    public bool stopPlayer;
    public string alias;
  }
}
