// Decompiled with JetBrains decompiler
// Type: ClassPresetChooser
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassPresetChooser : MonoBehaviour
{
  public GameObject bottomMenuItem;
  public Transform bottomMenuHolder;
  public ClassPresetChooser.PickerPreset[] presets;
  private string curKey;
  private List<ClassPresetChooser.PickerPreset> curPresets;
  public Slider health;
  public Slider wSpeed;
  public Slider rSpeed;
  public RawImage[] startItems;
  public RawImage avatar;
  public TextMeshProUGUI addInfo;

  public ClassPresetChooser()
  {
    base.\u002Ector();
  }

  public void RefreshBottomItems(string key)
  {
    this.curKey = key;
    int num = 0;
    foreach (ClassPresetChooser.PickerPreset preset in this.presets)
    {
      if (preset.classID == key)
      {
        ++num;
        this.curPresets.Add(preset);
      }
    }
    IEnumerator enumerator = this.bottomMenuHolder.GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
        Object.Destroy((Object) ((Component) enumerator.Current).get_gameObject());
    }
    finally
    {
      IDisposable disposable;
      if ((disposable = enumerator as IDisposable) != null)
        disposable.Dispose();
    }
    for (int i = 0; i < num; ++i)
    {
      GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) this.bottomMenuItem, this.bottomMenuHolder);
      ((BottomPickerItem) gameObject.GetComponent<BottomPickerItem>()).SetupButton(key, i);
      ((Text) gameObject.GetComponentInChildren<Text>()).set_text("ABCDEFGHIJKL"[i].ToString());
    }
  }

  private void Update()
  {
    if (this.curPresets.Count <= 0)
      return;
    ClassPresetChooser.PickerPreset curPreset = this.curPresets[PlayerPrefs.GetInt(this.curKey, 0)];
    this.health.set_value((float) curPreset.health);
    this.wSpeed.set_value(curPreset.wSpeed);
    this.rSpeed.set_value(curPreset.rSpeed);
    this.avatar.set_texture(curPreset.icon);
    for (int index = 0; index < this.startItems.Length; ++index)
    {
      if (index >= curPreset.startingItems.Length)
      {
        ((Graphic) this.startItems[index]).set_color(Color.get_clear());
      }
      else
      {
        ((Graphic) this.startItems[index]).set_color(Color.get_white());
        this.startItems[index].set_texture(curPreset.startingItems[index]);
      }
    }
  }

  [Serializable]
  public class PickerPreset
  {
    public string classID;
    public Texture icon;
    public int health;
    public float wSpeed;
    public float rSpeed;
    public float stamina;
    public Texture[] startingItems;
    public string en_additionalInfo;
    public string pl_additionalInfo;
  }
}
