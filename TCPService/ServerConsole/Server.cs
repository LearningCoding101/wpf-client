using ClientApp.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerConsole
{
    class Server
    {
        private static List<Client> clients = new List<Client>(); // Maintain a list of clients
        private static readonly object lockObj = new object(); // Synchronize access to the clients list

        /// <summary>
        /// Processes messages received from a client.
        /// </summary>
        /// <param name="param">The TcpClient representing the connected client.</param>
        private static void ProcessMessage(object param)
        {
            TcpClient tcpClient = param as TcpClient;
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[256];
            string username = "";

            try
            {
                //Read the username sent by the client
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                username = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                // Create and add the new client
                Client client = new Client(tcpClient, username);

                // Ensure only one thread can access this block of code for each time
                lock (lockObj)
                {
                    clients.Add(client);
                }

                Console.WriteLine($"[{DateTime.Now:t}] {username} has joined the chat.");


                //Handle incoming messages from this client
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                    string formattedMessage = $"{username}: {message} - {DateTime.Now:t}";

                    Console.WriteLine(formattedMessage);
                    BroadcastMessage(formattedMessage, tcpClient);
                }

                //Handle client disconnection
                DisconnectClient(client);
                Console.WriteLine($"{username} has disconnected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing client {username}: {ex.Message}");
                DisconnectClient(new Client(tcpClient, username));
            }
        }

        /// <summary>
        /// Broadcasts a message to all connected clients, excluding the sender.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="excludeClient">The client to exclude from broadcasting (e.g., the sender).</param>
        private static void BroadcastMessage(string message, TcpClient excludeClient = null)
        {
            byte[] msgBytes = Encoding.ASCII.GetBytes(message);

            lock (lockObj)
            {
                foreach (var client in clients)
                {
                    //Prevent the client to see their message twice
                    if (client.ClientSocket != excludeClient)
                    {
                        try
                        {
                            NetworkStream stream = client.ClientSocket.GetStream();
                            stream.Write(msgBytes, 0, msgBytes.Length);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error broadcasting to {client.Username}: {ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes a client from the list and closes their connection.
        /// </summary>
        /// <param name="client">The client to disconnect.</param>
        private static void DisconnectClient(Client client)
        {
            lock (lockObj)
            {
                clients.Remove(client);
            }
            try
            {
                client.ClientSocket.Close();
            }
            catch
            {
                // Ignore errors while closing the client socket
            }
        }

        /// <summary>
        /// Starts the server and listens for client connections.
        /// </summary>
        /// <param name="host">The IP address to bind the server to.</param>
        /// <param name="port">The port to bind the server to.</param>
        private static void ExecuteServer(string host, int port)
        {
            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Parse(host), port);
                server.Start();
                Console.WriteLine($"Server started on {host}:{port}. Waiting for connections...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("A client connected.");

                    // Start a thread to handle the client
                    Thread clientThread = new Thread(ProcessMessage);
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
            finally
            {
                server?.Stop();
                Console.WriteLine("Server stopped.");
            }
        }

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = 13000;
            ExecuteServer(host, port);
        }
    }
}
