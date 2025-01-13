using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.Controller
{
    class ServerController
    {
        private Action<string> _logCallback;
        private Action<List<string>> _clientsCallback;


        public ServerController(Action<string> logCallback, Action<List<string>> clientsCallback)
        {
            _logCallback = logCallback;
            _clientsCallback = clientsCallback;
        }

        public void SetCallbacks(Action<string> logCallback, Action<List<string>> clientsCallback)
        {
            _logCallback = logCallback;
            _clientsCallback = clientsCallback;
        }

        public void LogMessage(string message)
        {
            _logCallback?.Invoke(message);
        }

        public void UpdateClientsList(List<string> clients)
        {
            _clientsCallback?.Invoke(clients);
        }
    }


}
