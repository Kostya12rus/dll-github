// Decompiled with JetBrains decompiler
// Type: SpectatorInterface
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

public class SpectatorInterface : MonoBehaviour
{
  public GameObject rootPanel;
  public TextMeshProUGUI playerList;
  public TextMeshProUGUI playerInfo;
  public static SpectatorInterface singleton;

  public SpectatorInterface()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    SpectatorInterface.singleton = this;
  }
}
