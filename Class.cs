// Decompiled with JetBrains decompiler
// Type: Class
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.PostProcessing;

[Serializable]
public class Class
{
  public string fullName = "Chaos Insurgency";
  public Color classColor = Color.white;
  [Space]
  public int[] ammoTypes = new int[3]{ 100, 100, 100 };
  public int maxHP = 100;
  public float walkSpeed = 5f;
  public float runSpeed = 7f;
  public float jumpSpeed = 7f;
  public float classRecoil = 1f;
  public int forcedCrosshair = -1;
  [Multiline]
  public string description;
  public Team team;
  public PostProcessingProfile postprocessingProfile;
  public GameObject model_player;
  public Offset model_offset;
  public GameObject model_ragdoll;
  public Offset ragdoll_offset;
  public int[] startItems;
  [Space]
  public AudioClip[] stepClips;
  public bool banClass;
  public float iconHeightOffset;
  public bool useHeadBob;
  public int bloodType;
}
