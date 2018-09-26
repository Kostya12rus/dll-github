// Decompiled with JetBrains decompiler
// Type: ParticleMenu
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ParticleMenu : MonoBehaviour
{
  public ParticleExamples[] particleSystems;
  public GameObject gunGameObject;
  private int currentIndex;
  private GameObject currentGO;
  public Transform spawnLocation;
  public Text title;
  public Text description;
  public Text navigationDetails;

  private void Start()
  {
    this.Navigate(0);
    this.currentIndex = 0;
  }

  public void Navigate(int i)
  {
    this.currentIndex = (this.particleSystems.Length + this.currentIndex + i) % this.particleSystems.Length;
    if ((Object) this.currentGO != (Object) null)
      Object.Destroy((Object) this.currentGO);
    this.currentGO = Object.Instantiate<GameObject>(this.particleSystems[this.currentIndex].particleSystemGO, this.spawnLocation.position + this.particleSystems[this.currentIndex].particlePosition, Quaternion.Euler(this.particleSystems[this.currentIndex].particleRotation));
    this.gunGameObject.SetActive(this.particleSystems[this.currentIndex].isWeaponEffect);
    this.title.text = this.particleSystems[this.currentIndex].title;
    this.description.text = this.particleSystems[this.currentIndex].description;
    this.navigationDetails.text = string.Empty + (object) (this.currentIndex + 1) + " out of " + this.particleSystems.Length.ToString();
  }
}
