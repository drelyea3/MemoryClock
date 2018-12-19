using Common;
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

namespace MemoryClock.Settings
{
    public sealed partial class MessagesSettingsControl : UserControl, ISettingsPage
    {
        ObservableCollection<Event<string>> events;

        public MessagesSettingsControl()
        {
            this.InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delete.IsEnabled = messages.SelectedItem != null;
            var data = messages.SelectedItem as Event<string>;
            if (data != null)
            {
                message.Text = data.Value;
                time.SelectedItem = data.Start;
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            events.Remove((Event<string>)messages.SelectedItem);
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var value = new Event<string>((TimeSpan)time.SelectedItem, message.Text.Trim());

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
            add.IsEnabled = time.SelectedItem != null && !string.IsNullOrWhiteSpace(message.Text);
        }

        private void OnMessageChanged(object sender, TextChangedEventArgs e)
        {
            add.IsEnabled = time.SelectedItem != null && !string.IsNullOrWhiteSpace(message.Text);
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
            if (events == null)
            {
                events = new ObservableCollection<Event<string>>(Global.Settings.Message.Events);
                messages.ItemsSource = events;
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
                Global.Settings.Message.Clear();
                foreach (var e in events)
                {
                    Global.Settings.Message.Add(e);
                }
            }
        }
    }
}
