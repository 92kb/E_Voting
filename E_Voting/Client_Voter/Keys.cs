using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace Client_Voter
{
   
    public static class Keys
        {
       // public static string RSApublic = @"<RSAKeyValue><Modulus>oqlLp+64oFJ/4dGhxqlMwQ/kR+cJkAr8PbFtCPdg0iqaf0raoIuAMdR+aABe2TbbJaDGr9YrQGl/AIzwXH/iMRKqFhodnqmDDEbi+Oyw2tyqHhBu+54ZruXeA8FSPWzdd9djTFwUQKS0M4bi76n/PlySMVvgk0Qg7yXbj7R8CnE=</Modulus><Exponent>AQAB</Exponent><P>yq0WOZLYoqzMmx24hrc5SPf7cLYnKQAvaH37HpHEqbAyqMsM2BQ3ZOh6nFYKOF/Xq3DcLDOWHpvbIavU9bTVFw==</P><Q>zXUT5amt0vGqvfuXC88ELOsL/2o9OTWJhWZ93bKsKAYjB9s6H28+VbEC4wP8bXyFX4vRTWQZn8LKx3zV0JZhtw==</Q><DP>J1fmQpLg/uMwbMQeN/iFZEbPRpf1jh39Ffmur8Z4OMB9dQrFmYSDJFGEy6hgH4VrZlpoQyRYdeSnayfiFThfTQ==</DP><DQ>BJ/E8d9OxTehMyNtc9uV6XjkzTvT4uy8ip8S6CF0VHZG5Y9ekISNb5pLSVa2oLQzwEHCVS6SkRDuRW0e1tH7ow==</DQ><InverseQ>pKVV3OASFDRmvj6uRHpTOogm/HJmoaodNKAtAT+lOPNKTgDv/xCnHigRs2vjHNHR0H368pEKlMMB1vzlqAx8uA==</InverseQ><D>baBdzfN02RBhAcewGVz2ztMwDkmmxz6wG8AddUKMLXjrIUlIqZT7NBo7i0pcolZ3QhfmcJGOGt4+6xcR07WYslkJjEBMPmyS7NCJ7ipHuEeRfC1UalbB339d3mnKUBBtQnYLRLwvXaKDmBTh7/GKtIKwmu8CIt1+QK+4/dOOcsU=</D></RSAKeyValue>";
       // public static string RSAprivate = @"<RSAKeyValue><Modulus>oqlLp+64oFJ/4dGhxqlMwQ/kR+cJkAr8PbFtCPdg0iqaf0raoIuAMdR+aABe2TbbJaDGr9YrQGl/AIzwXH/iMRKqFhodnqmDDEbi+Oyw2tyqHhBu+54ZruXeA8FSPWzdd9djTFwUQKS0M4bi76n/PlySMVvgk0Qg7yXbj7R8CnE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public static RSACryptoServiceProvider clientRSA;
        public static RSACryptoServiceProvider serverRSA;
        public static RSAParameters private_client;
        public static RSAParameters public_client;
        public static RSAParameters public_server;

        public static void AddKey(string key)
        {
            RSACryptoServiceProvider rsa_s = new RSACryptoServiceProvider();
            rsa_s.FromXmlString(key);
            Keys.serverRSA = rsa_s;
            Keys.public_server = rsa_s.ExportParameters(false);
        }

        public static void GenerateKey()
        {
            RSACryptoServiceProvider rsa_c = new RSACryptoServiceProvider();
            rsa_c.ExportParameters(true);
            rsa_c.ExportParameters(false);
            Keys.clientRSA = rsa_c;
            Keys.public_client = rsa_c.ExportParameters(false);
            Keys.private_client = rsa_c.ExportParameters(true);
        }
        // public const string RSApublic_Server = @"<RSAKeyValue><Modulus>oqlLp+64oFJ/4dGhxqlMwQ/kR+cJkAr8PbFtCPdg0iqaf0raoIuAMdR+aABe2TbbJaDGr9YrQGl/AIzwXH/iMRKqFhodnqmDDEbi+Oyw2tyqHhBu+54ZruXeA8FSPWzdd9djTFwUQKS0M4bi76n/PlySMVvgk0Qg7yXbj7R8CnE=</Modulus><Exponent>AQAB</Exponent><P>yq0WOZLYoqzMmx24hrc5SPf7cLYnKQAvaH37HpHEqbAyqMsM2BQ3ZOh6nFYKOF/Xq3DcLDOWHpvbIavU9bTVFw==</P><Q>zXUT5amt0vGqvfuXC88ELOsL/2o9OTWJhWZ93bKsKAYjB9s6H28+VbEC4wP8bXyFX4vRTWQZn8LKx3zV0JZhtw==</Q><DP>J1fmQpLg/uMwbMQeN/iFZEbPRpf1jh39Ffmur8Z4OMB9dQrFmYSDJFGEy6hgH4VrZlpoQyRYdeSnayfiFThfTQ==</DP><DQ>BJ/E8d9OxTehMyNtc9uV6XjkzTvT4uy8ip8S6CF0VHZG5Y9ekISNb5pLSVa2oLQzwEHCVS6SkRDuRW0e1tH7ow==</DQ><InverseQ>pKVV3OASFDRmvj6uRHpTOogm/HJmoaodNKAtAT+lOPNKTgDv/xCnHigRs2vjHNHR0H368pEKlMMB1vzlqAx8uA==</InverseQ><D>baBdzfN02RBhAcewGVz2ztMwDkmmxz6wG8AddUKMLXjrIUlIqZT7NBo7i0pcolZ3QhfmcJGOGt4+6xcR07WYslkJjEBMPmyS7NCJ7ipHuEeRfC1UalbB339d3mnKUBBtQnYLRLwvXaKDmBTh7/GKtIKwmu8CIt1+QK+4/dOOcsU=</D></RSAKeyValue>";


    }
    
}
