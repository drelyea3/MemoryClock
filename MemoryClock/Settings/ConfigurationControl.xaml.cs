using MemoryClock.Workers;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MemoryClock.Settings
{
    public sealed partial class ConfigurationControl : UserControl, ISettingsPage
    {
        public class WorkerView
        {
            public IWorker Worker { get; set; }
            public string Name { get; set; }
            public bool IsEnabled { get; set; }
            public bool IsAlwaysEnabled { get; set; }
            public bool IsRaspberryPiOnly { get; set; }

            public bool AllowDisable { get { return !IsAlwaysEnabled; } }

            public WorkerView(string name, IWorker worker)
            {
                this.Worker = worker;
                this.Name = name;
                this.IsEnabled = Global.Settings.EnabledWorkers.Contains(name);
                this.IsAlwaysEnabled = Worker.IsAlwaysEnabled;
                this.IsRaspberryPiOnly = Worker.IsRaspberryPiOnly;
            }
        }

        private List<WorkerView> items;

        public ConfigurationControl()
        {
            this.InitializeComponent();
        }

        public void Cancel()
        {
            workers.ItemsSource = items = null;            
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void Save()
        {       
            Global.Settings.EnabledWorkers.Clear();

            foreach (var view in items)
            {
                if (view.IsEnabled || view.IsAlwaysEnabled)
                {
                    Global.Settings.EnabledWorkers.Add(view.Name);
                }
            }

            var app = (App)App.Current;
            app.StopWorkers();
            app.EnableWorkers(Global.Settings.EnabledWorkers);
            app.StartWorkers();
        }

        public void Show()
        {
            if (items == null)
            {
                items = new List<WorkerView>();

                foreach (var worker in ((App)App.Current).GetWorkers())
                {
                    var view = new WorkerView(worker.Item1, worker.Item2);
                    items.Add(view);
                }

                workers.ItemsSource = items;
            }
            this.Visibility = Visibility.Visible;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
