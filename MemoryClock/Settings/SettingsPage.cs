namespace MemoryClock.Settings
{
    public interface ISettingsPage
    {
        void Show();
        void Hide();
        void Cancel();
        void Save();
    }
}
