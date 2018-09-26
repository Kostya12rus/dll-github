// Decompiled with JetBrains decompiler
// Type: Dissonance.ValueProviderAttribute
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace Dissonance
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
  internal sealed class ValueProviderAttribute : Attribute
  {
    public ValueProviderAttribute([NotNull] string name)
    {
      this.Name = name;
    }

    [NotNull]
    public string Name { get; private set; }
  }
}
