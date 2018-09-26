// Decompiled with JetBrains decompiler
// Type: FlashEffect
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class FlashEffect : NetworkBehaviour
{
  private static int kCmdCmdBlind = -951436780;
  public CameraFilterPack_Colors_Brightness e1;
  public CameraFilterPack_TV_Vignetting e2;
  private float curP;
  [SyncVar]
  public bool sync_blind;
  public static bool isBlind;

  public FlashEffect()
  {
    base.\u002Ector();
  }

  [Command]
  private void CmdBlind(bool value)
  {
    this.Networksync_blind = value;
  }

  public void Play(float power)
  {
    if (!((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).IsHuman())
      return;
    this.curP = power;
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer())
      return;
    if ((double) this.curP > 0.0)
    {
      this.curP -= Time.get_deltaTime() / 0.0f;
      ((Behaviour) this.e1).set_enabled(false);
      ((Behaviour) this.e2).set_enabled(false);
      this.e1._Brightness = Mathf.Clamp((float) ((double) this.curP * 1.25 + 1.0), 1f, 2.5f);
      this.e2.Vignetting = Mathf.Clamp01(this.curP);
      this.e2.VignettingFull = Mathf.Clamp01(this.curP);
      this.e2.VignettingDirt = Mathf.Clamp01(this.curP);
    }
    else
    {
      this.curP = 0.0f;
      ((Behaviour) this.e1).set_enabled(false);
      ((Behaviour) this.e2).set_enabled(false);
    }
    FlashEffect.isBlind = (double) this.curP > 1.0;
    if (FlashEffect.isBlind == this.sync_blind)
      return;
    this.CallCmdBlind(FlashEffect.isBlind);
  }

  private void UNetVersion()
  {
  }

  public bool Networksync_blind
  {
    get
    {
      return this.sync_blind;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this.sync_blind, 1U);
    }
  }

  protected static void InvokeCmdCmdBlind(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdBlind called on client.");
    else
      ((FlashEffect) obj).CmdBlind(reader.ReadBoolean());
  }

  public void CallCmdBlind(bool value)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdBlind called on server.");
    else if (this.get_isServer())
    {
      this.CmdBlind(value);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) FlashEffect.kCmdCmdBlind);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(value);
      this.SendCommandInternal(networkWriter, 0, "CmdBlind");
    }
  }

  static FlashEffect()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (FlashEffect), FlashEffect.kCmdCmdBlind, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdBlind)));
    NetworkCRC.RegisterBehaviour(nameof (FlashEffect), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.sync_blind);
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
      writer.Write(this.sync_blind);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.sync_blind = reader.ReadBoolean();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.sync_blind = reader.ReadBoolean();
    }
  }
}
