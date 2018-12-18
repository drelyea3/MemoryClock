using Common;
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

namespace MemoryClock.Settings
{
    public sealed partial class SettingsControl : UserControl, ISettingsPage
    {
        ISettingsPage selectedPage;

        public SettingsControl()
        {
            this.InitializeComponent();
        }

        private void OnOKClick(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;

            if (selectedPage == null)
            {                
                var firstButton = buttonHost.GetChildren<RadioButton>().First();
                if (firstButton != null)
                {
                    firstButton.IsChecked = true;
                }
            }

            selectedPage?.Show();
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void Cancel()
        {
            foreach (var page in host.GetChildren<ISettingsPage>())
            {
                page.Cancel();
            }
        }

        public void Save()
        {
            foreach (var page in host.GetChildren<ISettingsPage>())
            {
                page.Save();
            }
        }

        private void OnPageSelected(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;

            selectedPage?.Hide();

            foreach (FrameworkElement page in host.GetChildren())
            {
                if (page.Name == element.Tag.ToString())
                {
                    selectedPage = (ISettingsPage)page;
                    selectedPage.Show();
                    break;
                }
            }
        }
    }
}
