using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#if NET10_0_OR_GREATER
using WinRT;
#endif

namespace Project2FA.UWP.Behaviors
{
    public class CloseFlyoutAction : DependencyObject, IAction
    {
#if NET10_0_OR_GREATER
        [DynamicWindowsRuntimeCast(typeof(Popup))]
        [DynamicWindowsRuntimeCast(typeof(DependencyObject))]
        [DynamicWindowsRuntimeCast(typeof(FlyoutPresenter))]
#endif
        public object Execute(object sender, object parameter)
        {
            var parent = TargetObject ?? sender as DependencyObject;
            while (parent != null)
            {
                if (parent is FlyoutPresenter)
                {
                    ((parent as FlyoutPresenter).Parent as Popup).IsOpen = false;
                    break;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }
            return null;
        }

        public Control TargetObject
        {
#if NET10_0_OR_GREATER
            [DynamicWindowsRuntimeCast(typeof(Control))]
#endif
            get { return (Control)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register(nameof(TargetObject), typeof(Control), typeof(CloseFlyoutAction), new PropertyMetadata(null));
    }
}
