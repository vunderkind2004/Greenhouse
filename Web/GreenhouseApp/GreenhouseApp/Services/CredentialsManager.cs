using GreenhouseApp.Interfaces;
using GreenhouseApp.Models;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace GreenhouseApp.Services
{
    public class CredentialsManager
    {
        readonly IFileHelper fileHelper;
        readonly string fileName;

        public CredentialsManager()
        {
            fileHelper = DependencyService.Get<IFileHelper>();
            fileName = Constants.Configuration.LoginFileName;
        }

        public Credentials GetSavedCredentials()
        {
            if (!fileHelper.Exists(fileName))
                return null;
            var data = fileHelper.ReadText(fileName);
            if (string.IsNullOrEmpty(data))
                return null;
            var credentials = JsonConvert.DeserializeObject<Credentials>(data);
            return credentials;
        }

        public void SaveCredentials(Credentials credentials)
        {
            var data = JsonConvert.SerializeObject(credentials);
            fileHelper.WriteText(fileName,data);
        }
    }
}
