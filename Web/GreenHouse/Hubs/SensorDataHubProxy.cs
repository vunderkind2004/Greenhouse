using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenHouse.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GreenHouse.Hubs
{
    public static class SensorDataHubProxy 
    {
        public static void AddData(string userName, SensorDataViewModel dataSetPart)
        {
            Context.Clients.Group(userName).AddData(dataSetPart);
        }

        private static IHubContext Context
        {
            get 
            {
                return GlobalHost.ConnectionManager.GetHubContext<SensorDataHub>();
            }
        }

    }    
}