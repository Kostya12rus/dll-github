﻿// Decompiled with JetBrains decompiler
// Type: SECTR_PriorityQueue`1
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class SECTR_PriorityQueue<T> where T : IComparable<T>
{
  private List<T> data;

  public SECTR_PriorityQueue()
  {
    this.data = new List<T>(64);
  }

  public SECTR_PriorityQueue(int capacity)
  {
    this.data = new List<T>(capacity);
  }

  public int Count
  {
    get
    {
      return this.data.Count;
    }
    set
    {
    }
  }

  public T this[int index]
  {
    get
    {
      if (index < this.data.Count)
        return this.data[index];
      return default (T);
    }
    set
    {
      if (index >= this.data.Count)
        return;
      this.data[index] = value;
      this._Update(index);
    }
  }

  public void Enqueue(T item)
  {
    this.data.Add(item);
    int j;
    for (int i = this.data.Count - 1; i > 0; i = j)
    {
      j = (i - 1) / 2;
      if (this.data[i].CompareTo(this.data[j]) >= 0)
        break;
      this._SwapElements(i, j);
    }
  }

  public T Dequeue()
  {
    int index1 = this.data.Count - 1;
    T obj = this.data[0];
    this.data[0] = this.data[index1];
    this.data.RemoveAt(index1);
    int num = index1 - 1;
    int i = 0;
    while (true)
    {
      int j = i * 2 + 1;
      if (j <= num)
      {
        int index2 = j + 1;
        if (index2 <= num && this.data[index2].CompareTo(this.data[j]) < 0)
          j = index2;
        if (this.data[i].CompareTo(this.data[j]) > 0)
        {
          this._SwapElements(i, j);
          i = j;
        }
        else
          break;
      }
      else
        break;
    }
    return obj;
  }

  public T Peek()
  {
    if (this.data.Count > 0)
      return this.data[0];
    return default (T);
  }

  public override string ToString()
  {
    string str = string.Empty;
    for (int index = 0; index < this.data.Count; ++index)
      str = str + this.data[index].ToString() + " ";
    return str + "count = " + (object) this.data.Count;
  }

  public bool IsConsistent()
  {
    if (this.data.Count > 0)
    {
      int num = this.data.Count - 1;
      for (int index1 = 0; index1 < this.data.Count; ++index1)
      {
        int index2 = 2 * index1 + 1;
        int index3 = 2 * index1 + 2;
        if (index2 <= num && this.data[index1].CompareTo(this.data[index2]) > 0 || index3 <= num && this.data[index1].CompareTo(this.data[index3]) > 0)
          return false;
      }
    }
    return true;
  }

  public void Clear()
  {
    this.data.Clear();
  }

  private void _SwapElements(int i, int j)
  {
    T obj = this.data[i];
    this.data[i] = this.data[j];
    this.data[j] = obj;
  }

  private void _Update(int i)
  {
    int i1;
    int j1;
    for (i1 = i; i1 > 0; i1 = j1)
    {
      j1 = (i1 - 1) / 2;
      if (this.data[i1].CompareTo(this.data[j1]) < 0)
        this._SwapElements(i1, j1);
      else
        break;
    }
    if (i1 < i)
      return;
    while (true)
    {
      int j2 = i1;
      int i2 = 2 * i1 + 1;
      int i3 = 2 * i1 + 2;
      if (this.data.Count > i2 && this.data[i1].CompareTo(this.data[i2]) > 0)
      {
        this._SwapElements(i2, j2);
        i1 = i2;
      }
      else if (this.data.Count > i3 && this.data[i1].CompareTo(this.data[i3]) > 0)
      {
        this._SwapElements(i3, j2);
        i1 = i3;
      }
      else
        break;
    }
  }
}
