using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Greenhouse.Core;
using GreenHouse.Interfaces;
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


            var grouped = GroupSensorDataQuery(data, request.GroupByTime)
                .ToList();

            var dates = grouped.Select(x => x.EventTime).Distinct().ToList();

            EnrichWithEmptyPoints(grouped, dates, sensorIds);

            var lables = context.Sensors.Where(x => sensorIds.Contains(x.Id))
                .Select(x => new
                {
                    SensorId = x.Id,
                    DeviceName = x.Device.Name,
                    SensorName = x.Name,
                    Dimension = x.SensorType.Dimension
                })
                .ToDictionary(x=>x.SensorId,x=>
                    NameHelper.GetDataChartLineName(x.DeviceName, x.SensorName, x.Dimension)
                    );

            var response = new GetGroupedSensorDataResponse 
                { 
                    GroupedData = grouped.OrderBy(x=>x.EventTime),
                    SensorIdLabels = lables 
                };
            return response;
             
        }

        private void EnrichWithEmptyPoints(List<SensorDataGrouped> grouped, List<DateTime> dates, int[] sensorIds)
        {
            foreach (var dateTime in dates)
            {
                foreach (var sensorId in sensorIds)
                {
                    if (!grouped.Any(x => x.SensorId == sensorId && x.EventTime == dateTime))
                    {
                        grouped.Add(new SensorDataGrouped
                        {
                            Date = dateTime,
                            SensorId = sensorId
                        });
                    }
                }
            }
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
