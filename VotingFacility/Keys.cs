using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace VotingFacility
{ 
   public static class Keys
    {
        public static RSACryptoServiceProvider serverRSA;
        public static RSACryptoServiceProvider clientRSA;
        public static RSAParameters private_server;
        public static RSAParameters public_client;
        public static RSAParameters public_server;

        public static void AddKey(string key)
        {
            
            RSACryptoServiceProvider rsa_p = new RSACryptoServiceProvider();
            rsa_p.FromXmlString(key);
            Keys.clientRSA = rsa_p;
            Keys.public_client = rsa_p.ExportParameters(false);


        }

        public static void GenerateKey()
        {
            RSACryptoServiceProvider rsa_s = new RSACryptoServiceProvider();
            rsa_s.ExportParameters(true);
            rsa_s.ExportParameters(false);
            Keys.serverRSA = rsa_s;
            Keys.public_server = rsa_s.ExportParameters(false);
            Keys.private_server = rsa_s.ExportParameters(true);

        }
    }

}
