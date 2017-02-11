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
        private static string deviceToken = "0fe4a435-81a4-49f1-b99d-1bcc7a0ca6de";

        private static Timer timer;

        static void Main(string[] args)
        {
            Console.WriteLine("Device simulation");

            timer = new Timer(GenerateAndSendData,null,0,30*1000);

            Console.ReadLine();

        }

        private static void GenerateAndSendData(object state)
        {
            var data = new
            {
                DeviceToken = deviceToken,
                SensorsData = new object[] { new
                        {
                            SensorId = 2,
                            Value = (new Random()).Next(20,25)
                        },
                        new
                        {
                            SensorId = 3,
                            Value = (new Random()).Next(30,60)
                        },
                        new
                        {
                            SensorId = 4,
                            Value = (new Random()).Next(20,25)
                        },
                        new
                        {
                            SensorId = 5,
                            Value = (new Random()).Next(740,760)
                        },
                    }
            };
            var message = JsonConvert.SerializeObject(data);
            Console.WriteLine("Sending data...");
            Console.WriteLine(message);
            SendData(message);
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
