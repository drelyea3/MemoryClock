using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Common
{
    public static class VisualTreeEnumerators
    {
        public static IEnumerable<T> GetAncestorsAndSelf<T>(this DependencyObject self) where T : DependencyObject
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

        public static IEnumerable<UIElement> GetChildren(this DependencyObject self)
        {
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(self); ++index)
            {
                UIElement child = VisualTreeHelper.GetChild(self, index) as UIElement;
                if (child != null)
                {
                    yield return child;
                }
            }
        }
        public static IEnumerable<T> GetDescendants<T>(this DependencyObject self) where T : DependencyObject
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
                    foreach (DependencyObject descendant in GetDescendants<T>(child))
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
