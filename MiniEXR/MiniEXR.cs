// Decompiled with JetBrains decompiler
// Type: MiniEXR.MiniEXR
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;

namespace MiniEXR
{
  public static class MiniEXR
  {
    public static void MiniEXRWrite(string _filePath, uint _width, uint _height, uint _channels, float[] _rgbaArray)
    {
      File.WriteAllBytes(_filePath, MiniEXR.MiniEXR.MiniEXRWrite(_width, _height, _channels, _rgbaArray));
    }

    public static void MiniEXRWrite(string _filePath, uint _width, uint _height, Color[] _colorArray)
    {
      File.WriteAllBytes(_filePath, MiniEXR.MiniEXR.MiniEXRWrite(_width, _height, _colorArray));
    }

    public static byte[] MiniEXRWrite(uint _width, uint _height, Color[] _colorArray)
    {
      float[] _rgbaArray = new float[_colorArray.Length * 3];
      for (int index = 0; index < _colorArray.Length; ++index)
      {
        _rgbaArray[index * 3] = (float) _colorArray[index].r;
        _rgbaArray[index * 3 + 1] = (float) _colorArray[index].g;
        _rgbaArray[index * 3 + 2] = (float) _colorArray[index].b;
      }
      return MiniEXR.MiniEXR.MiniEXRWrite(_width, _height, 3U, _rgbaArray);
    }

