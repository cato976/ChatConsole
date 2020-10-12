using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace ChatClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            byte[] outStream = Encoding.ASCII.GetBytes(txbChat.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void btnConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            readData = "Conected to Chat Server ...";
            msg();
            clientSocket.Connect("127.0.0.1", 8888);
            serverStream = clientSocket.GetStream();

            byte[] outStream = Encoding.ASCII.GetBytes(txbName.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        private void getMessage()
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

        private void msg()
        {
            Dispatcher.Invoke(() =>
            {
                txbChatWindow.Text = txbChatWindow.Text + Environment.NewLine + " >> " + readData;
                txbChatWindow.ScrollToEnd();
            });
        } 
    }
}
