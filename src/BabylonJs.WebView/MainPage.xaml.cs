using HybridWebApp.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BabylonJs.WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _LastJsonLoaded;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void WebHost_Ready(object sender, EventArgs e)
        {
            WebHost.WebRoute.Map("/", async (uri, success, errorCode) =>
            {
                if (success)
                {
                    await WebHost.Interpreter.LoadFrameworkAsync(HybridWebApp.Framework.WebToHostMessageChannel.IFrame);

                    await WebHost.Interpreter.EvalAsync("app.initScene('canvas');");

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { CommandBar.IsOpen = true; });
                }

            }, false, HybridWebApp.Framework.RouteTiming.Navigated);

            WebHost.Navigate(new Uri("ms-appx-web:///www/app.html"));
        }

        private void WebHost_MessageReceived(HybridWebApp.Toolkit.Controls.HybridWebView sender, HybridWebApp.Framework.Model.ScriptMessage args)
        {
            switch (args.Type)
            {
                case KnownMessageTypes.Log:
                    {
                        System.Diagnostics.Debug.WriteLine(args.Payload);
                        break;
                    }
            }
        }

        private async void ImportStlFile_Click(object sender, RoutedEventArgs e)
        {
            //clear old data
            _LastJsonLoaded = string.Empty;

            await WebHost.Interpreter.EvalAsync("app.clearModelFromScene();");

            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".stl");
            filePicker.FileTypeFilter.Add(".amf");

            var file = await filePicker.PickSingleFileAsync();

            await _LoadFileAsync(file);
        }

        private async void SaveConverted_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add("Babylon", new string[] { ".babylon" });

            var saveFile = await savePicker.PickSaveFileAsync();

            if (saveFile != null)
            {
                var bufferArray = System.Text.Encoding.UTF8.GetBytes(_LastJsonLoaded);
                var f = await saveFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                await f.WriteAsync(WindowsRuntimeBuffer.Create(bufferArray, 0, bufferArray.Length, bufferArray.Length));
            }
        }

        private async Task _LoadFileAsync(StorageFile file)
        {
            if (file != null)
            {
                ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ProgressBar.IsEnabled = true;

                var converter = await ConversionFactory.GetConverter(file);
                _LastJsonLoaded = await converter.ToJsonAsync();

                await WebHost.Interpreter.EvalAsync(string.Format("app.loadBabylonModel('{0}');", _LastJsonLoaded));

                ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ProgressBar.IsEnabled = false;

                SaveButton.IsEnabled = true;
            }
        }

    }
}
