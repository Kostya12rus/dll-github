// Decompiled with JetBrains decompiler
// Type: SECTR_DemoUI
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SECTR/Demos/SECTR Demo UI")]
[RequireComponent(typeof (SECTR_FPSController))]
public class SECTR_DemoUI : MonoBehaviour
{
  protected bool passedIntro;
  protected SECTR_FPSController cachedController;
  protected GUIStyle demoButtonStyle;
  protected SECTR_DemoUI.WatermarkLocation watermarkLocation;
  private List<SECTR_DemoUI.DemoButton> demoButtons;
  [SECTR_ToolTip("Texture to display as a watermark.")]
  public Texture2D Watermark;
  [SECTR_ToolTip("Link to a controllable ghost/spectator camera.")]
  public SECTR_GhostController PipController;
  [SECTR_ToolTip("Message to display at start of demo.")]
  [Multiline]
  public string DemoMessage;
  [SECTR_ToolTip("Disables HUD for video captures.")]
  public bool CaptureMode;

  public SECTR_DemoUI()
  {
    base.\u002Ector();
  }

  public bool PipActive
  {
    get
    {
      if (Object.op_Implicit((Object) this.PipController))
        return ((Behaviour) this.PipController).get_enabled();
      return false;
    }
  }

  protected virtual void OnEnable()
  {
    if (Object.op_Implicit((Object) this.PipController))
    {
      ((Behaviour) this.PipController).set_enabled(false);
      this.AddButton((KeyCode) 112, "Control Player", "Control PiP", new SECTR_DemoUI.DemoButtonPressedDelegate(this.PressedPip));
    }
    this.cachedController = (SECTR_FPSController) ((Component) this).GetComponent<SECTR_FPSController>();
    this.passedIntro = string.IsNullOrEmpty(this.DemoMessage) || this.CaptureMode;
    if (this.passedIntro)
      return;
    ((Behaviour) this.cachedController).set_enabled(false);
    if (!Object.op_Implicit((Object) this.PipController))
      return;
    ((Behaviour) ((Component) this.PipController).GetComponent<Camera>()).set_enabled(false);
  }

  protected virtual void OnDisable()
  {
    if (Object.op_Implicit((Object) this.PipController))
      ((Behaviour) this.PipController).set_enabled(false);
    this.cachedController = (SECTR_FPSController) null;
    this.demoButtons.Clear();
  }

