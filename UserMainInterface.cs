// Decompiled with JetBrains decompiler
// Type: UserMainInterface
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserMainInterface : MonoBehaviour
{
  public float lerpSpeed = 3f;
  public Slider sliderHP;
  public Slider searchProgress;
  public Text textHP;
  public Text specatorInfo;
  public Text playerlistText;
  public Text voiceInfo;
  public GameObject hpOBJ;
  public GameObject searchOBJ;
  public GameObject overloadMsg;
  public GameObject summary;
  [Space]
  public Text fps;
  public static UserMainInterface singleton;
  public float lerpedHP;

  private void Awake()
  {
    UserMainInterface.singleton = this;
  }

  private void Start()
  {
    this.playerlistText.text = "PRESS<b> " + NewInput.GetKey("Player List").ToString() + " </b>TO OPEN THE PLAYER LIST";
    this.voiceInfo.text = NewInput.GetKey("Voice Chat").ToString();
    ResolutionManager.RefreshScreen();
  }

  public void SearchProgress(float curProgress, float targetProgress)
  {
    this.searchProgress.maxValue = targetProgress;
    this.searchProgress.value = curProgress;
    this.searchOBJ.SetActive((double) curProgress != 0.0);
  }

  public void SetHP(int _hp, int _maxhp)
  {
    float num = (float) _maxhp;
    this.lerpedHP = Mathf.Lerp(this.lerpedHP, (float) _hp, Time.deltaTime * this.lerpSpeed);
    this.sliderHP.value = this.lerpedHP;
    this.textHP.text = ((double) Mathf.Clamp(Mathf.Round((float) ((double) this.sliderHP.value / (double) num * 100.0)), 1f, 100f)).ToString() + "%";
    this.sliderHP.maxValue = num;
  }

  private void Update()
  {
    try
    {
      this.fps.text = NetworkManager.singleton.client.GetRTT().ToString() + " ms";
    }
    catch
    {
    }
  }
}
