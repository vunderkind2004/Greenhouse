using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using GreenHouse.Repository.Repository;
using GreenHouse.ViewModels;

namespace GreenHouse.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository<SensorType> sensorRepository;
        private readonly IRepository<Device> deviceRepository;
        private readonly ISensorDataRepository dataReposytory;

        public HomeController(IRepository<SensorType> sensorRepository, IRepository<Device> deviceRepository,
            ISensorDataRepository dataReposytory)
        {
            this.sensorRepository = sensorRepository;
            this.deviceRepository = deviceRepository;
            this.dataReposytory = dataReposytory;
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddSensorType()
        {            
            return View();
        }

        [HttpPost]
        public ActionResult AddSensorType(SensorTypeViewModel model)
        {
            var sensorType = new SensorType{TypeName = model.TypeName, Dimension = model.Dimension};
            sensorRepository.Create(sensorType);
            return View("SensorTypeAdded",model);
        }

        [HttpGet]
        public ActionResult RegisterDevice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterDevice(DeviceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var device = new Device
            {
                Name = model.Name,
                Summary = model.Summary,
                RegistrationDate = DateTime.Now,
                Token = Guid.NewGuid().ToString()
            };

            deviceRepository.Create(device);

            return View("DeviceCreated", model);

        }

        public ActionResult GetDevices()
        {
            var devices = deviceRepository.GetAll().Select(x=>new DeviceViewModel{Name = x.Name, Summary = x.Summary});
            return View(devices);
        }

        public ActionResult GetSensorTypes()
        {
            var sensorTypes = sensorRepository.GetAll().Select(x=>new SensorTypeViewModel{TypeName=x.TypeName, Dimension= x.Dimension});
            return View(sensorTypes);
        }

        public ActionResult GetSensorData(int skipCount=0, int takeCount=100)
        {
            var data = dataReposytory.GetData(skipCount,takeCount);
            var model = data.Select(x => new SensorDataViewModel
            {
                DeviceName = x.DeviceName,
                EventDateTime = x.EventDateTime,
                SensorType = x.TypeName,
                Value = x.Value,
                SensorDimension = x.Dimension
            }).ToList();
            return View(model);
        }

    }
}
