// Decompiled with JetBrains decompiler
// Type: AlphaWarheadOutsitePanel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AlphaWarheadOutsitePanel : NetworkBehaviour
{
  public Animator panelButtonCoverAnim;
  public static AlphaWarheadNukesitePanel nukeside;
  private static AlphaWarheadController _host;
  public Text[] display;
  public GameObject[] inevitable;
  [SyncVar(hook = "SetKeycardState")]
  public bool keycardEntered;

  public AlphaWarheadOutsitePanel()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (Object.op_Equality((Object) AlphaWarheadOutsitePanel._host, (Object) null))
    {
      AlphaWarheadOutsitePanel._host = AlphaWarheadController.host;
    }
    else
    {
      ((Component) this).get_transform().set_localPosition(new Vector3(0.0f, 0.0f, 9f));
      foreach (Text text in this.display)
        text.set_text(AlphaWarheadOutsitePanel.GetTimeString());
      foreach (GameObject gameObject in this.inevitable)
        gameObject.SetActive((double) AlphaWarheadOutsitePanel._host.timeToDetonation <= 10.0 && (double) AlphaWarheadOutsitePanel._host.timeToDetonation > 0.0);
      this.panelButtonCoverAnim.SetBool("enabled", this.keycardEntered);
    }
  }

  public void SetKeycardState(bool b)
  {
    this.NetworkkeycardEntered = b;
  }

  public static string GetTimeString()
  {
    if (!AlphaWarheadOutsitePanel.nukeside.enabled && !AlphaWarheadOutsitePanel._host.inProgress)
      return "<size=180><color=red>DISABLED</color></size>";
    if (!AlphaWarheadOutsitePanel._host.inProgress)
      return (double) AlphaWarheadOutsitePanel._host.timeToDetonation > (double) AlphaWarheadController.host.RealDetonationTime() ? "<color=red><size=200>PLEASE WAIT</size></color>" : "<color=lime><size=180>READY</size></color>";
    if ((double) AlphaWarheadOutsitePanel._host.timeToDetonation == 0.0)
    {
      if ((int) ((double) Time.get_realtimeSinceStartup() * 4.0) % 2 == 0)
        return string.Empty;
      return "<color=orange><size=270>00:00:00</size></color>";
    }
    float num1 = (float) (((double) AlphaWarheadController.host.RealDetonationTime() - (double) AlphaWarheadController.alarmSource.get_time()) * 100.0) * (float) (1.0 + 2.5 / (double) AlphaWarheadController.host.RealDetonationTime());
    if ((double) num1 < 0.0)
      num1 = 0.0f;
    int num2 = 0;
    int num3 = 0;
    while ((double) num1 >= 100.0)
    {
      num1 -= 100f;
      ++num2;
    }
    while (num2 >= 60)
    {
      num2 -= 60;
      ++num3;
    }
    return "<color=orange><size=270>" + num3.ToString("00").Substring(0, 2) + ":" + num2.ToString("00").Substring(0, 2) + ":" + num1.ToString("00").Substring(0, 2) + "</size></color>";
  }

  private void UNetVersion()
  {
  }

  public bool NetworkkeycardEntered
  {
    get
    {
      return this.keycardEntered;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.keycardEntered;
      int num2 = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetKeycardState(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.keycardEntered);
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
      writer.Write(this.keycardEntered);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.keycardEntered = reader.ReadBoolean();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetKeycardState(reader.ReadBoolean());
    }
  }
}
