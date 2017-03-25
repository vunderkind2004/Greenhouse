using Greenhouse.Core;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using GreenHouse.Services;
using GreenHouse.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GreenHouse.Api
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IRepository<User> userRepository;
        private readonly DeviceManager deviceManager;

        public UserController(IRepository<User> userRepository, DeviceManager deviceManager)
        {
            this.userRepository = userRepository;
            this.deviceManager = deviceManager;
        }

        [HttpPost]
        [Route("GetDevices")]
        public async Task<IEnumerable<DeviceViewModel>> GetUserDevices(LoginUserViewModel credentials)
        {
            if (credentials == null)
                return null;

            var passwordHash = HashHelper.GetMd5Hash(credentials.Password);
            var user = (await userRepository.GetAsync(x => x.Login == credentials.Login && x.PasswordHash == passwordHash)).FirstOrDefault();
            if (user == null)
                return new DeviceViewModel[0];

            var devices = deviceManager.GetDevices(user.Id);
            return devices;
        }

    }
}
