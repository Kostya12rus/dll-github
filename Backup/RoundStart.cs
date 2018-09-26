// Decompiled with JetBrains decompiler
// Type: RoundStart
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoundStart : NetworkBehaviour
{
  public GameObject window;
  public GameObject forceButton;
  public TextMeshProUGUI playersNumber;
  public Image loadingbar;
  public static RoundStart singleton;
  [SyncVar(hook = "SetInfo")]
  public string info;

  public RoundStart()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    ((Transform) ((Component) this).GetComponent<RectTransform>()).set_localPosition(Vector3.get_zero());
    if (!NetworkServer.get_active())
      return;
    Timing.RunCoroutine(this._AntiNonclass(), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _AntiNonclass()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new RoundStart.\u003C_AntiNonclass\u003Ec__Iterator0() { \u0024this = this };
  }

  private void Awake()
  {
    RoundStart.singleton = this;
  }

  private void Update()
  {
    this.window.SetActive(this.info != string.Empty && this.info != "started");
    float result = 0.0f;
    float.TryParse(this.info, out result);
    this.loadingbar.set_fillAmount(Mathf.Lerp(this.loadingbar.get_fillAmount(), (result - 1f) / 19f, Time.get_deltaTime()));
    ((TMP_Text) this.playersNumber).set_text(PlayerManager.singleton.players.Length.ToString());
  }

  public void SetInfo(string i)
  {
    this.Networkinfo = i;
  }

  public void ShowButton()
  {
    this.forceButton.SetActive(true);
  }

  public void UseButton()
  {
    this.forceButton.SetActive(false);
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      CharacterClassManager component = (CharacterClassManager) player.GetComponent<CharacterClassManager>();
      if (component.get_isLocalPlayer() && ((Object) player).get_name() == "Host")
        component.ForceRoundStart();
    }
  }

  private void UNetVersion()
  {
  }

  public string Networkinfo
  {
    get
    {
      return this.info;
    }
    [param: In] set
    {
      string str = value;
      ref string local = ref this.info;
      int num = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetInfo(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<string>((M0) str, (M0&) ref local, (uint) num);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.info);
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
      writer.Write(this.info);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.info = reader.ReadString();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetInfo(reader.ReadString());
    }
  }
}
