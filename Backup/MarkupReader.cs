// Decompiled with JetBrains decompiler
// Type: MarkupReader
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

public class MarkupReader : MonoBehaviour
{
  public List<MarkupReader.TagStyleRelation> relations;
  public static MarkupReader singleton;

  public MarkupReader()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    MarkupReader.singleton = this;
  }

  public bool AddStyleFromURL(string url)
  {
    if (!this.VerifyURL(url))
      return false;
    Timing.RunCoroutine(this._DownloadStyle(url));
    return true;
  }

  [DebuggerHidden]
  private IEnumerator<float> _DownloadStyle(string url)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new MarkupReader.\u003C_DownloadStyle\u003Ec__Iterator0()
    {
      url = url,
      \u0024this = this
    };
  }

  private void LoadStyle(string _style, string _url)
  {
    string str1 = _style;
    for (int index1 = 0; index1 < 1000 && str1.Contains("<") && str1.Contains(">"); ++index1)
    {
      string str2 = str1;
      string str3 = str2.Remove(0, str2.IndexOf('<') + 1);
      string str4 = str3.Remove(str3.IndexOf('>'));
      string str5 = str3.Remove(0, str3.IndexOf('{') + 1);
      string str6 = str5.Remove(str5.IndexOf('}'));
      char[] chArray = new char[1]{ ';' };
      foreach (string str7 in str6.Split(chArray))
      {
        if (str7.Contains(":"))
        {
          string str8 = string.Empty;
          string str9 = string.Empty;
          bool flag = false;
          for (int index2 = 0; index2 < str7.Length; ++index2)
          {
            if (str7[index2] != ' ' && (str7[index2] != ':' || flag))
            {
              if (flag)
                str9 += (string) (object) str7[index2];
              else
                str8 += (string) (object) str7[index2];
            }
            if (str7[index2] == ':')
              flag = true;
          }
          if (str8.Contains(Environment.NewLine))
            str8 = str8.Remove(str8.IndexOf(Environment.NewLine), 1);
          if (str9.Contains(Environment.NewLine))
            str9 = str8.Remove(str8.IndexOf(Environment.NewLine), 1);
          this.OverrideTagStyle(str4.ToLower(), str8.ToLower(), str9, _url);
        }
      }
      str1 = str1.Remove(0, str1.IndexOf('}') + 1);
    }
  }

  private void OverrideTagStyle(string _tag, string _var, string _value, string _url)
  {
    int index1 = -1;
    for (int index2 = 0; index2 < this.relations.Count; ++index2)
    {
      if (this.relations[index2].tag == _tag)
        index1 = index2;
    }
    if (index1 == -1)
    {
      this.relations.Add(new MarkupReader.TagStyleRelation()
      {
        tag = _tag,
        style = new MarkupStyle(),
        sourceURL = string.Empty
      });
      index1 = this.relations.Count - 1;
    }
    this.relations[index1].sourceURL = _url;
    while (!Regex.IsMatch(_var[0].ToString(), "^[a-zA-Z]+$"))
      _var = _var.Remove(0, 1);
    if (_var == null)
      return;
    // ISSUE: reference to a compiler-generated field
    if (MarkupReader.\u003C\u003Ef__switch\u0024map5 == null)
    {
      // ISSUE: reference to a compiler-generated field
      MarkupReader.\u003C\u003Ef__switch\u0024map5 = new Dictionary<string, int>(13)
      {
        {
          "background-color",
          0
        },
        {
          "outline-color",
          1
        },
        {
          "text-color",
          2
        },
        {
          "text-outline-color",
          3
        },
        {
          "image-color",
          4
        },
        {
          "image-url",
          5
        },
        {
          "outline-size",
          6
        },
        {
          "text-outline-size",
          7
        },
        {
          "text-font-family",
          8
        },
        {
          "text-font-size",
          9
        },
        {
          "text-content",
          10
        },
        {
          "text-align",
          11
        },
        {
          "raycast-target",
          12
        }
      };
    }
    int num;
    // ISSUE: reference to a compiler-generated field
    if (!MarkupReader.\u003C\u003Ef__switch\u0024map5.TryGetValue(_var, out num))
      return;
    switch (num)
    {
      case 0:
        Color color1;
        if (!ColorUtility.TryParseHtmlString(_value, ref color1))
          break;
        this.relations[index1].style.mainColor = color1;
        break;
      case 1:
        Color color2;
        if (!ColorUtility.TryParseHtmlString(_value, ref color2))
          break;
        this.relations[index1].style.outlineColor = color2;
        break;
      case 2:
        Color color3;
        if (!ColorUtility.TryParseHtmlString(_value, ref color3))
          break;
        this.relations[index1].style.textColor = color3;
        break;
      case 3:
        Color color4;
        if (!ColorUtility.TryParseHtmlString(_value, ref color4))
          break;
        this.relations[index1].style.textOutlineColor = color4;
        break;
      case 4:
        Color color5;
        if (!ColorUtility.TryParseHtmlString(_value, ref color5))
          break;
        this.relations[index1].style.imageColor = color5;
        break;
      case 5:
        this.relations[index1].style.imageUrl = _value;
        break;
      case 6:
        float result1;
        if (!float.TryParse(_value, out result1))
          break;
        this.relations[index1].style.outlineSize = result1;
        break;
      case 7:
        float result2;
        if (!float.TryParse(_value, out result2))
          break;
        this.relations[index1].style.textOutlineSize = result2;
        break;
      case 8:
        int result3;
        if (!int.TryParse(_value, out result3))
          break;
        this.relations[index1].style.fontID = result3;
        break;
      case 9:
        int result4;
        if (!int.TryParse(_value, out result4))
          break;
        this.relations[index1].style.fontSize = result4;
        break;
      case 10:
        this.relations[index1].style.textContent = _value;
        break;
      case 11:
        foreach (string name in Enum.GetNames(typeof (TextAnchor)))
        {
          if (name.ToLower() == _value.ToLower())
            this.relations[index1].style.textAlignment = (TextAnchor) Enum.Parse(typeof (TextAnchor), _value, true);
        }
        break;
      case 12:
        this.relations[index1].style.raycast = _value.ToLower() == "true";
        break;
    }
  }

  public bool VerifyURL(string url)
  {
    if (url.StartsWith("https://"))
      url = url.Remove(0, 8);
    if (url.StartsWith("http://"))
      url = url.Remove(0, 7);
    if (!url.Contains("/"))
      return false;
    url = url.Remove(0, url.IndexOf("/"));
    int num = 0;
    foreach (char ch in url)
    {
      if (ch == '.')
        ++num;
    }
    return num == 1 && url.ToLower().EndsWith(".txt");
  }

  [Serializable]
  public class TagStyleRelation
  {
    public string tag;
    public MarkupStyle style;
    public string sourceURL;
  }
}
