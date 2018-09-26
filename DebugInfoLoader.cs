// Decompiled with JetBrains decompiler
// Type: DebugInfoLoader
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugInfoLoader : MonoBehaviour
{
  public Text GPU;
  public Text GPUMemory;
  public Text ShaderLevel;
  public Text GraphicAPI;
  public Text Resolution;
  public Text Fullscreen;
  public Text CPU;
  public Text CPUThreadsAndFrequency;
  public Text RAM;
  public Text Audio;
  public Text OS;
  public Text Steam;
  public Text UnityVersion;
  public Text GameVersion;
  public Text Build;
  public Text GameLanguage;
  public Text GameScene;
  public Text Errors;

  public DebugInfoLoader()
  {
    base.\u002Ector();
  }

  private void OnEnable()
  {
    this.GPU.set_text(SystemInfo.get_graphicsDeviceName());
    this.GPUMemory.set_text("VRAM: " + (object) SystemInfo.get_graphicsMemorySize() + "MB");
    this.ShaderLevel.set_text("ShaderLevel " + SystemInfo.get_graphicsShaderLevel().ToString().Insert(1, "."));
    this.GraphicAPI.set_text(SystemInfo.get_graphicsDeviceType().ToString() + " " + SystemInfo.get_graphicsDeviceVersion());
    this.Resolution.set_text(Screen.get_width().ToString() + "x" + (object) Screen.get_height() + "  " + (object) Application.get_targetFrameRate());
    this.Fullscreen.set_text("Fullscreen: " + (object) ResolutionManager.fullscreen);
    this.CPU.set_text(SystemInfo.get_processorType());
    this.CPUThreadsAndFrequency.set_text("Threads: " + (object) SystemInfo.get_processorCount() + "   " + (object) SystemInfo.get_processorFrequency() + "MHz");
    this.RAM.set_text("RAM: " + (object) SystemInfo.get_systemMemorySize() + "MB");
    this.Audio.set_text("Audio Supported: " + (object) SystemInfo.get_supportsAudio());
    this.OS.set_text(SystemInfo.get_operatingSystem().Replace("  ", " "));
    this.Steam.set_text("Steam Loaded: " + (object) SteamManager.Initialized);
    this.UnityVersion.set_text("Unity " + Application.get_unityVersion());
    this.GameVersion.set_text("Version: " + ((CustomNetworkManager) Object.FindObjectOfType<CustomNetworkManager>()).CompatibleVersions[0]);
    string str1 = Application.get_buildGUID();
    if (string.IsNullOrEmpty(str1))
      str1 = "Unity Editor";
    this.Build.set_text("Build: " + str1);
    this.GameLanguage.set_text("Language:" + PlayerPrefs.GetString("translation_path", "English (default)"));
    Text gameScene = this.GameScene;
    string str2 = "Scene: ";
    Scene activeScene = SceneManager.GetActiveScene();
    string name = ((Scene) ref activeScene).get_name();
    string str3 = str2 + name;
    gameScene.set_text(str3);
    this.Errors.set_text("Asserts: " + (object) DebugScreenController.asserts + " Errors: " + (object) DebugScreenController.errors + " Exceptions: " + (object) DebugScreenController.exceptions);
  }
}
