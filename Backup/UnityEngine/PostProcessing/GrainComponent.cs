// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.GrainComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public sealed class GrainComponent : PostProcessingComponentRenderTexture<GrainModel>
  {
    private RenderTexture m_GrainLookupRT;

    public override bool active
    {
      get
      {
        if (this.model.enabled && ((double) this.model.settings.intensity > 0.0 && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat) 2)))
          return !this.context.interrupted;
        return false;
      }
    }

    public override void OnDisable()
    {
      GraphicsUtils.Destroy((Object) this.m_GrainLookupRT);
      this.m_GrainLookupRT = (RenderTexture) null;
    }

    public override void Prepare(Material uberMaterial)
    {
      GrainModel.Settings settings = this.model.settings;
      uberMaterial.EnableKeyword("GRAIN");
      float realtimeSinceStartup = Time.get_realtimeSinceStartup();
      float num1 = Random.get_value();
      float num2 = Random.get_value();
      if (Object.op_Equality((Object) this.m_GrainLookupRT, (Object) null) || !this.m_GrainLookupRT.IsCreated())
      {
        GraphicsUtils.Destroy((Object) this.m_GrainLookupRT);
        RenderTexture renderTexture = new RenderTexture(192, 192, 0, (RenderTextureFormat) 2);
        ((Texture) renderTexture).set_filterMode((FilterMode) 1);
        ((Texture) renderTexture).set_wrapMode((TextureWrapMode) 0);
        ((Texture) renderTexture).set_anisoLevel(0);
        ((Object) renderTexture).set_name("Grain Lookup Texture");
        this.m_GrainLookupRT = renderTexture;
        this.m_GrainLookupRT.Create();
      }
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Grain Generator");
      material.SetFloat(GrainComponent.Uniforms._Phase, realtimeSinceStartup / 20f);
      Graphics.Blit((Texture) null, this.m_GrainLookupRT, material, !settings.colored ? 0 : 1);
      uberMaterial.SetTexture(GrainComponent.Uniforms._GrainTex, (Texture) this.m_GrainLookupRT);
      uberMaterial.SetVector(GrainComponent.Uniforms._Grain_Params1, Vector4.op_Implicit(new Vector2(settings.luminanceContribution, settings.intensity * 20f)));
      uberMaterial.SetVector(GrainComponent.Uniforms._Grain_Params2, new Vector4((float) this.context.width / (float) ((Texture) this.m_GrainLookupRT).get_width() / settings.size, (float) this.context.height / (float) ((Texture) this.m_GrainLookupRT).get_height() / settings.size, num1, num2));
    }

    private static class Uniforms
    {
      internal static readonly int _Grain_Params1 = Shader.PropertyToID(nameof (_Grain_Params1));
      internal static readonly int _Grain_Params2 = Shader.PropertyToID(nameof (_Grain_Params2));
      internal static readonly int _GrainTex = Shader.PropertyToID(nameof (_GrainTex));
      internal static readonly int _Phase = Shader.PropertyToID(nameof (_Phase));
    }
  }
}
