// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.DitheringComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public sealed class DitheringComponent : PostProcessingComponentRenderTexture<DitheringModel>
  {
    private Texture2D[] noiseTextures;
    private int textureIndex;
    private const int k_TextureCount = 64;

    public override bool active
    {
      get
      {
        if (this.model.enabled)
          return !this.context.interrupted;
        return false;
      }
    }

    public override void OnDisable()
    {
      this.noiseTextures = (Texture2D[]) null;
    }

    private void LoadNoiseTextures()
    {
      this.noiseTextures = new Texture2D[64];
      for (int index = 0; index < 64; ++index)
        this.noiseTextures[index] = (Texture2D) Resources.Load<Texture2D>("Bluenoise64/LDR_LLL1_" + (object) index);
    }

    public override void Prepare(Material uberMaterial)
    {
      if (++this.textureIndex >= 64)
        this.textureIndex = 0;
      float num1 = Random.get_value();
      float num2 = Random.get_value();
      if (this.noiseTextures == null)
        this.LoadNoiseTextures();
      Texture2D noiseTexture = this.noiseTextures[this.textureIndex];
      uberMaterial.EnableKeyword("DITHERING");
      uberMaterial.SetTexture(DitheringComponent.Uniforms._DitheringTex, (Texture) noiseTexture);
      uberMaterial.SetVector(DitheringComponent.Uniforms._DitheringCoords, new Vector4((float) this.context.width / (float) ((Texture) noiseTexture).get_width(), (float) this.context.height / (float) ((Texture) noiseTexture).get_height(), num1, num2));
    }

    private static class Uniforms
    {
      internal static readonly int _DitheringTex = Shader.PropertyToID(nameof (_DitheringTex));
      internal static readonly int _DitheringCoords = Shader.PropertyToID(nameof (_DitheringCoords));
    }
  }
}
