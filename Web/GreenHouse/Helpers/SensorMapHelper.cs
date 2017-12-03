using AutoMapper;
using GreenHouse.Models;
using GreenHouse.Repository.DataModel;
using GreenHouse.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenHouse.Helpers
{
    public static class SensorMapHelper
    {
        public static SensorMapViewModel GetSensorMap (IEnumerable<SensorViewModel> sensors, IEnumerable<SensorType> sensorTypes)
        {
            if (sensors == null || !sensors.Any())
                return null;
            var sensorInfos = Mapper.Map<IEnumerable<SensorMapInfo>>(sensors);

            CalculateCoordinates(sensorInfos);
            SetDimensions(sensorInfos, sensorTypes);

            var sensorMap = new SensorMapViewModel
            {
                SensorMapInfo = sensorInfos,
                TotalColumnsCount = sensorInfos.Max(x => x.Column) + 1,
                TotalRowsCount = sensorInfos.Max(y => y.Row) + 1,
            };

            return sensorMap;
        }

        private static void SetDimensions(IEnumerable<SensorMapInfo> sensorInfos, IEnumerable<SensorType> sensorTypes)
        {
            foreach (var sensorInfo in sensorInfos)
            {
                sensorInfo.Dimension = sensorTypes
                    .First(x => x.Id == sensorInfo.TypeId)
                    .Dimension;
            }
        }

        private static void CalculateCoordinates(IEnumerable<SensorMapInfo> sensorInfos)
        {
            foreach (var sensorInfo in sensorInfos)
            {
                if (string.IsNullOrEmpty(sensorInfo.Location))
                    continue;
                try
                {
                    var location = JsonConvert.DeserializeObject<SensorLocation>(sensorInfo.Location);
                    if (location != null)
                    {
                        sensorInfo.Row = location.Y;
                        sensorInfo.Column = location.X;
                    }
                }
                catch (Exception ex)
                {
                    sensorInfo.Name = ex.Message;
                }
            }
        }
    }
}