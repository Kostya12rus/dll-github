// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.PostProcessingBehaviour
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  [AddComponentMenu("Effects/Post-Processing Behaviour", -1)]
  [ImageEffectAllowedInSceneView]
  [RequireComponent(typeof (Camera))]
  public class PostProcessingBehaviour : MonoBehaviour
  {
    public PostProcessingProfile profile;
    public Func<Vector2, Matrix4x4> jitteredMatrixFunc;
    private Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>> m_CommandBuffers;
    private List<PostProcessingComponentBase> m_Components;
    private Dictionary<PostProcessingComponentBase, bool> m_ComponentStates;
    private MaterialFactory m_MaterialFactory;
    private RenderTextureFactory m_RenderTextureFactory;
    private PostProcessingContext m_Context;
    private Camera m_Camera;
    private PostProcessingProfile m_PreviousProfile;
    private bool m_RenderingInSceneView;
    private BuiltinDebugViewsComponent m_DebugViews;
    private AmbientOcclusionComponent m_AmbientOcclusion;
    private ScreenSpaceReflectionComponent m_ScreenSpaceReflection;
    private FogComponent m_FogComponent;
    private MotionBlurComponent m_MotionBlur;
    private TaaComponent m_Taa;
    private EyeAdaptationComponent m_EyeAdaptation;
    private DepthOfFieldComponent m_DepthOfField;
    private BloomComponent m_Bloom;
    private ChromaticAberrationComponent m_ChromaticAberration;
    private ColorGradingComponent m_ColorGrading;
    private UserLutComponent m_UserLut;
    private GrainComponent m_Grain;
    private VignetteComponent m_Vignette;
    private DitheringComponent m_Dithering;
    private FxaaComponent m_Fxaa;
    private List<PostProcessingComponentBase> m_ComponentsToEnable;
    private List<PostProcessingComponentBase> m_ComponentsToDisable;

    public PostProcessingBehaviour()
    {
      base.\u002Ector();
    }

    private void OnEnable()
    {
      this.m_CommandBuffers = new Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>>();
      this.m_MaterialFactory = new MaterialFactory();
      this.m_RenderTextureFactory = new RenderTextureFactory();
      this.m_Context = new PostProcessingContext();
      this.m_Components = new List<PostProcessingComponentBase>();
      this.m_DebugViews = this.AddComponent<BuiltinDebugViewsComponent>(new BuiltinDebugViewsComponent());
      this.m_AmbientOcclusion = this.AddComponent<AmbientOcclusionComponent>(new AmbientOcclusionComponent());
      this.m_ScreenSpaceReflection = this.AddComponent<ScreenSpaceReflectionComponent>(new ScreenSpaceReflectionComponent());
      this.m_FogComponent = this.AddComponent<FogComponent>(new FogComponent());
      this.m_MotionBlur = this.AddComponent<MotionBlurComponent>(new MotionBlurComponent());
      this.m_Taa = this.AddComponent<TaaComponent>(new TaaComponent());
      this.m_EyeAdaptation = this.AddComponent<EyeAdaptationComponent>(new EyeAdaptationComponent());
      this.m_DepthOfField = this.AddComponent<DepthOfFieldComponent>(new DepthOfFieldComponent());
      this.m_Bloom = this.AddComponent<BloomComponent>(new BloomComponent());
      this.m_ChromaticAberration = this.AddComponent<ChromaticAberrationComponent>(new ChromaticAberrationComponent());
      this.m_ColorGrading = this.AddComponent<ColorGradingComponent>(new ColorGradingComponent());
      this.m_UserLut = this.AddComponent<UserLutComponent>(new UserLutComponent());
      this.m_Grain = this.AddComponent<GrainComponent>(new GrainComponent());
      this.m_Vignette = this.AddComponent<VignetteComponent>(new VignetteComponent());
      this.m_Dithering = this.AddComponent<DitheringComponent>(new DitheringComponent());
      this.m_Fxaa = this.AddComponent<FxaaComponent>(new FxaaComponent());
      this.m_ComponentStates = new Dictionary<PostProcessingComponentBase, bool>();
      foreach (PostProcessingComponentBase component in this.m_Components)
        this.m_ComponentStates.Add(component, false);
      this.set_useGUILayout(false);
    }

    private void Start()
    {
      NetworkIdentity networkIdentity = (NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>();
      if (Object.op_Equality((Object) networkIdentity, (Object) null))
        networkIdentity = (NetworkIdentity) ((Component) this).GetComponentInChildren<NetworkIdentity>();
      if (!Object.op_Inequality((Object) networkIdentity, (Object) null) || networkIdentity.get_isLocalPlayer())
        return;
      Object.DestroyImmediate((Object) this);
    }

    private void OnPreCull()
    {
      this.m_Camera = (Camera) ((Component) this).GetComponent<Camera>();
      if (Object.op_Equality((Object) this.profile, (Object) null) || Object.op_Equality((Object) this.m_Camera, (Object) null))
        return;
      PostProcessingContext pcontext = this.m_Context.Reset();
      pcontext.profile = this.profile;
      pcontext.renderTextureFactory = this.m_RenderTextureFactory;
      pcontext.materialFactory = this.m_MaterialFactory;
      pcontext.camera = this.m_Camera;
      this.m_DebugViews.Init(pcontext, this.profile.debugViews);
      this.m_AmbientOcclusion.Init(pcontext, this.profile.ambientOcclusion);
      this.m_ScreenSpaceReflection.Init(pcontext, this.profile.screenSpaceReflection);
      this.m_FogComponent.Init(pcontext, this.profile.fog);
      this.m_MotionBlur.Init(pcontext, this.profile.motionBlur);
      this.m_Taa.Init(pcontext, this.profile.antialiasing);
      this.m_EyeAdaptation.Init(pcontext, this.profile.eyeAdaptation);
      this.m_DepthOfField.Init(pcontext, this.profile.depthOfField);
      this.m_Bloom.Init(pcontext, this.profile.bloom);
      this.m_ChromaticAberration.Init(pcontext, this.profile.chromaticAberration);
      this.m_ColorGrading.Init(pcontext, this.profile.colorGrading);
      this.m_UserLut.Init(pcontext, this.profile.userLut);
      this.m_Grain.Init(pcontext, this.profile.grain);
      this.m_Vignette.Init(pcontext, this.profile.vignette);
      this.m_Dithering.Init(pcontext, this.profile.dithering);
      this.m_Fxaa.Init(pcontext, this.profile.antialiasing);
      if (Object.op_Inequality((Object) this.m_PreviousProfile, (Object) this.profile))
      {
        this.DisableComponents();
        this.m_PreviousProfile = this.profile;
      }
      this.CheckObservers();
      DepthTextureMode depthTextureMode = pcontext.camera.get_depthTextureMode();
      foreach (PostProcessingComponentBase component in this.m_Components)
      {
        if (component.active)
          depthTextureMode = depthTextureMode | component.GetCameraFlags();
      }
      pcontext.camera.set_depthTextureMode(depthTextureMode);
      if (this.m_RenderingInSceneView || !this.m_Taa.active || this.profile.debugViews.willInterrupt)
        return;
      this.m_Taa.SetProjectionMatrix(this.jitteredMatrixFunc);
    }

    private void OnPreRender()
    {
      if (Object.op_Equality((Object) this.profile, (Object) null))
        return;
      this.TryExecuteCommandBuffer<BuiltinDebugViewsModel>((PostProcessingComponentCommandBuffer<BuiltinDebugViewsModel>) this.m_DebugViews);
      this.TryExecuteCommandBuffer<AmbientOcclusionModel>((PostProcessingComponentCommandBuffer<AmbientOcclusionModel>) this.m_AmbientOcclusion);
      this.TryExecuteCommandBuffer<ScreenSpaceReflectionModel>((PostProcessingComponentCommandBuffer<ScreenSpaceReflectionModel>) this.m_ScreenSpaceReflection);
      this.TryExecuteCommandBuffer<FogModel>((PostProcessingComponentCommandBuffer<FogModel>) this.m_FogComponent);
      if (this.m_RenderingInSceneView)
        return;
      this.TryExecuteCommandBuffer<MotionBlurModel>((PostProcessingComponentCommandBuffer<MotionBlurModel>) this.m_MotionBlur);
    }

    private void OnPostRender()
    {
      if (Object.op_Equality((Object) this.profile, (Object) null) || Object.op_Equality((Object) this.m_Camera, (Object) null) || (this.m_RenderingInSceneView || !this.m_Taa.active) || this.profile.debugViews.willInterrupt)
        return;
      this.m_Context.camera.ResetProjectionMatrix();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (Object.op_Equality((Object) this.profile, (Object) null) || Object.op_Equality((Object) this.m_Camera, (Object) null))
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        bool flag1 = false;
        bool active = this.m_Fxaa.active;
        bool antialiasCoC = this.m_Taa.active && !this.m_RenderingInSceneView;
        bool flag2 = this.m_DepthOfField.active && !this.m_RenderingInSceneView;
        Material material1 = this.m_MaterialFactory.Get("Hidden/Post FX/Uber Shader");
        material1.set_shaderKeywords((string[]) null);
        RenderTexture renderTexture1 = source;
        RenderTexture destination1 = destination;
        if (antialiasCoC)
        {
          RenderTexture destination2 = this.m_RenderTextureFactory.Get(renderTexture1);
          this.m_Taa.Render(renderTexture1, destination2);
          renderTexture1 = destination2;
        }
        Texture autoExposure = (Texture) GraphicsUtils.whiteTexture;
        if (this.m_EyeAdaptation.active)
        {
          flag1 = true;
          autoExposure = this.m_EyeAdaptation.Prepare(renderTexture1, material1);
        }
        material1.SetTexture("_AutoExposure", autoExposure);
        if (flag2)
        {
          flag1 = true;
          this.m_DepthOfField.Prepare(renderTexture1, material1, antialiasCoC, this.m_Taa.jitterVector, this.m_Taa.model.settings.taaSettings.motionBlending);
        }
        if (this.m_Bloom.active)
        {
          flag1 = true;
          this.m_Bloom.Prepare(renderTexture1, material1, autoExposure);
        }
        bool flag3 = flag1 | this.TryPrepareUberImageEffect<ChromaticAberrationModel>((PostProcessingComponentRenderTexture<ChromaticAberrationModel>) this.m_ChromaticAberration, material1) | this.TryPrepareUberImageEffect<ColorGradingModel>((PostProcessingComponentRenderTexture<ColorGradingModel>) this.m_ColorGrading, material1) | this.TryPrepareUberImageEffect<VignetteModel>((PostProcessingComponentRenderTexture<VignetteModel>) this.m_Vignette, material1) | this.TryPrepareUberImageEffect<UserLutModel>((PostProcessingComponentRenderTexture<UserLutModel>) this.m_UserLut, material1);
        Material material2 = !active ? (Material) null : this.m_MaterialFactory.Get("Hidden/Post FX/FXAA");
        if (active)
        {
          material2.set_shaderKeywords((string[]) null);
          this.TryPrepareUberImageEffect<GrainModel>((PostProcessingComponentRenderTexture<GrainModel>) this.m_Grain, material2);
          this.TryPrepareUberImageEffect<DitheringModel>((PostProcessingComponentRenderTexture<DitheringModel>) this.m_Dithering, material2);
          if (flag3)
          {
            RenderTexture renderTexture2 = this.m_RenderTextureFactory.Get(renderTexture1);
            Graphics.Blit((Texture) renderTexture1, renderTexture2, material1, 0);
            renderTexture1 = renderTexture2;
          }
          this.m_Fxaa.Render(renderTexture1, destination1);
        }
        else
        {
          flag3 = flag3 | this.TryPrepareUberImageEffect<GrainModel>((PostProcessingComponentRenderTexture<GrainModel>) this.m_Grain, material1) | this.TryPrepareUberImageEffect<DitheringModel>((PostProcessingComponentRenderTexture<DitheringModel>) this.m_Dithering, material1);
          if (flag3)
          {
            if (!GraphicsUtils.isLinearColorSpace)
              material1.EnableKeyword("UNITY_COLORSPACE_GAMMA");
            Graphics.Blit((Texture) renderTexture1, destination1, material1, 0);
          }
        }
        if (!flag3 && !active)
          Graphics.Blit((Texture) renderTexture1, destination1);
        this.m_RenderTextureFactory.ReleaseAll();
      }
    }

    private void OnGUI()
    {
      if (Event.get_current().get_type() != 7 || Object.op_Equality((Object) this.profile, (Object) null) || Object.op_Equality((Object) this.m_Camera, (Object) null))
        return;
      if (this.m_EyeAdaptation.active && this.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation))
        this.m_EyeAdaptation.OnGUI();
      else if (this.m_ColorGrading.active && this.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.LogLut))
      {
        this.m_ColorGrading.OnGUI();
      }
      else
      {
        if (!this.m_UserLut.active || !this.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.UserLut))
          return;
        this.m_UserLut.OnGUI();
      }
    }

    private void OnDisable()
    {
      using (Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>>.ValueCollection.Enumerator enumerator = this.m_CommandBuffers.Values.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<CameraEvent, CommandBuffer> current = enumerator.Current;
          this.m_Camera.RemoveCommandBuffer(current.Key, current.Value);
          current.Value.Dispose();
        }
      }
      this.m_CommandBuffers.Clear();
      if (Object.op_Inequality((Object) this.profile, (Object) null))
        this.DisableComponents();
      this.m_Components.Clear();
      this.m_MaterialFactory.Dispose();
      this.m_RenderTextureFactory.Dispose();
      GraphicsUtils.Dispose();
    }

    public void ResetTemporalEffects()
    {
      this.m_Taa.ResetHistory();
      this.m_MotionBlur.ResetHistory();
      this.m_EyeAdaptation.ResetHistory();
    }

    private void CheckObservers()
    {
      foreach (KeyValuePair<PostProcessingComponentBase, bool> componentState in this.m_ComponentStates)
      {
        PostProcessingComponentBase key = componentState.Key;
        bool enabled = key.GetModel().enabled;
        if (enabled != componentState.Value)
        {
          if (enabled)
            this.m_ComponentsToEnable.Add(key);
          else
            this.m_ComponentsToDisable.Add(key);
        }
      }
      for (int index1 = 0; index1 < this.m_ComponentsToDisable.Count; ++index1)
      {
        PostProcessingComponentBase index2 = this.m_ComponentsToDisable[index1];
        this.m_ComponentStates[index2] = false;
        index2.OnDisable();
      }
      for (int index1 = 0; index1 < this.m_ComponentsToEnable.Count; ++index1)
      {
        PostProcessingComponentBase index2 = this.m_ComponentsToEnable[index1];
        this.m_ComponentStates[index2] = true;
        index2.OnEnable();
      }
      this.m_ComponentsToDisable.Clear();
      this.m_ComponentsToEnable.Clear();
    }

    private void DisableComponents()
    {
      foreach (PostProcessingComponentBase component in this.m_Components)
      {
        PostProcessingModel model = component.GetModel();
        if (model != null && model.enabled)
          component.OnDisable();
      }
    }

    private CommandBuffer AddCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
    {
      CommandBuffer commandBuffer1 = new CommandBuffer();
      commandBuffer1.set_name(name);
      CommandBuffer commandBuffer2 = commandBuffer1;
      KeyValuePair<CameraEvent, CommandBuffer> keyValuePair = new KeyValuePair<CameraEvent, CommandBuffer>(evt, commandBuffer2);
      this.m_CommandBuffers.Add(typeof (T), keyValuePair);
      this.m_Camera.AddCommandBuffer(evt, keyValuePair.Value);
      return keyValuePair.Value;
    }

    private void RemoveCommandBuffer<T>() where T : PostProcessingModel
    {
      Type key = typeof (T);
      KeyValuePair<CameraEvent, CommandBuffer> keyValuePair;
      if (!this.m_CommandBuffers.TryGetValue(key, out keyValuePair))
        return;
      this.m_Camera.RemoveCommandBuffer(keyValuePair.Key, keyValuePair.Value);
      this.m_CommandBuffers.Remove(key);
      keyValuePair.Value.Dispose();
    }

    private CommandBuffer GetCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
    {
      KeyValuePair<CameraEvent, CommandBuffer> keyValuePair;
      CommandBuffer commandBuffer;
      if (!this.m_CommandBuffers.TryGetValue(typeof (T), out keyValuePair))
        commandBuffer = this.AddCommandBuffer<T>(evt, name);
      else if (keyValuePair.Key != evt)
      {
        this.RemoveCommandBuffer<T>();
        commandBuffer = this.AddCommandBuffer<T>(evt, name);
      }
      else
        commandBuffer = keyValuePair.Value;
      return commandBuffer;
    }

    private void TryExecuteCommandBuffer<T>(PostProcessingComponentCommandBuffer<T> component) where T : PostProcessingModel
    {
      if (component.active)
      {
        CommandBuffer commandBuffer = this.GetCommandBuffer<T>(component.GetCameraEvent(), component.GetName());
        commandBuffer.Clear();
        component.PopulateCommandBuffer(commandBuffer);
      }
      else
        this.RemoveCommandBuffer<T>();
    }

    private bool TryPrepareUberImageEffect<T>(PostProcessingComponentRenderTexture<T> component, Material material) where T : PostProcessingModel
    {
      if (!component.active)
        return false;
      component.Prepare(material);
      return true;
    }

    private T AddComponent<T>(T component) where T : PostProcessingComponentBase
    {
      this.m_Components.Add((PostProcessingComponentBase) component);
      return component;
    }
  }
}
