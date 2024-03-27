using System.Net;
using System.Net.Sockets;

namespace Lesson_03_UDP_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UDP_ChatServer server = new UDP_ChatServer();
            server.RecieveBroadcast();
        }
    }
}
