using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GreenHouse.Interfaces.ApiModels;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;

namespace GreenHouse.Api
{
    [RoutePrefix("api/sensor")]
    public class SensorController : ApiController    
    {
        private readonly IRepository<SensorType> sensorRepository;
        private readonly IRepository<Device> deviceRepository;
        private readonly ISensorDataRepository dataRepository;
        private readonly  IRepository<Sensor> userSensorRepository;

        public SensorController(IRepository<SensorType> sensorRepository, IRepository<Device> deviceRepository,
            ISensorDataRepository dataRepository, IRepository<Sensor> userSensorRepository)
        {
            this.sensorRepository = sensorRepository;
            this.deviceRepository = deviceRepository;
            this.dataRepository = dataRepository;
            this.userSensorRepository = userSensorRepository;
        }

        [HttpGet]
        [Route("test1")]
        public IHttpActionResult Test()
        {
            return Ok("Test done");
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetSensors()
        {
            var sensors = sensorRepository.GetAll().ToArray();
            return Ok(sensors);
        }

        [HttpGet]
        [Route("userSensors")]
        public IHttpActionResult GetUserSensors(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("wrong token");
            var device = deviceRepository.GetAll().FirstOrDefault(x => x.Token == token);
            if(device==null)
                return BadRequest("wrong token");
            var userSensors = userSensorRepository.GetAll().Where(x => x.DeviceId == device.Id);
            return Ok(userSensors);
        }

        [HttpPost]
        [Route("humidity")]
        public IHttpActionResult SetHumidity(HumidityData humidity)
        {
            return Ok(humidity);
 
        }

        [HttpPost]
        [Route("addsensordata")]
        public IHttpActionResult AddSensorData(SensorDataMessage message)
        {
            if (message == null)
                return BadRequest();
            try
            {
                dataRepository.WriteSensorData(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();

        }
    
    }

    
}
