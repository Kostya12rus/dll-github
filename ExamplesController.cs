// Decompiled with JetBrains decompiler
// Type: ExamplesController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CinematicEffects;

public class ExamplesController : MonoBehaviour
{
  public UBER_ExampleObjectParams[] objectsParams;
  public Camera mainCamera;
  public UBER_MouseOrbit_DynamicDistance mouseOrbitController;
  public GameObject InteractiveUI;
  [Space(10f)]
  public GameObject autorotateButtonOn;
  public GameObject autorotateButtonOff;
  public GameObject togglepostFXButtonOn;
  public GameObject togglepostFXButtonOff;
  public float autoRotationSpeed;
  public bool autoRotation;
  [Space(10f)]
  public GameObject skyboxSphere1;
  public Cubemap reflectionCubemap1;
  [Range(0.0f, 1f)]
  public float exposure1;
  public GameObject realTimeLight1;
  public Material skyboxMaterial1;
  public GameObject skyboxSphere2;
  public Cubemap reflectionCubemap2;
  [Range(0.0f, 1f)]
  public float exposure2;
  public GameObject realTimeLight2;
  public Material skyboxMaterial2;
  public GameObject skyboxSphere3;
  public Cubemap reflectionCubemap3;
  [Range(0.0f, 1f)]
  public float exposure3;
  public GameObject realTimeLight3;
  public Material skyboxMaterial3;
  public Material skyboxSphereMaterialActive;
  public Material skyboxSphereMaterialInactive;
  [Space(10f)]
  public Slider materialSlider;
  public Slider exposureSlider;
  public Text titleTextArea;
  public Text descriptionTextArea;
  public Text matParamTextArea;
  [Space(10f)]
  public Button buttonSun;
  public Button buttonFrost;
  [Space(10f)]
  public float hideTimeDelay;
  private MeshRenderer currentRenderer;
  private Material currentMaterial;
  private Material originalMaterial;
  private float hideTime;
  private int currentTargetIndex;
  private GameObject skyboxSphereActive;

  public ExamplesController()
  {
    base.\u002Ector();
  }

  public void Start()
  {
    RenderSettings.set_skybox(this.skyboxMaterial1);
    this.realTimeLight1.SetActive(true);
    this.realTimeLight2.SetActive(false);
    this.realTimeLight3.SetActive(false);
    RenderSettings.set_customReflection(this.reflectionCubemap1);
    RenderSettings.set_reflectionIntensity(this.exposure1);
    DynamicGI.UpdateEnvironment();
    this.skyboxSphereActive = this.skyboxSphere1;
    this.currentTargetIndex = 0;
    this.PrepareCurrentObject();
    for (int index = 1; index < this.objectsParams.Length; ++index)
      this.objectsParams[index].target.SetActive(false);
    this.hideTime = Time.get_time() + this.hideTimeDelay;
  }

  public void ClickedAutoRotation()
  {
    this.autoRotation = !this.autoRotation;
    this.autorotateButtonOn.SetActive(this.autoRotation);
    this.autorotateButtonOff.SetActive(!this.autoRotation);
  }

  public void ClickedArrow(bool rightFlag)
  {
    this.objectsParams[this.currentTargetIndex].target.get_transform().set_rotation(Quaternion.get_identity());
    this.objectsParams[this.currentTargetIndex].target.SetActive(false);
    if (Object.op_Inequality((Object) this.currentRenderer, (Object) null) && Object.op_Inequality((Object) this.originalMaterial, (Object) null))
    {
      Material[] sharedMaterials = ((Renderer) this.currentRenderer).get_sharedMaterials();
      sharedMaterials[this.objectsParams[this.currentTargetIndex].submeshIndex] = this.originalMaterial;
      ((Renderer) this.currentRenderer).set_sharedMaterials(sharedMaterials);
      Object.Destroy((Object) this.currentMaterial);
    }
    this.currentTargetIndex = !rightFlag ? (this.currentTargetIndex + this.objectsParams.Length - 1) % this.objectsParams.Length : (this.currentTargetIndex + 1) % this.objectsParams.Length;
    this.PrepareCurrentObject();
    this.objectsParams[this.currentTargetIndex].target.SetActive(true);
    this.mouseOrbitController.target = this.objectsParams[this.currentTargetIndex].target;
    this.mouseOrbitController.targetFocus = this.objectsParams[this.currentTargetIndex].target.get_transform().Find("Focus");
    this.mouseOrbitController.Reset();
  }

