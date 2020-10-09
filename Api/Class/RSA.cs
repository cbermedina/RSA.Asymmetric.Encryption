using Api.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class RSA
    {
        private const string ContainerName = "ContainerPrueba";
        UnicodeEncoding ByteConverter = new UnicodeEncoding();

        public RSAParameters GenerateKey()
        {
            return SaveKeysKey();
        }


        private RSAParameters SaveKeysKey()
        {
            var folder = Path.Combine(PathConstants.PathKey, "tmp");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            RSAParameters key = new RSAParameters();

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                var publicKey = rsa.ExportParameters(false);
                var privateKey = rsa.ExportParameters(true);

                using (var str = new FileStream(Path.Combine(folder, "publicKey.xml"), FileMode.Create))
                {
                    var xml = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    xml.Serialize(str, publicKey);
                }

                using (var str = new FileStream(Path.Combine(folder, "privateKey.xml"), FileMode.Create))
                {
                    var xml = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    xml.Serialize(str, privateKey);
                }
            }

            return key;
        }

        public RSAParameters GetPublicKey()
        {
            var folder = Path.Combine(PathConstants.PathKey, "tmp");
            RSAParameters key = new RSAParameters();
            using (var str = new FileStream(Path.Combine(folder, "publicKey.xml"), FileMode.Open))
            {
                var xml1 = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                key = (RSAParameters)xml1.Deserialize(str);
            }

            return key;
        }

        public RSAParameters GetPrivateKey()
        {
            var folder = Path.Combine(PathConstants.PathKey, "tmp");
            RSAParameters key = new RSAParameters();
            using (var str = new FileStream(Path.Combine(folder, "privateKey.xml"), FileMode.Open))
            {
                var xml1 = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                key = (RSAParameters)xml1.Deserialize(str);
            }

            return key;
        }

        public byte[] RSAEncrypt(byte[] data, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;

                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048))
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    encryptedData = RSA.Encrypt(data, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public T Decrypt<T>(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            var a = RSADecrypt(DataToDecrypt, RSAKeyInfo, DoOAEPPadding);
            var b = ByteConverter.GetString(a);
            return JsonConvert.DeserializeObject<T>(b);
        }


        private byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048))
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

    }

    public enum EncryptType
    {
        RSA = 1,
        DSA = 13
    }
}
