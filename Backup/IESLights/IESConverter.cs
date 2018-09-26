// Decompiled with JetBrains decompiler
// Type: IESLights.IESConverter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.IO;
using System.Linq;
using UnityEngine;

namespace IESLights
{
  [RequireComponent(typeof (IESToSpotlightCookie))]
  [RequireComponent(typeof (IESToCubemap))]
  public class IESConverter : MonoBehaviour
  {
    public int Resolution;
    public NormalizationMode NormalizationMode;
    private Texture2D _iesTexture;

    public IESConverter()
    {
      base.\u002Ector();
    }

    public void ConvertIES(string filePath, string targetPath, bool createSpotlightCookies, bool rawImport, bool applyVignette, out Cubemap pointLightCookie, out Texture2D spotlightCookie, out EXRData exrData, out string targetFilename)
    {
      IESData iesData = ParseIES.Parse(filePath, !rawImport ? this.NormalizationMode : NormalizationMode.Linear);
      this._iesTexture = IESToTexture.ConvertIesData(iesData);
      if (!rawImport)
      {
        exrData = new EXRData();
        this.RegularImport(filePath, targetPath, createSpotlightCookies, applyVignette, out pointLightCookie, out spotlightCookie, out targetFilename, iesData);
      }
      else
      {
        pointLightCookie = (Cubemap) null;
        spotlightCookie = (Texture2D) null;
        this.RawImport(iesData, filePath, targetPath, createSpotlightCookies, out exrData, out targetFilename);
      }
      if (!Object.op_Inequality((Object) this._iesTexture, (Object) null))
        return;
      Object.Destroy((Object) this._iesTexture);
    }

    private void RegularImport(string filePath, string targetPath, bool createSpotlightCookies, bool applyVignette, out Cubemap pointLightCookie, out Texture2D spotlightCookie, out string targetFilename, IESData iesData)
    {
      if (createSpotlightCookies && iesData.VerticalType != VerticalType.Full || iesData.PhotometricType == PhotometricType.TypeA)
      {
        pointLightCookie = (Cubemap) null;
        ((IESToSpotlightCookie) ((Component) this).GetComponent<IESToSpotlightCookie>()).CreateSpotlightCookie(this._iesTexture, iesData, this.Resolution, applyVignette, false, out spotlightCookie);
      }
      else
      {
        spotlightCookie = (Texture2D) null;
        ((IESToCubemap) ((Component) this).GetComponent<IESToCubemap>()).CreateCubemap(this._iesTexture, iesData, this.Resolution, out pointLightCookie);
      }
      this.BuildTargetFilename(Path.GetFileNameWithoutExtension(filePath), targetPath, Object.op_Inequality((Object) pointLightCookie, (Object) null), false, this.NormalizationMode, iesData, out targetFilename);
    }

    private void RawImport(IESData iesData, string filePath, string targetPath, bool createSpotlightCookie, out EXRData exrData, out string targetFilename)
    {
      if (createSpotlightCookie && iesData.VerticalType != VerticalType.Full || iesData.PhotometricType == PhotometricType.TypeA)
      {
        Texture2D cookie = (Texture2D) null;
        ((IESToSpotlightCookie) ((Component) this).GetComponent<IESToSpotlightCookie>()).CreateSpotlightCookie(this._iesTexture, iesData, this.Resolution, false, true, out cookie);
        exrData = new EXRData(cookie.GetPixels(), this.Resolution, this.Resolution);
        Object.DestroyImmediate((Object) cookie);
      }
      else
        exrData = new EXRData(((IESToCubemap) ((Component) this).GetComponent<IESToCubemap>()).CreateRawCubemap(this._iesTexture, iesData, this.Resolution), this.Resolution * 6, this.Resolution);
      this.BuildTargetFilename(Path.GetFileNameWithoutExtension(filePath), targetPath, false, true, NormalizationMode.Linear, iesData, out targetFilename);
    }

    private void BuildTargetFilename(string name, string folderHierarchy, bool isCubemap, bool isRaw, NormalizationMode normalizationMode, IESData iesData, out string targetFilePath)
    {
      if (!Directory.Exists(Path.Combine(Application.get_dataPath(), string.Format("IES/Imports/{0}", (object) folderHierarchy))))
        Directory.CreateDirectory(Path.Combine(Application.get_dataPath(), string.Format("IES/Imports/{0}", (object) folderHierarchy)));
      float num = 0.0f;
      if (iesData.PhotometricType == PhotometricType.TypeA)
        num = iesData.HorizontalAngles.Max() - iesData.HorizontalAngles.Min();
      string str1 = string.Empty;
      switch (normalizationMode)
      {
        case NormalizationMode.Logarithmic:
          str1 = "[E] ";
          break;
        case NormalizationMode.EqualizeHistogram:
          str1 = "[H] ";
          break;
      }
      string empty = string.Empty;
      string str2 = !isRaw ? (!isCubemap ? "asset" : "cubemap") : "exr";
      targetFilePath = Path.Combine(Path.Combine("Assets/IES/Imports/", folderHierarchy), string.Format("{0}{1}{2}.{3}", (object) str1, iesData.PhotometricType != PhotometricType.TypeA ? (object) string.Empty : (object) ("[FOV " + (object) num + "] "), (object) name, (object) str2));
      if (!File.Exists(targetFilePath))
        return;
      File.Delete(targetFilePath);
    }
  }
}
