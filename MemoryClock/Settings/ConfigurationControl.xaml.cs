using System;
using System.Collections.Generic;
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

namespace MemoryClock.Settings
{
    public sealed partial class ConfigurationControl : UserControl, ISettingsPage
    {
        public ConfigurationControl()
        {
            this.InitializeComponent();
        }

        public void Cancel()
        {
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void Save()
        {
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }
    }
}
