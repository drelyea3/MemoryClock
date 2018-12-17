using System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Common
{
    public abstract class WindowEvents
    {
        bool _isAttached;
        bool _getHandled;

        protected abstract void OnKeyDown(CoreWindow sender, KeyEventArgs args);
        protected abstract void OnPointerPressed(CoreWindow sender, PointerEventArgs args);

        protected virtual void OnActivity() { }

        public void Attach(bool getHandled = false)
        {
            var window = Window.Current.CoreWindow;
            if (window == null)
            {
                throw new Exception("Window has not been created yet");
            }

            if (_isAttached)
            {
                throw new Exception("Already attached");
            }

            _isAttached = true;
            _getHandled = getHandled;

            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.PointerMoved += (o, e) => OnActivity();
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (!args.Handled || _getHandled)
            {
                OnKeyDown(sender, args);
            }

            OnActivity();
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (!args.Handled || _getHandled)
            {
                OnPointerPressed(sender, args);
            }

            OnActivity();
        }
    }
}
