﻿using System;
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
                var sensor = db.Single<Sensor>(x => x.Id == message.SensorId);
                if (sensor == null)
                    throw new SensorNotFoundException();
                var data = new GreenHouse.Repository.DataModel.SensorData
                {
                    //DeviceId = device.Id,
                    EventDateTime = DateTime.Now,
                    SensorId = sensor.Id,
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
                var allData = db.LoadSelect<GreenHouse.Repository.DataModel.SensorData>()
                    .OrderByDescending(x=>x.EventDateTime)
                    .Skip(skipCount)
                    .Take(takeCount);

                var devices = db.Select<Device>(x => allData.Select(d => d.Sensor.DeviceId).Contains(x.Id));

                var sensorTypes = db.Select<SensorType>(x => allData.Select(st => st.Sensor.SensorTypeId).Contains(x.Id));
                
                var response = allData.Select(x => new SensorDataResponse 
                {
                    EventDateTime = x.EventDateTime,
                    DeviceName = devices.First(d=>d.Id == x.Sensor.DeviceId).Name,
                    TypeName = sensorTypes.First(sType => sType.Id == x.Sensor.SensorTypeId).TypeName,
                    Value = x.Value,
                    Dimension = sensorTypes.First(sType => sType.Id == x.Sensor.SensorTypeId).Dimension,
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
