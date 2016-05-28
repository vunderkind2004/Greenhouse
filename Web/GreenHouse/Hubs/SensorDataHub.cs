using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace GreenHouse.Hubs
{
    [Authorize]
    public class SensorDataHub : Hub
    {
        public override Task OnConnected()
        {
            Groups.Add(Context.ConnectionId, UserName);
            Clients.Caller.send("connected");
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            Groups.Add(Context.ConnectionId, UserName);
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Groups.Remove(Context.ConnectionId, UserName);
            return base.OnDisconnected(stopCalled);
        }

        private string UserName
        {
            get
            {
                return Context.User.Identity.Name;
            }
        }
    }
}