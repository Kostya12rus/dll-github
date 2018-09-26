// Decompiled with JetBrains decompiler
// Type: Cryptography.ECDSA
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Cryptography
{
  public class ECDSA
  {
    public static AsymmetricCipherKeyPair GenerateKeys(int size = 384)
    {
      ECKeyPairGenerator keyPairGenerator = new ECKeyPairGenerator();
      KeyGenerationParameters generationParameters = new KeyGenerationParameters(new SecureRandom(), size);
      keyPairGenerator.Init(generationParameters);
      return keyPairGenerator.GenerateKeyPair();
    }

    public static string Sign(string data, AsymmetricKeyParameter privKey)
    {
      return Convert.ToBase64String(ECDSA.SignBytes(data, privKey));
    }

    public static byte[] SignBytes(string data, AsymmetricKeyParameter privKey)
    {
      try
      {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(true, (ICipherParameters) privKey);
        signer.BlockUpdate(bytes, 0, data.Length);
        return signer.GenerateSignature();
      }
      catch
      {
        return (byte[]) null;
      }
    }

    public static bool Verify(string data, string signature, AsymmetricKeyParameter pubKey)
    {
      return ECDSA.VerifyBytes(data, Convert.FromBase64String(signature), pubKey);
    }

    public static bool VerifyBytes(string data, byte[] signature, AsymmetricKeyParameter pubKey)
    {
      try
      {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(false, (ICipherParameters) pubKey);
        signer.BlockUpdate(bytes, 0, data.Length);
        return signer.VerifySignature(signature);
      }
      catch (Exception ex)
      {
        GameConsole.Console.singleton.AddLog("ECDSA Verification Error (BouncyCastle): " + ex.Message + ", " + ex.StackTrace, Color32.op_Implicit(Color.get_red()), false);
        return false;
      }
    }

    public static AsymmetricKeyParameter PublicKeyFromString(string key)
    {
      return (AsymmetricKeyParameter) new PemReader((TextReader) new StringReader(key)).ReadObject();
    }

    public static string KeyToString(AsymmetricKeyParameter key)
    {
      TextWriter textWriter = (TextWriter) new StringWriter();
      PemWriter pemWriter = new PemWriter(textWriter);
      pemWriter.WriteObject((object) key);
      ((PemWriter) pemWriter).get_Writer().Flush();
      return textWriter.ToString();
    }
  }
}
