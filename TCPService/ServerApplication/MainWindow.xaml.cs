using ServerApplication.Controller;
using ServerApplication.Model;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatController _chatController;

        public MainWindow()
        {
            InitializeComponent();

            ChatServer chatServer = new ChatServer
            {
                OnLog = UpdateLog,
                OnClientUpdate = UpdateClientsList
            };

            _chatController = new ChatController(chatServer);
        }

        private void UpdateLog(string message)
        {
            Dispatcher.Invoke(() => LogTextBox.AppendText($"{message}\n"));
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _chatController.StartServer("127.0.0.1", 13000);
            UpdateLog("Server started...");
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                _chatController.BroadcastMessage(message);
                UpdateLog($"Server: {message}");
                MessageTextBox.Clear();
            }
        }
        private void UpdateClientsList(List<string> clients)
        {
            Dispatcher.Invoke(() =>
            {
                ClientsListBox.Items.Clear();
                foreach (var client in clients)
                {
                    ClientsListBox.Items.Add(client);
                }
            });
        }

    }
}