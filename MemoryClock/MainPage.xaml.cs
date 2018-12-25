using Common;
using MemoryClock.Commands;
using System;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

            Loaded += (o, e) => ((App)App.Current).StartWorkers();

            display.SetMover(TimeSpan.FromSeconds(2));
        }

        private void OnCommand(CustomRoutedCommand command, object parameter)
        {
            command.Handled = true;

            if (command is OpenSettingsCommand)
            {
                FindName(nameof(settings));
                Global.PushSettings(Clone.Create(Global.Settings));
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
            settingsButton.Visibility = IsAccessGranted ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
