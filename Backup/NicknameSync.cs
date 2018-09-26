// Decompiled with JetBrains decompiler
// Type: NicknameSync
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using GameConsole;
using Steamworks;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NicknameSync : NetworkBehaviour
{
  private static int kCmdCmdSetNick = 55613980;
  public LayerMask raycastMask;
  private Transform spectCam;
  private ServerRoles _role;
  private UnityEngine.UI.Text n_text;
  private float transparency;
  private bool _nickSet;
  public float viewRange;
  [SyncVar(hook = "SetNick")]
  public string myNick;

  public NicknameSync()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this._role = (ServerRoles) ((Component) this).GetComponent<ServerRoles>();
    if (!this.get_isLocalPlayer())
      return;
    string n;
    if (!ServerStatic.IsDedicated && SteamManager.Initialized)
    {
      n = SteamFriends.GetPersonaName() != null ? SteamFriends.GetPersonaName() : "Player";
    }
    else
    {
      Console.singleton.AddLog("Steam has been not initialized!", new Color32(byte.MaxValue, (byte) 0, (byte) 0, byte.MaxValue), false);
      if (PlayerPrefs.HasKey("nickname"))
      {
        n = PlayerPrefs.GetString("nickname");
      }
      else
      {
        string str = "Player " + SystemInfo.get_deviceName();
        PlayerPrefs.SetString("nickname", str);
        n = str;
      }
    }
    this.CallCmdSetNick(n);
    this.spectCam = ((Scp049PlayerScript) ((Component) this).GetComponent<Scp049PlayerScript>()).plyCam.get_transform();
    this.n_text = (UnityEngine.UI.Text) GameObject.Find("Nickname Text").GetComponent<UnityEngine.UI.Text>();
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer())
      return;
    bool flag = false;
    RaycastHit raycastHit = (RaycastHit) null;
    CharacterClassManager component1 = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    if (component1.curClass != 2 && Physics.Raycast(new Ray(this.spectCam.get_position(), this.spectCam.get_forward()), ref raycastHit, this.viewRange, LayerMask.op_Implicit(this.raycastMask)))
    {
      NicknameSync component2 = (NicknameSync) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponent<NicknameSync>();
      if (Object.op_Inequality((Object) component2, (Object) null) && !component2.get_isLocalPlayer())
      {
        CharacterClassManager component3 = (CharacterClassManager) ((Component) component2).GetComponent<CharacterClassManager>();
        flag = true;
        if (component3.curClass != -1)
        {
          if (!TutorialManager.status)
          {
            ((Graphic) this.n_text).set_color(component3.klasy[component3.curClass].classColor);
            this.n_text.set_text(component2._role.GetColoredRoleString(false) + "\n");
            UnityEngine.UI.Text nText1 = this.n_text;
            nText1.set_text(nText1.get_text() + component2.myNick);
            UnityEngine.UI.Text nText2 = this.n_text;
            nText2.set_text(nText2.get_text() + "\n" + component3.klasy[component3.curClass].fullName);
          }
        }
        try
        {
          if (component3.curClass >= 0)
          {
            if (component3.klasy[component3.curClass].team == Team.MTF)
            {
              if (component1.klasy[component1.curClass].team == Team.MTF)
              {
                int num1 = 0;
                int num2 = 0;
                switch (component3.curClass)
                {
                  case 4:
                  case 11:
                    num2 = 200;
                    break;
                  case 12:
                    num2 = 300;
                    break;
                  case 13:
                    num2 = 100;
                    break;
                }
                switch (component1.curClass)
                {
                  case 4:
                  case 11:
                    num1 = 200;
                    break;
                  case 12:
                    num1 = 300;
                    break;
                  case 13:
                    num1 = 100;
                    break;
                }
                UnityEngine.UI.Text nText1 = this.n_text;
                nText1.set_text(nText1.get_text() + " (" + ((NineTailedFoxUnits) GameObject.Find("Host").GetComponent<NineTailedFoxUnits>()).GetNameById(component3.ntfUnit) + ")\n\n<b>");
                int num3 = num1 - component1.ntfUnit;
                int num4 = num2 - component3.ntfUnit;
                if (num3 > num4)
                {
                  UnityEngine.UI.Text nText2 = this.n_text;
                  nText2.set_text(nText2.get_text() + TranslationReader.Get("Legancy_Interfaces", 0));
                }
                else if (num4 > num3)
                {
                  UnityEngine.UI.Text nText2 = this.n_text;
                  nText2.set_text(nText2.get_text() + TranslationReader.Get("Legancy_Interfaces", 1));
                }
                else if (num4 == num3)
                {
                  UnityEngine.UI.Text nText2 = this.n_text;
                  nText2.set_text(nText2.get_text() + TranslationReader.Get("Legancy_Interfaces", 2));
                }
                UnityEngine.UI.Text nText3 = this.n_text;
                nText3.set_text(nText3.get_text() + "</b>");
              }
            }
          }
        }
        catch
        {
          MonoBehaviour.print((object) "Error");
        }
      }
    }
    this.transparency += Time.get_deltaTime() * (!flag ? -3f : 3f);
    if (flag)
      this.transparency = Mathf.Clamp(this.transparency, 0.0f, (this.viewRange - Vector3.Distance(((Component) this).get_transform().get_position(), ((RaycastHit) ref raycastHit).get_point())) / this.viewRange);
    this.transparency = Mathf.Clamp01(this.transparency);
    ((CanvasRenderer) ((Component) this.n_text).GetComponent<CanvasRenderer>()).SetAlpha(this.transparency);
  }

  [Command(channel = 2)]
  private void CmdSetNick(string n)
  {
    if (this.get_isLocalPlayer())
    {
      this.NetworkmyNick = n;
    }
    else
    {
      if (ConfigFile.ServerConfig.GetBool("online_mode", true) || this._nickSet)
        return;
      this._nickSet = true;
      if (n == null)
      {
        ServerConsole.AddLog("Banned " + (string) this.get_connectionToClient().address + " for passing null name.");
        ((BanPlayer) PlayerManager.localPlayer.GetComponent<BanPlayer>()).BanUser(((Component) this).get_gameObject(), 26297460, string.Empty, "Server");
        this.SetNick("Null Name");
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder();
        char highSurrogate = '0';
        bool flag = false;
        foreach (char ch in n)
        {
          if (char.IsLetterOrDigit(ch) || char.IsPunctuation(ch) || char.IsSymbol(ch))
          {
            flag = true;
            stringBuilder.Append(ch);
          }
          else if (char.IsWhiteSpace(ch) && ch != '\n' && (ch != '\r' && ch != '\t'))
            stringBuilder.Append(ch);
          else if (char.IsHighSurrogate(ch))
            highSurrogate = ch;
          else if (char.IsLowSurrogate(ch) && char.IsSurrogatePair(highSurrogate, ch))
          {
            stringBuilder.Append(highSurrogate);
            stringBuilder.Append(ch);
            flag = true;
          }
        }
        string str = stringBuilder.ToString();
        if (str.Length > 32)
          str = str.Substring(0, 32);
        if (!flag)
        {
          ServerConsole.AddLog("Kicked " + (string) this.get_connectionToClient().address + " for having an empty name.");
          ServerConsole.Disconnect(this.get_connectionToClient(), "You may not have an empty name.");
          this.SetNick("Empty Name");
        }
        else
          this.SetNick(str.Replace("<", "＜").Replace(">", "＞").Replace("[", "(").Replace("]", ")"));
      }
    }
  }

  [ServerCallback]
  public void UpdateNickname(string n)
  {
    if (!NetworkServer.get_active())
      return;
    this._nickSet = true;
    if (n == null)
    {
      ServerConsole.AddLog("Banned " + (string) this.get_connectionToClient().address + " for passing null name.");
      ((BanPlayer) PlayerManager.localPlayer.GetComponent<BanPlayer>()).BanUser(((Component) this).get_gameObject(), 26297460, string.Empty, "Server");
      this.SetNick("Null Name");
    }
    else
    {
      StringBuilder stringBuilder = new StringBuilder();
      char highSurrogate = '0';
      bool flag = false;
      foreach (char ch in n)
      {
        if (char.IsLetterOrDigit(ch) || char.IsPunctuation(ch) || char.IsSymbol(ch))
        {
          flag = true;
          stringBuilder.Append(ch);
        }
        else if (char.IsWhiteSpace(ch) && ch != '\n' && (ch != '\r' && ch != '\t'))
          stringBuilder.Append(ch);
        else if (char.IsHighSurrogate(ch))
          highSurrogate = ch;
        else if (char.IsLowSurrogate(ch) && char.IsSurrogatePair(highSurrogate, ch))
        {
          stringBuilder.Append(highSurrogate);
          stringBuilder.Append(ch);
          flag = true;
        }
      }
      string str = stringBuilder.ToString();
      if (str.Length > 32)
        str = str.Substring(0, 32);
      if (!flag)
      {
        ServerConsole.AddLog("Kicked " + (string) this.get_connectionToClient().address + " for having an empty name.");
        ServerConsole.Disconnect(this.get_connectionToClient(), "You may not have an empty name.");
        this.SetNick("Empty Name");
      }
      else
        this.SetNick(str.Replace("<", "＜").Replace(">", "＞"));
    }
  }

  private void SetNick(string nick)
  {
    nick = nick.Replace("<", "＜");
    nick = nick.Replace(">", "＞");
    this.NetworkmyNick = nick;
  }

  private void UNetVersion()
  {
  }

  public string NetworkmyNick
  {
    get
    {
      return this.myNick;
    }
    [param: In] set
    {
      string str = value;
      ref string local = ref this.myNick;
      int num = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetNick(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<string>((M0) str, (M0&) ref local, (uint) num);
    }
  }

  protected static void InvokeCmdCmdSetNick(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSetNick called on client.");
    else
      ((NicknameSync) obj).CmdSetNick(reader.ReadString());
  }

  public void CallCmdSetNick(string n)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSetNick called on server.");
    else if (this.get_isServer())
    {
      this.CmdSetNick(n);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) NicknameSync.kCmdCmdSetNick);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(n);
      this.SendCommandInternal(networkWriter, 2, "CmdSetNick");
    }
  }

  static NicknameSync()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (NicknameSync), NicknameSync.kCmdCmdSetNick, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSetNick)));
    NetworkCRC.RegisterBehaviour(nameof (NicknameSync), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.myNick);
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
      writer.Write(this.myNick);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.myNick = reader.ReadString();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetNick(reader.ReadString());
    }
  }
}
