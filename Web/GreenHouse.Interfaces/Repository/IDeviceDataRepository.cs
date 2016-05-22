using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenHouse.Interfaces.ApiModels;

namespace GreenHouse.Interfaces.Repository
{
    public interface IDeviceDataRepository
    {
        void Write(GreenHouseDataMessage message);
    }
}
