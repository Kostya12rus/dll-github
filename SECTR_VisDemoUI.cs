// Decompiled with JetBrains decompiler
// Type: SECTR_VisDemoUI
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("SECTR/Demos/SECTR Vis Demo UI")]
public class SECTR_VisDemoUI : SECTR_DemoUI
{
  private string originalDemoMessage;
  [Multiline]
  public string Unity4PerfMessage;

  private void Start()
  {
    if (!Object.op_Implicit((Object) this.PipController) || !Object.op_Equality((Object) ((Component) this.PipController).GetComponent<SECTR_CullingCamera>(), (Object) null) || (!Object.op_Implicit((Object) ((Component) this).GetComponent<SECTR_CullingCamera>()) || !Object.op_Implicit((Object) ((Component) this).GetComponent<Camera>())))
      return;
    ((SECTR_CullingCamera) ((Component) this.PipController).get_gameObject().AddComponent<SECTR_CullingCamera>()).cullingProxy = (Camera) ((Component) this).GetComponent<Camera>();
  }

  protected override void OnEnable()
  {
    this.originalDemoMessage = this.DemoMessage;
    this.watermarkLocation = SECTR_DemoUI.WatermarkLocation.UpperCenter;
    this.AddButton((KeyCode) 99, "Enable Culling", "Disable Culling", new SECTR_DemoUI.DemoButtonPressedDelegate(this.ToggleCulling));
    base.OnEnable();
  }

  protected override void OnGUI()
  {
    if (Application.get_isEditor() && Application.get_isPlaying() && !string.IsNullOrEmpty(this.Unity4PerfMessage))
    {
      this.DemoMessage = this.originalDemoMessage;
      this.DemoMessage += "\n\n";
      this.DemoMessage += this.Unity4PerfMessage;
    }
    base.OnGUI();
    if (!this.passedIntro || this.CaptureMode)
      return;
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    SECTR_CullingCamera component = (SECTR_CullingCamera) ((Component) this).GetComponent<SECTR_CullingCamera>();
    if (Object.op_Implicit((Object) component))
    {
      num1 += component.RenderersCulled;
      num2 += component.LightsCulled;
      num3 += component.TerrainsCulled;
    }
    GUIContent guiContent = new GUIContent("Culling Stats\n" + "Renderers: " + (object) num1 + "\n" + "Lights: " + (object) num2 + "\n" + "Terrains: " + (object) num3);
    float num4 = (float) Screen.get_width() * 0.33f;
    float num5 = this.demoButtonStyle.CalcHeight(guiContent, num4);
    Rect rect;
    ((Rect) ref rect).\u002Ector((float) Screen.get_width() - num4, 0.0f, num4, num5);
    GUI.Box(rect, guiContent, this.demoButtonStyle);
  }

  protected void ToggleCulling(bool active)
  {
    SECTR_CullingCamera component = (SECTR_CullingCamera) ((Component) this).GetComponent<SECTR_CullingCamera>();
    if (!Object.op_Implicit((Object) component))
      return;
    ((Behaviour) component).set_enabled(!active);
    component.ResetStats();
  }
}
