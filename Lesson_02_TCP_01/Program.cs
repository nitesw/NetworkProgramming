using OpenAI_API;
using OpenAI_API.Models;
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
        static List<string> connectedUsers = new List<string>();

        static async Task SaveLogsToFile()
        {
            try
            {
                using (FileStream file = new FileStream("Logs.json", FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync(file, users);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task InitQuotes()
        {
            for (int i = 0; i < quotes.Length; i++)
            {
                quotes[i] = "Quote " + (i + 1);
            }
        }
        static async Task ConnectClient(Socket client)
        {
            try
            {
                bool connected = true;
                string clientEndPoint = client.RemoteEndPoint.ToString();
                users.Add(new User() { ConnectionTime = DateTime.Now, IPv4Address = clientEndPoint });
                connectedUsers.Add(clientEndPoint);

                byte[] buffer = new byte[1024];
                int numberOfReceivedBytes = 0;

                while (connected)
                {
                    numberOfReceivedBytes = await client.ReceiveAsync(buffer);
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
                        connectedUsers.Remove(clientEndPoint);
                        connected = false;
                    }
                    else if (receivedData.Equals("<BREAK>"))
                    {
                        break;
                    }
                    else if (receivedData.Equals("<GET>"))
                    {
                        int rnd = new Random().Next(quotes.Count());
                        foreach (var user in users)
                        {
                            if (user.IPv4Address == client.RemoteEndPoint.ToString())
                            {
                                user.Quotes.Add(quotes[rnd]);
                                break;
                            }
                        }
                        await client.SendAsync(Encoding.ASCII.GetBytes(quotes[rnd]));
                    }

                    Array.Clear(buffer, 0, buffer.Length);
                    numberOfReceivedBytes = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await SaveLogsToFile();
                client.Close();
                client.Dispose();
            }
        }

        static async Task Main(string[] args)
        {
            //OpenAIAPI api = new OpenAIAPI("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");
            //var result = await api.Chat.CreateChatCompletionAsync("Hello!", Model.Davinci, max_tokens: 256);
            //Console.WriteLine(result);

            Socket socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddr = IPAddress.Any;
            IPEndPoint port = new IPEndPoint(ipaddr, 8080);
            //await InitQuotes(chatGpt);
            
            socketListener.Bind(port);
            socketListener.Listen(5);

            Console.WriteLine("Server has been started...");

            while (true)
            {
                Socket client = await socketListener.AcceptAsync();
                Console.WriteLine($"New client has been connected... {client.ToString()} IP: {client.RemoteEndPoint.ToString()}");
                Task task = Task.Run(() => ConnectClient(client));
            }
        }
    }
}
