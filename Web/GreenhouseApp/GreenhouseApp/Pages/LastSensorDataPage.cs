using GreenhouseApp.Models;
using GreenhouseApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenhouseApp.Pages
{
    public class LastSensorDataPage : ContentPage
    {
        private MainService mainService;
        CredentialsManager credentialManager;

        public LastSensorDataPage()
        {
            mainService = new MainService();
            mainService.OnSensorDataUpdated += MainService_OnSensorDataUpdated;
            credentialManager = new CredentialsManager();

            var button = new Button
            {
                Text = "Get sensor data",
                VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center},
                HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center}
            };
            button.Clicked += Button_Clicked;

            Content = button;

            Padding = new Thickness(5, 5, 5, 5);

            Device.OnPlatform(iOS: () =>
             {
                 Padding = new Thickness(5, 25, 5, 5);
             });
        }

        private void MainService_OnSensorDataUpdated(object sender, SensorDataEventArgs e)
        {
            var view = GetSensorDataView(e.UserSensors);
            Device.BeginInvokeOnMainThread(() => Content = view);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await mainService.Run();
        }

        private View GetSensorDataView(Dictionary<string, Dictionary<int, UserSensorWithData>> userSensors)
        {
            var sensorStack = new StackLayout();
            foreach (var device in userSensors)
            {
                sensorStack.Children.Add(new Label
                {
                    Text = mainService.GetDeviceByToken(device.Key)?.Name,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                });

                foreach (var sensorData in device.Value)
                {                    
                    sensorStack.Children.Add(new Label
                    {
                        FormattedText = new FormattedString
                        {
                            Spans =
                            {
                                new Span {  Text = sensorData.Value.Name },
                                new Span {  Text = "    " },
                                new Span {  Text = sensorData.Value.LastValue?.ToString(), FontAttributes = FontAttributes.Bold },
                                new Span {  Text = "    " },
                                new Span {  Text = sensorData.Value.SensorType?.Dimension,  FontAttributes = FontAttributes.Italic },
                            }
                        }
                    });
                }
            }

            var scrollView = new ScrollView();
            scrollView.Content = sensorStack;
            return scrollView;
        }

        
    }
}
