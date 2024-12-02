using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        static IPAddress ServerIPAddress;
        static int ServerPort;
        static int MaxClient;
        static int Duration;
        static List<Client> AllClients = new List<Client>();

        static void Main(string[] args)
        {
            OnSettings();

        }

        static void OnSettings()
        {
            string Path = Directory.GetCurrentDirectory() + "/.config";
            if (File.Exists(Path))
            {
                StreamReader sr = new StreamReader(Path);
                ServerIPAddress = IPAddress.Parse(sr.ReadLine());
                ServerPort = int.Parse(sr.ReadLine());
                MaxClient = int.Parse(sr.ReadLine());
                Duration = int.Parse(sr.ReadLine());
                sr.Close();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Server IP-address: {ServerIPAddress.ToString()};\nServer port: {ServerPort};\nMax client: {MaxClient};\nDuration: {Duration};");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"Please, provide the IP-address: ");
                ServerIPAddress = IPAddress.Parse(Console.ReadLine());
                Console.Write($"Please, specify the port: ");
                ServerPort = int.Parse(Console.ReadLine());
                Console.Write($"Please, specify the maximum number of clients: ");
                MaxClient = int.Parse(Console.ReadLine());
                Console.Write($"Please, specify the duration of the license: ");
                Duration = int.Parse(Console.ReadLine());
                StreamWriter sw = new StreamWriter(Path);
                sw.WriteLine(ServerIPAddress.ToString());
                sw.WriteLine(ServerPort);
                sw.WriteLine(MaxClient);
                sw.WriteLine(Duration);
                sw.Close();
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("To change, write the command: /config");
        }

        static void SetCommand()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string Command = Console.ReadLine();
            switch (Command)
            {
                case "/config": File.Delete(Directory.GetCurrentDirectory() + "/.config"); OnSettings(); break;
                case "/status": GetStatus(); break;
                case "/help": Help(); break;
                default: if (Command.Contains("/disconnect")) DisconnectServer(Command); break;
            }
        }

        static string SetCommandClient(string Command)
        {
            if (Command == "/token")
                if (AllClients.Count < MaxClient)
                {
                    var newClient = new Client();
                    AllClients.Add(newClient);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"New client connection: {newClient.Token}");
                    return newClient.Token;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("There isn't enough space on the license server");
                    return "/limit";
                }
            else
            {
                var Client = AllClients.Find(x => x.Token == Command);
                return Client != null ? "/connect" : "/disconnect";
            }
        }

        static void ConnectServer()
        {
            IPEndPoint EndPoint = new IPEndPoint(ServerIPAddress, ServerPort);
            Socket SocketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketListener.Bind(EndPoint);
            SocketListener.Listen(MaxClient);
            while (true)
            {
                Socket Handler = SocketListener.Accept();
                byte[] bytes = new byte[10485760];
                int byteRec = Handler.Receive(bytes);
                string Message = Encoding.UTF8.GetString(bytes, 0, byteRec);
                string Response = SetCommandClient(Message);
                Handler.Send(Encoding.UTF8.GetBytes(Response));
            }
        }

        static void GetStatus()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Count clients: {AllClients.Count}");
            foreach (var client in AllClients)
            {
                int Duration = (int)DateTime.Now.Subtract(client.DateConnect).TotalSeconds;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Client: {client.Token}, time connection: {client.DateConnect.ToString("HH:mm:ss dd.MM")}, duration: {Duration}");
            }
        }

        static void Help()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Command to the clients: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("/config");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  - set initial settings");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("/disconnect");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - disconnect users from server");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("/status");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  - show list users");
        }
    }
}
