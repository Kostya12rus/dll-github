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
  public Transform list_parent;
  public GameObject list_element;
  private List<GameObject> instances;
  private bool working;

  public ChangeKeyBinding()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.RefreshList();
  }

  private void RefreshList()
  {
    this.working = false;
    using (List<GameObject>.Enumerator enumerator = this.instances.GetEnumerator())
    {
      while (enumerator.MoveNext())
        Object.Destroy((Object) enumerator.Current);
    }
    NewInput.Load();
    for (int index = 0; index < ((List<NewInput.Bind>) NewInput.bindings).Count; ++index)
    {
      GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) this.list_element, this.list_parent);
      gameObject.get_transform().set_localScale(Vector3.get_one());
      gameObject.SetActive(true);
      this.instances.Add(gameObject);
      ((Text) gameObject.GetComponentInChildren<Text>()).set_text(((object) ((List<NewInput.Bind>) NewInput.bindings)[index].axis).ToString());
      Button componentInChildren = (Button) gameObject.GetComponentInChildren<Button>();
      ((Text) ((Component) componentInChildren).GetComponentInChildren<Text>()).set_text(((List<NewInput.Bind>) NewInput.bindings)[index].key.ToString());
      ((KeyBindElement) ((Component) componentInChildren).GetComponent<KeyBindElement>()).axis = (string) ((List<NewInput.Bind>) NewInput.bindings)[index].axis;
    }
  }

  [DebuggerHidden]
  private IEnumerator<float> _AwaitPress(string axis)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ChangeKeyBinding.\u003C_AwaitPress\u003Ec__Iterator0()
    {
      axis = axis,
      \u0024this = this
    };
  }

  public void ChangeKey(string axis)
  {
    Timing.RunCoroutine(this._AwaitPress(axis), (Segment) 1);
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
    return (KeyCode) 0;
  }
}