  public void Update()
  {
    this.skyboxSphereActive.get_transform().Rotate(Vector3.get_up(), Time.get_deltaTime() * 200f, (Space) 0);
    if (this.objectsParams[this.currentTargetIndex].Title == "Ice block" && Input.GetKeyDown((KeyCode) 108))
    {
      GameObject gameObject = ((Component) this.objectsParams[this.currentTargetIndex].target.get_transform().Find("Amber")).get_gameObject();
      gameObject.SetActive(!gameObject.get_activeSelf());
    }
    if (Input.GetKeyDown((KeyCode) 275))
      this.ClickedArrow(true);
    else if (Input.GetKeyDown((KeyCode) 276))
      this.ClickedArrow(false);
    if (this.autoRotation)
      this.objectsParams[this.currentTargetIndex].target.get_transform().Rotate(Vector3.get_up(), Time.get_deltaTime() * this.autoRotationSpeed, (Space) 0);
    if ((double) Input.GetAxis("Mouse X") != 0.0 || (double) Input.GetAxis("Mouse Y") != 0.0)
    {
      this.hideTime = Time.get_time() + this.hideTimeDelay;
      this.InteractiveUI.SetActive(true);
    }
    if ((double) Time.get_time() <= (double) this.hideTime)
      return;
    this.InteractiveUI.SetActive(false);
  }

  public void ButtonPressed(Button button)
  {
    RectTransform component = (RectTransform) ((Component) button).GetComponent<RectTransform>();
    Vector3 vector3 = Vector2.op_Implicit(component.get_anchoredPosition());
    ref Vector3 local1 = ref vector3;
    local1.x = (__Null) (local1.x + 2.0);
    ref Vector3 local2 = ref vector3;
    local2.y = (__Null) (local2.y - 2.0);
    component.set_anchoredPosition(Vector2.op_Implicit(vector3));
  }

  public void ButtonReleased(Button button)
  {
    RectTransform component = (RectTransform) ((Component) button).GetComponent<RectTransform>();
    Vector3 vector3 = Vector2.op_Implicit(component.get_anchoredPosition());
    ref Vector3 local1 = ref vector3;
    local1.x = (__Null) (local1.x - 2.0);
    ref Vector3 local2 = ref vector3;
    local2.y = (__Null) (local2.y + 2.0);
    component.set_anchoredPosition(Vector2.op_Implicit(vector3));
  }

  public void ButtonEnterScale(Button button)
  {
    ((Transform) ((Component) button).GetComponent<RectTransform>()).set_localScale(new Vector3(1.1f, 1.1f, 1.1f));
  }

  public void ButtonLeaveScale(Button button)
  {
    ((Transform) ((Component) button).GetComponent<RectTransform>()).set_localScale(new Vector3(1f, 1f, 1f));
  }

  public void SliderChanged(Slider slider)
  {
    this.mouseOrbitController.disableSteering = true;
    if (this.objectsParams[this.currentTargetIndex].materialProperty == "fallIntensity")
      ((UBER_GlobalParams) ((Component) this.mainCamera).GetComponent<UBER_GlobalParams>()).fallIntensity = slider.get_value();
    else if (this.objectsParams[this.currentTargetIndex].materialProperty == "_SnowColorAndCoverage")
    {
      Color color = this.currentMaterial.GetColor("_SnowColorAndCoverage");
      color.a = (__Null) (double) slider.get_value();
      this.currentMaterial.SetColor("_SnowColorAndCoverage", color);
      slider.set_wholeNumbers(false);
    }
    else if (this.objectsParams[this.currentTargetIndex].materialProperty == "SPECIAL_Tiling")
    {
      this.currentMaterial.SetTextureScale("_MainTex", new Vector2(slider.get_value(), slider.get_value()));
      slider.set_wholeNumbers(true);
    }
    else
    {
      this.currentMaterial.SetFloat(this.objectsParams[this.currentTargetIndex].materialProperty, slider.get_value());
      slider.set_wholeNumbers(false);
    }
  }

  public void ExposureChanged(Slider slider)
  {
    TonemappingColorGrading component = (TonemappingColorGrading) ((Component) this.mainCamera).get_gameObject().GetComponent<TonemappingColorGrading>();
    TonemappingColorGrading.TonemappingSettings tonemapping = component.get_tonemapping();
    tonemapping.exposure = (__Null) (double) slider.get_value();
    component.set_tonemapping(tonemapping);
  }

