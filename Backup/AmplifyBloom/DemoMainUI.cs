// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoMainUI
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AmplifyBloom
{
  public class DemoMainUI : MonoBehaviour
  {
    private const string VERTICAL_GAMEPAD = "Vertical";
    private const string HORIZONTAL_GAMEPAD = "Horizontal";
    private const string SUBMIT_BUTTON = "Submit";
    public Toggle BloomToggle;
    public Toggle HighPrecision;
    public Toggle UpscaleType;
    public Toggle TemporalFilter;
    public Toggle BokehToggle;
    public Toggle LensFlareToggle;
    public Toggle LensGlareToggle;
    public Toggle LensDirtToggle;
    public Toggle LensStarburstToggle;
    public Slider ThresholdSlider;
    public Slider DownscaleAmountSlider;
    public Slider IntensitySlider;
    public Slider ThresholdSizeSlider;
    private AmplifyBloomEffect _amplifyBloomEffect;
    private Camera _camera;
    private DemoUIElement[] m_uiElements;
    private bool m_gamePadMode;
    private int m_currentOption;
    private int m_lastOption;
    private int m_lastAxisValue;

    public DemoMainUI()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this._camera = Camera.get_main();
      this._amplifyBloomEffect = (AmplifyBloomEffect) ((Component) this._camera).GetComponent<AmplifyBloomEffect>();
      this.BloomToggle.set_isOn(((Behaviour) this._amplifyBloomEffect).get_enabled());
      this.HighPrecision.set_isOn(this._amplifyBloomEffect.HighPrecision);
      this.UpscaleType.set_isOn(this._amplifyBloomEffect.UpscaleQuality == UpscaleQualityEnum.Realistic);
      this.TemporalFilter.set_isOn(this._amplifyBloomEffect.TemporalFilteringActive);
      this.BokehToggle.set_isOn(this._amplifyBloomEffect.BokehFilterInstance.ApplyBokeh);
      this.LensFlareToggle.set_isOn(this._amplifyBloomEffect.LensFlareInstance.ApplyLensFlare);
      this.LensGlareToggle.set_isOn(this._amplifyBloomEffect.LensGlareInstance.ApplyLensGlare);
      this.LensDirtToggle.set_isOn(this._amplifyBloomEffect.ApplyLensDirt);
      this.LensStarburstToggle.set_isOn(this._amplifyBloomEffect.ApplyLensStardurst);
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.BloomToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnBloomToggle)));
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.HighPrecision.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnHighPrecisionToggle)));
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.UpscaleType.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnUpscaleTypeToogle)));
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.TemporalFilter.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnTemporalFilterToggle)));
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.BokehToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnBokehFilterToggle)));
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.LensFlareToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnLensFlareToggle)));
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.LensGlareToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnLensGlareToggle)));
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.LensDirtToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnLensDirtToggle)));
      // ISSUE: method pointer
      ((UnityEvent<bool>) this.LensStarburstToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnLensStarburstToggle)));
      this.ThresholdSlider.set_value(this._amplifyBloomEffect.OverallThreshold);
      // ISSUE: method pointer
      ((UnityEvent<float>) this.ThresholdSlider.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(OnThresholdSlider)));
      this.DownscaleAmountSlider.set_value((float) this._amplifyBloomEffect.BloomDownsampleCount);
      // ISSUE: method pointer
      ((UnityEvent<float>) this.DownscaleAmountSlider.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(OnDownscaleAmount)));
      this.IntensitySlider.set_value(this._amplifyBloomEffect.OverallIntensity);
      // ISSUE: method pointer
      ((UnityEvent<float>) this.IntensitySlider.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(OnIntensitySlider)));
      this.ThresholdSizeSlider.set_value((float) this._amplifyBloomEffect.MainThresholdSize);
      // ISSUE: method pointer
      ((UnityEvent<float>) this.ThresholdSizeSlider.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(OnThresholdSize)));
      if (Input.GetJoystickNames().Length <= 0)
        return;
      this.m_gamePadMode = true;
      this.m_uiElements = new DemoUIElement[13];
      this.m_uiElements[0] = (DemoUIElement) ((Component) this.BloomToggle).GetComponent<DemoUIElement>();
      this.m_uiElements[1] = (DemoUIElement) ((Component) this.HighPrecision).GetComponent<DemoUIElement>();
      this.m_uiElements[2] = (DemoUIElement) ((Component) this.UpscaleType).GetComponent<DemoUIElement>();
      this.m_uiElements[3] = (DemoUIElement) ((Component) this.TemporalFilter).GetComponent<DemoUIElement>();
      this.m_uiElements[4] = (DemoUIElement) ((Component) this.BokehToggle).GetComponent<DemoUIElement>();
      this.m_uiElements[5] = (DemoUIElement) ((Component) this.LensFlareToggle).GetComponent<DemoUIElement>();
      this.m_uiElements[6] = (DemoUIElement) ((Component) this.LensGlareToggle).GetComponent<DemoUIElement>();
      this.m_uiElements[7] = (DemoUIElement) ((Component) this.LensDirtToggle).GetComponent<DemoUIElement>();
      this.m_uiElements[8] = (DemoUIElement) ((Component) this.LensStarburstToggle).GetComponent<DemoUIElement>();
      this.m_uiElements[9] = (DemoUIElement) ((Component) this.ThresholdSlider).GetComponent<DemoUIElement>();
      this.m_uiElements[10] = (DemoUIElement) ((Component) this.DownscaleAmountSlider).GetComponent<DemoUIElement>();
      this.m_uiElements[11] = (DemoUIElement) ((Component) this.IntensitySlider).GetComponent<DemoUIElement>();
      this.m_uiElements[12] = (DemoUIElement) ((Component) this.ThresholdSizeSlider).GetComponent<DemoUIElement>();
      for (int index = 0; index < this.m_uiElements.Length; ++index)
        this.m_uiElements[index].Init();
      this.m_uiElements[this.m_currentOption].Select = true;
    }

    public void OnThresholdSize(float selection)
    {
      this._amplifyBloomEffect.MainThresholdSize = (MainThresholdSizeEnum) selection;
    }

    public void OnThresholdSlider(float value)
    {
      this._amplifyBloomEffect.OverallThreshold = value;
    }

    public void OnDownscaleAmount(float value)
    {
      this._amplifyBloomEffect.BloomDownsampleCount = (int) value;
    }

    public void OnIntensitySlider(float value)
    {
      this._amplifyBloomEffect.OverallIntensity = value;
    }

    public void OnBloomToggle(bool value)
    {
      ((Behaviour) this._amplifyBloomEffect).set_enabled(this.BloomToggle.get_isOn());
    }

    public void OnHighPrecisionToggle(bool value)
    {
      this._amplifyBloomEffect.HighPrecision = value;
    }

    public void OnUpscaleTypeToogle(bool value)
    {
      this._amplifyBloomEffect.UpscaleQuality = !value ? UpscaleQualityEnum.Natural : UpscaleQualityEnum.Realistic;
    }

    public void OnTemporalFilterToggle(bool value)
    {
      this._amplifyBloomEffect.TemporalFilteringActive = value;
    }

    public void OnBokehFilterToggle(bool value)
    {
      this._amplifyBloomEffect.BokehFilterInstance.ApplyBokeh = this.BokehToggle.get_isOn();
    }

    public void OnLensFlareToggle(bool value)
    {
      this._amplifyBloomEffect.LensFlareInstance.ApplyLensFlare = this.LensFlareToggle.get_isOn();
    }

    public void OnLensGlareToggle(bool value)
    {
      this._amplifyBloomEffect.LensGlareInstance.ApplyLensGlare = this.LensGlareToggle.get_isOn();
    }

    public void OnLensDirtToggle(bool value)
    {
      this._amplifyBloomEffect.ApplyLensDirt = this.LensDirtToggle.get_isOn();
    }

    public void OnLensStarburstToggle(bool value)
    {
      this._amplifyBloomEffect.ApplyLensStardurst = this.LensStarburstToggle.get_isOn();
    }

    public void OnQuitButton()
    {
      Application.Quit();
    }

    private void Update()
    {
      if (this.m_gamePadMode)
      {
        int axis1 = (int) Input.GetAxis("Vertical");
        if (axis1 != this.m_lastAxisValue)
        {
          this.m_lastAxisValue = axis1;
          switch (axis1)
          {
            case -1:
              this.m_currentOption = this.m_currentOption != 0 ? this.m_currentOption - 1 : this.m_uiElements.Length - 1;
              break;
            case 1:
              this.m_currentOption = (this.m_currentOption + 1) % this.m_uiElements.Length;
              break;
          }
          this.m_uiElements[this.m_lastOption].Select = false;
          this.m_uiElements[this.m_currentOption].Select = true;
          this.m_lastOption = this.m_currentOption;
        }
        if (Input.GetButtonDown("Submit"))
          this.m_uiElements[this.m_currentOption].DoAction(DemoUIElementAction.Press);
        float axis2 = Input.GetAxis("Horizontal");
        if ((double) Mathf.Abs(axis2) > 0.0)
          this.m_uiElements[this.m_currentOption].DoAction(DemoUIElementAction.Slide, (object) (float) ((double) axis2 * (double) Time.get_deltaTime()));
        else
          this.m_uiElements[this.m_currentOption].Idle();
      }
      if (Input.GetKey((KeyCode) 308) && Input.GetKey((KeyCode) 113))
        this.OnQuitButton();
      if (Input.GetKeyDown((KeyCode) 48))
        this._camera.set_orthographic(!this._camera.get_orthographic());
      if (Input.GetKeyDown((KeyCode) 49))
      {
        AmplifyBloomEffect amplifyBloomEffect = this._amplifyBloomEffect;
        bool flag = !this.BloomToggle.get_isOn();
        this.BloomToggle.set_isOn(flag);
        int num = flag ? 1 : 0;
        ((Behaviour) amplifyBloomEffect).set_enabled(num != 0);
      }
      Toggle bokehToggle = this.BokehToggle;
      bool isOn = this.BloomToggle.get_isOn();
      ((Selectable) this.IntensitySlider).set_interactable(isOn);
      bool flag1 = isOn;
      ((Selectable) this.HighPrecision).set_interactable(flag1);
      bool flag2 = flag1;
      ((Selectable) this.DownscaleAmountSlider).set_interactable(flag2);
      bool flag3 = flag2;
      ((Selectable) this.ThresholdSlider).set_interactable(flag3);
      bool flag4 = flag3;
      ((Selectable) this.LensStarburstToggle).set_interactable(flag4);
      bool flag5 = flag4;
      ((Selectable) this.LensDirtToggle).set_interactable(flag5);
      bool flag6 = flag5;
      ((Selectable) this.LensGlareToggle).set_interactable(flag6);
      bool flag7 = flag6;
      ((Selectable) this.LensFlareToggle).set_interactable(flag7);
      int num1 = flag7 ? 1 : 0;
      ((Selectable) bokehToggle).set_interactable(num1 != 0);
      if (!this.BloomToggle.get_isOn())
        return;
      if (Input.GetKeyDown((KeyCode) 50))
      {
        AmplifyBokeh bokehFilterInstance = this._amplifyBloomEffect.BokehFilterInstance;
        bool flag8 = !this.BokehToggle.get_isOn();
        this.BokehToggle.set_isOn(flag8);
        int num2 = flag8 ? 1 : 0;
        bokehFilterInstance.ApplyBokeh = num2 != 0;
      }
      if (Input.GetKeyDown((KeyCode) 51))
      {
        AmplifyLensFlare lensFlareInstance = this._amplifyBloomEffect.LensFlareInstance;
        bool flag8 = !this.LensFlareToggle.get_isOn();
        this.LensFlareToggle.set_isOn(flag8);
        int num2 = flag8 ? 1 : 0;
        lensFlareInstance.ApplyLensFlare = num2 != 0;
      }
      if (Input.GetKeyDown((KeyCode) 52))
      {
        AmplifyGlare lensGlareInstance = this._amplifyBloomEffect.LensGlareInstance;
        bool flag8 = !this.LensGlareToggle.get_isOn();
        this.LensGlareToggle.set_isOn(flag8);
        int num2 = flag8 ? 1 : 0;
        lensGlareInstance.ApplyLensGlare = num2 != 0;
      }
      if (Input.GetKeyDown((KeyCode) 53))
      {
        AmplifyBloomEffect amplifyBloomEffect = this._amplifyBloomEffect;
        bool flag8 = !this.LensDirtToggle.get_isOn();
        this.LensDirtToggle.set_isOn(flag8);
        int num2 = flag8 ? 1 : 0;
        amplifyBloomEffect.ApplyLensDirt = num2 != 0;
      }
      if (!Input.GetKeyDown((KeyCode) 54))
        return;
      AmplifyBloomEffect amplifyBloomEffect1 = this._amplifyBloomEffect;
      bool flag9 = !this.LensStarburstToggle.get_isOn();
      this.LensStarburstToggle.set_isOn(flag9);
      int num3 = flag9 ? 1 : 0;
      amplifyBloomEffect1.ApplyLensStardurst = num3 != 0;
    }
  }
}
