// Decompiled with JetBrains decompiler
// Type: IESLights.RuntimeIESImporter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;

namespace IESLights
{
  public class RuntimeIESImporter : MonoBehaviour
  {
    public static void Import(string path, out Texture2D spotlightCookie, out Cubemap pointLightCookie, int resolution = 128, bool enhancedImport = false, bool applyVignette = true)
    {
      spotlightCookie = (Texture2D) null;
      pointLightCookie = (Cubemap) null;
      if (!RuntimeIESImporter.IsFileValid(path))
        return;
      GameObject cubemapSphere;
      IESConverter iesConverter;
      RuntimeIESImporter.GetIESConverterAndCubeSphere(enhancedImport, resolution, out cubemapSphere, out iesConverter);
      RuntimeIESImporter.ImportIES(path, iesConverter, true, applyVignette, out spotlightCookie, out pointLightCookie);
      Object.Destroy((Object) cubemapSphere);
    }

    public static Texture2D ImportSpotlightCookie(string path, int resolution = 128, bool enhancedImport = false, bool applyVignette = true)
    {
      if (!RuntimeIESImporter.IsFileValid(path))
        return (Texture2D) null;
      GameObject cubemapSphere;
      IESConverter iesConverter;
      RuntimeIESImporter.GetIESConverterAndCubeSphere(enhancedImport, resolution, out cubemapSphere, out iesConverter);
      Texture2D spotlightCookie;
      Cubemap pointlightCookie;
      RuntimeIESImporter.ImportIES(path, iesConverter, true, applyVignette, out spotlightCookie, out pointlightCookie);
      Object.Destroy((Object) cubemapSphere);
      return spotlightCookie;
    }

    public static Cubemap ImportPointLightCookie(string path, int resolution = 128, bool enhancedImport = false)
    {
      if (!RuntimeIESImporter.IsFileValid(path))
        return (Cubemap) null;
      GameObject cubemapSphere;
      IESConverter iesConverter;
      RuntimeIESImporter.GetIESConverterAndCubeSphere(enhancedImport, resolution, out cubemapSphere, out iesConverter);
      Texture2D spotlightCookie;
      Cubemap pointlightCookie;
      RuntimeIESImporter.ImportIES(path, iesConverter, false, false, out spotlightCookie, out pointlightCookie);
      Object.Destroy((Object) cubemapSphere);
      return pointlightCookie;
    }

    private static void GetIESConverterAndCubeSphere(bool logarithmicNormalization, int resolution, out GameObject cubemapSphere, out IESConverter iesConverter)
    {
      Object original = Resources.Load("IES cubemap sphere");
      cubemapSphere = (GameObject) Object.Instantiate(original);
      iesConverter = cubemapSphere.GetComponent<IESConverter>();
      iesConverter.NormalizationMode = !logarithmicNormalization ? NormalizationMode.Linear : NormalizationMode.Logarithmic;
      iesConverter.Resolution = resolution;
    }

    private static void ImportIES(string path, IESConverter iesConverter, bool allowSpotlightCookies, bool applyVignette, out Texture2D spotlightCookie, out Cubemap pointlightCookie)
    {
      string targetFilename = (string) null;
      spotlightCookie = (Texture2D) null;
      pointlightCookie = (Cubemap) null;
      try
      {
        EXRData exrData;
        iesConverter.ConvertIES(path, string.Empty, allowSpotlightCookies, false, applyVignette, out pointlightCookie, out spotlightCookie, out exrData, out targetFilename);
      }
      catch (IESParseException ex)
      {
        Debug.LogError((object) string.Format("[IES] Encountered invalid IES data in {0}. Error message: {1}", (object) path, (object) ex.Message));
      }
      catch (Exception ex)
      {
        Debug.LogError((object) string.Format("[IES] Error while parsing {0}. Please contact me through the forums or thomasmountainborn.com. Error message: {1}", (object) path, (object) ex.Message));
      }
    }

    private static bool IsFileValid(string path)
    {
      if (!File.Exists(path))
      {
        Debug.LogWarningFormat("[IES] The file \"{0}\" does not exist.", (object) path);
        return false;
      }
      if (!(Path.GetExtension(path).ToLower() != ".ies"))
        return true;
      Debug.LogWarningFormat("[IES] The file \"{0}\" is not an IES file.", (object) path);
      return false;
    }
  }
}
