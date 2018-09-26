// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TMP_ExampleScript_01
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class TMP_ExampleScript_01 : MonoBehaviour
  {
    public TMP_ExampleScript_01.objectType ObjectType;
    public bool isStatic;
    private TMP_Text m_text;
    private const string k_label = "The count is <#0080ff>{0}</color>";
    private int count;

    private void Awake()
    {
      this.m_text = this.ObjectType != TMP_ExampleScript_01.objectType.TextMeshPro ? (TMP_Text) (this.GetComponent<TextMeshProUGUI>() ?? this.gameObject.AddComponent<TextMeshProUGUI>()) : (TMP_Text) (this.GetComponent<TextMeshPro>() ?? this.gameObject.AddComponent<TextMeshPro>());
      this.m_text.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Anton SDF");
      this.m_text.fontSharedMaterial = Resources.Load<Material>("Fonts & Materials/Anton SDF - Drop Shadow");
      this.m_text.fontSize = 120f;
      this.m_text.text = "A <#0080ff>simple</color> line of text.";
      Vector2 preferredValues = this.m_text.GetPreferredValues(float.PositiveInfinity, float.PositiveInfinity);
      this.m_text.rectTransform.sizeDelta = new Vector2(preferredValues.x, preferredValues.y);
    }

    private void Update()
    {
      if (this.isStatic)
        return;
      this.m_text.SetText("The count is <#0080ff>{0}</color>", (float) (this.count % 1000));
      ++this.count;
    }

    public enum objectType
    {
      TextMeshPro,
      TextMeshProUGUI,
    }
  }
}