  public void ClickedSkybox1()
  {
    this.skyboxSphereActive.get_transform().set_rotation(Quaternion.get_identity());
    ((Renderer) this.skyboxSphereActive.GetComponentInChildren<Renderer>()).set_sharedMaterial(this.skyboxSphereMaterialInactive);
    this.skyboxSphereActive = this.skyboxSphere1;
    ((Renderer) this.skyboxSphereActive.GetComponentInChildren<Renderer>()).set_sharedMaterial(this.skyboxSphereMaterialActive);
    RenderSettings.set_customReflection(this.reflectionCubemap1);
    RenderSettings.set_reflectionIntensity(this.exposure1);
    RenderSettings.set_skybox(this.skyboxMaterial1);
    this.realTimeLight1.SetActive(true);
    this.realTimeLight2.SetActive(false);
    this.realTimeLight3.SetActive(false);
    DynamicGI.UpdateEnvironment();
  }

  public void ClickedSkybox2()
  {
    this.skyboxSphereActive.get_transform().set_rotation(Quaternion.get_identity());
    ((Renderer) this.skyboxSphereActive.GetComponentInChildren<Renderer>()).set_sharedMaterial(this.skyboxSphereMaterialInactive);
    this.skyboxSphereActive = this.skyboxSphere2;
    ((Renderer) this.skyboxSphereActive.GetComponentInChildren<Renderer>()).set_sharedMaterial(this.skyboxSphereMaterialActive);
    RenderSettings.set_customReflection(this.reflectionCubemap2);
    RenderSettings.set_reflectionIntensity(this.exposure2);
    RenderSettings.set_skybox(this.skyboxMaterial2);
    this.realTimeLight1.SetActive(false);
    this.realTimeLight2.SetActive(true);
    this.realTimeLight3.SetActive(false);
    DynamicGI.UpdateEnvironment();
  }

  public void ClickedSkybox3()
  {
    this.skyboxSphereActive.get_transform().set_rotation(Quaternion.get_identity());
    ((Renderer) this.skyboxSphereActive.GetComponentInChildren<Renderer>()).set_sharedMaterial(this.skyboxSphereMaterialInactive);
    this.skyboxSphereActive = this.skyboxSphere3;
    ((Renderer) this.skyboxSphereActive.GetComponentInChildren<Renderer>()).set_sharedMaterial(this.skyboxSphereMaterialActive);
    RenderSettings.set_customReflection(this.reflectionCubemap3);
    RenderSettings.set_reflectionIntensity(this.exposure3);
    RenderSettings.set_skybox(this.skyboxMaterial3);
    this.realTimeLight1.SetActive(false);
    this.realTimeLight2.SetActive(false);
    this.realTimeLight3.SetActive(true);
    DynamicGI.UpdateEnvironment();
  }

  public void TogglePostFX()
  {
    TonemappingColorGrading component = (TonemappingColorGrading) ((Component) this.mainCamera).get_gameObject().GetComponent<TonemappingColorGrading>();
    this.togglepostFXButtonOn.SetActive(!((Behaviour) component).get_enabled());
    this.togglepostFXButtonOff.SetActive(((Behaviour) component).get_enabled());
    ((Selectable) this.exposureSlider).set_interactable(!((Behaviour) component).get_enabled());
    ((Behaviour) component).set_enabled(!((Behaviour) component).get_enabled());
    ((Behaviour) ((Component) this.mainCamera).get_gameObject().GetComponent<Bloom>()).set_enabled(((Behaviour) component).get_enabled());
  }

  public void SetTemperatureSun()
  {
    ColorBlock colors1 = ((Selectable) this.buttonSun).get_colors();
    ((ColorBlock) ref colors1).set_normalColor(new Color(1f, 1f, 1f, 0.7f));
    ((Selectable) this.buttonSun).set_colors(colors1);
    ColorBlock colors2 = ((Selectable) this.buttonFrost).get_colors();
    ((ColorBlock) ref colors2).set_normalColor(new Color(1f, 1f, 1f, 0.2f));
    ((Selectable) this.buttonFrost).set_colors(colors2);
    ((UBER_GlobalParams) ((Component) this.mainCamera).GetComponent<UBER_GlobalParams>()).temperature = 20f;
  }

