using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace StarSteadyLANSettingLabs
{
    public sealed partial class MainPage : Page
    {
        private string portName;
        private string modeName;
        private string macAddress;

        public MainPage()
        {
            this.InitializeComponent();
        }



        private async void applyButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Apply SteadyLAN Setting");
            CommunicationResult result;
            string message = null;
            byte[] commands;


            if (steadyLANSettingComboBox.SelectedIndex == 1)
            {
                // IMPORTANT: SteadyLAN function is not available from the UWP application.
                //            Because StarIO of UWP is not supported the USB I/F.
                commands = new byte[]{ 0x1b, 0x1d, 0x29, 0x4e, 0x03, 0x00, 0x39, 0x01, 0x03,  //set to SteadyLAN(for Windows)
                                        0x1b, 0x1d, 0x29, 0x4e, 0x03, 0x00, 0x70, 0x01, 0x00}; //apply setting. Note: The printer is reset to apply setting when writing this command is completed.

                //The settings for other OSs are as follows. But it will not work on Windows Devices.
            //  commands = new byte[]{ 0x1b, 0x1d, 0x29, 0x4e, 0x03, 0x00, 0x39, 0x01, 0x01,  //set to SteadyLAN(for iOS)
            //                          0x1b, 0x1d, 0x29, 0x4e, 0x03, 0x00, 0x70, 0x01, 0x00}; //apply setting. Note: The printer is reset to apply setting when writing this command is completed.

            //  commands = new byte[]{ 0x1b, 0x1d, 0x29, 0x4e, 0x03, 0x00, 0x39, 0x01, 0x02,  //set to SteadyLAN(for Android)
            //                          0x1b, 0x1d, 0x29, 0x4e, 0x03, 0x00, 0x70, 0x01, 0x00}; //apply setting. Note: The printer is reset to apply setting when writing this command is completed.

            }
            else
            {
                commands = new byte[]{ 0x1b, 0x1d, 0x29, 0x4e, 0x03, 0x00, 0x39, 0x01, 0x00,  //set to SteadyLAN(Disable)
                                        0x1b, 0x1d, 0x29, 0x4e, 0x03, 0x00, 0x70, 0x01, 0x00}; //apply setting. Note: The printer is reset to apply setting when writing this command is completed.
            }

            result = await Communication.SendCommands(commands.AsBuffer(), portName, "", 10000);

            message = Communication.GetCommunicationResultMessage(result);

            if (message != null)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(message, "Communication Result");

                await dialog.ShowAsync();
            }

        }

        private async void readSettingButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Read SteadyLAN Setting");
            SteadyLANSettingResult steadyLANSettingResult = new SteadyLANSettingResult();

            steadyLANSettingResult = await Communication.ConfirmSteadyLANSetting(portName, "", 10000);

            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(steadyLANSettingResult.Message, steadyLANSettingResult.Title);

            await dialog.ShowAsync();
        }


        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Search Star Printer");

            StarIOPort.ProductInformationCollection productCollection = null;

            try
            {
                productCollection = await StarIOPort.ProductInformation.FindAllAsync(); //ALL
//              productCollection = await StarIOPort.ProductInformation.FindAllAsync(StarIOPort.PrinterInterfaceType.LAN); //LAN
//              productCollection = await StarIOPort.ProductInformation.FindAllAsync(StarIOPort.PrinterInterfaceType.Bluetooth); //Bluetooth

                ObservableCollection<ProductList> collection = new ObservableCollection<ProductList>();

                foreach (var item in productCollection)
                {
                    collection.Add(new ProductList(item));
                }

                ProductListSource.Source = collection;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }


        private void ProductListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ProductListBox.SelectedItem != null)
            {
                ProductList selectedPortInfo = (ProductList)ProductListBox.SelectedItem;

                this.portName = selectedPortInfo.PortName;
                this.modeName = selectedPortInfo.ModelName;
                this.macAddress = selectedPortInfo.MacAddress;
            }

        }
    }
}
