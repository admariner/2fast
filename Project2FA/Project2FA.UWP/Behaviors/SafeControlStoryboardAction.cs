using Microsoft.Xaml.Interactivity;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#if NET10_0_OR_GREATER
using WinRT;
#endif

namespace Project2FA.UWP.Behaviors
{
    public class SafeControlStoryboardAction : DependencyObject, IAction
    {
        public Storyboard Storyboard
        {
#if NET10_0_OR_GREATER
            [DynamicWindowsRuntimeCast(typeof(Storyboard))]
#endif
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }

        public static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.Register(nameof(Storyboard), typeof(Storyboard), typeof(SafeControlStoryboardAction), new PropertyMetadata(null));

        public ControlStoryboardOption ControlStoryboardOption
        {
            get { return (ControlStoryboardOption)GetValue(ControlStoryboardOptionProperty); }
            set { SetValue(ControlStoryboardOptionProperty, value); }
        }

        public static readonly DependencyProperty ControlStoryboardOptionProperty =
            DependencyProperty.Register(nameof(ControlStoryboardOption), typeof(ControlStoryboardOption), typeof(SafeControlStoryboardAction), new PropertyMetadata(ControlStoryboardOption.Play));

        public object Execute(object sender, object parameter)
        {
            if (Storyboard == null)
            {
                return false;
            }

            // If the sender is a UI element that is no longer in the visual tree,
            // skip execution to avoid a native COM crash in .NET Native / AOT
            // (McgMarshal.ThrowOnExternalCallFailed) that bypasses managed catch blocks.
            if (sender is FrameworkElement fe && VisualTreeHelper.GetParent(fe) == null)
            {
                return false;
            }

            try
            {
                switch (ControlStoryboardOption)
                {
                    case ControlStoryboardOption.Play:
                        Storyboard.Begin();
                        break;
                    case ControlStoryboardOption.Stop:
                        Storyboard.Stop();
                        break;
                    case ControlStoryboardOption.TogglePlayPause:
                        var state = ClockState.Stopped;
                        try
                        {
                            state = Storyboard.GetCurrentState();
                        }
                        catch
                        {
                            // ignore
                        }
                        if (state == ClockState.Active)
                        {
                            Storyboard.Pause();
                        }
                        else
                        {
                            Storyboard.Begin();
                        }
                        break;
                    case ControlStoryboardOption.Pause:
                        Storyboard.Pause();
                        break;
                    case ControlStoryboardOption.Resume:
                        Storyboard.Resume();
                        break;
                }
            }
            catch (Exception)
            {
                // Storyboard target not found — ignore gracefully
            }

            return true;
        }
    }
}
