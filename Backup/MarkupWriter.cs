// Decompiled with JetBrains decompiler
// Type: MarkupWriter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarkupWriter : MonoBehaviour
{
  public static MarkupWriter singleton;
  public GameObject sample;
  public List<string> errorLogs;
  private List<GameObject> spawnedElements;

  public MarkupWriter()
  {
    base.\u002Ector();
  }

  public static event MarkupWriter.OnCreateAction OnCreateObject;

  private void Awake()
  {
    MarkupWriter.singleton = this;
  }

  private void ClearAll()
  {
    using (List<GameObject>.Enumerator enumerator = this.spawnedElements.GetEnumerator())
    {
      while (enumerator.MoveNext())
        Object.Destroy((Object) enumerator.Current);
    }
    this.spawnedElements.Clear();
  }

  public void ReadTag(string input)
  {
    string str1 = input;
    char[] chArray = new char[1]{ ';' };
    foreach (string str2 in str1.Split(chArray))
    {
      if (str2.Contains(" "))
      {
        List<string> list = ((IEnumerable<string>) str2.Split(' ')).ToList<string>();
        for (int index = 0; index < 10; ++index)
          list.Add("empty");
        if (list[0].ToLower() == "clear")
        {
          this.ClearAll();
        }
        else
        {
          float result1;
          if (!float.TryParse(list[1], out result1))
            result1 = 0.0f;
          float result2;
          if (!float.TryParse(list[2], out result2))
            result2 = 0.0f;
          float result3;
          if (!float.TryParse(list[3], out result3))
            result3 = 100f;
          float result4;
          if (!float.TryParse(list[4], out result4))
            result4 = 100f;
          float result5;
          if (!float.TryParse(list[5], out result5))
            result5 = 0.0f;
          MarkupElement component = (MarkupElement) ((GameObject) Object.Instantiate<GameObject>((M0) this.sample, ((Component) MarkupCanvas.singleton).get_transform())).GetComponent<MarkupElement>();
          this.spawnedElements.Add(((Component) component).get_gameObject());
          component.markupStyle.position = Vector2.op_Implicit(new Vector3(result1, result2, 0.0f));
          component.markupStyle.size = new Vector2(result3, result4);
          component.markupStyle.rotation = result5;
          component.RefreshStyle(list[0]);
          // ISSUE: reference to a compiler-generated field
          if (MarkupWriter.OnCreateObject != null)
          {
            // ISSUE: reference to a compiler-generated field
            MarkupWriter.OnCreateObject(component);
          }
        }
      }
    }
  }

  public delegate void OnCreateAction(MarkupElement objectCreated);
}
