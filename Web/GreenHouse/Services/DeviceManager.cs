using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using GreenHouse.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenHouse.Services
{
    public class DeviceManager
    {
        IRepository<Device> deviceRepository;

        public DeviceManager(IRepository<Device> deviceRepository)
        {
            this.deviceRepository = deviceRepository;
        }

        public IEnumerable<DeviceViewModel> GetDevices(int userId)
        {
            var devices = deviceRepository
                .GetAll()
                .Where(x => x.UserId == userId)
                .Select(x => new DeviceViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Summary = x.Summary,
                    Token = x.Token,
                    ViewId = x.ViewId
                });
            return devices;
        }
    }
}