using GreenHouse.Interfaces.ViewModels;
using GreenhouseApp.Models;
using GreenhouseApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenhouseApp.Services
{
    public class MainService
    {
        private readonly CredentialsManager credentialManager;
        private readonly CommunicationService communication;
        private IEnumerable<SensorType> allSensorTypes;
        private Dictionary<string, Dictionary<int, UserSensorWithData>> _userSensors;
        private Dictionary<string, DeviceViewModel> _devices;
        private volatile bool isCanceled;
        private int sampleIntervalSeconds = 10;

        public delegate void SensorDataUpdatedHandler(object sender, SensorDataEventArgs e);

        public event SensorDataUpdatedHandler OnSensorDataUpdated;


        public MainService()
        {
            credentialManager = new CredentialsManager();
            communication = new CommunicationService();
            _userSensors = new Dictionary<string, Dictionary<int, UserSensorWithData>>();
            _devices = new Dictionary<string, DeviceViewModel>();
        }

        public DeviceViewModel GetDeviceByToken(string token)
        {
            if (_devices.ContainsKey(token))
                return _devices[token];
            return null;
        }

        public async Task Run()
        {
            var credentials = credentialManager.GetSavedCredentials();
            if (credentials == null)
            {
                //TODO: credentials = AskUserForCredentials();
                credentials = new Models.Credentials { Login = "Oleksandr", Password = "test_123" };
            }
            var devices = await communication.GetUserDevices(credentials);
            if (devices == null || !devices.Any())
            {
                Debug.WriteLine("Error. No devices was found.");
                return;
            }

            allSensorTypes = await communication.GetAllSensors();

            foreach (var device in devices)
            {
                _devices.Add(device.Token, device);
                await SetUserSensors(device.Token);                
            }
            var tokens = devices.Select(x => x.Token).ToArray();

            RequstDataAndUpdateUI(tokens);

            Device.StartTimer(TimeSpan.FromSeconds(sampleIntervalSeconds), () =>
            {
                if (isCanceled)
                    return false;

                RequstDataAndUpdateUI(tokens);
                return true;
            });
            
        }

        private void RequstDataAndUpdateUI(string[] tokens)
        {
            RequestingAllData(tokens).Wait();
            RaiseOnSensorDataUpdated();
        }

        protected virtual void RaiseOnSensorDataUpdated()
        {
            var temp = OnSensorDataUpdated;
            if (temp != null)
            {
                temp(this, new SensorDataEventArgs (_userSensors) );
            }
        }

        public void Stop()
        {
            isCanceled = true;
        }

        private async Task RequestingAllData(string[] tokens)
        {        
            
            foreach (var token in tokens)
            {
                try
                {
                    await RequestingData(token);                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error while requesting data. \n" + ex.ToString());
                }
            }                        
            
        }

        private async Task RequestingData(string token)
        {
            var sensorData =await communication.GetActualSensorData(token);
            foreach (var sensor in _userSensors[token])
            {
                sensor.Value.LastValue = null;
            }
            if (sensorData == null)
                return;

            foreach (var data in sensorData)
            {
                _userSensors[token][data.SensorId].LastValue = data.Value;
            }
            
        }

        private async Task SetUserSensors(string token)
        {
            var userSensors = await communication.GetUserSensors(token);
            if (userSensors == null)
            {
                Debug.WriteLine("WARNING! userSensors==null");
                return;
            }
            Dictionary<int, UserSensorWithData> userSensorPerDevice = new Dictionary<int, UserSensorWithData>();
            foreach (var sensor in userSensors)
            {
                sensor.SensorType = allSensorTypes.FirstOrDefault(x => x.Id == sensor.SensorTypeId);
                userSensorPerDevice.Add(sensor.Id,new UserSensorWithData (sensor));
            }
            _userSensors.Add(token, userSensorPerDevice);
        }

        
    }
}
