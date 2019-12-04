
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using System.IO;


namespace Client_Voter
{



    class Program
    {

        public static byte[] AddSignature(byte[] e, byte[] s)
        {
            byte[] signedEncryp = new byte[256];
            for (int i = 0; i < 128; i++)
            {
                signedEncryp[i] = e[i];
            }
            int j = 0;
            for (int i = 128; i < 256; i++)
            {
                signedEncryp[i] = s[j];
                j++;
            }
            return signedEncryp;
        }

        public static string  DisplayOption_GetInput(string name)
        {
            string choice;
            Console.WriteLine(" Welcome, " + name + "\n Main Menu \n  Please enter a number (1-4) \n  1. Vote  \n 2. My vote history \n 3. Election result \n 4. Quit  ");
            choice = Console.ReadLine();
            return choice;
        }

      

        static void Main(string[] args)
        {
            Keys.GenerateKey();
           
            Console.WriteLine("Please enter the server IP and port in the format 192.168.0.1:10000 and press return:");
            string serverInfo = Console.ReadLine();
           
            var client_pub_string = Keys.clientRSA.ToXmlString(false);

            string serverIP = serverInfo.Split(':').First();
            int serverPort = int.Parse(serverInfo.Split(':').Last());
            var server_public_key = NetworkComms.SendReceiveObject<string,string>("keyExchangeClient", serverIP, serverPort, "keyExchangeServer", 1000000, client_pub_string);
            Keys.AddKey(server_public_key);

          //  Console.WriteLine(server_public_key);
           // NetworkComms.SendObject("ClientPublicKey", serverIP, serverPort, clientpubKey);

            Console.WriteLine("Enter Name");
            string name = Console.ReadLine();
            Console.WriteLine("Enter RegisterId");
            string regId = Console.ReadLine();
         
            var encryp = Keys.serverRSA.Encrypt(Encoding.ASCII.GetBytes(name + "|| " + regId ),false);
            
           var signedText = Keys.clientRSA.SignData(Encoding.ASCII.GetBytes(name), new SHA1CryptoServiceProvider());
            var signedEncryptedDataBytes = AddSignature(encryp, signedText);
   

            
          string user_option = "-1";
          string tryVote = "-1";
          string voteChoice = string.Empty;
         

            string authentication = NetworkComms.SendReceiveObject<byte[], string>("userinfo", serverIP, serverPort, "request_validity", 1000000, signedEncryptedDataBytes);
            if (authentication == "0")
                Console.WriteLine("Invalid User");
            else
            {
                do
                {
                    user_option = DisplayOption_GetInput(name);

                    if (user_option == "1")
                    {
                        tryVote = NetworkComms.SendReceiveObject<string, string>("tryvote", serverIP, serverPort, "vote_status", 1000000, regId);
                        if (tryVote == "1")
                        {
                            Console.WriteLine("Please enter a number (1-2) \n 1.Tim \n 2.Linda");
                            voteChoice = Console.ReadLine();
                            var EncrypVote = Keys.serverRSA.Encrypt(Encoding.ASCII.GetBytes(voteChoice + "||" + regId), false);
                            string vote_cast = NetworkComms.SendReceiveObject<byte[], string>("vote_cast", serverIP, serverPort, "result_status", 1000000, EncrypVote);
                            if (vote_cast == "1")
                            Console.WriteLine("vote casted sucessfully");
                        }
                        else
                        {
                            Console.WriteLine("you have already voted");
                            
                        }
                       user_option = DisplayOption_GetInput(name);
                    }
                    else if(user_option == "2")
                    {
                        string history = NetworkComms.SendReceiveObject<string, string>("history", serverIP, serverPort, "history_out", 1000000, regId);
                        Console.WriteLine(history);
                        user_option = DisplayOption_GetInput(name);
                    }
                    else if (user_option == "3")
                    {
                        string result = NetworkComms.SendReceiveObject<string, string>("result", serverIP, serverPort, "result_out", 1000000,"0");
                        if (result == "0")
                            Console.WriteLine("REsults not available");
                        else
                            Console.WriteLine(result);
                        user_option = DisplayOption_GetInput(name);

                    }
                    else if (user_option == "4")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Bad Choice");
                    }
                } while (user_option != "4");
            }
                
            NetworkComms.Shutdown();
            Console.ReadLine();

            
        }
    }

}

