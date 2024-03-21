using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Lesson_02_TCP_01
{
    internal class Program
    {
        static List<User> users = new List<User>();
        static string[] quotes = new string[100];

        static void SaveLogsToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(users);

                File.WriteAllText("Logs.json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void InitQuotes()
        {
            for (int i = 0; i < quotes.Length; i++)
            {
                quotes[i] = "Quote " + (i + 1);
            }
        }

        static void Main(string[] args)
        {
            Socket socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddr = IPAddress.Any;
            IPEndPoint port = new IPEndPoint(ipaddr, 8080);
            bool clientConnected = true;
            InitQuotes();

            socketListener.Bind(port);
            socketListener.Listen(5);

            Console.WriteLine("Server has been started...");

            Socket client = socketListener.Accept();
            Console.WriteLine($"New client has been connected... {client.ToString()} IP: {client.RemoteEndPoint.ToString()}");

            try
            {
                users.Add(new User() { ConnectionTime = DateTime.Now, IPv4Address = client.RemoteEndPoint.ToString() });
                byte[] buffer = new byte[1024];
                int numberOfReceivedBytes = 0;

                while (clientConnected)
                {
                    string clientEndPoint = client.RemoteEndPoint.ToString();



                    numberOfReceivedBytes = client.Receive(buffer);
                    Console.WriteLine($"Number of received bytes: {numberOfReceivedBytes}");
                    Console.WriteLine($"Data sent: {buffer}");

                    string receivedData = Encoding.ASCII.GetString(buffer, 0, numberOfReceivedBytes);
                    Console.WriteLine($"Command from client: {receivedData}");

                    if (receivedData.Equals("<EXIT>"))
                    {
                        foreach (var user in users)
                        {
                            if (user.IPv4Address == clientEndPoint)
                            {
                                user.DisconnectionTime = DateTime.Now;
                                break;
                            }
                        }
                        clientConnected = false;
                    }
                    else if (receivedData.Equals("<BREAK>"))
                    {
                        clientConnected = false;
                    }
                    else if (receivedData.Equals("<GET>"))
                    {
                        int rnd = new Random().Next(quotes.Count());
                        foreach (var user in users)
                        {
                            if(user.IPv4Address == client.RemoteEndPoint.ToString())
                            {
                                user.Quotes.Add(quotes[rnd]);
                                break;
                            }
                        }
                        client.Send(Encoding.ASCII.GetBytes(quotes[rnd]));
                    }

                    Array.Clear(buffer, 0, buffer.Length);
                    numberOfReceivedBytes = 0;
                }
                SaveLogsToFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
