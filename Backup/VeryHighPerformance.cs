// Decompiled with JetBrains decompiler
// Type: VeryHighPerformance
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class VeryHighPerformance : MonoBehaviour
{
  public VeryHighPerformance()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (PlayerPrefs.GetInt("gfxsets_hp", 0) == 0)
      return;
    foreach (Component component in (Light[]) Object.FindObjectsOfType<Light>())
      Object.Destroy((Object) ((Component) component.get_transform()).get_gameObject());
    RenderSettings.set_ambientEquatorColor(new Color(0.5f, 0.5f, 0.5f));
    RenderSettings.set_ambientGroundColor(new Color(0.5f, 0.5f, 0.5f));
    RenderSettings.set_ambientSkyColor(new Color(0.5f, 0.5f, 0.5f));
  }
}
