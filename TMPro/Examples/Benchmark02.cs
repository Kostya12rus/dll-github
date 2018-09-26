// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.Benchmark02
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class Benchmark02 : MonoBehaviour
  {
    public int NumberOfNPC = 12;
    public int SpawnType;
    private TextMeshProFloatingText floatingText_Script;

    private void Start()
    {
      for (int index = 0; index < this.NumberOfNPC; ++index)
      {
        if (this.SpawnType == 0)
        {
          GameObject gameObject = new GameObject();
          gameObject.transform.position = new Vector3(Random.Range(-95f, 95f), 0.25f, Random.Range(-95f, 95f));
          TextMeshPro textMeshPro = gameObject.AddComponent<TextMeshPro>();
          textMeshPro.autoSizeTextContainer = true;
          textMeshPro.rectTransform.pivot = new Vector2(0.5f, 0.0f);
          textMeshPro.alignment = TextAlignmentOptions.Bottom;
          textMeshPro.fontSize = 96f;
          textMeshPro.enableKerning = false;
          textMeshPro.color = (Color) new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue);
          textMeshPro.text = "!";
          this.floatingText_Script = gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 0;
        }
        else if (this.SpawnType == 1)
        {
          GameObject gameObject = new GameObject();
          gameObject.transform.position = new Vector3(Random.Range(-95f, 95f), 0.25f, Random.Range(-95f, 95f));
          TextMesh textMesh = gameObject.AddComponent<TextMesh>();
          textMesh.font = Resources.Load<Font>("Fonts/ARIAL");
          textMesh.GetComponent<Renderer>().sharedMaterial = textMesh.font.material;
          textMesh.anchor = TextAnchor.LowerCenter;
          textMesh.fontSize = 96;
          textMesh.color = (Color) new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue);
          textMesh.text = "!";
          this.floatingText_Script = gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 1;
        }
        else if (this.SpawnType == 2)
        {
          GameObject gameObject = new GameObject();
          gameObject.AddComponent<Canvas>().worldCamera = Camera.main;
          gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
          gameObject.transform.position = new Vector3(Random.Range(-95f, 95f), 5f, Random.Range(-95f, 95f));
          TextMeshProUGUI textMeshProUgui = new GameObject().AddComponent<TextMeshProUGUI>();
          textMeshProUgui.rectTransform.SetParent(gameObject.transform, false);
          textMeshProUgui.color = (Color) new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue);
          textMeshProUgui.alignment = TextAlignmentOptions.Bottom;
          textMeshProUgui.fontSize = 96f;
          textMeshProUgui.text = "!";
          this.floatingText_Script = gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 0;
        }
      }
    }
  }
}
