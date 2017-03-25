using GreenhouseApp.Pages;
using GreenhouseApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenhouseApp
{
    public class App : Application
    {
        CredentialsManager credentialManager;

        public App()
        {
            // The root page of your application
            //var content = new ContentPage
            //{
            //    Title = "GreenhouseApp",
            //    Content = new StackLayout
            //    {
            //        VerticalOptions = LayoutOptions.Center,
            //        Children = {
            //            new Label {
            //                HorizontalTextAlignment = TextAlignment.Center,
            //                Text = "Welcome to Xamarin Forms!"
            //            }
            //        }
            //    }
            //};

            //MainPage = new NavigationPage(content);

            //credentialManager = new CredentialsManager();
            //Task.Run(() => Login());
            MainPage = new LastSensorDataPage();
        }

        private async Task Login()
        {
            var credentials = credentialManager.GetSavedCredentials();
            if (credentials == null)
            {
                //TODO: credentials = AskUserForCredentials();
                credentials = new Models.Credentials { Login = "Oleksandr", Password = "test_123" };
            }
            var communication = new CommunicationService();
            var devices = await communication.GetUserDevices(credentials);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
