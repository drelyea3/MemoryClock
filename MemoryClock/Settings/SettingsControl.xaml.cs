using Common;
using MemoryClock.Commands;
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
            CustomRoutedCommand.SetHandler(this, OnCommand);
        }

        private void OnCommand(CustomRoutedCommand command, object parameter)
        {
            if (command is StartTestingCommand)
            {
                // Don't mark as handled
                Save();
            }
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
            Global.PopSettings();
            foreach (var page in host.GetChildren<ISettingsPage>())
            {
                page.Cancel();
            }
        }

        public void Save()
        {
            // Don't do anything with the setting; just keep them
            foreach (var page in host.GetChildren<ISettingsPage>())
            {
                page.Save();
            }
        }

        private void OnPageSelected(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            selectedPage?.Hide();
            // This will find the page and cause it to be loaded if x:Load==false
            selectedPage = host.FindName(element.Tag.ToString()) as ISettingsPage;
            selectedPage?.Show();
        }
    }
}
