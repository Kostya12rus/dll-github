// Decompiled with JetBrains decompiler
// Type: NGSS_ContactShadows
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class NGSS_ContactShadows : MonoBehaviour
{
  public Light mainDirectionalLight;
  public Shader contactShadowsShader;
  public bool noiseFilter;
  [Range(0.0f, 3f)]
  public float shadowsSoftness;
  [Range(1f, 4f)]
  public float shadowsDistance;
  [Range(0.1f, 4f)]
  public float shadowsFade;
  [Range(0.0f, 0.02f)]
  public float shadowsBias;
  [Range(0.0f, 1f)]
  public float rayWidth;
  [Range(16f, 128f)]
  public int raySamples;
  private CommandBuffer blendShadowsCB;
  private CommandBuffer computeShadowsCB;
  private bool isInitialized;
  private Camera _mCamera;
  private Material _mMaterial;

  public NGSS_ContactShadows()
  {
    base.\u002Ector();
  }

  private Camera mCamera
  {
    get
    {
      if (Object.op_Equality((Object) this._mCamera, (Object) null))
      {
        this._mCamera = (Camera) ((Component) this).GetComponent<Camera>();
        if (Object.op_Equality((Object) this._mCamera, (Object) null))
          this._mCamera = Camera.get_main();
        if (Object.op_Equality((Object) this._mCamera, (Object) null))
        {
          Debug.LogError((object) "NGSS Error: No MainCamera found, please provide one.", (Object) this);
        }
        else
        {
          Camera mCamera = this._mCamera;
          mCamera.set_depthTextureMode((DepthTextureMode) (mCamera.get_depthTextureMode() | 1));
        }
      }
      return this._mCamera;
    }
  }

  private Material mMaterial
  {
    get
    {
      if (Object.op_Equality((Object) this._mMaterial, (Object) null))
      {
        if (Object.op_Equality((Object) this.contactShadowsShader, (Object) null))
          Shader.Find("Hidden/NGSS_ContactShadows");
        this._mMaterial = new Material(this.contactShadowsShader);
        if (Object.op_Equality((Object) this._mMaterial, (Object) null))
        {
          Debug.LogWarning((object) "NGSS Warning: can't find NGSS_ContactShadows shader, make sure it's on your project.", (Object) this);
          ((Behaviour) this).set_enabled(false);
          return (Material) null;
        }
      }
      return this._mMaterial;
    }
  }

  private void AddCommandBuffers()
  {
    CommandBuffer commandBuffer1 = new CommandBuffer();
    commandBuffer1.set_name("NGSS ContactShadows: Compute");
    this.computeShadowsCB = commandBuffer1;
    CommandBuffer commandBuffer2 = new CommandBuffer();
    commandBuffer2.set_name("NGSS ContactShadows: Mix");
    this.blendShadowsCB = commandBuffer2;
    bool flag = this.mCamera.get_actualRenderingPath() == 1;
    if (Object.op_Implicit((Object) this.mCamera))
    {
      foreach (CommandBuffer commandBuffer3 in this.mCamera.GetCommandBuffers(!flag ? (CameraEvent) 6 : (CameraEvent) 1))
      {
        if (commandBuffer3.get_name() == this.computeShadowsCB.get_name())
          return;
      }
      this.mCamera.AddCommandBuffer(!flag ? (CameraEvent) 6 : (CameraEvent) 1, this.computeShadowsCB);
    }
    if (!Object.op_Implicit((Object) this.mainDirectionalLight))
      return;
    foreach (CommandBuffer commandBuffer3 in this.mainDirectionalLight.GetCommandBuffers((LightEvent) 3))
    {
      if (commandBuffer3.get_name() == this.blendShadowsCB.get_name())
        return;
    }
    this.mainDirectionalLight.AddCommandBuffer((LightEvent) 3, this.blendShadowsCB);
  }

  private void RemoveCommandBuffers()
  {
    this._mMaterial = (Material) null;
    bool flag = this.mCamera.get_actualRenderingPath() == 1;
    if (Object.op_Implicit((Object) this.mCamera))
      this.mCamera.RemoveCommandBuffer(!flag ? (CameraEvent) 6 : (CameraEvent) 1, this.computeShadowsCB);
    if (Object.op_Implicit((Object) this.mainDirectionalLight))
      this.mainDirectionalLight.RemoveCommandBuffer((LightEvent) 3, this.blendShadowsCB);
    this.isInitialized = false;
  }

  private void Init()
  {
    if (this.isInitialized || Object.op_Equality((Object) this.mainDirectionalLight, (Object) null))
      return;
    if (this.mCamera.get_renderingPath() == -1 || this.mCamera.get_renderingPath() == null)
    {
      Debug.LogWarning((object) "Please set your camera rendering path to either Forward or Deferred and re-enable this component.", (Object) this);
      ((Behaviour) this).set_enabled(false);
    }
    else
    {
      this.AddCommandBuffers();
      int id1 = Shader.PropertyToID("NGSS_ContactShadowRT");
      int id2 = Shader.PropertyToID("NGSS_DepthSourceRT");
      this.computeShadowsCB.GetTemporaryRT(id1, -1, -1, 0, (FilterMode) 1, (RenderTextureFormat) 16);
      this.computeShadowsCB.GetTemporaryRT(id2, -1, -1, 0, (FilterMode) 0, (RenderTextureFormat) 14);
      this.computeShadowsCB.Blit(RenderTargetIdentifier.op_Implicit(id1), RenderTargetIdentifier.op_Implicit(id2), this.mMaterial, 0);
      this.computeShadowsCB.Blit(RenderTargetIdentifier.op_Implicit(id2), RenderTargetIdentifier.op_Implicit(id1), this.mMaterial, 1);
      this.computeShadowsCB.Blit(RenderTargetIdentifier.op_Implicit(id1), RenderTargetIdentifier.op_Implicit(id2), this.mMaterial, 2);
      this.blendShadowsCB.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 0), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 1), this.mMaterial, 3);
      this.computeShadowsCB.SetGlobalTexture("NGSS_ContactShadowsTexture", RenderTargetIdentifier.op_Implicit(id2));
      this.isInitialized = true;
    }
  }

  private void OnEnable()
  {
    this.Init();
  }

  private void OnDisable()
  {
    if (!this.isInitialized)
      return;
    this.RemoveCommandBuffers();
  }

  private void OnApplicationQuit()
  {
    if (!this.isInitialized)
      return;
    this.RemoveCommandBuffers();
  }

  private void OnPreRender()
  {
    this.Init();
    if (!this.isInitialized || Object.op_Equality((Object) this.mainDirectionalLight, (Object) null))
      return;
    this.mMaterial.SetVector("LightDir", Vector4.op_Implicit(((Component) this.mCamera).get_transform().InverseTransformDirection(((Component) this.mainDirectionalLight).get_transform().get_forward())));
    this.mMaterial.SetFloat("ShadowsSoftness", this.shadowsSoftness);
    this.mMaterial.SetFloat("ShadowsDistance", this.shadowsDistance);
    this.mMaterial.SetFloat("ShadowsFade", this.shadowsFade);
    this.mMaterial.SetFloat("ShadowsBias", this.shadowsBias);
    this.mMaterial.SetFloat("RayWidth", this.rayWidth);
    this.mMaterial.SetInt("RaySamples", this.raySamples);
    if (this.noiseFilter)
      this.mMaterial.EnableKeyword("NGSS_CONTACT_SHADOWS_USE_NOISE");
    else
      this.mMaterial.DisableKeyword("NGSS_CONTACT_SHADOWS_USE_NOISE");
  }
}
