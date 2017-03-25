using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GreenHouse.Helpers;
using GreenHouse.Hubs;
using GreenHouse.Interfaces.ApiModels;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using System.Runtime.Caching;

namespace GreenHouse.Api
{
    [RoutePrefix("api/device")]
    public class DeviceController : ApiController    
    {
        private readonly MemoryCache cache;
        private readonly IDeviceDataRepository dataRepository;
        private readonly int cacheTimeoutMinutes;
        
        public DeviceController(IDeviceDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
            cache = MemoryCache.Default;
            cacheTimeoutMinutes = 3;
        }


        [HttpGet]
        [Route("LastSensorData")]
        public IHttpActionResult GetLastSensorData(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("emty token");

            var data = (Interfaces.ApiModels.SensorData[]) cache.Get(token);
            return Ok(data);

        }
        
        [HttpPost]
        [Route("AddGreenhouseData")]
        public IHttpActionResult AddDeviceData(GreenHouseDataMessage message)
        {
            //Example:
            /*{
                'DeviceToken': 'e0559f6b-30fa-47b1-9a07-915a015129a3',
                'SensorsData' :
                [
                    {
                    'SensorId' : 1,
                    'Value' : 31
                    }
                ]
            }*/


            if (message == null)
                return BadRequest();
            try
            {
                var responce = dataRepository.Write(message);

                var datasets = ChartHelper.GetDataSets(responce.SensorDataResponse);

                SensorDataHubProxy.AddData(responce.UserName, datasets);

                cache.Set(message.DeviceToken, message.SensorsData, 
                    new CacheItemPolicy {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeoutMinutes)
                        });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();

        }
    
    }

    
}
