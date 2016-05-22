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
        private readonly IRepository<SensorType> sensorTypeRepository;
        private readonly IRepository<Sensor> sensorRepository;
        private readonly IRepository<Device> deviceRepository;
        private readonly ISensorDataRepository dataReposytory;
        private readonly IRepository<User> userRepository;

        public HomeController(IRepository<SensorType> sensorTypeRepository, IRepository<Device> deviceRepository,
            ISensorDataRepository dataReposytory, IRepository<User> userRepository, IRepository<Sensor> sensorRepository)
        {
            this.sensorTypeRepository = sensorTypeRepository;
            this.sensorRepository = sensorRepository;
            this.deviceRepository = deviceRepository;
            this.dataReposytory = dataReposytory;
            this.userRepository = userRepository;
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
            sensorTypeRepository.Create(sensorType);
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
                Token = Guid.NewGuid().ToString(),
                UserId = GetUserId()
            };

            deviceRepository.Create(device);

            return View("DeviceCreated", model);

        }

        public ActionResult GetDevices()
        {
            var userId = GetUserId();
            var devices = GetDevices(userId);
            return View(devices);
        }

        public ActionResult GetSensors()
        {
            var userId = GetUserId();

            var userDevices = deviceRepository.Select(x => x.UserId == userId).ToList();
            var userDeviceIds = userDevices.Select(x=>x.Id).ToArray();
            var sensors = sensorRepository.Select(x=>userDeviceIds.Contains(x.DeviceId))
                .Select(x => new SensorViewModel 
                { 
                    Id = x.Id,
                    Name = x.Name, 
                    TypeId = x.SensorTypeId, 
                    DeviceId = x.DeviceId, 
                    Location = x.Location, 
                    DeviceName = userDevices.First(d=>d.Id==x.DeviceId).Name 
                }).ToList();
            return View(sensors);
        }

        [HttpGet]
        public ActionResult AddSensor()
        {
            var userId = GetUserId();
            var model = new SensorViewModel
            {
                AveableDevices = GetDevices(userId)
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddSensor(SensorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var device = deviceRepository.GetAll().Where(x => x.Id == model.DeviceId).FirstOrDefault();
            var userId = GetUserId();
            if (device == null || device.UserId != userId)
            {
                model.AveableDevices = GetDevices(userId);
                ModelState.AddModelError("DeviceId", "wrong deviceId");
                return View(model);
            }
            var sensor = new Sensor{DeviceId = model.DeviceId, Name = model.Name, SensorTypeId = model.TypeId, Location = model.Location};
            sensorRepository.Create(sensor);
            return RedirectToAction("GetSensors");
        }

        public ActionResult GetSensorTypes()
        {
            var sensorTypes = sensorTypeRepository.GetAll().Select(x=>new SensorTypeViewModel{TypeName=x.TypeName, Dimension= x.Dimension, TypeId =  x.Id});
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

        private int GetUserId()
        {
            var login = User.Identity.Name;
            var user = userRepository.GetAll().Where(x => x.Login == login).First();
            return user.Id;
        }

        private IEnumerable<DeviceViewModel> GetDevices(int userId)
        {
            var devices =  deviceRepository.GetAll().Where(x => x.UserId == userId).Select(x => new DeviceViewModel {Id = x.Id, Name = x.Name, Summary = x.Summary, Token = x.Token });
            return devices;
        }

    }
}
