using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenHouse.Interfaces.ApiModels;
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

        public static void SendCurrentSensorValues(string userName,SensorData[] sensorsData)
        {
            Context.Clients.Group(userName).UpdateSensorValues(sensorsData);
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