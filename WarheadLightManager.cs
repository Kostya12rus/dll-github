// Decompiled with JetBrains decompiler
// Type: WarheadLightManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

public class WarheadLightManager : MonoBehaviour
{
  public static WarheadLightManager singleton;
  private WarheadLight[] lightlist;
  public WarheadLightManager.MaterialColorChange[] materials;
  private bool prevStatus;

  public WarheadLightManager()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    WarheadLightManager.singleton = this;
    foreach (WarheadLightManager.MaterialColorChange material in this.materials)
      material.SetStatus(false);
  }

  public static void AddLight(WarheadLight l)
  {
    int length = WarheadLightManager.singleton.lightlist.Length;
    Array.Resize<WarheadLight>(ref WarheadLightManager.singleton.lightlist, length + 1);
    WarheadLightManager.singleton.lightlist[length] = l;
  }

  private void LateUpdate()
  {
    bool b = Object.op_Inequality((Object) AlphaWarheadController.host, (Object) null) && AlphaWarheadController.host.inProgress;
    if (this.prevStatus == b)
      return;
    this.prevStatus = b;
    foreach (WarheadLight warheadLight in this.lightlist)
    {
      if (b)
        warheadLight.WarheadEnable();
      else
        warheadLight.WarheadDisable();
    }
    foreach (WarheadLightManager.MaterialColorChange material in this.materials)
      material.SetStatus(b);
  }

  [Serializable]
  public class MaterialColorChange
  {
    public Material targetMaterial;
    public Color normalColor;
    public Color targetColor;
    public float multiplier;

    public void SetStatus(bool b)
    {
      this.targetMaterial.SetColor("_EmissionColor", Color.op_Multiply(!b ? this.normalColor : this.targetColor, this.multiplier));
    }
  }
}
