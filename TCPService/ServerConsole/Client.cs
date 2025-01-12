using System.Net.Sockets;

namespace ClientApp.Models
{
    public class Client
    {
        public string Username { get; set; }
        public TcpClient ClientSocket { get; set; }

        public Client(TcpClient client, string username)
        {
            ClientSocket = client;
            Username = username;

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with Username: {Username}");
        }
    }
}
