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
      return this.Get(baseRenderTexture.width, baseRenderTexture.height, baseRenderTexture.depth, baseRenderTexture.format, !baseRenderTexture.sRGB ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB, baseRenderTexture.filterMode, baseRenderTexture.wrapMode, "FactoryTempTexture");
    }

    public RenderTexture Get(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, RenderTextureReadWrite rw = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "FactoryTempTexture")
    {
      RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw);
      temporary.filterMode = filterMode;
      temporary.wrapMode = wrapMode;
      temporary.name = name;
      this.m_TemporaryRTs.Add(temporary);
      return temporary;
    }

    public void Release(RenderTexture rt)
    {
      if ((Object) rt == (Object) null)
        return;
      if (!this.m_TemporaryRTs.Contains(rt))
        throw new ArgumentException(string.Format("Attempting to remove a RenderTexture that was not allocated: {0}", (object) rt));
      this.m_TemporaryRTs.Remove(rt);
      RenderTexture.ReleaseTemporary(rt);
    }

    public void ReleaseAll()
    {
      HashSet<RenderTexture>.Enumerator enumerator = this.m_TemporaryRTs.GetEnumerator();
      while (enumerator.MoveNext())
        RenderTexture.ReleaseTemporary(enumerator.Current);
      this.m_TemporaryRTs.Clear();
    }

    public void Dispose()
    {
      this.ReleaseAll();
    }
  }
}
