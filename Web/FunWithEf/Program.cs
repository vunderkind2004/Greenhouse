using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenHouse.EfRepository;
using GreenHouse.EfRepository.Repositories;
using GreenHouse.Interfaces.Enums;
using GreenHouse.Interfaces.Requests;
using GreenHouse.Interfaces.Responses;

namespace FunWithEf
{
    class Program
    {
        static void Main(string[] args)
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            var repository = new EfSensorDataRepository();

            test(repository);

            //Console.WriteLine("--------------------");

            //test(repository);

            Console.Read();
        }

        static void test(EfSensorDataRepository repository)
        {
            
            //var data = repository.GetData("Oleksandr", 1, 10);

            var startTime = new DateTime(2016, 05, 26);
            var request = new GetGroupedSensorDataRequest
            {
                UserName = "Oleksandr",
                FromDate = startTime,
                ToDate = startTime.AddDays(3),
                GroupByTime = GroupByTime.Hour,
                FilterByTime = FilterByTime.Day
            };
            var data = repository.GetData(request);

            data.SensorIdLabels.Count();

            //var data = repository.GetData("Oleksandr", startTime, startTime.AddDays(1), GroupByTime.Hour);
            //PrintData(data);

            //data = repository.GetData("Oleksandr", startTime, startTime.AddDays(3), GroupByTime.DayNight);
            //PrintData(data);

            //data = repository.GetData("Oleksandr", startTime, startTime.AddDays(3), GroupByTime.Daily);
            //PrintData(data);

            //data = repository.GetData("Oleksandr", startTime.AddDays(-14), startTime.AddDays(3), GroupByTime.Week);
            //PrintData(data);

            //data = repository.GetData("Oleksandr", startTime.AddDays(-40), startTime.AddDays(3), GroupByTime.Month);
            //PrintData(data);

            
        }

        static void PrintData(IEnumerable<SensorDataResponse> data)
        {
            Console.WriteLine("sensor data:");

            foreach (var item in data)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", item.EventDateTime, item.Value, item.SensorName, item.Dimension);
            }
        }

        static void PrintData(IEnumerable<SensorData> data)
        {
            Console.WriteLine("sensor data:");

            foreach (var item in data)
            {
                Console.WriteLine("{0}\t{1}\t{2}", item.EventDateTime, item.Value, item.SensorId);
            }
        }
    }
}
