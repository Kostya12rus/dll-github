// Decompiled with JetBrains decompiler
// Type: MarkupImageRequest
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class MarkupImageRequest : MonoBehaviour
{
  public static List<MarkupImageRequest.CachedImage> cachedImages = new List<MarkupImageRequest.CachedImage>();
  public string[] allowedExtensions;
  public int maxSizeInBytes;
  public Texture errorTexture;
  public Image progressImage;
  public Image dimmerImage;
  private RawImage targetImage;

  public bool VerifyURL(string url)
  {
    if (url.StartsWith("https://"))
      url = url.Remove(0, 8);
    if (url.StartsWith("http://"))
      url = url.Remove(0, 7);
    if (!url.Contains("/"))
      return false;
    url = url.Remove(0, url.IndexOf("/"));
    int num = 0;
    foreach (char ch in url)
    {
      if (ch == '.')
        ++num;
    }
    if (num == 1)
    {
      foreach (string allowedExtension in this.allowedExtensions)
      {
        if (url.ToLower().EndsWith("." + allowedExtension.ToLower()))
          return true;
      }
    }
    return false;
  }

  public void DownloadImage(string url, Color c)
  {
    this.targetImage = this.GetComponent<RawImage>();
    foreach (MarkupImageRequest.CachedImage cachedImage in MarkupImageRequest.cachedImages)
    {
      if (cachedImage.url == url)
      {
        this.targetImage.color = c;
        this.targetImage.texture = cachedImage.texture;
        this.dimmerImage.enabled = false;
        this.progressImage.enabled = false;
        return;
      }
    }
    if (this.VerifyURL(url))
    {
      this.StopAllCoroutines();
      this.StartCoroutine(this.RequestImage(url, c));
    }
    else
    {
      this.targetImage.texture = this.errorTexture;
      this.dimmerImage.color = Color.clear;
      this.progressImage.enabled = false;
      Debug.Log((object) "Verification failed");
    }
  }

  [DebuggerHidden]
  private IEnumerator RequestImage(string url, Color col)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new MarkupImageRequest.\u003CRequestImage\u003Ec__Iterator0() { url = url, col = col, \u0024this = this };
  }

  [Serializable]
  public class CachedImage
  {
    public Texture texture;
    public string url;
  }
}
