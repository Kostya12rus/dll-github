// Decompiled with JetBrains decompiler
// Type: VolumetricLight
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (Light))]
public class VolumetricLight : MonoBehaviour
{
  private Light _light;
  private Material _material;
  private CommandBuffer _commandBuffer;
  private CommandBuffer _cascadeShadowCommandBuffer;
  [Range(1f, 64f)]
  public int SampleCount;
  [Range(0.0f, 1f)]
  public float ScatteringCoef;
  [Range(0.0f, 0.1f)]
  public float ExtinctionCoef;
  [Range(0.0f, 1f)]
  public float SkyboxExtinctionCoef;
  [Range(0.0f, 0.999f)]
  public float MieG;
  public bool HeightFog;
  [Range(0.0f, 0.5f)]
  public float HeightScale;
  public float GroundLevel;
  public bool Noise;
  public float NoiseScale;
  public float NoiseIntensity;
  public float NoiseIntensityOffset;
  public Vector2 NoiseVelocity;
  [Tooltip("")]
  public float MaxRayLength;
  private Vector4[] _frustumCorners;
  private bool _reversedZ;

  public VolumetricLight()
  {
    base.\u002Ector();
  }

  public event Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> CustomRenderEvent;

  public Light Light
  {
    get
    {
      return this._light;
    }
  }

  public Material VolumetricMaterial
  {
    get
    {
      return this._material;
    }
  }

  private void Start()
  {
    if (SystemInfo.get_graphicsDeviceType() == 2 || SystemInfo.get_graphicsDeviceType() == 18 || (SystemInfo.get_graphicsDeviceType() == 16 || SystemInfo.get_graphicsDeviceType() == 13) || (SystemInfo.get_graphicsDeviceType() == 21 || SystemInfo.get_graphicsDeviceType() == 14))
      this._reversedZ = true;
    this._commandBuffer = new CommandBuffer();
    this._commandBuffer.set_name("Light Command Buffer");
    this._cascadeShadowCommandBuffer = new CommandBuffer();
    this._cascadeShadowCommandBuffer.set_name("Dir Light Command Buffer");
    this._cascadeShadowCommandBuffer.SetGlobalTexture("_CascadeShadowMapTexture", new RenderTargetIdentifier((BuiltinRenderTextureType) 1));
    this._light = (Light) ((Component) this).GetComponent<Light>();
    if (this._light.get_type() == 1)
    {
      this._light.AddCommandBuffer((LightEvent) 2, this._commandBuffer);
      this._light.AddCommandBuffer((LightEvent) 1, this._cascadeShadowCommandBuffer);
    }
    else
      this._light.AddCommandBuffer((LightEvent) 1, this._commandBuffer);
    Shader shader = Shader.Find("Sandbox/VolumetricLight");
    if (Object.op_Equality((Object) shader, (Object) null))
      throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
    this._material = new Material(shader);
  }

  private void OnEnable()
  {
    VolumetricLightRenderer.PreRenderEvent += new Action<VolumetricLightRenderer, Matrix4x4>(this.VolumetricLightRenderer_PreRenderEvent);
  }

  private void OnDisable()
  {
    VolumetricLightRenderer.PreRenderEvent -= new Action<VolumetricLightRenderer, Matrix4x4>(this.VolumetricLightRenderer_PreRenderEvent);
  }

  public void OnDestroy()
  {
    Object.Destroy((Object) this._material);
  }

