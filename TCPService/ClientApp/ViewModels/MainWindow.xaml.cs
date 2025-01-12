using ClientApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> ChatHistory { get; set; }
        public ObservableCollection<string> ConnectedClients { get; set; }
        private ClientService _client;
        private string username;


        public MainWindow()
        {
            InitializeComponent();
            ChatHistory = new ObservableCollection<string>();
            DataContext = this;
            _client = new ClientService();
        }


        private void ConnectServer(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(UsernameInput.Text))
            {
                MessageBox.Show("Please provide a username", "Missing username");
                return;
            }
            username = UsernameInput.Text;
            try
            {
                _client.ConnectServer("127.0.0.1", 13000, username);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot connect to the server", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Show or hide the placeholder text depending on whether the input is empty
            PlaceholderText.Visibility = string.IsNullOrEmpty(MessageInput.Text) ?
                                 Visibility.Visible : Visibility.Hidden;
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageInput.Text))
            {
                // Format the message with client text and the timestamp
                string formattedMessage = $"Client: {MessageInput.Text} - {DateTime.Now:t}";

                // Send message to the server
                _client.SendMessages(MessageInput.Text);

                // Clear the input box after sending the message
                MessageInput.Clear();
            }
        }

        private void HandleWindowClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _client.Disconnect();
        }
    }
}
