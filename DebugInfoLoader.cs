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

  private void OnEnable()
  {
    this.GPU.text = SystemInfo.graphicsDeviceName;
    this.GPUMemory.text = "VRAM: " + (object) SystemInfo.graphicsMemorySize + "MB";
    this.ShaderLevel.text = "ShaderLevel " + SystemInfo.graphicsShaderLevel.ToString().Insert(1, ".");
    this.GraphicAPI.text = ((int) SystemInfo.graphicsDeviceType).ToString() + " " + SystemInfo.graphicsDeviceVersion;
    this.Resolution.text = Screen.width.ToString() + "x" + (object) Screen.height + "  " + (object) Application.targetFrameRate;
    this.Fullscreen.text = "Fullscreen: " + (object) ResolutionManager.fullscreen;
    this.CPU.text = SystemInfo.processorType;
    this.CPUThreadsAndFrequency.text = "Threads: " + (object) SystemInfo.processorCount + "   " + (object) SystemInfo.processorFrequency + "MHz";
    this.RAM.text = "RAM: " + (object) SystemInfo.systemMemorySize + "MB";
    this.Audio.text = "Audio Supported: " + (object) SystemInfo.supportsAudio;
    this.OS.text = SystemInfo.operatingSystem.Replace("  ", " ");
    this.Steam.text = "Steam Loaded: " + (object) SteamManager.Initialized;
    this.UnityVersion.text = "Unity " + Application.unityVersion;
    this.GameVersion.text = "Version: " + Object.FindObjectOfType<CustomNetworkManager>().CompatibleVersions[0];
    string str = Application.buildGUID;
    if (string.IsNullOrEmpty(str))
      str = "Unity Editor";
    this.Build.text = "Build: " + str;
    this.GameLanguage.text = "Language:" + PlayerPrefs.GetString("translation_path", "English (default)");
    this.GameScene.text = "Scene: " + SceneManager.GetActiveScene().name;
    this.Errors.text = "Asserts: " + (object) DebugScreenController.asserts + " Errors: " + (object) DebugScreenController.errors + " Exceptions: " + (object) DebugScreenController.exceptions;
  }
}