  public void SetTemperatureFrost()
  {
    ColorBlock colors1 = ((Selectable) this.buttonSun).get_colors();
    ((ColorBlock) ref colors1).set_normalColor(new Color(1f, 1f, 1f, 0.2f));
    ((Selectable) this.buttonSun).set_colors(colors1);
    ColorBlock colors2 = ((Selectable) this.buttonFrost).get_colors();
    ((ColorBlock) ref colors2).set_normalColor(new Color(1f, 1f, 1f, 0.7f));
    ((Selectable) this.buttonFrost).set_colors(colors2);
    ((UBER_GlobalParams) ((Component) this.mainCamera).GetComponent<UBER_GlobalParams>()).temperature = -20f;
  }

  private void PrepareCurrentObject()
  {
    this.currentRenderer = this.objectsParams[this.currentTargetIndex].renderer;
    if (Object.op_Implicit((Object) this.currentRenderer))
    {
      this.originalMaterial = ((Renderer) this.currentRenderer).get_sharedMaterials()[this.objectsParams[this.currentTargetIndex].submeshIndex];
      this.currentMaterial = (Material) Object.Instantiate<Material>((M0) this.originalMaterial);
      Material[] sharedMaterials = ((Renderer) this.currentRenderer).get_sharedMaterials();
      sharedMaterials[this.objectsParams[this.currentTargetIndex].submeshIndex] = this.currentMaterial;
      ((Renderer) this.currentRenderer).set_sharedMaterials(sharedMaterials);
    }
    bool flag = this.objectsParams[this.currentTargetIndex].materialProperty == null || this.objectsParams[this.currentTargetIndex].materialProperty == string.Empty;
    if (flag)
    {
      ((Component) this.materialSlider).get_gameObject().SetActive(false);
    }
    else
    {
      ((Component) this.materialSlider).get_gameObject().SetActive(true);
      this.materialSlider.set_minValue((float) this.objectsParams[this.currentTargetIndex].SliderRange.x);
      this.materialSlider.set_maxValue((float) this.objectsParams[this.currentTargetIndex].SliderRange.y);
      if (this.objectsParams[this.currentTargetIndex].materialProperty == "fallIntensity")
      {
        UBER_GlobalParams component = (UBER_GlobalParams) ((Component) this.mainCamera).GetComponent<UBER_GlobalParams>();
        this.materialSlider.set_value(component.fallIntensity);
        component.UseParticleSystem = true;
        ((Component) this.buttonSun).get_gameObject().SetActive(true);
        ((Component) this.buttonFrost).get_gameObject().SetActive(true);
      }
      else
      {
        ((UBER_GlobalParams) ((Component) this.mainCamera).GetComponent<UBER_GlobalParams>()).UseParticleSystem = false;
        ((Component) this.buttonSun).get_gameObject().SetActive(false);
        ((Component) this.buttonFrost).get_gameObject().SetActive(false);
        if (this.originalMaterial.HasProperty(this.objectsParams[this.currentTargetIndex].materialProperty))
        {
          if (this.objectsParams[this.currentTargetIndex].materialProperty == "_SnowColorAndCoverage")
            this.materialSlider.set_value((float) this.originalMaterial.GetColor("_SnowColorAndCoverage").a);
          else
            this.materialSlider.set_value(this.originalMaterial.GetFloat(this.objectsParams[this.currentTargetIndex].materialProperty));
        }
        else if (this.objectsParams[this.currentTargetIndex].materialProperty == "SPECIAL_Tiling")
          this.materialSlider.set_value(1f);
      }
    }
    this.titleTextArea.set_text(this.objectsParams[this.currentTargetIndex].Title);
    this.descriptionTextArea.set_text(this.objectsParams[this.currentTargetIndex].Description);
    this.matParamTextArea.set_text(this.objectsParams[this.currentTargetIndex].MatParamName);
    Vector2 anchoredPosition = ((Graphic) this.titleTextArea).get_rectTransform().get_anchoredPosition();
    anchoredPosition.y = (__Null) ((!flag ? 110.0 : 50.0) + (double) this.descriptionTextArea.get_preferredHeight());
    ((Graphic) this.titleTextArea).get_rectTransform().set_anchoredPosition(anchoredPosition);
  }
}
