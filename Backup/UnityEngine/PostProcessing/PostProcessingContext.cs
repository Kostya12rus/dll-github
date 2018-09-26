// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.PostProcessingContext
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public class PostProcessingContext
  {
    public PostProcessingProfile profile;
    public Camera camera;
    public MaterialFactory materialFactory;
    public RenderTextureFactory renderTextureFactory;

    public bool interrupted { get; private set; }

    public void Interrupt()
    {
      this.interrupted = true;
    }

    public PostProcessingContext Reset()
    {
      this.profile = (PostProcessingProfile) null;
      this.camera = (Camera) null;
      this.materialFactory = (MaterialFactory) null;
      this.renderTextureFactory = (RenderTextureFactory) null;
      this.interrupted = false;
      return this;
    }

    public bool isGBufferAvailable
    {
      get
      {
        return this.camera.get_actualRenderingPath() == 3;
      }
    }

    public bool isHdr
    {
      get
      {
        return this.camera.get_allowHDR();
      }
    }

    public int width
    {
      get
      {
        return this.camera.get_pixelWidth();
      }
    }

    public int height
    {
      get
      {
        return this.camera.get_pixelHeight();
      }
    }

    public Rect viewport
    {
      get
      {
        return this.camera.get_rect();
      }
    }
  }
}
