// Decompiled with JetBrains decompiler
// Type: UBER_MaterialPresetCollection
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class UBER_MaterialPresetCollection : ScriptableObject
{
  [HideInInspector]
  [SerializeField]
  public string currentPresetName;
  [HideInInspector]
  [SerializeField]
  public UBER_PresetParamSection whatToRestore;
  [HideInInspector]
  [SerializeField]
  public UBER_MaterialPreset[] matPresets;
  [HideInInspector]
  [SerializeField]
  public string[] names;
}
