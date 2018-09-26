// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.UIController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
  public class UIController : MonoBehaviour
  {
    public GameObject root_login;
    public GameObject root_panel;
    public GameObject root_tbra;
    public GameObject root_root;
    public Texture wrongPasswordTexture;
    public Button confirmButton;
    public InputField passwordField;
    public bool loggedIn;
    public bool opened;
    public int awaitingLogin;
    public bool textBasedVersion;
    public static UIController singleton;

    private void Awake()
    {
      UIController.singleton = this;
    }

    private void Update()
    {
      if (!Input.GetKeyDown(NewInput.GetKey("Remote Admin")))
        return;
      this.ChangeConsoleStage();
    }

    public bool IsAnyInputFieldFocused()
    {
      foreach (InputField componentsInChild in this.GetComponentsInChildren<InputField>())
      {
        if (componentsInChild.isFocused)
          return true;
      }
      return false;
    }

    public void ChangeConsoleStage()
    {
      this.opened = !this.opened;
      this.RefreshStatus();
    }

    public void CallSendPassword()
    {
      Timing.RunCoroutine(this._SendPassword(), Segment.FixedUpdate);
    }

    public void ChangeTextMode(bool b)
    {
      this.textBasedVersion = b;
      this.RefreshStatus();
    }

    public void RefreshStatus()
    {
      if (this.IsAnyInputFieldFocused())
        this.opened = true;
      CursorManager.raOp = this.opened;
      this.root_panel.SetActive(this.opened && this.loggedIn && !this.textBasedVersion);
      this.root_tbra.SetActive(this.opened && this.loggedIn && this.textBasedVersion);
      this.root_login.SetActive(this.opened && !this.loggedIn);
      this.root_root.SetActive(this.opened);
      FirstPersonController.usingRemoteAdmin = this.opened;
    }

    public void ActivateRemoteAdmin()
    {
      this.loggedIn = true;
      this.RefreshStatus();
    }

    [DebuggerHidden]
    private IEnumerator<float> _SendPassword()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator<float>) new UIController.\u003C_SendPassword\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
