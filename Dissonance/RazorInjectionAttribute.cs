// Decompiled with JetBrains decompiler
// Type: Dissonance.RazorInjectionAttribute
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace Dissonance
{
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  internal sealed class RazorInjectionAttribute : Attribute
  {
    public RazorInjectionAttribute([NotNull] string type, [NotNull] string fieldName)
    {
      this.Type = type;
      this.FieldName = fieldName;
    }

    [NotNull]
    public string Type { get; private set; }

    [NotNull]
    public string FieldName { get; private set; }
  }
}
