using GreenHouse.Interfaces.ViewModels;
using GreenhouseApp.Constants;
using GreenhouseApp.Models;
using GreenhouseApp.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseApp.Services
{
    public class CommunicationService
    {

        public async Task<IEnumerable<DeviceViewModel>> GetUserDevices(Credentials credentials)
        {
            var devices = await Post<IEnumerable<DeviceViewModel>>("/api/user/GetDevices", credentials);
            return devices;
        }

        public async Task<IEnumerable<SensorType>> GetAllSensors()
        {
            var sensors = await Get<IEnumerable<SensorType>>("/api/sensor");
            return sensors;
        }

        public async Task<IEnumerable<SensorViewModel>> GetUserSensors(string token)
        {
            var userSensors = await GetDataByToken<IEnumerable<SensorViewModel>>("/api/sensor/userSensors", token);
            return userSensors;
        }

        

        public async Task<IEnumerable<SensorData>> GetActualSensorData(string deviceToken)
        {
            var sensorData = await GetDataByToken<IEnumerable<SensorData>>("/api/device/LastSensorData", deviceToken);
            return sensorData;
        }

        

        private async Task<T> Get<T>(string url, Dictionary<string, object> parameters = null)
        {
            var client = GetClient();

            if (parameters != null)
            {
                bool isFirst = true;
                foreach (var p in parameters)
                {
                    if (isFirst)
                    {
                        url += $"?{p.Key}={p.Value}";
                    }
                    else
                    {
                        url += $"&{p.Key}={p.Value}";
                    }
                }
            }

            try
            {
                var requestTask = client.GetAsync(new Uri(url));
                var response = Task.Run(() => requestTask).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Responseis not ok for url=" + url + " " + response.ReasonPhrase);
                    return default(T);
                }

                var data = await response.Content.ReadAsStringAsync();

                var returnObject = JsonConvert.DeserializeObject<T>(data);
                return returnObject;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while GET data. for url=" + url + "\n" + ex.ToString());
                return default(T); ;
            }
        }

        private HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Configuration.ApiUrl);
            return client;
        }

        private async Task<T> GetDataByToken<T>(string url, string token)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("token", token);
            var data = await Get<T>(url, parameters);
            return data;
        }

        private async Task<T> Post<T>(string url, Object body)
        {
            var client = GetClient();
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(new Uri(url), content);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Responseis not ok for url=" + url + " " + response.ReasonPhrase);
                    return default(T);
                }

                var data = await response.Content.ReadAsStringAsync();

                var returnObject = JsonConvert.DeserializeObject<T>(data);
                return returnObject;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while POST data. for url=" + url + "\n" + ex.ToString());
                return default(T); ;
            }
        }
    }
}
