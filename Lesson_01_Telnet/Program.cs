﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lesson_01_Telnet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint port = new IPEndPoint(ipAddress, 8080);

            socketListener.Bind(port);
            socketListener.Listen(5);

            Console.WriteLine("Server has been started...");

            Socket client = socketListener.Accept();
            Console.WriteLine($"New client has been connected... {client.ToString()} IP: {client.RemoteEndPoint.ToString()}");

            byte[] buffer = new byte[1024];
            int numberOfReceivedBytes = 0;

            while (true)
            {
                numberOfReceivedBytes = client.Receive(buffer);
                Console.WriteLine($"Number of received bytes: {numberOfReceivedBytes}");
                Console.WriteLine($"Data sent: {buffer}");

                string receivedData = Encoding.ASCII.GetString(buffer, 0, numberOfReceivedBytes);
                Console.WriteLine($"Command from client: {receivedData}");
                client.Send(buffer);

                if(receivedData == "<EXIT>")
                {
                    break;
                }

                Array.Clear(buffer, 0, buffer.Length);
                numberOfReceivedBytes = 0;
            }
        }
    }
}
