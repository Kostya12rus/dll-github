// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.UserLutComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public sealed class UserLutComponent : PostProcessingComponentRenderTexture<UserLutModel>
  {
    public override bool active
    {
      get
      {
        UserLutModel.Settings settings = this.model.settings;
        if (this.model.enabled && Object.op_Inequality((Object) settings.lut, (Object) null) && ((double) settings.contribution > 0.0 && ((Texture) settings.lut).get_height() == (int) Mathf.Sqrt((float) ((Texture) settings.lut).get_width())))
          return !this.context.interrupted;
        return false;
      }
    }

    public override void Prepare(Material uberMaterial)
    {
      UserLutModel.Settings settings = this.model.settings;
      uberMaterial.EnableKeyword("USER_LUT");
      uberMaterial.SetTexture(UserLutComponent.Uniforms._UserLut, (Texture) settings.lut);
      uberMaterial.SetVector(UserLutComponent.Uniforms._UserLut_Params, new Vector4(1f / (float) ((Texture) settings.lut).get_width(), 1f / (float) ((Texture) settings.lut).get_height(), (float) ((Texture) settings.lut).get_height() - 1f, settings.contribution));
    }

    public void OnGUI()
    {
      UserLutModel.Settings settings = this.model.settings;
      Rect rect;
      ref Rect local = ref rect;
      Rect viewport = this.context.viewport;
      double num1 = (double) ((Rect) ref viewport).get_x() * (double) Screen.get_width() + 8.0;
      double num2 = 8.0;
      double width = (double) ((Texture) settings.lut).get_width();
      double height = (double) ((Texture) settings.lut).get_height();
      ((Rect) ref local).\u002Ector((float) num1, (float) num2, (float) width, (float) height);
      GUI.DrawTexture(rect, (Texture) settings.lut);
    }

    private static class Uniforms
    {
      internal static readonly int _UserLut = Shader.PropertyToID(nameof (_UserLut));
      internal static readonly int _UserLut_Params = Shader.PropertyToID(nameof (_UserLut_Params));
    }
  }
}
