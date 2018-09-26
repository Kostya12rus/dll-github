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
  protected SECTR_DemoUI.WatermarkLocation watermarkLocation = SECTR_DemoUI.WatermarkLocation.UpperRight;
  private List<SECTR_DemoUI.DemoButton> demoButtons = new List<SECTR_DemoUI.DemoButton>(4);
  protected bool passedIntro;
  protected SECTR_FPSController cachedController;
  protected GUIStyle demoButtonStyle;
  [SECTR_ToolTip("Texture to display as a watermark.")]
  public Texture2D Watermark;
  [SECTR_ToolTip("Link to a controllable ghost/spectator camera.")]
  public SECTR_GhostController PipController;
  [SECTR_ToolTip("Message to display at start of demo.")]
  [Multiline]
  public string DemoMessage;
  [SECTR_ToolTip("Disables HUD for video captures.")]
  public bool CaptureMode;

  public bool PipActive
  {
    get
    {
      if ((bool) ((Object) this.PipController))
        return this.PipController.enabled;
      return false;
    }
  }

  protected virtual void OnEnable()
  {
    if ((bool) ((Object) this.PipController))
    {
      this.PipController.enabled = false;
      this.AddButton(KeyCode.P, "Control Player", "Control PiP", new SECTR_DemoUI.DemoButtonPressedDelegate(this.PressedPip));
    }
    this.cachedController = this.GetComponent<SECTR_FPSController>();
    this.passedIntro = string.IsNullOrEmpty(this.DemoMessage) || this.CaptureMode;
    if (this.passedIntro)
      return;
    this.cachedController.enabled = false;
    if (!(bool) ((Object) this.PipController))
      return;
    this.PipController.GetComponent<Camera>().enabled = false;
  }

  protected virtual void OnDisable()
  {
    if ((bool) ((Object) this.PipController))
      this.PipController.enabled = false;
    this.cachedController = (SECTR_FPSController) null;
    this.demoButtons.Clear();
  }

  protected virtual void OnGUI()
  {
    if (this.CaptureMode)
      return;
    float num1 = 25f;
    if ((bool) ((Object) this.Watermark))
    {
      float height = (float) Screen.height * 0.1f;
      float width = (float) this.Watermark.width / (float) this.Watermark.height * height;
      GUI.color = new Color(1f, 1f, 1f, 0.2f);
      switch (this.watermarkLocation)
      {
        case SECTR_DemoUI.WatermarkLocation.UpperLeft:
          GUI.DrawTexture(new Rect(num1, num1, width, height), (Texture) this.Watermark);
          break;
        case SECTR_DemoUI.WatermarkLocation.UpperCenter:
          GUI.DrawTexture(new Rect((float) ((double) Screen.width * 0.5 - (double) width * 0.5), num1, width, height), (Texture) this.Watermark);
          break;
        case SECTR_DemoUI.WatermarkLocation.UpperRight:
          GUI.DrawTexture(new Rect((float) Screen.width - num1 - width, num1, width, height), (Texture) this.Watermark);
          break;
      }
      GUI.color = Color.white;
    }
    if (this.demoButtonStyle == null)
    {
      this.demoButtonStyle = new GUIStyle(GUI.skin.box);
      this.demoButtonStyle.alignment = TextAnchor.UpperCenter;
      this.demoButtonStyle.wordWrap = true;
      this.demoButtonStyle.fontSize = 20;
      this.demoButtonStyle.normal.textColor = Color.white;
    }
    if (!this.passedIntro)
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      string demoMessage = this.DemoMessage;
      GUIContent content = new GUIContent(!Input.multiTouchEnabled || Application.isEditor ? demoMessage + "\n\nPress Any Key to Continue" : demoMessage + "\n\nPress to Continue");
      float width = (float) Screen.width * 0.75f;
      float height = this.demoButtonStyle.CalcHeight(content, width);
      GUI.Box(new Rect((float) ((double) Screen.width * 0.5 - (double) width * 0.5), (float) ((double) Screen.height * 0.5 - (double) height * 0.5), width, height), content, this.demoButtonStyle);
      if (Event.current.type != EventType.KeyDown)
        return;
      this.passedIntro = true;
      this.cachedController.enabled = true;
      if (!(bool) ((Object) this.PipController))
        return;
      this.PipController.GetComponent<Camera>().enabled = true;
    }
    else
    {
      if (this.demoButtons.Count <= 0)
        return;
      int count = this.demoButtons.Count;
      float width1 = Mathf.Min(150f, (float) (Screen.width / count) - num1 * 2f);
      float num2 = (float) ((double) Screen.width * 0.5 - ((double) count * (double) width1 + (double) (count - 1) * (double) num1) * 0.5);
      for (int index = 0; index < count; ++index)
      {
        SECTR_DemoUI.DemoButton demoButton = this.demoButtons[index];
        bool flag = Input.multiTouchEnabled && !Application.isEditor;
        string text = demoButton.key.ToString();
        switch (demoButton.key)
        {
          case KeyCode.Alpha0:
          case KeyCode.Alpha1:
          case KeyCode.Alpha2:
          case KeyCode.Alpha3:
          case KeyCode.Alpha4:
          case KeyCode.Alpha5:
          case KeyCode.Alpha6:
          case KeyCode.Alpha7:
          case KeyCode.Alpha8:
          case KeyCode.Alpha9:
            text = text.Replace("Alpha", string.Empty);
            break;
        }
        GUIContent content1 = new GUIContent(text);
        float num3 = !flag ? 5f : 0.0f;
        float width2 = 50f;
        float height1 = !flag ? this.demoButtonStyle.CalcHeight(content1, width2) : 0.0f;
        GUIContent content2 = new GUIContent(!demoButton.active ? demoButton.inactiveHint : demoButton.activeHint);
        float height2 = this.demoButtonStyle.CalcHeight(content2, width1);
        Rect position = new Rect(num2 + (width1 + num1) * (float) index, (float) Screen.height - height2 - num3 - height1 - num1, width1, height2);
        if (flag && !demoButton.pressed)
          demoButton.pressed = GUI.Button(position, content2, this.demoButtonStyle);
        else if (!flag)
        {
          GUI.Box(position, content2, this.demoButtonStyle);
          GUI.Box(new Rect((float) ((double) num2 + ((double) width1 + (double) num1) * (double) index + (double) width1 * 0.5 - (double) width2 * 0.5), (float) Screen.height - height1 - num1, width2, height1), content1, this.demoButtonStyle);
        }
        if (demoButton.pressed || Event.current.type == EventType.KeyUp && Event.current.keyCode == demoButton.key)
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
      this.PipController.enabled = false;
      this.cachedController.enabled = true;
    }
    else
    {
      this.PipController.enabled = true;
      this.cachedController.enabled = false;
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
