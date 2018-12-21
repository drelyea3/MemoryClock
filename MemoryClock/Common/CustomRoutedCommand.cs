using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Common
{
    public abstract class CustomRoutedCommand : DependencyObject
    {
        public interface ICanRaiseCommand
        {
            CustomRoutedCommandHandler GetCommandHandler();
        }

        #region Handler property

        public delegate void CustomRoutedCommandHandler(CustomRoutedCommand command, object parameter);

        public static CustomRoutedCommandHandler GetHandler(DependencyObject obj)
        {
            return (CustomRoutedCommandHandler)obj.GetValue(HandlerProperty);
        }

        public static void SetHandler(DependencyObject obj, CustomRoutedCommandHandler value)
        {
            obj.SetValue(HandlerProperty, value);
        }

        public static void SetHandler(object onCommand)
        {
            throw new NotImplementedException();
        }

        public static readonly DependencyProperty HandlerProperty =
            DependencyProperty.RegisterAttached("Handler", typeof(CustomRoutedCommandHandler), typeof(CustomRoutedCommand), new PropertyMetadata(null));

        #endregion

        #region Command property

        public static CustomRoutedCommand GetCommand(DependencyObject obj)
        {
            return (CustomRoutedCommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, CustomRoutedCommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(CustomRoutedCommand), typeof(CustomRoutedCommand), new PropertyMetadata(null, OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
            if (button != null)
            {
                CustomRoutedCommand command = e.NewValue as CustomRoutedCommand;
                if (command != null)
                {
                    button.Click -= CommandClick;
                    button.Click += CommandClick;
                }
                else
                {
                    button.Click -= CommandClick;
                }
                return;
            }

            var hyperlink = d as Hyperlink;
            if (hyperlink != null)
            {
                CustomRoutedCommand command = e.NewValue as CustomRoutedCommand;
                if (command != null)
                {
                    hyperlink.Click -= CommandClick;
                    hyperlink.Click += CommandClick;
                }
                else
                {
                    hyperlink.Click -= CommandClick;
                }
                return;
            }

            var menuFlyoutItem = d as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                CustomRoutedCommand command = e.NewValue as CustomRoutedCommand;
                if (command != null)
                {
                    hyperlink.Click -= CommandClick;
                    hyperlink.Click += CommandClick;
                }
                else
                {
                    hyperlink.Click -= CommandClick;
                }
                return;
            }
        }

        private static void CommandClick(object sender, RoutedEventArgs e)
        {
            var command = CustomRoutedCommand.GetCommand((DependencyObject)sender);
            object parameter = CustomRoutedCommand.GetParameter((DependencyObject)sender);
            var element = CustomRoutedCommand.GetSource((DependencyObject)sender) ?? sender as FrameworkElement;

            if (element != null)
            {
                Raise(element, command, parameter);
            }
            else
            {
                throw new InvalidOperationException("Can't raise command");
            }
        }

        #endregion

        #region Source property

        public static FrameworkElement GetSource(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(FrameworkElement), typeof(CustomRoutedCommand), new PropertyMetadata(null));

        #endregion

        #region Parameter property

        public static object GetParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(ParameterProperty);
        }

        public static void SetParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ParameterProperty, value);
        }

        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.RegisterAttached("Parameter", typeof(object), typeof(CustomRoutedCommand), new PropertyMetadata(null));

        #endregion

        public bool Handled { get; set; }

        public static bool Raise(CustomRoutedCommand command, object parameter)
        {
            return Raise(null, command, parameter);
        }

        public static bool Raise(FrameworkElement element, CustomRoutedCommand command, object parameter)
        {
            Logger.Log($"Raising {command}");
            command.Handled = false;

            if (element != null)
            {
                foreach (FrameworkElement ancestor in element.GetAncestorsAndSelf<FrameworkElement>())
                {
                    GetHandler(ancestor)?.Invoke(command, parameter);
                    if (command.Handled)
                    {
                        Logger.Log($"Raise - command {command} was handled");
                        break;
                    }
                }
            }

            if (!command.Handled)
            {
                Logger.Log($"Raise - command {command} was not handled, trying Application");
                var sink = Application.Current as ICanRaiseCommand;
                sink?.GetCommandHandler()?.Invoke(command, parameter);
            }

            Logger.Log($"Raise - command {command} handled = {command.Handled}");

            return command.Handled;
        }
    }
}
