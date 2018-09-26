// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.GraphicsUtils
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public static class GraphicsUtils
  {
    private static Texture2D s_WhiteTexture;
    private static Mesh s_Quad;

    public static bool isLinearColorSpace
    {
      get
      {
        return QualitySettings.get_activeColorSpace() == 1;
      }
    }

    public static bool supportsDX11
    {
      get
      {
        if (SystemInfo.get_graphicsShaderLevel() >= 50)
          return SystemInfo.get_supportsComputeShaders();
        return false;
      }
    }

    public static Texture2D whiteTexture
    {
      get
      {
        if (Object.op_Inequality((Object) GraphicsUtils.s_WhiteTexture, (Object) null))
          return GraphicsUtils.s_WhiteTexture;
        GraphicsUtils.s_WhiteTexture = new Texture2D(1, 1, (TextureFormat) 5, false);
        GraphicsUtils.s_WhiteTexture.SetPixel(0, 0, new Color(1f, 1f, 1f, 1f));
        GraphicsUtils.s_WhiteTexture.Apply();
        return GraphicsUtils.s_WhiteTexture;
      }
    }

    public static Mesh quad
    {
      get
      {
        if (Object.op_Inequality((Object) GraphicsUtils.s_Quad, (Object) null))
          return GraphicsUtils.s_Quad;
        Vector3[] vector3Array = new Vector3[4]{ new Vector3(-1f, -1f, 0.0f), new Vector3(1f, 1f, 0.0f), new Vector3(1f, -1f, 0.0f), new Vector3(-1f, 1f, 0.0f) };
        Vector2[] vector2Array = new Vector2[4]{ new Vector2(0.0f, 0.0f), new Vector2(1f, 1f), new Vector2(1f, 0.0f), new Vector2(0.0f, 1f) };
        int[] numArray = new int[6]{ 0, 1, 2, 1, 0, 3 };
        Mesh mesh = new Mesh();
        mesh.set_vertices(vector3Array);
        mesh.set_uv(vector2Array);
        mesh.set_triangles(numArray);
        GraphicsUtils.s_Quad = mesh;
        GraphicsUtils.s_Quad.RecalculateNormals();
        GraphicsUtils.s_Quad.RecalculateBounds();
        return GraphicsUtils.s_Quad;
      }
    }

    public static void Blit(Material material, int pass)
    {
      GL.PushMatrix();
      GL.LoadOrtho();
      material.SetPass(pass);
      GL.Begin(5);
      GL.TexCoord2(0.0f, 0.0f);
      GL.Vertex3(0.0f, 0.0f, 0.1f);
      GL.TexCoord2(1f, 0.0f);
      GL.Vertex3(1f, 0.0f, 0.1f);
      GL.TexCoord2(0.0f, 1f);
      GL.Vertex3(0.0f, 1f, 0.1f);
      GL.TexCoord2(1f, 1f);
      GL.Vertex3(1f, 1f, 0.1f);
      GL.End();
      GL.PopMatrix();
    }

    public static void ClearAndBlit(Texture source, RenderTexture destination, Material material, int pass, bool clearColor = true, bool clearDepth = false)
    {
      RenderTexture active = RenderTexture.get_active();
      RenderTexture.set_active(destination);
      GL.Clear(false, clearColor, Color.get_clear());
      GL.PushMatrix();
      GL.LoadOrtho();
      material.SetTexture("_MainTex", source);
      material.SetPass(pass);
      GL.Begin(5);
      GL.TexCoord2(0.0f, 0.0f);
      GL.Vertex3(0.0f, 0.0f, 0.1f);
      GL.TexCoord2(1f, 0.0f);
      GL.Vertex3(1f, 0.0f, 0.1f);
      GL.TexCoord2(0.0f, 1f);
      GL.Vertex3(0.0f, 1f, 0.1f);
      GL.TexCoord2(1f, 1f);
      GL.Vertex3(1f, 1f, 0.1f);
      GL.End();
      GL.PopMatrix();
      RenderTexture.set_active(active);
    }

    public static void Destroy(Object obj)
    {
      if (!Object.op_Inequality(obj, (Object) null))
        return;
      Object.Destroy(obj);
    }

    public static void Dispose()
    {
      GraphicsUtils.Destroy((Object) GraphicsUtils.s_Quad);
    }
  }
}
