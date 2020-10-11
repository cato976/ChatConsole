namespace ChatClient
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    public class ChatClient
    {
        static void Main(string[] args)
        {
            TcpClient socketForServer;
            bool status = true;

            try 
            {
                socketForServer = new TcpClient("localhost", 8888);
                Console.WriteLine("Connected to Server");
            }
            catch
            {
                Console.WriteLine("Failed to Connect to server{0}:999", "localhost");
                return;
            }

            NetworkStream networkStream = socketForServer.GetStream();
            StreamReader streamReader = new StreamReader(networkStream);
            StreamWriter streamWriter = new StreamWriter(networkStream);

            try
            {
                string clientMessage = string.Empty;
                string serverMessage = string.Empty;

                while(status)
                {
                    Console.Write("Client: ");
                    clientMessage = Console.ReadLine();
                    if((clientMessage == "bye") || (clientMessage == "BYE"))
                    {
                        status = false;
                        streamWriter.WriteLine("bye");
                        streamWriter.Flush();
                    }
                    if(clientMessage != "bye" && clientMessage != "BYE")
                    {
                        streamWriter.WriteLine(clientMessage);
                        streamWriter.Flush();
                        serverMessage = streamReader.ReadLine();
                        Console.WriteLine("Server:"+serverMessage);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Exception reading from server");
            }

            streamReader.Close();
            networkStream.Close();
            streamWriter.Close();
        }
    }
}
