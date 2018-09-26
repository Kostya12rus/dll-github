// Decompiled with JetBrains decompiler
// Type: ResolutionManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ResolutionManager : MonoBehaviour
{
  public static List<ResolutionManager.ResolutionPreset> presets = new List<ResolutionManager.ResolutionPreset>();
  public static int preset;
  public static bool fullscreen;

  public ResolutionManager()
  {
    base.\u002Ector();
  }

  private bool FindResolution(Resolution res)
  {
    foreach (ResolutionManager.ResolutionPreset preset in ResolutionManager.presets)
    {
      if (preset.height == ((Resolution) ref res).get_height() && preset.width == ((Resolution) ref res).get_width())
        return true;
    }
    return false;
  }

  private void Start()
  {
    ResolutionManager.presets.Clear();
    foreach (Resolution resolution in Screen.get_resolutions())
    {
      if (!this.FindResolution(resolution))
        ResolutionManager.presets.Add(new ResolutionManager.ResolutionPreset(resolution));
    }
    ResolutionManager.preset = Mathf.Clamp(PlayerPrefs.GetInt("SavedResolutionSet", ResolutionManager.presets.Count - 1), 0, ResolutionManager.presets.Count - 1);
    ResolutionManager.fullscreen = PlayerPrefs.GetInt("SavedFullscreen", 1) != 0;
    int num = PlayerPrefs.GetInt("MaxFramerate", 969);
    if (num == 969)
      Application.set_targetFrameRate(-1);
    else
      Application.set_targetFrameRate(num);
    ResolutionManager.RefreshScreen();
    // ISSUE: method pointer
    SceneManager.add_sceneLoaded(new UnityAction<Scene, LoadSceneMode>((object) this, __methodptr(OnSceneWasLoaded)));
  }

  private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
  {
    ResolutionManager.RefreshScreen();
  }

  public static void RefreshScreen()
  {
    ResolutionManager.presets[Mathf.Clamp(ResolutionManager.preset, 0, ResolutionManager.presets.Count - 1)].SetResolution();
    try
    {
      ((ResolutionText) Object.FindObjectOfType<ResolutionText>()).txt.set_text(ResolutionManager.presets[Mathf.Clamp(ResolutionManager.preset, 0, ResolutionManager.presets.Count - 1)].width.ToString() + " × " + (object) ResolutionManager.presets[Mathf.Clamp(ResolutionManager.preset, 0, ResolutionManager.presets.Count - 1)].height);
    }
    catch
    {
    }
  }

  public static void ChangeResolution(int id)
  {
    ResolutionManager.preset = Mathf.Clamp(ResolutionManager.preset + id, 0, ResolutionManager.presets.Count - 1);
    PlayerPrefs.SetInt("SavedResolutionSet", ResolutionManager.preset);
    ResolutionManager.RefreshScreen();
  }

  public static void ChangeFullscreen(bool isTrue)
  {
    ResolutionManager.fullscreen = isTrue;
    PlayerPrefs.SetInt("SavedFullscreen", !isTrue ? 0 : 1);
    ResolutionManager.RefreshScreen();
  }

  [Serializable]
  public class ResolutionPreset
  {
    public int width;
    public int height;

    public ResolutionPreset(Resolution template)
    {
      this.width = ((Resolution) ref template).get_width();
      this.height = ((Resolution) ref template).get_height();
    }

    public void SetResolution()
    {
      Screen.SetResolution(this.width, this.height, ResolutionManager.fullscreen);
    }
  }
}
