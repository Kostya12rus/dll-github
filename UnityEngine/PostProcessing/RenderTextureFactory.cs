// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.RenderTextureFactory
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace UnityEngine.PostProcessing
{
  public sealed class RenderTextureFactory : IDisposable
  {
    private HashSet<RenderTexture> m_TemporaryRTs;

    public RenderTextureFactory()
    {
      this.m_TemporaryRTs = new HashSet<RenderTexture>();
    }

    public RenderTexture Get(RenderTexture baseRenderTexture)
    {
      return this.Get(((Texture) baseRenderTexture).get_width(), ((Texture) baseRenderTexture).get_height(), baseRenderTexture.get_depth(), baseRenderTexture.get_format(), !baseRenderTexture.get_sRGB() ? (RenderTextureReadWrite) 1 : (RenderTextureReadWrite) 2, ((Texture) baseRenderTexture).get_filterMode(), ((Texture) baseRenderTexture).get_wrapMode(), "FactoryTempTexture");
    }

    public RenderTexture Get(int width, int height, int depthBuffer = 0, RenderTextureFormat format = 2, RenderTextureReadWrite rw = 0, FilterMode filterMode = 1, TextureWrapMode wrapMode = 1, string name = "FactoryTempTexture")
    {
      RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw);
      ((Texture) temporary).set_filterMode(filterMode);
      ((Texture) temporary).set_wrapMode(wrapMode);
      ((Object) temporary).set_name(name);
      this.m_TemporaryRTs.Add(temporary);
      return temporary;
    }

    public void Release(RenderTexture rt)
    {
      if (Object.op_Equality((Object) rt, (Object) null))
        return;
      if (!this.m_TemporaryRTs.Contains(rt))
        throw new ArgumentException(string.Format("Attempting to remove a RenderTexture that was not allocated: {0}", (object) rt));
      this.m_TemporaryRTs.Remove(rt);
      RenderTexture.ReleaseTemporary(rt);
    }

    public void ReleaseAll()
    {
      foreach (RenderTexture temporaryRt in this.m_TemporaryRTs)
        RenderTexture.ReleaseTemporary(temporaryRt);
      this.m_TemporaryRTs.Clear();
    }

    public void Dispose()
    {
      this.ReleaseAll();
    }
  }
}
