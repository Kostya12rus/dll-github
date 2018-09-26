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
    if (!(bool) ((Object) this.PipController) || !((Object) this.PipController.GetComponent<SECTR_CullingCamera>() == (Object) null) || (!(bool) ((Object) this.GetComponent<SECTR_CullingCamera>()) || !(bool) ((Object) this.GetComponent<Camera>())))
      return;
    this.PipController.gameObject.AddComponent<SECTR_CullingCamera>().cullingProxy = this.GetComponent<Camera>();
  }

  protected override void OnEnable()
  {
    this.originalDemoMessage = this.DemoMessage;
    this.watermarkLocation = SECTR_DemoUI.WatermarkLocation.UpperCenter;
    this.AddButton(KeyCode.C, "Enable Culling", "Disable Culling", new SECTR_DemoUI.DemoButtonPressedDelegate(this.ToggleCulling));
    base.OnEnable();
  }

  protected override void OnGUI()
  {
    if (Application.isEditor && Application.isPlaying && !string.IsNullOrEmpty(this.Unity4PerfMessage))
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
    SECTR_CullingCamera component = this.GetComponent<SECTR_CullingCamera>();
    if ((bool) ((Object) component))
    {
      num1 += component.RenderersCulled;
      num2 += component.LightsCulled;
      num3 += component.TerrainsCulled;
    }
    GUIContent content = new GUIContent("Culling Stats\n" + "Renderers: " + (object) num1 + "\n" + "Lights: " + (object) num2 + "\n" + "Terrains: " + (object) num3);
    float width = (float) Screen.width * 0.33f;
    float height = this.demoButtonStyle.CalcHeight(content, width);
    GUI.Box(new Rect((float) Screen.width - width, 0.0f, width, height), content, this.demoButtonStyle);
  }

  protected void ToggleCulling(bool active)
  {
    SECTR_CullingCamera component = this.GetComponent<SECTR_CullingCamera>();
    if (!(bool) ((Object) component))
      return;
    component.enabled = !active;
    component.ResetStats();
  }
}
