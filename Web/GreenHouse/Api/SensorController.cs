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
        public SensorController(IRepository<SensorType> sensorRepository, IRepository<Device> deviceRepository,
            ISensorDataRepository dataRepository)
        {
            this.sensorRepository = sensorRepository;
            this.deviceRepository = deviceRepository;
            this.dataRepository = dataRepository;
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
