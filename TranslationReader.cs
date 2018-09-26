// Decompiled with JetBrains decompiler
// Type: TranslationReader
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TranslationReader : MonoBehaviour
{
  public List<TranslationReader.TranslatedElement> elements = new List<TranslationReader.TranslatedElement>();
  public static TranslationReader singleton;
  public static string path;

  private void Awake()
  {
    TranslationReader.singleton = this;
  }

  private void Start()
  {
    NewInput.Load();
    this.Refresh();
    SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneWasLoaded);
  }

  private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
  {
    if (scene.buildIndex != 0)
      return;
    this.Refresh();
  }

  public static string Get(string n, int v)
  {
    try
    {
      if (n.Contains("."))
        n = n.Remove(46);
    }
    catch
    {
    }
    try
    {
      foreach (TranslationReader.TranslatedElement element in TranslationReader.singleton.elements)
      {
        if (element.fileName == n)
          return element.values[v].Replace("\\n", Environment.NewLine);
      }
      return "NO_TRANSLATION";
    }
    catch
    {
      return "TRANSLATION_ERROR";
    }
  }

  private void Refresh()
  {
    TranslationReader.path = "Translations/" + PlayerPrefs.GetString("translation_path", "English (default)");
    if (!Directory.Exists(TranslationReader.path))
      TranslationReader.path = "Translations/English (default)";
    TranslationReader.singleton.elements.Clear();
    foreach (string file in Directory.GetFiles(TranslationReader.path))
    {
      string path = file.Replace("\\", "/");
      try
      {
        StreamReader streamReader = new StreamReader(path);
        string end = streamReader.ReadToEnd();
        streamReader.Close();
        string str = path.Remove(0, path.LastIndexOf("/") + 1);
        TranslationReader.TranslatedElement translatedElement = new TranslationReader.TranslatedElement() { fileName = str.Remove(str.IndexOf('.')), values = end.Split(new string[1]{ Environment.NewLine }, StringSplitOptions.None) };
        TranslationReader.singleton.elements.Add(translatedElement);
      }
      catch
      {
      }
    }
    SimpleMenu.LoadCorrectScene();
  }

  [Serializable]
  public class TranslatedElement
  {
    public string fileName;
    public string[] values;
  }
}
