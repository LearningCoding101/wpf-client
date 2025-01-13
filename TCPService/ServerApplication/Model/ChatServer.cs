using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.Model
{
    class ChatServer
    {
        private readonly List<Client> _clients = new List<Client>();
        private readonly object _lockObj = new Object();
        public Action<string> OnLog { get; set; }

        public Action<List<string>> OnClientUpdate { get; set; }


        public void StartServer(string host, int port) {
            Thread serverThread = new Thread(() =>
            {
                TcpListener server = null;
                try
                {
                    server = new TcpListener(IPAddress.Any, port);

                    server.Start();

                    OnLog?.Invoke("Server started!");
                    string localIP = GetLocalIPAddress();
                    OnLog?.Invoke($"Server started on {localIP}:{port}. Waiting for connections...");

                    while (true)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        OnLog?.Invoke("A client connected");

                        Thread clientThread = new Thread(() => { ProcessClient(client); });

                        clientThread.IsBackground = true;

                        clientThread.Start();

                    }
                } catch (Exception ex)
                {
                    OnLog?.Invoke($"Server error: {ex.Message}");

                } finally
                {
                    server?.Stop();
                }


            });
            serverThread.IsBackground = true;
            serverThread.Start();
        
        }

        public void ProcessClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();

            byte[] buffer = new byte[256];
            string username = "";


            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                username = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                lock(_lockObj)
                {
                    _clients.Add(new Client(tcpClient, username));
                }

                OnLog?.Invoke($"{username} joined");
                UpdateClientsList();

                while((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

                    OnLog?.Invoke($"{username}: {message}");
                    BroadcastMessage($"{username}: {message}", tcpClient);

                }


            } catch
            {
                OnLog?.Invoke($"{username} disconnected");
            } finally
            {
                lock (_lockObj)
                {
                    _clients.RemoveAll(c => c.ClientSocket == tcpClient);
                }
                UpdateClientsList();
                tcpClient.Close();
            }


        }
        private string GetLocalIPAddress()
        {
            string localIP = "127.0.0.1"; // Default to localhost if unable to detect

            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    // Filter for IPv4 addresses and non-loopback addresses
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"Error getting local IP address: {ex.Message}");
            }

            return localIP;
        }

        public void BroadcastMessage(string message, TcpClient excludeClient = null)
        {
            byte[] msgBytes = Encoding.ASCII.GetBytes(message);

            lock (_lockObj)
            {
                foreach (var client in _clients)
                {
                    if (client.ClientSocket != excludeClient)
                    {
                        try
                        {
                            NetworkStream stream = client.ClientSocket.GetStream();
                            stream.Write(msgBytes, 0, msgBytes.Length);
                        }
                        catch { }
                    }
                }
            }
        }

        private void UpdateClientsList()
        {
            lock (_lockObj)
            {
                OnClientUpdate?.Invoke(_clients.ConvertAll(c => c.Username));
            }
        }
    }
}
