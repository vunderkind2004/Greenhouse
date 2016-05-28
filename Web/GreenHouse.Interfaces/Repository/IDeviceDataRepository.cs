using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenHouse.Interfaces.ApiModels;
using GreenHouse.Interfaces.Responses;

namespace GreenHouse.Interfaces.Repository
{
    public interface IDeviceDataRepository
    {
        AddSensorDataResponse Write(GreenHouseDataMessage message);
    }
}
