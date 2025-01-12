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
using System.Net.Http;

namespace DemoHttpClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClear_Click(Object sender, RoutedEventArgs e)
        {
            txtUrl.Text = "";
            webBrowser.NavigateToString(" ");
        }

        private async void btnViewHtml_Click(Object sender, RoutedEventArgs e)
        {
            string uri = txtUrl.Text;
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                webBrowser.NavigateToString(responseBody);
            }
            catch (HttpRequestException ex)
            {
                webBrowser.NavigateToString($"<html><body><h2>Error</h2><p>{ex.Message}</p></body></html>");
            }
        }
    }
}
