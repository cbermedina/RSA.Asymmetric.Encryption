using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClienteA
{
    class Program
    {
        private const string url = "https://localhost:44363/";
        

        static string ruta = @"C:\Users\cbermudez\source\repos\Api\tmp";
        async static Task Main(string[] args)
        {
            Message message = new Message
            {
                Campo1 = 1,
                Campo2 = 2,
                Campo3 = 3,
                Campo4 = 4,
                Campo5 = 5,
                Campo6 = 6
            };

            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            RSA rsa = new RSA();

            // se obtiene la llave publica
            var publicKey = await GetRequest<RSAParameters>($"{url}", "values", "GetKey");

            // Se realiza encriptación
            var encryptData = rsa.RSAEncrypt(ByteConverter.GetBytes(JsonConvert.SerializeObject(message)), publicKey, false);

            var param = new Parameters
            {
                DataEncrypter = Convert.ToBase64String(encryptData)
            };

            var decryptData2 = await PostRequest<Message>($"{url}", "values", "DecryptData", param);

        }

        
        async static Task<T> GetRequest<T>(string url, string controlador, string accion, string data = "")
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();

                var result = await client.GetAsync($"{controlador}/{accion}{(!string.IsNullOrEmpty(data) ? "?value="+data: "")}");
                return await result.Content.ReadAsAsync<T>();
            }
        }

        async static Task<T> PostRequest<T>(string url, string controlador, string accion, Parameters data)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            
            var contenidoHttp = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();

                var result = await client.PostAsync($"{controlador}/{accion}", contenidoHttp);
                return await result.Content.ReadAsAsync<T>();
            }
        }

        // Esta clase se realizo para pruebas dentro del mismo proyecto
        public class RSA
        {
            string folder = @"C:\Users\cbermudez\source\repos\Api\tmp";

            public RSAParameters GenerateKey()
            {
                return SaveKeysKey();
            }


            private RSAParameters SaveKeysKey()
            {
                var folder = ruta;
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                RSAParameters key = new RSAParameters();

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
                {
                    key = rsa.ExportParameters(false);
                    var privateKey = rsa.ExportParameters(true);
                    using (var str = new FileStream(Path.Combine(folder, $"publicKey.xml"), FileMode.Create))
                    {
                        var xml = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                        xml.Serialize(str, key);
                    }

                    using (var str = new FileStream(Path.Combine(folder, "privateKey.xml"), FileMode.Create))
                    {
                        var xml = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                        xml.Serialize(str, privateKey);
                    }
                }

                return key;
            }

            public RSAParameters GenerateOtherPublicKey()
            {
                var folder = ruta;
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                RSAParameters key = new RSAParameters();

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
                {
                    //rsa.ImportParameters(GetPrivateKey());
                    var publicKey = rsa.ExportParameters(false);
                    using (var str = new FileStream(Path.Combine(folder, $"publicKey.xml"), FileMode.Create))
                    {
                        var xml = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                        xml.Serialize(str, publicKey);
                    }
                }

                return key;
            }

            public RSAParameters GetPublicKey()
            {
                RSAParameters key = new RSAParameters();
                using (var str = new FileStream(Path.Combine(folder, $"publicKey.xml"), FileMode.Open))
                {
                    var xml1 = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    key = (RSAParameters)xml1.Deserialize(str);
                }

                return key;
            }

            public RSAParameters GetPrivateKey()
            {
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
                    byte[] encryptedData;// = ByteConverter.GetBytes(data);

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


            public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
            {
                try
                {
                    byte[] decryptedData;// = ByteConverter.GetBytes(DataToDecrypt);
                    using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048))
                    {
                        RSA.ImportParameters(RSAKeyInfo);
                        decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                    }
                    return decryptedData;//JsonConvert.DeserializeObject<T>(ByteConverter.GetString(decryptedData));
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.ToString());
                    return null;
                }
            }

        }

    }

    class Key
    {
        public RSAParameters Private { get; set; }
        public RSAParameters Public { get; set; }
    }

    class Message
    {
        public int Campo1 { get; set; }
        public int Campo2 { get; set; }
        public int Campo3 { get; set; }
        public int Campo4 { get; set; }
        public int Campo5 { get; set; }
        public int Campo6 { get; set; }
    }
}
