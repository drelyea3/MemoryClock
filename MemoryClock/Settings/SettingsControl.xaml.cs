using Common;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
