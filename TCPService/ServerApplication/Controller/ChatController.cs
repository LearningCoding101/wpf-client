using ServerApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.Controller
{
    class ChatController
    {
        private readonly ChatServer _server;

        public ChatController(ChatServer server)
        {
            _server = server;
        }

        public void StartServer(string host, int port)
        {
            _server.StartServer(host, port);
        }

        public void BroadcastMessage(string message)
        {
            _server.BroadcastMessage($"Server: {message}");
        }

    }
}
