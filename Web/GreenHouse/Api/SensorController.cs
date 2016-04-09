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
        private readonly ISensorDataRepository dataReposytory;
        public SensorController(IRepository<SensorType> sensorRepository, IRepository<Device> deviceRepository,
            ISensorDataRepository dataReposytory)
        {
            this.sensorRepository = sensorRepository;
            this.deviceRepository = deviceRepository;
            this.dataReposytory = dataReposytory;
        }

        [HttpGet]
        [Route("test1")]
        public IHttpActionResult Test()
        {
            return Ok("Test done");
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
            //var device = deviceRepository.GetAll().FirstOrDefault(x => x.Token == message.Token);
            //if (device == null)
            //    return BadRequest("wrong device token");
            //var sensor = sensorRepository.GetAll().FirstOrDefault(x => x.Id == message.SensorType);
            //if (sensor == null)
            //    return BadRequest("wrong sensor type");
            //var sensorData = new SensorData
            //{
            //    Device = device,
            //    SensorType = sensor,
            //    EventDateTime = DateTime.Now,
            //    Value = message.Value
            //};
            //dataReposytory.Create(sensorData);
            try
            {
                dataReposytory.WriteSensorData(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();

        }
    
    }

    
}
