using GreenHouse.Helpers;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using GreenHouse.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace GreenHouse.Controllers
{
    public class PublicController : Controller
    {
        private readonly IRepository<Sensor> sensorRepository;
        private readonly IRepository<Device> deviceRepository;
        private readonly IRepository<SensorType> sensorTypeRepository;
        private readonly MemoryCache Cache;
        private readonly string keyPrefix = "publicViewId_";

        public PublicController(ISensorDataRepository repository
            ,IRepository<Device> deviceRepository
            , IRepository<Sensor> sensorRepository
            , IRepository<SensorType> sensorTypeRepository)
        {
            this.sensorTypeRepository = sensorTypeRepository;
            this.sensorRepository = sensorRepository;
            this.deviceRepository = deviceRepository;
            Repository = repository;
            Cache = MemoryCache.Default;

        }

        public ISensorDataRepository Repository { get; }

        // GET: Public
        public ActionResult Index(Guid viewId)
        {
            if (viewId == Guid.Empty)
                return null;

            ViewBag.ViewId = viewId;

            var key = keyPrefix + viewId.ToString("n");
            if(Cache.Contains(key))
                return View((SensorMapViewModel)Cache[key]);            

            var sensorTypes = sensorTypeRepository.GetAll();
            var sensors = GetSensorViewModels(viewId);
            var sensorMap1 = SensorMapHelper.GetSensorMap(sensors, sensorTypes);
            Cache.Add(new CacheItem(key, sensorMap1),
                new CacheItemPolicy
                {
                    SlidingExpiration = new TimeSpan(0, 5, 0)
                });
            return View(sensorMap1);
        }

        private IEnumerable<SensorViewModel> GetSensorViewModels(Guid viewId)
        {
            var device = deviceRepository.Single(x => x.ViewId == viewId);
            var sensors = sensorRepository.Select(x => x.DeviceId == device.Id)
                .Select(x => new SensorViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    TypeId = x.SensorTypeId,
                    DeviceId = x.DeviceId,
                    Location = x.Location,
                    DeviceName = device.Name
                }).ToList();
            return sensors;
            
        }
    }
}