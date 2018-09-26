// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.PostProcessingComponent`1
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public abstract class PostProcessingComponent<T> : PostProcessingComponentBase where T : PostProcessingModel
  {
    public T model { get; internal set; }

    public virtual void Init(PostProcessingContext pcontext, T pmodel)
    {
      this.context = pcontext;
      this.model = pmodel;
    }

    public override PostProcessingModel GetModel()
    {
      return (PostProcessingModel) this.model;
    }
  }
}
