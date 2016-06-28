using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenHouse.Interfaces.Responses;
using GreenHouse.ViewModels;

namespace GreenHouse.Helpers
{
    public static class ChartHelper
    {
        public static SensorDataViewModel GetDataSets(IEnumerable<SensorDataResponse> data)
        {
            var timeData = data.Select(x => x.EventDateTime).ToArray().Distinct();
            var datasetNames = data.Select(x => x.DataSetName).ToList().Distinct();

            var datasets = new Dictionary<string, DataSet>();

            foreach (var name in datasetNames)
            {
                var color = name.ToLower().Contains("temperature") ? "rgba(250, 0, 0,0.4)" : "rgba(0, 0, 200,0.4)";
                datasets.Add(name, new GreenHouse.ViewModels.DataSet
                {
                    Label = name,
                    Data = new float?[timeData.Count()],
                    LineColor = color,
                    PointBorderColor = color
                });
            }

            var i = 0;
            foreach (var time in timeData)
            {
                var items = data.Where(x => x.EventDateTime == time);

                foreach (var item in items)
                {
                    var datasetName = item.DataSetName;

                    datasets[datasetName].Data[i] = item.Value;
                }
                i++;
            }
                        
            var model = new SensorDataViewModel
            {
                Timestamps = timeData.Select(x => x.ToString()).ToArray(),
                DataSets = datasets.ToList().Select(x => x.Value).ToArray()
            };
            return model;
        }

        public static SensorDataViewModel GetDataSets(GetGroupedSensorDataResponse gropedData)
        {
            var timeStamps = gropedData.GroupedData.Select(x => x.EventTime.ToString()).Distinct().ToArray();
            var dataSets = gropedData.GroupedData
                .GroupBy(x => x.SensorId)
                .Select(x => new DataSet
                {
                    Data = x.Select(y => (float?)y.AverageValue).ToArray(),
                    Label = gropedData.SensorIdLabels[x.Key]                    
                }).ToArray();

            var model = new SensorDataViewModel
            {
                Timestamps = timeStamps,
                DataSets = dataSets
            };
            return model;
        }                
    
    }
}