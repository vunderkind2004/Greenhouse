using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GreenHouse.Hubs
{
    public class PublicHub : Hub
    {
        public void AddViewer(Guid? viewId)
        {
            if (viewId == null || viewId == Guid.Empty)
                return;
            Groups.Add(Context.ConnectionId, viewId.ToString());
        }        
    }
}