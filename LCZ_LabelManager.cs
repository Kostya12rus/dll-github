// Decompiled with JetBrains decompiler
// Type: LCZ_LabelManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LCZ_LabelManager : MonoBehaviour
{
  private List<LCZ_Label> labels;
  public LCZ_LabelManager.LCZ_Label_Preset[] chars;
  public Material[] numbers;
  private List<GameObject> rooms;

  public LCZ_LabelManager()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.labels = ((IEnumerable<LCZ_Label>) Object.FindObjectsOfType<LCZ_Label>()).ToList<LCZ_Label>();
    for (int index = 0; index < ((Component) this).get_transform().get_childCount(); ++index)
    {
      if (((Object) ((Component) this).get_transform().GetChild(index)).get_name().StartsWith("Root_"))
        this.rooms.Add(((Component) ((Component) this).get_transform().GetChild(index)).get_gameObject());
    }
  }

  public void RefreshLabels()
  {
    foreach (LCZ_Label label in this.labels)
    {
      bool flag = true;
      Vector3 vector3 = Vector3.op_Addition(((Component) label).get_transform().get_position(), Vector3.op_Multiply(((Component) label).get_transform().get_forward(), 10f));
      Debug.DrawLine(vector3, Vector3.op_Addition(vector3, Vector3.op_Multiply(Vector3.get_up(), 30f)), Color.get_red(), 20f);
      using (List<GameObject>.Enumerator enumerator = this.rooms.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          GameObject current = enumerator.Current;
          if ((double) Vector3.Distance(current.get_transform().get_position(), vector3) < 10.0)
          {
            int num = 0;
            foreach (LCZ_LabelManager.LCZ_Label_Preset lczLabelPreset in this.chars)
            {
              if (((Object) current).get_name().Contains(lczLabelPreset.nameToContain))
              {
                flag = false;
                int index = 0;
                if (((Object) current).get_name().Contains("("))
                {
                  try
                  {
                    string str = ((Object) current).get_name().Remove(0, ((Object) current).get_name().IndexOf('(') + 1);
                    index = int.Parse(str.Remove(str.IndexOf(')')));
                  }
                  catch
                  {
                  }
                }
                label.Refresh(lczLabelPreset.mat, this.numbers[index], num.ToString());
              }
            }
            ++num;
          }
        }
      }
      if (flag)
        label.Refresh(this.chars[0].mat, this.numbers[0], "none");
    }
  }

  [Serializable]
  public class LCZ_Label_Preset
  {
    public string nameToContain;
    public Material mat;
  }
}
