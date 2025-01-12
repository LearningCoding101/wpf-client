using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace ClientApp.Services
{
    public class ClientService
    {
        private TcpClient client;
        private NetworkStream stream;
        private string username;

        public void ConnectServer(string ip, int port, string username)
        {
            this.username = username;
            try
            {
                client = new TcpClient(ip, port);
                stream = client.GetStream();

                // Step 1: Send the username to the server
                byte[] usernameBytes = Encoding.ASCII.GetBytes(username);
                stream.Write(usernameBytes, 0, usernameBytes.Length);

                // Step 2: Start a thread to listen for incoming messages
                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                Console.WriteLine("Connected to the server.");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.SendMessageButton.IsEnabled = true;
                        mainWindow.ConnectButton.IsEnabled = false;
                        mainWindow.ConnectButton.Content = "Connected";
                        mainWindow.UsernameInput.IsEnabled = false;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to server: {ex.Message}");
            }
        }

        public void SendMessages(string message)
        {
            try
            {
                if (client != null && stream != null && stream.CanWrite)
                {
                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                    stream.Write(messageBytes, 0, messageBytes.Length);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (Application.Current.MainWindow is MainWindow mainWindow)
                        {
                            string formattedMessage = $"{username}: {message} - {DateTime.Now:t}";
                            mainWindow.ChatHistory.Add(formattedMessage);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[256];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (Application.Current.MainWindow is MainWindow mainWindow)
                        {
                            mainWindow.ChatHistory.Add(message);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving messages: {ex.Message}");
            }
        }

        // Disconnect method to close the connection and stream
        public void Disconnect()
        {
            try
            {
                // Close the connection and notify disconnection
                stream?.Close();
                client?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disconnecting: {ex.Message}");
            }
        }
    }
}