  private void VolumetricLightRenderer_PreRenderEvent(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
  {
    if (!((Component) this._light).get_gameObject().get_activeInHierarchy() || !((Behaviour) this._light).get_enabled())
      return;
    this._material.SetVector("_CameraForward", Vector4.op_Implicit(((Component) Camera.get_current()).get_transform().get_forward()));
    this._material.SetInt("_SampleCount", this.SampleCount);
    this._material.SetVector("_NoiseVelocity", Vector4.op_Multiply(new Vector4((float) this.NoiseVelocity.x, (float) this.NoiseVelocity.y), this.NoiseScale));
    this._material.SetVector("_NoiseData", new Vector4(this.NoiseScale, this.NoiseIntensity, this.NoiseIntensityOffset));
    this._material.SetVector("_MieG", new Vector4((float) (1.0 - (double) this.MieG * (double) this.MieG), (float) (1.0 + (double) this.MieG * (double) this.MieG), 2f * this.MieG, 0.07957747f));
    this._material.SetVector("_VolumetricLight", new Vector4(this.ScatteringCoef, this.ExtinctionCoef, this._light.get_range(), 1f - this.SkyboxExtinctionCoef));
    this._material.SetTexture("_CameraDepthTexture", (Texture) renderer.GetVolumeLightDepthBuffer());
    this._material.SetFloat("_ZTest", 8f);
    if (this.HeightFog)
    {
      this._material.EnableKeyword("HEIGHT_FOG");
      this._material.SetVector("_HeightFog", new Vector4(this.GroundLevel, this.HeightScale));
    }
    else
      this._material.DisableKeyword("HEIGHT_FOG");
    if (this._light.get_type() == 2)
      this.SetupPointLight(renderer, viewProj);
    else if (this._light.get_type() == null)
    {
      this.SetupSpotLight(renderer, viewProj);
    }
    else
    {
      if (this._light.get_type() != 1)
        return;
      this.SetupDirectionalLight(renderer, viewProj);
    }
  }

  private void Update()
  {
    this._commandBuffer.Clear();
  }

  private void SetupPointLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
  {
    int num1 = 0;
    if (!this.IsCameraInPointLightBounds())
      num1 = 2;
    this._material.SetPass(num1);
    Mesh pointLightMesh = VolumetricLightRenderer.GetPointLightMesh();
    float num2 = this._light.get_range() * 2f;
    Matrix4x4 matrix4x4_1 = Matrix4x4.TRS(((Component) this).get_transform().get_position(), ((Component) this._light).get_transform().get_rotation(), new Vector3(num2, num2, num2));
    this._material.SetMatrix("_WorldViewProj", Matrix4x4.op_Multiply(viewProj, matrix4x4_1));
    this._material.SetMatrix("_WorldView", Matrix4x4.op_Multiply(Camera.get_current().get_worldToCameraMatrix(), matrix4x4_1));
    if (this.Noise)
      this._material.EnableKeyword("NOISE");
    else
      this._material.DisableKeyword("NOISE");
    this._material.SetVector("_LightPos", new Vector4((float) ((Component) this._light).get_transform().get_position().x, (float) ((Component) this._light).get_transform().get_position().y, (float) ((Component) this._light).get_transform().get_position().z, (float) (1.0 / ((double) this._light.get_range() * (double) this._light.get_range()))));
    this._material.SetColor("_LightColor", Color.op_Multiply(this._light.get_color(), this._light.get_intensity()));
    if (Object.op_Equality((Object) this._light.get_cookie(), (Object) null))
    {
      this._material.EnableKeyword("POINT");
      this._material.DisableKeyword("POINT_COOKIE");
    }
    else
    {
      Matrix4x4 matrix4x4_2 = Matrix4x4.TRS(((Component) this._light).get_transform().get_position(), ((Component) this._light).get_transform().get_rotation(), Vector3.get_one());
      this._material.SetMatrix("_MyLightMatrix0", ((Matrix4x4) ref matrix4x4_2).get_inverse());
      this._material.EnableKeyword("POINT_COOKIE");
      this._material.DisableKeyword("POINT");
      this._material.SetTexture("_LightTexture0", this._light.get_cookie());
    }
    bool flag = false;
    Vector3 vector3 = Vector3.op_Subtraction(((Component) this._light).get_transform().get_position(), ((Component) Camera.get_current()).get_transform().get_position());
    if ((double) ((Vector3) ref vector3).get_magnitude() >= (double) QualitySettings.get_shadowDistance())
      flag = true;
    if (this._light.get_shadows() != null && !flag)
    {
      this._material.EnableKeyword("SHADOWS_CUBE");
      this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 1));
      this._commandBuffer.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture) renderer.GetVolumeLightBuffer()));
      this._commandBuffer.DrawMesh(pointLightMesh, matrix4x4_1, this._material, 0, num1);
      // ISSUE: reference to a compiler-generated field
      if (this.CustomRenderEvent == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
    }
    else
    {
      this._material.DisableKeyword("SHADOWS_CUBE");
      renderer.GlobalCommandBuffer.DrawMesh(pointLightMesh, matrix4x4_1, this._material, 0, num1);
      // ISSUE: reference to a compiler-generated field
      if (this.CustomRenderEvent == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
    }
  }

  private void SetupSpotLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
  {
    int num1 = 1;
    if (!this.IsCameraInSpotLightBounds())
      num1 = 3;
    Mesh spotLightMesh = VolumetricLightRenderer.GetSpotLightMesh();
    float range = this._light.get_range();
    float num2 = Mathf.Tan((float) (((double) this._light.get_spotAngle() + 1.0) * 0.5 * (Math.PI / 180.0))) * this._light.get_range();
    Matrix4x4 matrix4x4_1 = Matrix4x4.TRS(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), new Vector3(num2, num2, range));
    Matrix4x4 matrix4x4_2 = Matrix4x4.TRS(((Component) this._light).get_transform().get_position(), ((Component) this._light).get_transform().get_rotation(), Vector3.get_one());
    Matrix4x4 inverse = ((Matrix4x4) ref matrix4x4_2).get_inverse();
    this._material.SetMatrix("_MyLightMatrix0", Matrix4x4.op_Multiply(Matrix4x4.op_Multiply(Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.0f), Quaternion.get_identity(), new Vector3(-0.5f, -0.5f, 1f)), Matrix4x4.Perspective(this._light.get_spotAngle(), 1f, 0.0f, 1f)), inverse));
    this._material.SetMatrix("_WorldViewProj", Matrix4x4.op_Multiply(viewProj, matrix4x4_1));
    this._material.SetVector("_LightPos", new Vector4((float) ((Component) this._light).get_transform().get_position().x, (float) ((Component) this._light).get_transform().get_position().y, (float) ((Component) this._light).get_transform().get_position().z, (float) (1.0 / ((double) this._light.get_range() * (double) this._light.get_range()))));
    this._material.SetVector("_LightColor", Color.op_Implicit(Color.op_Multiply(this._light.get_color(), this._light.get_intensity())));
    Vector3 position = ((Component) this).get_transform().get_position();
    Vector3 forward = ((Component) this).get_transform().get_forward();
    this._material.SetFloat("_PlaneD", -Vector3.Dot(Vector3.op_Addition(position, Vector3.op_Multiply(forward, this._light.get_range())), forward));
    this._material.SetFloat("_CosAngle", Mathf.Cos((float) (((double) this._light.get_spotAngle() + 1.0) * 0.5 * (Math.PI / 180.0))));
    this._material.SetVector("_ConeApex", new Vector4((float) position.x, (float) position.y, (float) position.z));
    this._material.SetVector("_ConeAxis", new Vector4((float) forward.x, (float) forward.y, (float) forward.z));
    this._material.EnableKeyword("SPOT");
    if (this.Noise)
      this._material.EnableKeyword("NOISE");
    else
      this._material.DisableKeyword("NOISE");
    if (Object.op_Equality((Object) this._light.get_cookie(), (Object) null))
      this._material.SetTexture("_LightTexture0", VolumetricLightRenderer.GetDefaultSpotCookie());
    else
      this._material.SetTexture("_LightTexture0", this._light.get_cookie());
    bool flag = false;
    Vector3 vector3 = Vector3.op_Subtraction(((Component) this._light).get_transform().get_position(), ((Component) Camera.get_current()).get_transform().get_position());
    if ((double) ((Vector3) ref vector3).get_magnitude() >= (double) QualitySettings.get_shadowDistance())
      flag = true;
    if (this._light.get_shadows() != null && !flag)
    {
      Matrix4x4 matrix4x4_3 = Matrix4x4.op_Multiply(Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.get_identity(), new Vector3(0.5f, 0.5f, 0.5f)), !this._reversedZ ? Matrix4x4.Perspective(this._light.get_spotAngle(), 1f, this._light.get_shadowNearPlane(), this._light.get_range()) : Matrix4x4.Perspective(this._light.get_spotAngle(), 1f, this._light.get_range(), this._light.get_shadowNearPlane()));
      // ISSUE: variable of a reference type
      Matrix4x4& local1;
      ((Matrix4x4) (local1 = ref matrix4x4_3)).set_Item(0, 2, ((Matrix4x4) ref local1).get_Item(0, 2) * -1f);
      // ISSUE: variable of a reference type
      Matrix4x4& local2;
      ((Matrix4x4) (local2 = ref matrix4x4_3)).set_Item(1, 2, ((Matrix4x4) ref local2).get_Item(1, 2) * -1f);
      // ISSUE: variable of a reference type
      Matrix4x4& local3;
      ((Matrix4x4) (local3 = ref matrix4x4_3)).set_Item(2, 2, ((Matrix4x4) ref local3).get_Item(2, 2) * -1f);
      // ISSUE: variable of a reference type
      Matrix4x4& local4;
      ((Matrix4x4) (local4 = ref matrix4x4_3)).set_Item(3, 2, ((Matrix4x4) ref local4).get_Item(3, 2) * -1f);
      this._material.SetMatrix("_MyWorld2Shadow", Matrix4x4.op_Multiply(matrix4x4_3, inverse));
      this._material.SetMatrix("_WorldView", Matrix4x4.op_Multiply(matrix4x4_3, inverse));
      this._material.EnableKeyword("SHADOWS_DEPTH");
      this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 1));
      this._commandBuffer.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture) renderer.GetVolumeLightBuffer()));
      this._commandBuffer.DrawMesh(spotLightMesh, matrix4x4_1, this._material, 0, num1);
      // ISSUE: reference to a compiler-generated field
      if (this.CustomRenderEvent == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
    }
    else
    {
      this._material.DisableKeyword("SHADOWS_DEPTH");
      renderer.GlobalCommandBuffer.DrawMesh(spotLightMesh, matrix4x4_1, this._material, 0, num1);
      // ISSUE: reference to a compiler-generated field
      if (this.CustomRenderEvent == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
    }
  }

  private void SetupDirectionalLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
  {
    int num = 4;
    this._material.SetPass(num);
    if (this.Noise)
      this._material.EnableKeyword("NOISE");
    else
      this._material.DisableKeyword("NOISE");
    this._material.SetVector("_LightDir", new Vector4((float) ((Component) this._light).get_transform().get_forward().x, (float) ((Component) this._light).get_transform().get_forward().y, (float) ((Component) this._light).get_transform().get_forward().z, (float) (1.0 / ((double) this._light.get_range() * (double) this._light.get_range()))));
    this._material.SetVector("_LightColor", Color.op_Implicit(Color.op_Multiply(this._light.get_color(), this._light.get_intensity())));
    this._material.SetFloat("_MaxRayLength", this.MaxRayLength);
    if (Object.op_Equality((Object) this._light.get_cookie(), (Object) null))
    {
      this._material.EnableKeyword("DIRECTIONAL");
      this._material.DisableKeyword("DIRECTIONAL_COOKIE");
    }
    else
    {
      this._material.EnableKeyword("DIRECTIONAL_COOKIE");
      this._material.DisableKeyword("DIRECTIONAL");
      this._material.SetTexture("_LightTexture0", this._light.get_cookie());
    }
    this._frustumCorners[0] = Vector4.op_Implicit(Camera.get_current().ViewportToWorldPoint(new Vector3(0.0f, 0.0f, Camera.get_current().get_farClipPlane())));
    this._frustumCorners[2] = Vector4.op_Implicit(Camera.get_current().ViewportToWorldPoint(new Vector3(0.0f, 1f, Camera.get_current().get_farClipPlane())));
    this._frustumCorners[3] = Vector4.op_Implicit(Camera.get_current().ViewportToWorldPoint(new Vector3(1f, 1f, Camera.get_current().get_farClipPlane())));
    this._frustumCorners[1] = Vector4.op_Implicit(Camera.get_current().ViewportToWorldPoint(new Vector3(1f, 0.0f, Camera.get_current().get_farClipPlane())));
    this._material.SetVectorArray("_FrustumCorners", this._frustumCorners);
    Texture texture = (Texture) null;
    if (this._light.get_shadows() != null)
    {
      this._material.EnableKeyword("SHADOWS_DEPTH");
      this._commandBuffer.Blit(texture, RenderTargetIdentifier.op_Implicit((Texture) renderer.GetVolumeLightBuffer()), this._material, num);
      // ISSUE: reference to a compiler-generated field
      if (this.CustomRenderEvent == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
    }
    else
    {
      this._material.DisableKeyword("SHADOWS_DEPTH");
      renderer.GlobalCommandBuffer.Blit(texture, RenderTargetIdentifier.op_Implicit((Texture) renderer.GetVolumeLightBuffer()), this._material, num);
      // ISSUE: reference to a compiler-generated field
      if (this.CustomRenderEvent == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
    }
  }

  private bool IsCameraInPointLightBounds()
  {
    Vector3 vector3 = Vector3.op_Subtraction(((Component) this._light).get_transform().get_position(), ((Component) Camera.get_current()).get_transform().get_position());
    float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
    float num = this._light.get_range() + 1f;
    return (double) sqrMagnitude < (double) num * (double) num;
  }

  private bool IsCameraInSpotLightBounds()
  {
    if ((double) Vector3.Dot(((Component) this._light).get_transform().get_forward(), Vector3.op_Subtraction(((Component) Camera.get_current()).get_transform().get_position(), ((Component) this._light).get_transform().get_position())) > (double) (this._light.get_range() + 1f))
      return false;
    Vector3 forward = ((Component) this).get_transform().get_forward();
    Vector3 vector3 = Vector3.op_Subtraction(((Component) Camera.get_current()).get_transform().get_position(), ((Component) this._light).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Mathf.Acos(Vector3.Dot(forward, normalized)) * 57.2957801818848 <= ((double) this._light.get_spotAngle() + 3.0) * 0.5;
  }
}
