using GreenhouseApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseApp.Models
{
    public class UserSensorWithData : SensorViewModel
    {
        public UserSensorWithData(SensorViewModel viewModel)
        {
            this.DeviceId = viewModel.DeviceId;
            this.Id = viewModel.Id;
            this.Location = viewModel.Location;
            this.Name = viewModel.Name;
            this.SensorType = viewModel.SensorType;
            SensorTypeId = viewModel.SensorTypeId;
        }
        public float? LastValue { get; set; }          
    }
}
