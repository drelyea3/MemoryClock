using Common;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MemoryClock.Settings
{
    public sealed partial class NoCallSettingsControl : UserControl, ISettingsPage
    {
        ObservableCollection<Event<bool>> events;

        public NoCallSettingsControl()
        {
            this.InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delete.IsEnabled = schedule.SelectedItem != null;
            var data = schedule.SelectedItem as Event<bool>;
            if (data != null)
            {
                if (data.Value)
                {
                    showing.IsChecked = true;
                }
                else
                {
                    notShowing.IsChecked = true;
                }
                time.SelectedItem = data.Start;
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            events.Remove((Event<bool>)schedule.SelectedItem);
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var value = new Event<bool>((TimeSpan)time.SelectedItem, showing.IsChecked.Value);

            var index = 0;

            while (index < events.Count && events[index].Start < value.Start)
            {
                ++index;
            }

            if (index < events.Count)
            {
                if (events[index].Start == value.Start)
                {
                    events.RemoveAt(index);
                }
            }

            events.Insert(index, value);
        }

        private void OnTimeChanged(object sender, SelectionChangedEventArgs e)
        {
            add.IsEnabled = time.SelectedItem != null && (showing.IsChecked.Value || notShowing.IsChecked.Value);
        }

        private void OnIsShowingChecked(object sender, RoutedEventArgs e)
        {
            add.IsEnabled = time.SelectedItem != null && (showing.IsChecked.Value || notShowing.IsChecked.Value);
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
            if (events == null)
            {
                events = new ObservableCollection<Event<bool>>(Global.Settings.IsNoPhone.Events);
                schedule.ItemsSource = events;
            }
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void Cancel()
        {
            events = null;
        }

        public void Save()
        {
            if (events != null)
            {
                Global.Settings.IsNoPhone.Clear();
                foreach (var e in events)
                {
                    Global.Settings.IsNoPhone.Add(e);
                }
            }
        }
    }
}
