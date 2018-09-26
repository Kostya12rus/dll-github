// Decompiled with JetBrains decompiler
// Type: SteamManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Steamworks;
using System;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
  private static SteamManager s_instance;
  private static bool s_EverInialized;
  private bool m_bInitialized;
  private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

  public SteamManager()
  {
    base.\u002Ector();
  }

  private static SteamManager Instance
  {
    get
    {
      if (Object.op_Equality((Object) SteamManager.s_instance, (Object) null))
        return (SteamManager) new GameObject(nameof (SteamManager)).AddComponent<SteamManager>();
      return SteamManager.s_instance;
    }
  }

  public static bool Initialized
  {
    get
    {
      return SteamManager.Instance.m_bInitialized;
    }
  }

  private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
  {
    Debug.LogWarning((object) pchDebugText);
  }

  private void Awake()
  {
    if (Object.op_Inequality((Object) SteamManager.s_instance, (Object) null))
    {
      Object.Destroy((Object) ((Component) this).get_gameObject());
    }
    else
    {
      SteamManager.s_instance = this;
      if (SteamManager.s_EverInialized)
        throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
      Object.DontDestroyOnLoad((Object) ((Component) this).get_gameObject());
      if (!Packsize.Test())
        Debug.LogError((object) "[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", (Object) this);
      if (!DllCheck.Test())
        Debug.LogError((object) "[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", (Object) this);
      try
      {
        if (SteamAPI.RestartAppIfNecessary((AppId_t) AppId_t.Invalid))
        {
          Application.Quit();
          return;
        }
      }
      catch (DllNotFoundException ex)
      {
        Debug.LogError((object) ("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + (object) ex), (Object) this);
        Application.Quit();
        return;
      }
      this.m_bInitialized = SteamAPI.Init();
      if (!this.m_bInitialized)
        Debug.LogError((object) "[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", (Object) this);
      else
        SteamManager.s_EverInialized = true;
    }
  }

  private void OnEnable()
  {
    if (Object.op_Equality((Object) SteamManager.s_instance, (Object) null))
      SteamManager.s_instance = this;
    if (!this.m_bInitialized || this.m_SteamAPIWarningMessageHook != null)
      return;
    // ISSUE: method pointer
    this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t((object) null, __methodptr(SteamAPIDebugTextHook));
    SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
  }

  private void OnDestroy()
  {
    if (Object.op_Inequality((Object) SteamManager.s_instance, (Object) this))
      return;
    SteamManager.s_instance = (SteamManager) null;
    if (!this.m_bInitialized)
      return;
    SteamAPI.Shutdown();
  }

  private void Update()
  {
    if (!this.m_bInitialized)
      return;
    SteamAPI.RunCallbacks();
  }
}
