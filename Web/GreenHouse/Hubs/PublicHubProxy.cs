using GreenHouse.Interfaces.ApiModels;
using GreenHouse.ViewModels;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenHouse.Hubs
{
    public class PublicHubProxy
    {
        public static void AddData(Guid? viewId, SensorDataViewModel dataSetPart)
        {
            if (viewId == null || viewId == Guid.Empty)
                return;
            Context.Clients.Group(viewId.ToString()).AddData(dataSetPart);
        }

        public static void SendCurrentSensorValues(Guid? viewId, SensorData[] sensorsData)
        {
            if (viewId == null || viewId == Guid.Empty)
                return;
            Context.Clients.Group(viewId.ToString()).UpdateSensorValues(sensorsData);
        }

        private static IHubContext Context
        {
            get
            {
                return GlobalHost.ConnectionManager.GetHubContext<PublicHub>();
            }
        }
    }
}