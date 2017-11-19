using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DeviceSimulator
{
    class Program
    {
        private static string url = @"http://localhost:2898/api/device/AddGreenhouseData";
        private static string deviceToken = "5c87849c-ec57-4b8b-a000-b87aae1f8209";

        private static Timer timer;

        static void Main(string[] args)
        {
            Console.WriteLine("Device simulation");

            timer = new Timer(GenerateAndSendData,null,0,10*1000);

            Console.ReadLine();

        }

        private static void GenerateAndSendData(object state)
        {
            //var data = GetOleksandrData();

            var data = GetOfficeData();

            var message = JsonConvert.SerializeObject(data);
            Console.WriteLine("Sending data...");
            Console.WriteLine(message);
            SendData(message);
        }

        private static object GetOfficeData()
        {
            var objects = new List<object>();
            for (var i = 8; i <= 14; i++)
            {
                var t = Math.Round( (decimal) (new Random()).Next(2000, 2500) / 100 , 2);
                objects.Add(new
                {
                    SensorId = i,
                    Value = t
                });
                Thread.Sleep(500);
            }

            var data = new
            {
                DeviceToken = "aed33c85-0583-403f-986c-4e8816f3d5f9",
                SensorsData = objects.ToArray()
            };

            return data;
        }

        private static object GetOleksandrData()
        {
            var t1 = (new Random()).Next(20, 25);
            Thread.Sleep(200);
            var t2 = (new Random()).Next(20, 25);
            Thread.Sleep(200);
            var t3 = (new Random()).Next(20, 25);

            var data = new
            {
                DeviceToken = deviceToken,
                SensorsData = new object[] { new
                        {
                            SensorId = 2,
                            Value = t1
                        },
                        new
                        {
                            SensorId = 3,
                            Value = (new Random()).Next(30,60)
                        },
                        new
                        {
                            SensorId = 4,
                            Value = t2
                        },
                        new
                        {
                            SensorId = 5,
                            Value = (new Random()).Next(740,760)
                        },
                        new
                        {
                            SensorId = 7,
                            Value = t3
                        },
                    }
            };
            return data;
        }

        private static void SendData(string message)
        {
            var client = new HttpClient();
            try
            {
                var result = client.PostAsync(url, new StringContent(message, Encoding.Unicode, "application/json")).Result;
                if (!result.IsSuccessStatusCode) 
                    Console.WriteLine(result.ReasonPhrase);
                else
                {
                    Console.WriteLine("OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
