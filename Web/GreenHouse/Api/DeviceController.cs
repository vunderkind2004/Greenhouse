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
    [RoutePrefix("api/device")]
    public class DeviceController : ApiController    
    {
        private readonly IDeviceDataRepository dataRepository;
        public DeviceController(IDeviceDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        
        [HttpPost]
        [Route("AddGreenhouseData")]
        public IHttpActionResult AddDeviceData(GreenHouseDataMessage message)
        {
            //Example:
            //{
            //    'DeviceToken': 'e0559f6b-30fa-47b1-9a07-915a015129a3',
            //    'SensorsData' :
            //    [
            //        {
            //        'SensorId' : 1,
            //        'Value' : 31
            //        }
            //    ]
            //}


            if (message == null)
                return BadRequest();
            try
            {
                dataRepository.Write(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();

        }
    
    }

    
}
