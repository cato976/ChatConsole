using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable();

        static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(ipAddress, 8888);
            serverSocket.Start();
            Console.WriteLine("Chat Server Started ...");
            int counter = 0;
            while (true)
            {
                counter += 1;
                TcpClient clientSocket = serverSocket.AcceptTcpClient();

                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                Console.WriteLine(clientSocket.ReceiveBufferSize);
                networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                dataFromClient = Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientsList.Add(dataFromClient, clientSocket);

                broadcast(dataFromClient + " Joined ", dataFromClient, false);

                Console.WriteLine(dataFromClient + " Joined chat room ");
                HandleClient client = new HandleClient();
                client.startClient(clientSocket, dataFromClient, clientsList);
            }

            serverSocket.Stop();
            Console.WriteLine("exit");
        }

        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
    }
}
