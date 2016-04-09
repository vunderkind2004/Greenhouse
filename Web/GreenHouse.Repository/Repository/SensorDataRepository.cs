using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenHouse.Interfaces.ApiModels;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Interfaces.Responses;
using GreenHouse.Repository.DataModel;
using ServiceStack.OrmLite;

namespace GreenHouse.Repository.Repository
{
    public class SensorDataRepository : ISensorDataRepository
    {

        private readonly string connectionString;

        public SensorDataRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void WriteSensorData(SensorDataMessage message)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                var device = db.Single<Device>(x => x.Token == message.Token);
                if (device == null)
                    throw new DeviceNotFoundException();
                var sensor = db.Single<SensorType>(x => x.Id == message.SensorType);
                if (sensor == null)
                    throw new SensorTypeNotFoundException();
                var data = new SensorData
                {
                    DeviceId = device.Id,
                    EventDateTime = DateTime.Now,
                    SensorTypeId = sensor.Id,
                    Value = message.Value
                };
                db.Insert(data);
            }
        }

        public IEnumerable<SensorDataResponse> GetData(int skipCount, int takeCount)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                var allData = db.LoadSelect<SensorData>()
                    .OrderByDescending(x=>x.EventDateTime)
                    .Skip(skipCount)
                    .Take(takeCount);
                var response = allData.Select(x => new SensorDataResponse 
                {
                    EventDateTime = x.EventDateTime,
                    DeviceName = x.Device.Name,
                    TypeName = x.SensorType.TypeName,
                    Value = x.Value,
                    Dimension = x.SensorType.Dimension
                });
                return response;
            }
        }

        private OrmLiteConnectionFactory GetFactory()
        {
            return new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);
        }




        

    }
}
