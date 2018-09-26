// Decompiled with JetBrains decompiler
// Type: Dissonance.RazorDirectiveAttribute
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace Dissonance
{
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  internal sealed class RazorDirectiveAttribute : Attribute
  {
    public RazorDirectiveAttribute([NotNull] string directive)
    {
      this.Directive = directive;
    }

    [NotNull]
    public string Directive { get; private set; }
  }
}
