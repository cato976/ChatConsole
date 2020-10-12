namespace ChatClient
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    public class ChatClient
    {
        static TcpClient clientSocket = new TcpClient();
        static NetworkStream serverStream = default(NetworkStream);
        static string readData = null;
        static string chatName = null;
        static Thread ctThread = new Thread(getMessage);
        static string clientMessage;

        static void Main(string[] args)
        {
            bool showHelp = false;
            bool status = true;

            NDesk.Options.OptionSet argParser = new NDesk.Options.OptionSet()
                .Add("chat-name=", "chat name to use",
                        delegate(string v) { chatName = v; })
                .Add("h|?|help", delegate(string v) { showHelp = (v != null); });
            argParser.Parse(args);

            if (!showHelp && (!string.IsNullOrEmpty(chatName)))
            {
                ConnectToServer();
                while(status)
                {
                    Console.Write("Client: ");
                    clientMessage = Console.ReadLine();
                    if((clientMessage == "bye") || (clientMessage == "BYE"))
                    {
                        status = false;
                    }
                    if(clientMessage != "bye" && clientMessage != "BYE")
                    {
                        SendMessage();
                    }
                }

                ctThread.Abort();
            }
            else
            {
                Console.WriteLine($"Usage: {Assembly.GetExecutingAssembly().GetName().Name} [OPTION]...");
                Console.WriteLine("A chat client");
                Console.WriteLine("Options:");
                argParser.WriteOptionDescriptions(Console.Out);
            }
        }

        private static void ConnectToServer()
        {
            readData = "Conected to Chat Server ...";
            msg();
            clientSocket.Connect("127.0.0.1", 8888);
            serverStream = clientSocket.GetStream();

            byte[] outStream = Encoding.ASCII.GetBytes(chatName + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        private static void SendMessage()
        {
            byte[] outStream = Encoding.ASCII.GetBytes(clientMessage + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private static void msg()
        {
            Console.WriteLine(Environment.NewLine + " >> " + readData);
        } 

        private static void getMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                buffSize = clientSocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);

                int size = 0;
                foreach(byte b in inStream)
                {
                    if (b != 0)
                    {
                        size++;
                    }
                }

                byte[] inStream2 = new byte[size];
                Buffer.BlockCopy(inStream, 0, inStream2, 0, size);
                string returndata = Encoding.ASCII.GetString(inStream2);
                readData = "" + returndata;
                msg();
            }
        }
    }
}
