﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greenhouse.Core;
using GreenHouse.Interfaces.ApiModels;
using GreenHouse.Interfaces.Enums;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Interfaces.Requests;
using GreenHouse.Interfaces.Responses;

namespace GreenHouse.EfRepository.Repositories
{
    public class EfSensorDataRepository : ISensorDataRepository
    {
        private GreenHouseDbEntities context;

        public EfSensorDataRepository()
        {
            context = new GreenHouseDbEntities();
        }

        public void WriteSensorData(SensorDataMessage message)
        {
            var sensor = context.Sensors.FirstOrDefault(x => x.Id == message.SensorId && x.Device.Token == message.Token);
            if (sensor == null)
                throw new SensorIdOrTokenNotFoundException();
            var data = new SensorData
            {
                EventDateTime = DateTime.Now,
                SensorId = sensor.Id,
                Value = message.Value
            };
            context.SensorData.Add(data);
            context.SaveChanges();
        }             

        public GetGroupedSensorDataResponse GetData(GetGroupedSensorDataRequest request)
        {
            var sensorIds = context.Users.Where(x => x.Login == request.UserName)
                .SelectMany(x => x.Devices
                    .SelectMany(y => y.Sensors.Select(z => z.Id))).ToArray();
            var data = context.SensorDataView
                .Where(x => x.EventDateTime >= request.FromDate && x.EventDateTime <= request.ToDate
                && sensorIds.Contains(x.SensorId))
                .AsNoTracking();

            if (request.FilterByTime == FilterByTime.Day)
                data = data.Where(x => x.time >= Constants.SunRize && x.time < Constants.SunSet);
            else if (request.FilterByTime == FilterByTime.Night)
                data = data.Where(x => x.time >= Constants.SunSet && x.time < Constants.SunRize);


            var grouped = GroupSensorDataQuery(data, request.GroupByTime).ToList();

            var lables = context.Sensors.Where(x => sensorIds.Contains(x.Id))
                .Select(x => new { SensorId = x.Id, Lable = x.Device.Name + "_" + x.Name })
                .ToDictionary(x=>x.SensorId,x=>x.Lable);

            var response = new GetGroupedSensorDataResponse 
                { 
                    GroupedData = grouped,
                    SensorIdLabels = lables 
                };
            return response;
             
        }
        
        private IQueryable<SensorDataGrouped> GroupSensorDataQuery(IQueryable<SensorDataView> data, GroupByTime groupByTime)
        {
            IQueryable<SensorDataGrouped> grouped = null;
            switch (groupByTime)
            {
                case GroupByTime.Minute:
                    grouped = data.GroupBy(
                        x => new { x.SensorId, x.date, x.hour, x.minute },
                        (group, groupedData) =>
                        new SensorDataGrouped
                        {
                            Date = group.date,
                            Hour = group.hour,
                            Minute = group.minute,
                            AverageValue = groupedData.Average(x => x.Value),
                            SensorId = group.SensorId,
                        });
                    break;
                case GroupByTime.Hour:
                    grouped = data.GroupBy(
                        x => new { x.SensorId, x.date, x.hour },
                        (group, groupedData) =>
                        new SensorDataGrouped
                        {
                            Date = group.date,
                            Hour = group.hour,
                            AverageValue = groupedData.Average(x => x.Value),
                            SensorId = group.SensorId,
                        });
                    break;
                case GroupByTime.Daily:
                    grouped = data.GroupBy(x =>
                        new { x.SensorId, x.date},
                        (group, groupedData) =>
                        new SensorDataGrouped
                        {
                            Date = group.date,
                            AverageValue = groupedData.Average(x => x.Value),
                            SensorId = group.SensorId,
                        });
                    break;
                case GroupByTime.Week:
                    grouped = data.GroupBy(x =>
                        new { x.SensorId,x.year, x.week},
                        (group, groupedData) =>
                        new SensorDataGrouped
                        {
                            Year = group.year,
                            Week = group.week,
                            AverageValue = groupedData.Average(x => x.Value),
                            SensorId = group.SensorId,
                        });
                    break;
                case GroupByTime.Month:
                    grouped = data.GroupBy(x =>
                        new { x.SensorId,x.year, x.month},
                        (group, groupedData) =>
                        new SensorDataGrouped
                        {
                            Year = group.year,
                            Month = group.month,
                            AverageValue = groupedData.Average(x => x.Value),
                            SensorId = group.SensorId,
                        });
                    break;
                default:
                    break;
            }

            return grouped;
        }

    }

    
}