using Microsoft.AspNetCore.SignalR;
using OA.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OA.Web.Helper
{
    public interface IExtendedHubHelper
    {
        void SendOutMessage(string message, string statusCode);
        void SendOutMessage(string mobileNo, string data, string statusCode);
        void SendLoggedInMessage(string mobileNo, string data, string statusCode);
    }
    public class ExtendedHubHelper : IExtendedHubHelper
    {
        private IHubContext<NotificationsHub> _hubContext;
        public ExtendedHubHelper(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public void SendOutMessage(string mobileNo, string data, string statusCode)
        {
            //_hubContext.Clients.Client(mobileNo).SendAsync("ReceiveMessage", data, statusCode);
            _hubContext.Clients.All.SendAsync("ReceiveMessage", data, statusCode);
        }
        public void SendLoggedInMessage(string mobileNo, string data, string statusCode)
        {
            _hubContext.Clients.All.SendAsync("ReceiveLoggedInMessage", data, statusCode);
        }
        public void SendOutMessage(string message, string statusCode)
        {
            _hubContext.Clients.All.SendAsync("SendMessage", message, statusCode);
        }

    }
}
