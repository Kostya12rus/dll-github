// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.MaterialFactory
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace UnityEngine.PostProcessing
{
  public sealed class MaterialFactory : IDisposable
  {
    private Dictionary<string, Material> m_Materials;

    public MaterialFactory()
    {
      this.m_Materials = new Dictionary<string, Material>();
    }

    public Material Get(string shaderName)
    {
      Material material1;
      if (!this.m_Materials.TryGetValue(shaderName, out material1))
      {
        Shader shader = Shader.Find(shaderName);
        if ((Object) shader == (Object) null)
          throw new ArgumentException(string.Format("Shader not found ({0})", (object) shaderName));
        Material material2 = new Material(shader);
        material2.name = string.Format("PostFX - {0}", (object) shaderName.Substring(shaderName.LastIndexOf("/") + 1));
        material2.hideFlags = HideFlags.DontSave;
        material1 = material2;
        this.m_Materials.Add(shaderName, material1);
      }
      return material1;
    }

    public void Dispose()
    {
      Dictionary<string, Material>.Enumerator enumerator = this.m_Materials.GetEnumerator();
      while (enumerator.MoveNext())
        GraphicsUtils.Destroy((Object) enumerator.Current.Value);
      this.m_Materials.Clear();
    }
  }
}
