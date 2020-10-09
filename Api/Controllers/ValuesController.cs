using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GenerateKey()
        {
            RSA rsa = new RSA();
            return Ok(rsa.GenerateKey());
        }

        [HttpGet]
        public IActionResult GetKey()
        {
            RSA rsa = new RSA();
            return Ok(rsa.GetPublicKey());
        }

        [HttpGet]
        public IActionResult GetPrivateKey()
        {
            RSA rsa = new RSA();
            return Ok(rsa.GetPrivateKey());
        }

        [HttpGet]
        public void EncryptData([FromBody] byte[] value)
        {
            //JsonConvert.SerializeObject(data)
            //RSA rsa = new RSA();
            //return Ok(rsa.RSAEncrypt(value));
        }

        [HttpPost]
        public IActionResult DecryptData(Parameters value)
        {
            //var val = value.ToObject<byte[]>();
            var val = Convert.FromBase64String(value.DataEncrypter);

            RSA rsa = new RSA();
            var privateKey = rsa.GetPrivateKey();
            return Ok(rsa.Decrypt<Message>(val, privateKey, false));
        }

        public class Parameters
        {
            public string DataEncrypter { get; set; }
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
}
