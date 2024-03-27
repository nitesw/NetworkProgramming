using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_03_UDP_Server
{
    internal class UDP_ChatServer
    {
        private readonly int BUFFERSIZE = 1024;

        Socket socketBroadcastReceiver;
        IPEndPoint ipAdress;
        IPEndPoint broadcastEP;

        public UDP_ChatServer()
        {
            socketBroadcastReceiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ipAdress = new IPEndPoint(IPAddress.Any, 8080);
            broadcastEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 23000);
        }

        private void RecieveCompletedCallback(object sender, SocketAsyncEventArgs saea)
        {
            string textReceived = string.Empty;

            textReceived = Encoding.ASCII.GetString(saea.Buffer, 0, saea.BytesTransferred);
            Console.WriteLine($"Text recieved: {textReceived}");
            Console.WriteLine($"Number of bytes: {saea.BytesTransferred}");
            Console.WriteLine($"Recieved from: {saea.RemoteEndPoint}");

            Array.Clear(saea.Buffer, 0, saea.BytesTransferred);

            (sender as Socket).ReceiveFromAsync(saea);

            SocketAsyncEventArgs saeaRecieve = new SocketAsyncEventArgs();

            byte[] sendBuffer = Encoding.ASCII.GetBytes("<ENTER>");
            saeaRecieve.SetBuffer(sendBuffer, 0, sendBuffer.Length);
            saeaRecieve.RemoteEndPoint = saea.RemoteEndPoint;
            saeaRecieve.Completed += SendDiscoverResponseCompleted;

            (sender as Socket).SendToAsync(saeaRecieve);
        }

        private void SendDiscoverResponseCompleted(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine("Discover response send complete.");
        }

        public void RecieveBroadcast()
        {
            try
            {
                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                saea.SetBuffer(new byte[BUFFERSIZE], 0, BUFFERSIZE);
                saea.RemoteEndPoint = ipAdress;
                saea.Completed += RecieveCompletedCallback;
                socketBroadcastReceiver.Bind(ipAdress);

                if(!socketBroadcastReceiver.ReceiveFromAsync(saea))
                {
                    Console.WriteLine("Failed to recieve broadcast.");
                    return;
                }
                else
                {
                    Console.WriteLine("Server has been started...");

                    try
                    {

                        while (true)
                        {
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
