using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using ProtoBuf;

namespace VotingFacility
{
    class Program
    {
        public static bool CheckUser(string name,string regID)
        {
            bool name_exists = false;
            bool id_exists = false;
            List<string> details = new List<string>();
            using (StreamReader sr = new StreamReader(@"E:\CS_Bing\Comp Security\E_Voting\VotingFacility\voterInfo.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    string st = sr.ReadLine();
                    string[] strArray;
                    strArray = st.Split(' ');
                    foreach(var item in strArray)
                    {
                        details.Add(item);
                    }
                }
            };
         
            foreach(var item in details)
            {
                if (item == name.Trim())
                    name_exists = true;
                if (item == regID.Trim())
                    id_exists = true;
            }
            if (name_exists && id_exists)
                return true;
            else
                return false;
        }

        public static byte[] GetEncryped(byte[] i)
        {
            byte[] e = new byte[128];
            for(int k= 0;k<128;k++)
            {
                e[k] = i[k];
            }
            return e;
        }

        public static byte[] GetSignature(byte[] i)
        {
            byte[] e = new byte[128];
            int j = 0;
            for (int k = 128; k < 256; k++)
            {
                e[j] = i[k];
                j++;
            }
            return e;
        }
    

        static void Main(string[] args)
        {
            Keys.GenerateKey();
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("keyExchangeClient", KeyExchange);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("tryvote", Check_Vote_History);
            NetworkComms.AppendGlobalIncomingPacketHandler<byte[]>("userinfo", CheckUser);
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0));
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
            Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);
            
        

            Console.WriteLine("\nPress any key to close server.");
            Console.ReadKey(true);

            //We have used NetworkComms so we should ensure that we correctly call shutdown
            NetworkComms.Shutdown();
        }

        private static void Check_Vote_History(PacketHeader header, Connection connection, string message)
        {
            List<string> details = new List<string>();
            bool vote_status = false;
            using (StreamReader sr = new StreamReader(@"E:\CS_Bing\Comp Security\E_Voting\VotingFacility\history.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    string st = sr.ReadLine();
                    string[] strArray;
                    strArray = st.Split(' ');
                    details.Add(strArray[0]);
                }
                if (details.Count == 0)
                    vote_status = true;
                foreach(var item in details)
                {
                    if (item.Trim() == message.Trim())
                        vote_status = true;
                }
            };

            if(vote_status)
                connection.SendObject("vote_status", "1");
            else
                connection.SendObject("vote_status", "0");

        }
        /// <summary>
        /// Writes the provided message to the console window
        /// </summary>
        /// <param name="header">The packet header associated with the incoming message</param>
        /// <param name="connection">The connection used by the incoming message</param>
        /// <param name="message">The message to be printed to the console</param>
        private static void CheckUser(PacketHeader header, Connection connection, byte[] message)
        {
            
               var decrypt = Keys.serverRSA.Decrypt(GetEncryped(message), false);
               var decryString = System.Text.Encoding.ASCII.GetString(decrypt);
               string [] delimiter1 = new string[] { "||" };  
               string[] spilt_encryted = decryString.Split(delimiter1,StringSplitOptions.None);
               string name = spilt_encryted[0];
               string regId = spilt_encryted[1];
                bool check_exists = false;
               bool verify = Keys.clientRSA.VerifyData(Encoding.ASCII.GetBytes(spilt_encryted[0]), new SHA1CryptoServiceProvider(), GetSignature(message));
                if(verify)
                {
                check_exists = CheckUser(name, regId);
                }
              
            if(check_exists)
            connection.SendObject("request_validity", "1");
            else
            connection.SendObject("request_validity", "0");


        }

        private static void KeyExchange(PacketHeader header, Connection connection, string message)
        {
            Keys.AddKey(message);
            var server_pub  = Keys.serverRSA.ToXmlString(false);
            connection.SendObject("keyExchangeServer", server_pub);
        }

    }
}