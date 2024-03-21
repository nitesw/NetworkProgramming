using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lesson_02_TCP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket client = null;

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddr = null;

            try
            {
                Console.WriteLine("====ITSTEP Client====");

                Console.Write("Enter IPv4 address: ");
                string strIpAddress = Console.ReadLine();

                Console.Write("Enter port (from 0 to 65535): ");
                string strPortInput = Console.ReadLine();
                int nPortInput = 0;

                if (strIpAddress == " ") strIpAddress = "192.168.1.106";
                if (strPortInput == " ") strPortInput = "8080";

                if (!IPAddress.TryParse(strIpAddress, out ipaddr))
                {
                    Console.WriteLine("Invalid IPv4 address");
                    return;
                }
                if (!int.TryParse(strPortInput.Trim(), out nPortInput))
                {
                    Console.WriteLine("Invalid port number");
                    return;
                } 
                if (nPortInput <= 0 || nPortInput > 65535)
                {
                    Console.WriteLine("Port must be in range of 0 and 65535");
                    return;
                }

                Console.WriteLine($"IP address: {ipaddr}. Port: {nPortInput}");

                client.Connect(ipaddr, nPortInput);
                Console.WriteLine("Connected to server successfully...");
                Console.WriteLine("Type some text and press enter...");

                string inputCommand = string.Empty;
                while (true)
                {
                    inputCommand = Console.ReadLine();

                    if(inputCommand.Equals("<EXIT>"))
                    {
                        byte[] bufferSendExit = Encoding.ASCII.GetBytes(inputCommand);
                        client.Send(bufferSendExit);
                        break;
                    }

                    byte[] bufferSend = Encoding.ASCII.GetBytes(inputCommand);
                    client.Send(bufferSend);

                    byte[] bufferReceive = new byte[512];
                    int nReceive = client.Receive(bufferReceive);

                    Console.WriteLine("Data received: {0}", Encoding.ASCII.GetString(bufferReceive, 0, nReceive));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if(client != null)
                {
                    if(client.Connected)
                    {
                        client.Shutdown(SocketShutdown.Both);
                    }
                }
                client.Close();
                client.Dispose();
            }
        }
    }
}
