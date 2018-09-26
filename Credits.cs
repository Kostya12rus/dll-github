// Decompiled with JetBrains decompiler
// Type: Credits
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
  [Range(0.2f, 2.5f)]
  public float speed = 1f;
  private List<GameObject> spawnedLogs = new List<GameObject>();
  public Transform maskPosition;
  public Credits.CreditLogType[] logTypes;
  public Credits.CreditLog[] logQueue;

  private void SpawnType(Credits.CreditLogType l, string txt1, string txt2)
  {
    GameObject gameObject = Object.Instantiate<GameObject>(l.preset, this.maskPosition);
    Text[] componentsInChildren = gameObject.GetComponentsInChildren<Text>();
    componentsInChildren[0].text = txt1;
    if (componentsInChildren.Length > 1)
      componentsInChildren[1].text = txt2;
    Object.Destroy((Object) gameObject, 12f / this.speed);
    this.spawnedLogs.Add(gameObject);
    CreditText component = gameObject.GetComponent<CreditText>();
    component.move = true;
    component.speed *= this.speed;
  }

  private void OnEnable()
  {
    Timing.RunCoroutine(this._Play(), Segment.FixedUpdate);
  }

  private void OnDisable()
  {
    this.StopAllCoroutines();
    foreach (GameObject spawnedLog in this.spawnedLogs)
    {
      if ((Object) spawnedLog != (Object) null)
        Object.Destroy((Object) spawnedLog);
    }
    this.spawnedLogs.Clear();
  }

  [DebuggerHidden]
  private IEnumerator<float> _Play()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Credits.\u003C_Play\u003Ec__Iterator0() { \u0024this = this };
  }

  [Serializable]
  public class CreditLogType
  {
    public float preTime;
    public float postTime;
    public GameObject preset;
  }

  [Serializable]
  public class CreditLog
  {
    public string text1_en;
    public string text2_en;
    public int type;
  }
}
