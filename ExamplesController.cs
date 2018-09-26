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
  public float autoRotationSpeed = 30f;
  public bool autoRotation = true;
  [Range(0.0f, 1f)]
  public float exposure1 = 1f;
  [Range(0.0f, 1f)]
  public float exposure2 = 1f;
  [Range(0.0f, 1f)]
  public float exposure3 = 1f;
  [Space(10f)]
  public float hideTimeDelay = 10f;
  public UBER_ExampleObjectParams[] objectsParams;
  public Camera mainCamera;
  public UBER_MouseOrbit_DynamicDistance mouseOrbitController;
  public GameObject InteractiveUI;
  [Space(10f)]
  public GameObject autorotateButtonOn;
  public GameObject autorotateButtonOff;
  public GameObject togglepostFXButtonOn;
  public GameObject togglepostFXButtonOff;
  [Space(10f)]
  public GameObject skyboxSphere1;
  public Cubemap reflectionCubemap1;
  public GameObject realTimeLight1;
  public Material skyboxMaterial1;
  public GameObject skyboxSphere2;
  public Cubemap reflectionCubemap2;
  public GameObject realTimeLight2;
  public Material skyboxMaterial2;
  public GameObject skyboxSphere3;
  public Cubemap reflectionCubemap3;
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
  private MeshRenderer currentRenderer;
  private Material currentMaterial;
  private Material originalMaterial;
  private float hideTime;
  private int currentTargetIndex;
  private GameObject skyboxSphereActive;

  public void Start()
  {
    RenderSettings.skybox = this.skyboxMaterial1;
    this.realTimeLight1.SetActive(true);
    this.realTimeLight2.SetActive(false);
    this.realTimeLight3.SetActive(false);
    RenderSettings.customReflection = this.reflectionCubemap1;
    RenderSettings.reflectionIntensity = this.exposure1;
    DynamicGI.UpdateEnvironment();
    this.skyboxSphereActive = this.skyboxSphere1;
    this.currentTargetIndex = 0;
    this.PrepareCurrentObject();
    for (int index = 1; index < this.objectsParams.Length; ++index)
      this.objectsParams[index].target.SetActive(false);
    this.hideTime = Time.time + this.hideTimeDelay;
  }

  public void ClickedAutoRotation()
  {
    this.autoRotation = !this.autoRotation;
    this.autorotateButtonOn.SetActive(this.autoRotation);
    this.autorotateButtonOff.SetActive(!this.autoRotation);
  }

  public void ClickedArrow(bool rightFlag)
  {
    this.objectsParams[this.currentTargetIndex].target.transform.rotation = Quaternion.identity;
    this.objectsParams[this.currentTargetIndex].target.SetActive(false);
    if ((Object) this.currentRenderer != (Object) null && (Object) this.originalMaterial != (Object) null)
    {
      Material[] sharedMaterials = this.currentRenderer.sharedMaterials;
      sharedMaterials[this.objectsParams[this.currentTargetIndex].submeshIndex] = this.originalMaterial;
      this.currentRenderer.sharedMaterials = sharedMaterials;
      Object.Destroy((Object) this.currentMaterial);
    }
    this.currentTargetIndex = !rightFlag ? (this.currentTargetIndex + this.objectsParams.Length - 1) % this.objectsParams.Length : (this.currentTargetIndex + 1) % this.objectsParams.Length;
    this.PrepareCurrentObject();
    this.objectsParams[this.currentTargetIndex].target.SetActive(true);
    this.mouseOrbitController.target = this.objectsParams[this.currentTargetIndex].target;
    this.mouseOrbitController.targetFocus = this.objectsParams[this.currentTargetIndex].target.transform.Find("Focus");
    this.mouseOrbitController.Reset();
  }

  public void Update()
  {
    this.skyboxSphereActive.transform.Rotate(Vector3.up, Time.deltaTime * 200f, Space.World);
    if (this.objectsParams[this.currentTargetIndex].Title == "Ice block" && Input.GetKeyDown(KeyCode.L))
    {
      GameObject gameObject = this.objectsParams[this.currentTargetIndex].target.transform.Find("Amber").gameObject;
      gameObject.SetActive(!gameObject.activeSelf);
    }
    if (Input.GetKeyDown(KeyCode.RightArrow))
      this.ClickedArrow(true);
    else if (Input.GetKeyDown(KeyCode.LeftArrow))
      this.ClickedArrow(false);
    if (this.autoRotation)
      this.objectsParams[this.currentTargetIndex].target.transform.Rotate(Vector3.up, Time.deltaTime * this.autoRotationSpeed, Space.World);
    if ((double) Input.GetAxis("Mouse X") != 0.0 || (double) Input.GetAxis("Mouse Y") != 0.0)
    {
      this.hideTime = Time.time + this.hideTimeDelay;
      this.InteractiveUI.SetActive(true);
    }
    if ((double) Time.time <= (double) this.hideTime)
      return;
    this.InteractiveUI.SetActive(false);
  }

  public void ButtonPressed(Button button)
  {
    RectTransform component = button.GetComponent<RectTransform>();
    Vector3 anchoredPosition = (Vector3) component.anchoredPosition;
    anchoredPosition.x += 2f;
    anchoredPosition.y -= 2f;
    component.anchoredPosition = (Vector2) anchoredPosition;
  }

  public void ButtonReleased(Button button)
  {
    RectTransform component = button.GetComponent<RectTransform>();
    Vector3 anchoredPosition = (Vector3) component.anchoredPosition;
    anchoredPosition.x -= 2f;
    anchoredPosition.y += 2f;
    component.anchoredPosition = (Vector2) anchoredPosition;
  }

  public void ButtonEnterScale(Button button)
  {
    button.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
  }

  public void ButtonLeaveScale(Button button)
  {
    button.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
  }

  public void SliderChanged(Slider slider)
  {
    this.mouseOrbitController.disableSteering = true;
    if (this.objectsParams[this.currentTargetIndex].materialProperty == "fallIntensity")
      this.mainCamera.GetComponent<UBER_GlobalParams>().fallIntensity = slider.value;
    else if (this.objectsParams[this.currentTargetIndex].materialProperty == "_SnowColorAndCoverage")
    {
      Color color = this.currentMaterial.GetColor("_SnowColorAndCoverage");
      color.a = slider.value;
      this.currentMaterial.SetColor("_SnowColorAndCoverage", color);
      slider.wholeNumbers = false;
    }
    else if (this.objectsParams[this.currentTargetIndex].materialProperty == "SPECIAL_Tiling")
    {
      this.currentMaterial.SetTextureScale("_MainTex", new Vector2(slider.value, slider.value));
      slider.wholeNumbers = true;
    }
    else
    {
      this.currentMaterial.SetFloat(this.objectsParams[this.currentTargetIndex].materialProperty, slider.value);
      slider.wholeNumbers = false;
    }
  }

  public void ExposureChanged(Slider slider)
  {
    TonemappingColorGrading component = this.mainCamera.gameObject.GetComponent<TonemappingColorGrading>();
    TonemappingColorGrading.TonemappingSettings tonemapping = component.tonemapping;
    tonemapping.exposure = slider.value;
    component.tonemapping = tonemapping;
  }

  public void ClickedSkybox1()
  {
    this.skyboxSphereActive.transform.rotation = Quaternion.identity;
    this.skyboxSphereActive.GetComponentInChildren<Renderer>().sharedMaterial = this.skyboxSphereMaterialInactive;
    this.skyboxSphereActive = this.skyboxSphere1;
    this.skyboxSphereActive.GetComponentInChildren<Renderer>().sharedMaterial = this.skyboxSphereMaterialActive;
    RenderSettings.customReflection = this.reflectionCubemap1;
    RenderSettings.reflectionIntensity = this.exposure1;
    RenderSettings.skybox = this.skyboxMaterial1;
    this.realTimeLight1.SetActive(true);
    this.realTimeLight2.SetActive(false);
    this.realTimeLight3.SetActive(false);
    DynamicGI.UpdateEnvironment();
  }

  public void ClickedSkybox2()
  {
    this.skyboxSphereActive.transform.rotation = Quaternion.identity;
    this.skyboxSphereActive.GetComponentInChildren<Renderer>().sharedMaterial = this.skyboxSphereMaterialInactive;
    this.skyboxSphereActive = this.skyboxSphere2;
    this.skyboxSphereActive.GetComponentInChildren<Renderer>().sharedMaterial = this.skyboxSphereMaterialActive;
    RenderSettings.customReflection = this.reflectionCubemap2;
    RenderSettings.reflectionIntensity = this.exposure2;
    RenderSettings.skybox = this.skyboxMaterial2;
    this.realTimeLight1.SetActive(false);
    this.realTimeLight2.SetActive(true);
    this.realTimeLight3.SetActive(false);
    DynamicGI.UpdateEnvironment();
  }

  public void ClickedSkybox3()
  {
    this.skyboxSphereActive.transform.rotation = Quaternion.identity;
    this.skyboxSphereActive.GetComponentInChildren<Renderer>().sharedMaterial = this.skyboxSphereMaterialInactive;
    this.skyboxSphereActive = this.skyboxSphere3;
    this.skyboxSphereActive.GetComponentInChildren<Renderer>().sharedMaterial = this.skyboxSphereMaterialActive;
    RenderSettings.customReflection = this.reflectionCubemap3;
    RenderSettings.reflectionIntensity = this.exposure3;
    RenderSettings.skybox = this.skyboxMaterial3;
    this.realTimeLight1.SetActive(false);
    this.realTimeLight2.SetActive(false);
    this.realTimeLight3.SetActive(true);
    DynamicGI.UpdateEnvironment();
  }

  public void TogglePostFX()
  {
    TonemappingColorGrading component = this.mainCamera.gameObject.GetComponent<TonemappingColorGrading>();
    this.togglepostFXButtonOn.SetActive(!component.enabled);
    this.togglepostFXButtonOff.SetActive(component.enabled);
    this.exposureSlider.interactable = !component.enabled;
    component.enabled = !component.enabled;
    this.mainCamera.gameObject.GetComponent<Bloom>().enabled = component.enabled;
  }

  public void SetTemperatureSun()
  {
    ColorBlock colors1 = this.buttonSun.colors;
    colors1.normalColor = new Color(1f, 1f, 1f, 0.7f);
    this.buttonSun.colors = colors1;
    ColorBlock colors2 = this.buttonFrost.colors;
    colors2.normalColor = new Color(1f, 1f, 1f, 0.2f);
    this.buttonFrost.colors = colors2;
    this.mainCamera.GetComponent<UBER_GlobalParams>().temperature = 20f;
  }

  public void SetTemperatureFrost()
  {
    ColorBlock colors1 = this.buttonSun.colors;
    colors1.normalColor = new Color(1f, 1f, 1f, 0.2f);
    this.buttonSun.colors = colors1;
    ColorBlock colors2 = this.buttonFrost.colors;
    colors2.normalColor = new Color(1f, 1f, 1f, 0.7f);
    this.buttonFrost.colors = colors2;
    this.mainCamera.GetComponent<UBER_GlobalParams>().temperature = -20f;
  }

  private void PrepareCurrentObject()
  {
    this.currentRenderer = this.objectsParams[this.currentTargetIndex].renderer;
    if ((bool) ((Object) this.currentRenderer))
    {
      this.originalMaterial = this.currentRenderer.sharedMaterials[this.objectsParams[this.currentTargetIndex].submeshIndex];
      this.currentMaterial = Object.Instantiate<Material>(this.originalMaterial);
      Material[] sharedMaterials = this.currentRenderer.sharedMaterials;
      sharedMaterials[this.objectsParams[this.currentTargetIndex].submeshIndex] = this.currentMaterial;
      this.currentRenderer.sharedMaterials = sharedMaterials;
    }
    bool flag = this.objectsParams[this.currentTargetIndex].materialProperty == null || this.objectsParams[this.currentTargetIndex].materialProperty == string.Empty;
    if (flag)
    {
      this.materialSlider.gameObject.SetActive(false);
    }
    else
    {
      this.materialSlider.gameObject.SetActive(true);
      this.materialSlider.minValue = this.objectsParams[this.currentTargetIndex].SliderRange.x;
      this.materialSlider.maxValue = this.objectsParams[this.currentTargetIndex].SliderRange.y;
      if (this.objectsParams[this.currentTargetIndex].materialProperty == "fallIntensity")
      {
        UBER_GlobalParams component = this.mainCamera.GetComponent<UBER_GlobalParams>();
        this.materialSlider.value = component.fallIntensity;
        component.UseParticleSystem = true;
        this.buttonSun.gameObject.SetActive(true);
        this.buttonFrost.gameObject.SetActive(true);
      }
      else
      {
        this.mainCamera.GetComponent<UBER_GlobalParams>().UseParticleSystem = false;
        this.buttonSun.gameObject.SetActive(false);
        this.buttonFrost.gameObject.SetActive(false);
        if (this.originalMaterial.HasProperty(this.objectsParams[this.currentTargetIndex].materialProperty))
          this.materialSlider.value = !(this.objectsParams[this.currentTargetIndex].materialProperty == "_SnowColorAndCoverage") ? this.originalMaterial.GetFloat(this.objectsParams[this.currentTargetIndex].materialProperty) : this.originalMaterial.GetColor("_SnowColorAndCoverage").a;
        else if (this.objectsParams[this.currentTargetIndex].materialProperty == "SPECIAL_Tiling")
          this.materialSlider.value = 1f;
      }
    }
    this.titleTextArea.text = this.objectsParams[this.currentTargetIndex].Title;
    this.descriptionTextArea.text = this.objectsParams[this.currentTargetIndex].Description;
    this.matParamTextArea.text = this.objectsParams[this.currentTargetIndex].MatParamName;
    Vector2 anchoredPosition = this.titleTextArea.rectTransform.anchoredPosition;
    anchoredPosition.y = (!flag ? 110f : 50f) + this.descriptionTextArea.preferredHeight;
    this.titleTextArea.rectTransform.anchoredPosition = anchoredPosition;
  }
}
