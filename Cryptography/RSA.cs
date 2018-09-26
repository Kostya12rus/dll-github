// Decompiled with JetBrains decompiler
// Type: Cryptography.RSA
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Text;

namespace Cryptography
{
  public class RSA
  {
    public static bool Verify(string data, string signature, string key)
    {
      AsymmetricKeyParameter asymmetricKeyParameter = (AsymmetricKeyParameter) new PemReader((TextReader) new StringReader(key)).ReadObject();
      ISigner signer = SignerUtilities.GetSigner("SHA256withRSA");
      signer.Init(false, (ICipherParameters) asymmetricKeyParameter);
      byte[] numArray = Convert.FromBase64String(signature);
      byte[] bytes = Encoding.UTF8.GetBytes(data);
      signer.BlockUpdate(bytes, 0, bytes.Length);
      return signer.VerifySignature(numArray);
    }
  }
}
