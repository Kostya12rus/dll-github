// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TMP_TextInfoDebugTool
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  [ExecuteInEditMode]
  public class TMP_TextInfoDebugTool : MonoBehaviour
  {
    public bool ShowCharacters;
    public bool ShowWords;
    public bool ShowLinks;
    public bool ShowLines;
    public bool ShowMeshBounds;
    public bool ShowTextBounds;
    [Space(10f)]
    [TextArea(2, 2)]
    public string ObjectStats;
    private TMP_Text m_TextComponent;
    private Transform m_Transform;
  }
}
