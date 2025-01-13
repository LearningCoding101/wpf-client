using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.Model
{
    public class Client
    {
        public TcpClient ClientSocket { get; }
        public string Username { get; }

        public Client(TcpClient clientSocket, string username)
        {
            this.ClientSocket = clientSocket;
            this.Username = username;
        }

    }
}
