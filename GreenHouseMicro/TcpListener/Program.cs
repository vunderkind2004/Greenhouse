using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyTcpListener
{
    class Program    
    {
        private static TcpListener listener;

        static void Main(string[] args)
        {
            Start();
            

            Console.Read();
            listener.Stop();
        }

        private static void Start()
        {
            listener = new TcpListener(IPAddress.Parse("192.168.1.106"), 60000);
            listener.Start();
            listener.BeginAcceptTcpClient(callback, listener);
        }

        private static void callback(IAsyncResult ar)
        {
            Console.WriteLine("Connected.");
            var listener = ar.AsyncState as TcpListener;
            var client = listener.EndAcceptTcpClient(ar);
            using (var stream = client.GetStream())
            {
                var buffer = new byte[1024];
                var totalReaded=0;
                Console.WriteLine("Reading...");
                while(true)
                {
                    var readed = stream.Read(buffer, totalReaded, buffer.Length - totalReaded);
                    if (readed <= 0)
                        break;
                    totalReaded += readed;
                    break;
                }

                var messageBuffer = new byte[totalReaded];
                Array.Copy(buffer, messageBuffer, messageBuffer.Length);

                var message = Encoding.UTF8.GetString(messageBuffer);
                Console.WriteLine("msg: {0}", message);
                client.Client.Send(messageBuffer);
            }
            //client.Client.Disconnect(false);
            listener.BeginAcceptTcpClient(callback, listener);
        }
    }
}
