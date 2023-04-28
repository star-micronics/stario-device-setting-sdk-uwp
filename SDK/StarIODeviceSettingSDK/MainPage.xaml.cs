using StarMicronics.StarIODeviceSetting;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StarIODeviceSettingSDK
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Load SteadyLAN Setting");

            StarNetworkManager starNetworkManager = new StarNetworkManager(PortName.Text); //Please refer to the SDK manual for portName argument which using for communicating with the printer.(https://www.star-m.jp/products/s_print/sdk/starprnt_sdk/manual/uwp_csharp/en/api_stario_port.html#getport)
            StarNetworkSetting starNetworkSetting = new StarNetworkSetting();

            try
            {
                starNetworkSetting = await starNetworkManager.LoadAsync();
             
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("SteadyLAN Setting : " + starNetworkSetting.SteadyLan.ToString(), "Communication Result");

                await dialog.ShowAsync();
            }
            catch (StarIODeviceSettingException ex)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Communication Result");

                await dialog.ShowAsync();
            }
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Apply SteadyLAN Setting");

            StarNetworkManager starNetworkManager = new StarNetworkManager(PortName.Text);//Please refer to the SDK manual for portName argument which using for communicating with the printer.(https://www.star-m.jp/products/s_print/sdk/starprnt_sdk/manual/uwp_csharp/en/api_stario_port.html#getport)
            StarNetworkSetting starNetworkSetting = new StarNetworkSetting();

            switch (steadyLANSettingComboBox.SelectedIndex)
            {
                default:
                case 0: // Unspecified
                    starNetworkSetting.SteadyLan = SteadyLanSetting.Unspecified;
                    break;

                case 1: // Disable
                    starNetworkSetting.SteadyLan = SteadyLanSetting.Disable;
                    break;

                case 2: // Enable(for iOS)
                    starNetworkSetting.SteadyLan = SteadyLanSetting.iOS;
                    break;

                case 3: // Enable(for Android)
                    starNetworkSetting.SteadyLan = SteadyLanSetting.Android;
                    break;

                case 4: // Enable(for Windows)
                    starNetworkSetting.SteadyLan = SteadyLanSetting.Windows;
                    break;
            }

            try
            {
                await starNetworkManager.ApplyAsync(starNetworkSetting);

                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Data transmission succeeded. \nPlease confirm the current settings by LoadAsync method after a printer reset is executed.", "Communication Result");

                await dialog.ShowAsync();
            }
            catch (StarIODeviceSettingException ex)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Communication Result");

                await dialog.ShowAsync();
            }
        }
    }
}
