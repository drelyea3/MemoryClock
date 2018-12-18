using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Common
{
    public static class VisualTreeEnumerators
    {
        public static IEnumerable<T> GetAncestorsAndSelf<T>(this DependencyObject self) where T : class
        {
            DependencyObject parent = self;
            while (parent != null)
            {
                T parentElement = parent as T;
                if (parentElement != null)
                {
                    yield return parentElement;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        public static IEnumerable<UIElement> GetAncestorsAndSelf(this DependencyObject self)
        {
            return GetAncestorsAndSelf<UIElement>(self);
        }

        public static IEnumerable<T> GetAncestors<T>(this DependencyObject self) where T : DependencyObject
        {
            DependencyObject parent = self;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                T parentElement = parent as T;
                if (parentElement != null)
                {
                    yield return parentElement;
                }
            }
        }

        public static IEnumerable<UIElement> GetAncestors(this DependencyObject self)
        {
            return GetAncestors<UIElement>(self);
        }

        public static IEnumerable<T> GetChildren<T>(this DependencyObject self) where T : class
        {
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(self); ++index)
            {
                DependencyObject child = VisualTreeHelper.GetChild(self, index);
                T typedChild = child as T;
                if (typedChild != null)
                {
                    yield return typedChild;
                }
            }
        }

        public static IEnumerable<UIElement> GetChildren(this DependencyObject self)
        {
            return GetChildren<UIElement>(self);
        }

        public static IEnumerable<T> GetDescendants<T>(this DependencyObject self) where T : class
        {
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(self); ++index)
            {
                DependencyObject child = VisualTreeHelper.GetChild(self, index);
                if (child != null)
                {
                    T typedChild = child as T;
                    if (typedChild != null)
                    {
                        yield return typedChild;
                    }
                    foreach (var descendant in GetDescendants<T>(child))
                    {
                        T typedDescendant = descendant as T;
                        if (typedDescendant != null)
                        {
                            yield return typedDescendant;
                        }
                    }
                }
            }
        }

        public static IEnumerable<UIElement> GetDescendants(this DependencyObject self)
        {
            return GetDescendants<UIElement>(self);
        }
    }
}
