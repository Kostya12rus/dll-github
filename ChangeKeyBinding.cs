// Decompiled with JetBrains decompiler
// Type: ChangeKeyBinding
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ChangeKeyBinding : MonoBehaviour
{
  private List<GameObject> instances = new List<GameObject>();
  public Transform list_parent;
  public GameObject list_element;
  private bool working;

  private void Start()
  {
    this.RefreshList();
  }

  private void RefreshList()
  {
    this.working = false;
    foreach (Object instance in this.instances)
      Object.Destroy(instance);
    NewInput.Load();
    for (int index = 0; index < NewInput.bindings.Count; ++index)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.list_element, this.list_parent);
      gameObject.transform.localScale = Vector3.one;
      gameObject.SetActive(true);
      this.instances.Add(gameObject);
      gameObject.GetComponentInChildren<Text>().text = NewInput.bindings[index].axis.ToString();
      Button componentInChildren = gameObject.GetComponentInChildren<Button>();
      componentInChildren.GetComponentInChildren<Text>().text = NewInput.bindings[index].key.ToString();
      componentInChildren.GetComponent<KeyBindElement>().axis = NewInput.bindings[index].axis;
    }
  }

  [DebuggerHidden]
  private IEnumerator<float> _AwaitPress(string axis)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ChangeKeyBinding.\u003C_AwaitPress\u003Ec__Iterator0() { axis = axis, \u0024this = this };
  }

  public void ChangeKey(string axis)
  {
    Timing.RunCoroutine(this._AwaitPress(axis), Segment.FixedUpdate);
  }

  public void Revent()
  {
    NewInput.Revent();
    this.RefreshList();
  }

  private KeyCode GetCurrentKey()
  {
    IEnumerator enumerator = Enum.GetValues(typeof (KeyCode)).GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
        if (Input.GetKey((KeyCode) current))
          return (KeyCode) current;
      }
    }
    finally
    {
      IDisposable disposable;
      if ((disposable = enumerator as IDisposable) != null)
        disposable.Dispose();
    }
    return KeyCode.None;
  }
}