  protected virtual void OnGUI()
  {
    if (this.CaptureMode)
      return;
    float num1 = 25f;
    if (Object.op_Implicit((Object) this.Watermark))
    {
      float num2 = (float) Screen.get_height() * 0.1f;
      float num3 = (float) ((Texture) this.Watermark).get_width() / (float) ((Texture) this.Watermark).get_height() * num2;
      GUI.set_color(new Color(1f, 1f, 1f, 0.2f));
      switch (this.watermarkLocation)
      {
        case SECTR_DemoUI.WatermarkLocation.UpperLeft:
          GUI.DrawTexture(new Rect(num1, num1, num3, num2), (Texture) this.Watermark);
          break;
        case SECTR_DemoUI.WatermarkLocation.UpperCenter:
          GUI.DrawTexture(new Rect((float) ((double) Screen.get_width() * 0.5 - (double) num3 * 0.5), num1, num3, num2), (Texture) this.Watermark);
          break;
        case SECTR_DemoUI.WatermarkLocation.UpperRight:
          GUI.DrawTexture(new Rect((float) Screen.get_width() - num1 - num3, num1, num3, num2), (Texture) this.Watermark);
          break;
      }
      GUI.set_color(Color.get_white());
    }
    if (this.demoButtonStyle == null)
    {
      this.demoButtonStyle = new GUIStyle(GUI.get_skin().get_box());
      this.demoButtonStyle.set_alignment((TextAnchor) 1);
      this.demoButtonStyle.set_wordWrap(true);
      this.demoButtonStyle.set_fontSize(20);
      this.demoButtonStyle.get_normal().set_textColor(Color.get_white());
    }
    if (!this.passedIntro)
    {
      Cursor.set_lockState((CursorLockMode) 1);
      Cursor.set_visible(false);
      string demoMessage = this.DemoMessage;
      GUIContent guiContent = new GUIContent(!Input.get_multiTouchEnabled() || Application.get_isEditor() ? demoMessage + "\n\nPress Any Key to Continue" : demoMessage + "\n\nPress to Continue");
      float num2 = (float) Screen.get_width() * 0.75f;
      float num3 = this.demoButtonStyle.CalcHeight(guiContent, num2);
      Rect rect;
      ((Rect) ref rect).\u002Ector((float) ((double) Screen.get_width() * 0.5 - (double) num2 * 0.5), (float) ((double) Screen.get_height() * 0.5 - (double) num3 * 0.5), num2, num3);
      GUI.Box(rect, guiContent, this.demoButtonStyle);
      if (Event.get_current().get_type() != 4)
        return;
      this.passedIntro = true;
      ((Behaviour) this.cachedController).set_enabled(true);
      if (!Object.op_Implicit((Object) this.PipController))
        return;
      ((Behaviour) ((Component) this.PipController).GetComponent<Camera>()).set_enabled(true);
    }
    else
    {
      if (this.demoButtons.Count <= 0)
        return;
      int count = this.demoButtons.Count;
      float num2 = Mathf.Min(150f, (float) (Screen.get_width() / count) - num1 * 2f);
      float num3 = (float) ((double) Screen.get_width() * 0.5 - ((double) count * (double) num2 + (double) (count - 1) * (double) num1) * 0.5);
      for (int index = 0; index < count; ++index)
      {
        SECTR_DemoUI.DemoButton demoButton = this.demoButtons[index];
        bool flag = Input.get_multiTouchEnabled() && !Application.get_isEditor();
        string str = demoButton.key.ToString();
        switch (demoButton.key - 48)
        {
          case 0:
          case 1:
          case 2:
          case 3:
          case 4:
          case 5:
          case 6:
          case 7:
          case 8:
          case 9:
            str = str.Replace("Alpha", string.Empty);
            break;
        }
        GUIContent guiContent1 = new GUIContent(str);
        float num4 = !flag ? 5f : 0.0f;
        float num5 = 50f;
        float num6 = !flag ? this.demoButtonStyle.CalcHeight(guiContent1, num5) : 0.0f;
        GUIContent guiContent2 = new GUIContent(!demoButton.active ? demoButton.inactiveHint : demoButton.activeHint);
        float num7 = this.demoButtonStyle.CalcHeight(guiContent2, num2);
        Rect rect1;
        ((Rect) ref rect1).\u002Ector(num3 + (num2 + num1) * (float) index, (float) Screen.get_height() - num7 - num4 - num6 - num1, num2, num7);
        if (flag && !demoButton.pressed)
          demoButton.pressed = GUI.Button(rect1, guiContent2, this.demoButtonStyle);
        else if (!flag)
        {
          GUI.Box(rect1, guiContent2, this.demoButtonStyle);
          Rect rect2;
          ((Rect) ref rect2).\u002Ector((float) ((double) num3 + ((double) num2 + (double) num1) * (double) index + (double) num2 * 0.5 - (double) num5 * 0.5), (float) Screen.get_height() - num6 - num1, num5, num6);
          GUI.Box(rect2, guiContent1, this.demoButtonStyle);
        }
        if (demoButton.pressed || Event.get_current().get_type() == 5 && Event.get_current().get_keyCode() == demoButton.key)
        {
          demoButton.pressed = false;
          demoButton.active = !demoButton.active;
          if (demoButton.demoButtonPressed != null)
            demoButton.demoButtonPressed(demoButton.active);
        }
      }
    }
  }

  protected void AddButton(KeyCode key, string activeHint, string inactiveHint, SECTR_DemoUI.DemoButtonPressedDelegate buttonPressedDelegate)
  {
    this.demoButtons.Add(new SECTR_DemoUI.DemoButton(key, activeHint, inactiveHint, buttonPressedDelegate));
  }

  private void PressedPip(bool active)
  {
    if (this.PipActive)
    {
      ((Behaviour) this.PipController).set_enabled(false);
      ((Behaviour) this.cachedController).set_enabled(true);
    }
    else
    {
      ((Behaviour) this.PipController).set_enabled(true);
      ((Behaviour) this.cachedController).set_enabled(false);
    }
  }

  protected enum WatermarkLocation
  {
    UpperLeft,
    UpperCenter,
    UpperRight,
  }

  protected delegate void DemoButtonPressedDelegate(bool active);

  private class DemoButton
  {
    public KeyCode key;
    public string activeHint;
    public string inactiveHint;
    public bool active;
    public bool pressed;
    public SECTR_DemoUI.DemoButtonPressedDelegate demoButtonPressed;

    public DemoButton(KeyCode key, string activeHint, string inactiveHint, SECTR_DemoUI.DemoButtonPressedDelegate demoButtonPressed)
    {
      this.key = key;
      this.activeHint = activeHint;
      this.inactiveHint = !string.IsNullOrEmpty(inactiveHint) ? inactiveHint : activeHint;
      this.demoButtonPressed = demoButtonPressed;
    }
  }
}
