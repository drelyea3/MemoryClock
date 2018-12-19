using Common;
using MemoryClock.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MemoryClock
{
    public sealed partial class MainPage : Page
    {
        private bool IsAccessAllowed { get; set; }
        private bool IsAccessGranted { get; set; }

        public MainPage()
        {
            //Global.DeleteSettings();
            Global.Initialize();

            this.InitializeComponent();

            CustomRoutedCommand.SetHandler(this, OnCommand);
            //if (System.Diagnostics.Debugger.IsAttached)
            {
                CustomRoutedCommand.Raise(this, new AccessGrantedCommand(), null);
            }
            UpdateAccess(true);
        }

        private void OnCommand(CustomRoutedCommand command, object parameter)
        {
            command.Handled = true;

            if (command is OpenSettingsCommand)
            {
                FindName(nameof(settings));

                settings.Show();
                UpdateAccess(false);
            }
            else if (command is CloseSettingsCommand)
            {
                FindName(nameof(settings));

                settings.Hide();
                UpdateAccess(true);
            }
            else if (command is StartTestingCommand)
            {
                FindName(nameof(settings));
                FindName(nameof(testTime));

                settings.Hide();
                testTime.Visibility = Visibility.Visible;
                testTime.IsActive = true;
            }
            else if (command is StopTestingCommand)
            {
                FindName(nameof(settings));
                FindName(nameof(testTime));

                settings.Show();
                testTime.Visibility = Visibility.Collapsed;
                testTime.IsActive = false;
            }
            else if (command is AccessGrantedCommand)
            {
                IsAccessAllowed = true;
                UpdateAccess(true);
            }
            else if (command is AccessRevokedCommand)
            {
                IsAccessAllowed = false;
                UpdateAccess(false);
            }
            else
            {
                command.Handled = false;
            }
        }

        private void UpdateAccess(bool isDesired)
        {
            IsAccessGranted = isDesired && IsAccessAllowed;
            settingsButton.Visibility =  IsAccessGranted ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
