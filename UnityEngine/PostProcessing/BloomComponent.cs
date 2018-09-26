// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.BloomComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public sealed class BloomComponent : PostProcessingComponentRenderTexture<BloomModel>
  {
    private readonly RenderTexture[] m_BlurBuffer1 = new RenderTexture[16];
    private readonly RenderTexture[] m_BlurBuffer2 = new RenderTexture[16];
    private const int k_MaxPyramidBlurLevel = 16;

    public override bool active
    {
      get
      {
        if (this.model.enabled && (double) this.model.settings.bloom.intensity > 0.0)
          return !this.context.interrupted;
        return false;
      }
    }

    public void Prepare(RenderTexture source, Material uberMaterial, Texture autoExposure)
    {
      BloomModel.BloomSettings bloom = this.model.settings.bloom;
      BloomModel.LensDirtSettings lensDirt = this.model.settings.lensDirt;
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Bloom");
      material.set_shaderKeywords((string[]) null);
      material.SetTexture(BloomComponent.Uniforms._AutoExposure, autoExposure);
      int width = this.context.width / 2;
      int height = this.context.height / 2;
      RenderTextureFormat format = !Application.get_isMobilePlatform() ? (RenderTextureFormat) 9 : (RenderTextureFormat) 7;
      float num1 = (float) ((double) Mathf.Log((float) height, 2f) + (double) bloom.radius - 8.0);
      int num2 = (int) num1;
      int num3 = Mathf.Clamp(num2, 1, 16);
      float thresholdLinear = bloom.thresholdLinear;
      material.SetFloat(BloomComponent.Uniforms._Threshold, thresholdLinear);
      float num4 = (float) ((double) thresholdLinear * (double) bloom.softKnee + 9.99999974737875E-06);
      Vector3 vector3;
      ((Vector3) ref vector3).\u002Ector(thresholdLinear - num4, num4 * 2f, 0.25f / num4);
      material.SetVector(BloomComponent.Uniforms._Curve, Vector4.op_Implicit(vector3));
      material.SetFloat(BloomComponent.Uniforms._PrefilterOffs, !bloom.antiFlicker ? 0.0f : -0.5f);
      float num5 = 0.5f + num1 - (float) num2;
      material.SetFloat(BloomComponent.Uniforms._SampleScale, num5);
      if (bloom.antiFlicker)
        material.EnableKeyword("ANTI_FLICKER");
      RenderTexture rt = this.context.renderTextureFactory.Get(width, height, 0, format, (RenderTextureReadWrite) 0, (FilterMode) 1, (TextureWrapMode) 1, "FactoryTempTexture");
      Graphics.Blit((Texture) source, rt, material, 0);
      RenderTexture renderTexture1 = rt;
      for (int index = 0; index < num3; ++index)
      {
        this.m_BlurBuffer1[index] = this.context.renderTextureFactory.Get(((Texture) renderTexture1).get_width() / 2, ((Texture) renderTexture1).get_height() / 2, 0, format, (RenderTextureReadWrite) 0, (FilterMode) 1, (TextureWrapMode) 1, "FactoryTempTexture");
        int num6 = index != 0 ? 2 : 1;
        Graphics.Blit((Texture) renderTexture1, this.m_BlurBuffer1[index], material, num6);
        renderTexture1 = this.m_BlurBuffer1[index];
      }
      for (int index = num3 - 2; index >= 0; --index)
      {
        RenderTexture renderTexture2 = this.m_BlurBuffer1[index];
        material.SetTexture(BloomComponent.Uniforms._BaseTex, (Texture) renderTexture2);
        this.m_BlurBuffer2[index] = this.context.renderTextureFactory.Get(((Texture) renderTexture2).get_width(), ((Texture) renderTexture2).get_height(), 0, format, (RenderTextureReadWrite) 0, (FilterMode) 1, (TextureWrapMode) 1, "FactoryTempTexture");
        Graphics.Blit((Texture) renderTexture1, this.m_BlurBuffer2[index], material, 3);
        renderTexture1 = this.m_BlurBuffer2[index];
      }
      RenderTexture renderTexture3 = renderTexture1;
      for (int index = 0; index < 16; ++index)
      {
        if (Object.op_Inequality((Object) this.m_BlurBuffer1[index], (Object) null))
          this.context.renderTextureFactory.Release(this.m_BlurBuffer1[index]);
        if (Object.op_Inequality((Object) this.m_BlurBuffer2[index], (Object) null) && Object.op_Inequality((Object) this.m_BlurBuffer2[index], (Object) renderTexture3))
          this.context.renderTextureFactory.Release(this.m_BlurBuffer2[index]);
        this.m_BlurBuffer1[index] = (RenderTexture) null;
        this.m_BlurBuffer2[index] = (RenderTexture) null;
      }
      this.context.renderTextureFactory.Release(rt);
      uberMaterial.SetTexture(BloomComponent.Uniforms._BloomTex, (Texture) renderTexture3);
      uberMaterial.SetVector(BloomComponent.Uniforms._Bloom_Settings, Vector4.op_Implicit(new Vector2(num5, bloom.intensity)));
      if ((double) lensDirt.intensity > 0.0 && Object.op_Inequality((Object) lensDirt.texture, (Object) null))
      {
        uberMaterial.SetTexture(BloomComponent.Uniforms._Bloom_DirtTex, lensDirt.texture);
        uberMaterial.SetFloat(BloomComponent.Uniforms._Bloom_DirtIntensity, lensDirt.intensity);
        uberMaterial.EnableKeyword("BLOOM_LENS_DIRT");
      }
      else
        uberMaterial.EnableKeyword("BLOOM");
    }

    private static class Uniforms
    {
      internal static readonly int _AutoExposure = Shader.PropertyToID(nameof (_AutoExposure));
      internal static readonly int _Threshold = Shader.PropertyToID(nameof (_Threshold));
      internal static readonly int _Curve = Shader.PropertyToID(nameof (_Curve));
      internal static readonly int _PrefilterOffs = Shader.PropertyToID(nameof (_PrefilterOffs));
      internal static readonly int _SampleScale = Shader.PropertyToID(nameof (_SampleScale));
      internal static readonly int _BaseTex = Shader.PropertyToID(nameof (_BaseTex));
      internal static readonly int _BloomTex = Shader.PropertyToID(nameof (_BloomTex));
      internal static readonly int _Bloom_Settings = Shader.PropertyToID(nameof (_Bloom_Settings));
      internal static readonly int _Bloom_DirtTex = Shader.PropertyToID(nameof (_Bloom_DirtTex));
      internal static readonly int _Bloom_DirtIntensity = Shader.PropertyToID(nameof (_Bloom_DirtIntensity));
    }
  }
}