    public static byte[] MiniEXRWrite(uint _width, uint _height, uint _channels, float[] _rgbaArray)
    {
      uint num1 = _width - 1U;
      uint num2 = _height - 1U;
      byte[] numArray1 = new byte[313]{ (byte) 118, (byte) 47, (byte) 49, (byte) 1, (byte) 2, (byte) 0, (byte) 0, (byte) 0, (byte) 99, (byte) 104, (byte) 97, (byte) 110, (byte) 110, (byte) 101, (byte) 108, (byte) 115, (byte) 0, (byte) 99, (byte) 104, (byte) 108, (byte) 105, (byte) 115, (byte) 116, (byte) 0, (byte) 55, (byte) 0, (byte) 0, (byte) 0, (byte) 66, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 71, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 82, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 99, (byte) 111, (byte) 109, (byte) 112, (byte) 114, (byte) 101, (byte) 115, (byte) 115, (byte) 105, (byte) 111, (byte) 110, (byte) 0, (byte) 99, (byte) 111, (byte) 109, (byte) 112, (byte) 114, (byte) 101, (byte) 115, (byte) 115, (byte) 105, (byte) 111, (byte) 110, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 100, (byte) 97, (byte) 116, (byte) 97, (byte) 87, (byte) 105, (byte) 110, (byte) 100, (byte) 111, (byte) 119, (byte) 0, (byte) 98, (byte) 111, (byte) 120, (byte) 50, (byte) 105, (byte) 0, (byte) 16, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) (num1 & (uint) byte.MaxValue), (byte) (num1 >> 8 & (uint) byte.MaxValue), (byte) (num1 >> 16 & (uint) byte.MaxValue), (byte) (num1 >> 24 & (uint) byte.MaxValue), (byte) (num2 & (uint) byte.MaxValue), (byte) (num2 >> 8 & (uint) byte.MaxValue), (byte) (num2 >> 16 & (uint) byte.MaxValue), (byte) (num2 >> 24 & (uint) byte.MaxValue), (byte) 100, (byte) 105, (byte) 115, (byte) 112, (byte) 108, (byte) 97, (byte) 121, (byte) 87, (byte) 105, (byte) 110, (byte) 100, (byte) 111, (byte) 119, (byte) 0, (byte) 98, (byte) 111, (byte) 120, (byte) 50, (byte) 105, (byte) 0, (byte) 16, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) (num1 & (uint) byte.MaxValue), (byte) (num1 >> 8 & (uint) byte.MaxValue), (byte) (num1 >> 16 & (uint) byte.MaxValue), (byte) (num1 >> 24 & (uint) byte.MaxValue), (byte) (num2 & (uint) byte.MaxValue), (byte) (num2 >> 8 & (uint) byte.MaxValue), (byte) (num2 >> 16 & (uint) byte.MaxValue), (byte) (num2 >> 24 & (uint) byte.MaxValue), (byte) 108, (byte) 105, (byte) 110, (byte) 101, (byte) 79, (byte) 114, (byte) 100, (byte) 101, (byte) 114, (byte) 0, (byte) 108, (byte) 105, (byte) 110, (byte) 101, (byte) 79, (byte) 114, (byte) 100, (byte) 101, (byte) 114, (byte) 0, (byte) 1, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 112, (byte) 105, (byte) 120, (byte) 101, (byte) 108, (byte) 65, (byte) 115, (byte) 112, (byte) 101, (byte) 99, (byte) 116, (byte) 82, (byte) 97, (byte) 116, (byte) 105, (byte) 111, (byte) 0, (byte) 102, (byte) 108, (byte) 111, (byte) 97, (byte) 116, (byte) 0, (byte) 4, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 128, (byte) 63, (byte) 115, (byte) 99, (byte) 114, (byte) 101, (byte) 101, (byte) 110, (byte) 87, (byte) 105, (byte) 110, (byte) 100, (byte) 111, (byte) 119, (byte) 67, (byte) 101, (byte) 110, (byte) 116, (byte) 101, (byte) 114, (byte) 0, (byte) 118, (byte) 50, (byte) 102, (byte) 0, (byte) 8, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 115, (byte) 99, (byte) 114, (byte) 101, (byte) 101, (byte) 110, (byte) 87, (byte) 105, (byte) 110, (byte) 100, (byte) 111, (byte) 119, (byte) 87, (byte) 105, (byte) 100, (byte) 116, (byte) 104, (byte) 0, (byte) 102, (byte) 108, (byte) 111, (byte) 97, (byte) 116, (byte) 0, (byte) 4, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 128, (byte) 63, (byte) 0 };
      uint length = (uint) numArray1.Length;
      uint num3 = 8U * _height;
      uint num4 = (uint) ((int) _width * 3 * 2);
      uint num5 = num4 + 8U;
      byte[] numArray2 = new byte[(IntPtr) (uint) ((int) length + (int) num3 + (int) _height * (int) num5)];
      int index1 = 0;
      for (int index2 = 0; (long) index2 < (long) length; ++index2)
      {
        numArray2[index1] = numArray1[index2];
        ++index1;
      }
      uint num6 = length + num3;
      for (int index2 = 0; (long) index2 < (long) _height; ++index2)
      {
        byte[] numArray3 = numArray2;
        int index3 = index1;
        int num7 = index3 + 1;
        int num8 = (int) (byte) (num6 & (uint) byte.MaxValue);
        numArray3[index3] = (byte) num8;
        byte[] numArray4 = numArray2;
        int index4 = num7;
        int num9 = index4 + 1;
        int num10 = (int) (byte) (num6 >> 8 & (uint) byte.MaxValue);
        numArray4[index4] = (byte) num10;
        byte[] numArray5 = numArray2;
        int index5 = num9;
        int num11 = index5 + 1;
        int num12 = (int) (byte) (num6 >> 16 & (uint) byte.MaxValue);
        numArray5[index5] = (byte) num12;
        byte[] numArray6 = numArray2;
        int index6 = num11;
        int num13 = index6 + 1;
        int num14 = (int) (byte) (num6 >> 24 & (uint) byte.MaxValue);
        numArray6[index6] = (byte) num14;
        byte[] numArray7 = numArray2;
        int index7 = num13;
        int num15 = index7 + 1;
        int num16 = 0;
        numArray7[index7] = (byte) num16;
        byte[] numArray8 = numArray2;
        int index8 = num15;
        int num17 = index8 + 1;
        int num18 = 0;
        numArray8[index8] = (byte) num18;
        byte[] numArray9 = numArray2;
        int index9 = num17;
        int num19 = index9 + 1;
        int num20 = 0;
        numArray9[index9] = (byte) num20;
        byte[] numArray10 = numArray2;
        int index10 = num19;
        index1 = index10 + 1;
        int num21 = 0;
        numArray10[index10] = (byte) num21;
        num6 += num5;
      }
      ushort[] numArray11 = new ushort[_rgbaArray.Length];
      for (int index2 = 0; index2 < _rgbaArray.Length; ++index2)
      {
        _rgbaArray[index2] = Mathf.Pow(_rgbaArray[index2], 2.2f);
        numArray11[index2] = HalfHelper.SingleToHalf(_rgbaArray[index2]);
      }
      uint num22 = 0;
      for (int index2 = 0; (long) index2 < (long) _height; ++index2)
      {
        byte[] numArray3 = numArray2;
        int index3 = index1;
        int num7 = index3 + 1;
        int num8 = (int) (byte) (index2 & (int) byte.MaxValue);
        numArray3[index3] = (byte) num8;
        byte[] numArray4 = numArray2;
        int index4 = num7;
        int num9 = index4 + 1;
        int num10 = (int) (byte) (index2 >> 8 & (int) byte.MaxValue);
        numArray4[index4] = (byte) num10;
        byte[] numArray5 = numArray2;
        int index5 = num9;
        int num11 = index5 + 1;
        int num12 = (int) (byte) (index2 >> 16 & (int) byte.MaxValue);
        numArray5[index5] = (byte) num12;
        byte[] numArray6 = numArray2;
        int index6 = num11;
        int num13 = index6 + 1;
        int num14 = (int) (byte) (index2 >> 24 & (int) byte.MaxValue);
        numArray6[index6] = (byte) num14;
        byte[] numArray7 = numArray2;
        int index7 = num13;
        int num15 = index7 + 1;
        int num16 = (int) (byte) (num4 & (uint) byte.MaxValue);
        numArray7[index7] = (byte) num16;
        byte[] numArray8 = numArray2;
        int index8 = num15;
        int num17 = index8 + 1;
        int num18 = (int) (byte) (num4 >> 8 & (uint) byte.MaxValue);
        numArray8[index8] = (byte) num18;
        byte[] numArray9 = numArray2;
        int index9 = num17;
        int num19 = index9 + 1;
        int num20 = (int) (byte) (num4 >> 16 & (uint) byte.MaxValue);
        numArray9[index9] = (byte) num20;
        byte[] numArray10 = numArray2;
        int index10 = num19;
        index1 = index10 + 1;
        int num21 = (int) (byte) (num4 >> 24 & (uint) byte.MaxValue);
        numArray10[index10] = (byte) num21;
        uint num23 = num22;
        for (int index11 = 0; (long) index11 < (long) _width; ++index11)
        {
          byte[] bytes = BitConverter.GetBytes(numArray11[(IntPtr) (num23 + 2U)]);
          byte[] numArray12 = numArray2;
          int index12 = index1;
          int num24 = index12 + 1;
          int num25 = (int) bytes[0];
          numArray12[index12] = (byte) num25;
          byte[] numArray13 = numArray2;
          int index13 = num24;
          index1 = index13 + 1;
          int num26 = (int) bytes[1];
          numArray13[index13] = (byte) num26;
          num23 += _channels;
        }
        uint num27 = num22;
        for (int index11 = 0; (long) index11 < (long) _width; ++index11)
        {
          byte[] bytes = BitConverter.GetBytes(numArray11[(IntPtr) (num27 + 1U)]);
          byte[] numArray12 = numArray2;
          int index12 = index1;
          int num24 = index12 + 1;
          int num25 = (int) bytes[0];
          numArray12[index12] = (byte) num25;
          byte[] numArray13 = numArray2;
          int index13 = num24;
          index1 = index13 + 1;
          int num26 = (int) bytes[1];
          numArray13[index13] = (byte) num26;
          num27 += _channels;
        }
        uint num28 = num22;
        for (int index11 = 0; (long) index11 < (long) _width; ++index11)
        {
          byte[] bytes = BitConverter.GetBytes(numArray11[(IntPtr) num28]);
          byte[] numArray12 = numArray2;
          int index12 = index1;
          int num24 = index12 + 1;
          int num25 = (int) bytes[0];
          numArray12[index12] = (byte) num25;
          byte[] numArray13 = numArray2;
          int index13 = num24;
          index1 = index13 + 1;
          int num26 = (int) bytes[1];
          numArray13[index13] = (byte) num26;
          num28 += _channels;
        }
        num22 += _width * _channels;
      }
      return numArray2;
    }
  }
}
