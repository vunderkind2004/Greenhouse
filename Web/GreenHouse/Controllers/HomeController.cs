using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GreenHouse.Helpers;
using GreenHouse.Interfaces.Enums;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Interfaces.Requests;
using GreenHouse.Interfaces.Responses;
using GreenHouse.Repository.DataModel;
using GreenHouse.Repository.Repository;
using GreenHouse.ViewModels;
using Newtonsoft.Json;
using GreenHouse.Services;
using AutoMapper;
using GreenHouse.Models;

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
        private readonly DeviceManager deviceManager;

        public HomeController(IRepository<SensorType> sensorTypeRepository, IRepository<Device> deviceRepository,
            ISensorDataRepository dataReposytory, IRepository<User> userRepository, IRepository<Sensor> sensorRepository, DeviceManager deviceManager)
        {
            this.sensorTypeRepository = sensorTypeRepository;
            this.sensorRepository = sensorRepository;
            this.deviceRepository = deviceRepository;
            this.dataReposytory = dataReposytory;
            this.userRepository = userRepository;
            this.deviceManager = deviceManager;
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var sensorMap1 = GetSensorMap();
            ViewBag.SensorMap1 = sensorMap1;
            return View();
        }

        private SensorMapViewModel GetSensorMap()
        {
            var sensors = GetSensorsViewModel();
            if (sensors == null || !sensors.Any())
                return null;
            var sensorInfos = Mapper.Map<IEnumerable<SensorMapInfo>>(sensors);

            CalculateCoordinates(sensorInfos);
            SetDimensions(sensorInfos);
            //CalculateTemperatureColor(sensorInfos);

            var sensorMap = new SensorMapViewModel
            {
                SensorMapInfo = sensorInfos,
                TotalColumnsCount = sensorInfos.Max(x=>x.Column) + 1,
                TotalRowsCount = sensorInfos.Max(y => y.Row) + 1,
            };

            return sensorMap;
        }

        //private void CalculateTemperatureColor(IEnumerable<SensorMapInfo> sensorInfos)
        //{
        //    var temperatureSensorTypeId = 1;
        //    var temperatureSensorInfos = sensorInfos.Where(x => x.TypeId == temperatureSensorTypeId);
        //    var minT = temperatureSensorInfos.Min(x => x.CurrentState);
        //    var maxT = temperatureSensorInfos.Max(x => x.CurrentState);
        //    var rgbConverter = new RgbHelper(minT, maxT);
        //}

        private void SetDimensions(IEnumerable<SensorMapInfo> sensorInfos)
        {
            var sensorTypes = sensorTypeRepository.GetAll();
            foreach (var sensorInfo in sensorInfos)
            {
                sensorInfo.Dimension = sensorTypes
                    .First(x => x.Id == sensorInfo.TypeId)
                    .Dimension;
            }
        }

        private void CalculateCoordinates(IEnumerable<SensorMapInfo> sensorInfos)
        {
            foreach (var sensorInfo in sensorInfos)
            {                
                var location = JsonConvert.DeserializeObject<SensorLocation>(sensorInfo.Location);
                if (location != null)
                {
                    sensorInfo.Row = location.Y;
                    sensorInfo.Column = location.X;
                }
            }
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
            var devices = deviceManager.GetDevices(userId);
            return View(devices);
        }

        public ActionResult GetSensors()
        {
            var sensors = GetSensorsViewModel();
            return View(sensors);
        }

        private IEnumerable<SensorViewModel> GetSensorsViewModel()
        {
            var userId = GetUserId();

            var userDevices = deviceRepository.Select(x => x.UserId == userId).ToList();
            var userDeviceIds = userDevices.Select(x => x.Id).ToArray();
            var sensors = sensorRepository.Select(x => userDeviceIds.Contains(x.DeviceId))
                .Select(x => new SensorViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    TypeId = x.SensorTypeId,
                    DeviceId = x.DeviceId,
                    Location = x.Location,
                    DeviceName = userDevices.First(d => d.Id == x.DeviceId).Name
                }).ToList();
            return sensors;
        }

        [HttpGet]
        public ActionResult AddSensor()
        {
            var userId = GetUserId();
            var model = new SensorViewModel
            {
                AveableDevices = deviceManager.GetDevices(userId)
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
                model.AveableDevices = deviceManager.GetDevices(userId);
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

        //public ActionResult GetSensorData(int skipCount=0, int takeCount=100)
        //{
        //    var data = dataReposytory.GetData(User.Identity.Name,skipCount,takeCount);
        //    var model = ChartHelper.GetDataSets(data);
        //    return View(model);
        //}

        public ActionResult GetSensorData()
        {
            //var fromDate = DateTime.Now.AddMonths(-3);
            //var toDate = DateTime.Now;
            //var groupByTime = GroupByTime.Daily;

            //var model = GetSensorData(User.Identity.Name, fromDate, toDate, FilterByTime.All, groupByTime);
            //return View(model);
            return View();
        }

        public JsonResult GetSensorDataNow(FilterByTime filter = FilterByTime.All)
        {
            var fromDate = DateTime.Now.AddHours(-1);
            var toDate = DateTime.Now;
            var groupByTime = GroupByTime.Minute;

            var model = GetSensorData(User.Identity.Name, fromDate, toDate, filter, groupByTime);

            return new JsonNetResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetSensorDataDay(FilterByTime filter = FilterByTime.All)
        {
            var fromDate = DateTime.Now.AddDays(-1);
            var toDate = DateTime.Now;
            var groupByTime = GroupByTime.Hour;

            var model = GetSensorData(User.Identity.Name, fromDate, toDate, filter, groupByTime);

            return new JsonNetResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetSensorDataWeek(FilterByTime filter = FilterByTime.All)
        {
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var groupByTime = GroupByTime.Daily;

            var model = GetSensorData(User.Identity.Name, fromDate, toDate, filter, groupByTime);

            return new JsonNetResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetSensorDataMonth(FilterByTime filter = FilterByTime.All)
        {
            var fromDate = DateTime.Now.AddMonths(-1);
            var toDate = DateTime.Now;
            var groupByTime = GroupByTime.Week;

            var model = GetSensorData(User.Identity.Name, fromDate, toDate, filter, groupByTime);

            return new JsonNetResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetSensorDataYear(FilterByTime filter = FilterByTime.All)
        {
            var fromDate = DateTime.Now.AddYears(-1);
            var toDate = DateTime.Now;
            var groupByTime = GroupByTime.Month;

            var model = GetSensorData(User.Identity.Name, fromDate, toDate, filter, groupByTime);

            return new JsonNetResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private SensorDataViewModel GetSensorData(string userName, DateTime fromDate, DateTime toDate, FilterByTime filter, GroupByTime group)
        {
            var request = new GetGroupedSensorDataRequest
            {
                UserName = userName,
                FromDate = fromDate,
                ToDate = toDate,
                GroupByTime = group,
                FilterByTime = filter
            };

            var data = dataReposytory.GetData(request);
            var model = ChartHelper.GetDataSets(data);
            return model;
        }

        private int GetUserId()
        {
            var login = User.Identity.Name;
            var user = userRepository.GetAll().Where(x => x.Login == login).First();
            return user.Id;
        }

        

    }
}
