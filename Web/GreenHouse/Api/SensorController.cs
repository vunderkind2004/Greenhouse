using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GreenHouse.Interfaces.ApiModels;

namespace GreenHouse.Api
{
    [RoutePrefix("api/sensor")]
    public class SensorController : ApiController    
    {
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
    }

    
}
