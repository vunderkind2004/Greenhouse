using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenHouse.Interfaces.ApiModels;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Interfaces.Responses;
using GreenHouse.Repository.DataModel;
using GreenHouse.Repository.Exceptions;
using ServiceStack.OrmLite;

namespace GreenHouse.Repository.Repository
{
    public class DeviceDataRepository : IDeviceDataRepository
    {

        private readonly string connectionString;

        public DeviceDataRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Write(GreenHouseDataMessage message)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                var device = db.Single<Device>(x => x.Token == message.DeviceToken);
                if (device == null)
                    throw new DeviceNotFoundException();

                var sensorIds = message.SensorsData.Select(x=>x.SensorId).ToArray();                
                var sensors = db.Select<Sensor>(s => sensorIds.Contains(s.Id));
                if(sensors.Any(x=>x.DeviceId!=device.Id) || sensorIds.Count()!=sensors.Count())
                    throw new WrongSensorIdException();
                var data = message.SensorsData.Select(x => new GreenHouse.Repository.DataModel.SensorData
                    {
                        //DeviceId = device.Id,
                        EventDateTime = DateTime.Now,
                        SensorId = x.SensorId,
                        Value = x.Value
                    }
                );               
                db.InsertAll(data);
            }
        }

        
        private OrmLiteConnectionFactory GetFactory()
        {
            return new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);
        }
    }
}
