﻿using System;
using UNOversal.Ioc;
using UNOversal.Services.Settings;
using Project2FA.Services.Enums;
using Project2FA.Core;
using UNOversal.Services.Logging;
using UNOversal.Extensions;

#if WINDOWS_UWP
using Project2FA.UWP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using Project2FA.UnoApp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace Project2FA.Services
{
    public class SettingsService
    {
        SettingsHelper _helper;
        private SettingsService()
        {
            _helper = new SettingsHelper(App.Current.Container.Resolve<ISettingsAdapter>());
        }

        /// <summary>
        /// Gets public singleton property.
        /// </summary>
        public static SettingsService Instance { get; } = new SettingsService();

        public bool UseHeaderBackButton
        {
            get => _helper.SafeRead(nameof(UseHeaderBackButton), true);
            set => _helper.TryWrite(nameof(UseHeaderBackButton), value);
        }

        public bool UseRoundCorner
        {
            get => _helper.SafeRead(nameof(UseRoundCorner), false);
            set
            {
                _helper.TryWrite(nameof(UseRoundCorner), value);
                // workaround for switching the resources...
                switch (AppTheme)
                {
                    case Theme.System:
                        if (OriginalAppTheme == ApplicationTheme.Dark)
                        {
                            (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Light;
                            (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark;
                        }
                        else
                        {
                            (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark;
                            (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Light;
                        }
                        break;
                    case Theme.Dark:
                        (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Light;
                        (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark;
                        break;
                    case Theme.Light:
                        (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark;
                        (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Light;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool ShowAvailableProFeatures
        {
            get => _helper.SafeRead(nameof(ShowAvailableProFeatures), true);
            set => _helper.TryWrite(nameof(ShowAvailableProFeatures), value);
        }

        public bool AutoLogoutMinutesIsMDMManaged
        {
            get => _helper.SafeRead(nameof(AutoLogoutMinutesIsMDMManaged), false);
            set
            {
                if (_helper.SafeRead(nameof(AutoLogoutMinutesIsMDMManaged), false) != value)
                {
                    _helper.TryWrite(nameof(AutoLogoutMinutesIsMDMManaged), value);
                }
            }
        }

        public int AutoLogoutMinutes
        {
            get
            {
                if (IsProVersion)
                {
                    var(successful, result) = _helper.Read<int>(nameof(AutoLogoutMinutes), Constants.EnterpriseAppManagementContainer);
                    if (successful)
                    {
                        AutoLogoutMinutesIsMDMManaged = true;
                        return result;
                    }
                    else
                    {
                        return _helper.SafeRead(nameof(AutoLogoutMinutes), 10);
                    }
                }
                else
                {
                    AutoLogoutMinutesIsMDMManaged = false;
                    return _helper.SafeRead(nameof(AutoLogoutMinutes), 10);
                }
            }
            set => _helper.TryWrite(nameof(AutoLogoutMinutes), value);
        }

        /// <summary>
        /// Indicates whether the app has been rated.
        /// </summary>
        public bool AppRated
        {
            get => _helper.SafeRead(nameof(AppRated), false);
            set => _helper.TryWrite(nameof(AppRated), value);
        }

        /// <summary>
        /// Indicates whether the application is the pro version.
        /// </summary>
        public bool IsProVersion
        {
#if WINDOWS_UWP
            get => _helper.SafeRead(nameof(IsProVersion), false);
            internal set => _helper.TryWrite(nameof(IsProVersion), value);
#else
            get => false;
#endif
        }

#if WINDOWS_UWP


        public bool UseProFeatures
        {
            get => _helper.SafeRead(nameof(UseProFeatures), false);
            set => _helper.TryWrite(nameof(UseProFeatures), value);
        }
#endif

        public bool UseExtendedHash
        {
            get => _helper.SafeRead(nameof(UseExtendedHash), false);
            set => _helper.TryWrite(nameof(UseExtendedHash), value);
        }

        public bool UseAutoLogoutIsMDMManaged
        {
            get => _helper.SafeRead(nameof(UseAutoLogoutIsMDMManaged), false);
            set => _helper.TryWrite(nameof(UseAutoLogoutIsMDMManaged), value);
        }

        public bool UseAutoLogout
        {
            get
            {
                if (IsProVersion)
                {
                    var (successful, result) = _helper.Read<bool>(nameof(UseAutoLogout), Constants.EnterpriseAppManagementContainer);
                    if (successful)
                    {
                        UseAutoLogoutIsMDMManaged = true;
                        return result;
                    }
                    else
                    {
                        return _helper.SafeRead(nameof(UseAutoLogout), true);
                    }
                }
                else
                {
                    UseAutoLogoutIsMDMManaged = false;
                    return _helper.SafeRead(nameof(UseAutoLogout), true);
                }
            }
                
            set => _helper.TryWrite(nameof(UseAutoLogout), value);
        }

        public bool UseNTPServerCorrectionIsMDMManaged
        {
            get => _helper.SafeRead(nameof(UseNTPServerCorrectionIsMDMManaged), false);
            set => _helper.TryWrite(nameof(UseNTPServerCorrectionIsMDMManaged), value);
        }

        public bool UseNTPServerCorrection
        {
            get
            {
                if (IsProVersion)
                {
                    var (successful, result) = _helper.Read<bool>(nameof(UseNTPServerCorrection), Constants.EnterpriseAppManagementContainer);
                    if (successful)
                    {
                        UseNTPServerCorrectionIsMDMManaged = true;
                        return result;
                    }
                    else
                    {
                        return _helper.SafeRead(nameof(UseNTPServerCorrection), false);
                    }
                }
                else
                {
                    UseNTPServerCorrectionIsMDMManaged = false;
                    return _helper.SafeRead(nameof(UseNTPServerCorrection), false);
                }
            }
                
            set => _helper.TryWrite(nameof(UseNTPServerCorrection), value);
        }

        public bool AdvancedPasswordSecurity
        {
            get => _helper.SafeRead(nameof(AdvancedPasswordSecurity), false);
            set => _helper.TryWrite(nameof(AdvancedPasswordSecurity), value);
        }

        public bool UseHiddenTOTPIsMDMManaged
        {
            get => _helper.SafeRead(nameof(UseHiddenTOTPIsMDMManaged), false);
            set => _helper.TryWrite(nameof(UseHiddenTOTPIsMDMManaged), value);
        }

        public bool UseHiddenTOTP
        {
            get
            {
                if (IsProVersion)
                {
                    var (successful, result) = _helper.Read<bool>(nameof(UseHiddenTOTP), Constants.EnterpriseAppManagementContainer);
                    if (successful)
                    {
                        UseHiddenTOTPIsMDMManaged = true;
                        return result;
                    }
                    else
                    {
                        return _helper.SafeRead(nameof(UseHiddenTOTP), false);
                    }
                }
                else
                {
                    UseHiddenTOTPIsMDMManaged = false;
                    return _helper.SafeRead(nameof(UseHiddenTOTP), false);
                }
            } 
            set => _helper.TryWrite(nameof(UseHiddenTOTP), value);
        }

        public bool PrideMonthDesign
        {
            get => _helper.SafeRead(nameof(PrideMonthDesign), false);
            set => _helper.TryWrite(nameof(PrideMonthDesign), value);
        }

        public bool NTPServerStringIsMDMManaged
        {
            get => _helper.SafeRead(nameof(NTPServerStringIsMDMManaged), false);
            set => _helper.TryWrite(nameof(NTPServerStringIsMDMManaged), value);
        }

        public string NTPServerString
        {
            get
            {
                if (IsProVersion)
                {
                    var (successful, result) = _helper.Read<string>(nameof(NTPServerString), Constants.EnterpriseAppManagementContainer);
                    if (successful)
                    {
                        NTPServerStringIsMDMManaged = true;
                        return result;
                    }
                    else
                    {
                        return _helper.SafeRead(nameof(NTPServerString), "time.windows.com");
                    }
                }
                else
                {
                    NTPServerStringIsMDMManaged = false;
                    return _helper.SafeRead(nameof(NTPServerString), "time.windows.com");
                }
            }
            set => _helper.WriteString(nameof(NTPServerString), value);
        }

#region DataFileSettings

        public string DataFilePasswordHash
        {
            get => _helper.ReadString(nameof(DataFilePasswordHash), string.Empty);
            set => _helper.WriteString(nameof(DataFilePasswordHash), value);
        }

        public string DataFileName
        {
            get => _helper.ReadString(nameof(DataFileName), string.Empty);
            set => _helper.WriteString(nameof(DataFileName), value);
        }

        public string DataFilePath
        {
            get => _helper.ReadString(nameof(DataFilePath), string.Empty);
            set => _helper.WriteString(nameof(DataFilePath), value);
        }

        public bool DataFileWebDAVEnabled
        {
            get => _helper.SafeRead(nameof(DataFileWebDAVEnabled), false);
            set => _helper.TryWrite(nameof(DataFileWebDAVEnabled), value);
        }
#endregion

        public string UnhandledExceptionStr
        {
            get => _helper.ReadString(nameof(UnhandledExceptionStr), string.Empty);
            set => _helper.WriteString(nameof(UnhandledExceptionStr), value);
        }

        public bool ActivateWindowsHelloIsMDMManaged
        {
            get => _helper.SafeRead(nameof(ActivateWindowsHelloIsMDMManaged), false);
            set => _helper.TryWrite(nameof(ActivateWindowsHelloIsMDMManaged), value);
        }

        public bool ActivateWindowsHello
        {
            get
            {
                if (IsProVersion)
                {
                    var (successful, result) = _helper.Read<bool>(nameof(ActivateWindowsHello), Constants.EnterpriseAppManagementContainer);
                    if (successful)
                    {
                        ActivateWindowsHelloIsMDMManaged = true;
                        return result;
                    }
                    else
                    {
                        return _helper.SafeRead(nameof(ActivateWindowsHello), true);
                    }
                }
                else
                {
                    ActivateWindowsHelloIsMDMManaged = false;
                    return _helper.SafeRead(nameof(ActivateWindowsHello), true);
                }
            }
            set => _helper.TryWrite(nameof(ActivateWindowsHello), value);
        }

        public BiometricPreferEnum PreferWindowsHello
        {
            get => _helper.SafeReadEnum(nameof(PreferWindowsHello), BiometricPreferEnum.None);
            set => _helper.WriteEnum(nameof(PreferWindowsHello), value);
        }

        public BiometricPreferEnum PreferBiometricLogin
        {
            get => _helper.SafeReadEnum(nameof(PreferBiometricLogin), BiometricPreferEnum.None);
            set => _helper.WriteEnum(nameof(PreferBiometricLogin), value);
        }

        public Theme AppTheme
        {
            get => _helper.SafeReadEnum(nameof(AppTheme), Theme.System);
            set
            {
                _helper.WriteEnum(nameof(AppTheme), value);
                switch (value)
                {
                    default:
                    case Theme.System:
                        (Window.Current.Content as FrameworkElement).RequestedTheme = OriginalAppTheme.ToElementTheme();
                        break;
                    case Theme.Dark:
                        (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark;
                        break;
                    case Theme.Light:
                        (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Light;
                        break;
                }
            }
        }

        public ApplicationTheme AppStartSetTheme(ApplicationTheme startTheme)
        {
            OriginalAppTheme = startTheme;
            if (AppTheme == Theme.Dark || AppTheme == Theme.Light)
            {
                switch (AppTheme)
                {
                    default:
                    case Theme.System:
                        return startTheme;
                    case Theme.Dark:
                        return ApplicationTheme.Dark;
                    case Theme.Light:
                        return ApplicationTheme.Light;
                }
            }
            else
            {
                return startTheme;
            }
        }

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public ApplicationTheme OriginalAppTheme
        {
            get => _helper.SafeReadEnum(nameof(OriginalAppTheme), ApplicationTheme.Light);
            set => _helper.WriteEnum(nameof(OriginalAppTheme), value);
        }

        public void ResetSystemTheme(ApplicationTheme theme)
        {
            OriginalAppTheme = theme;
            switch (theme)
            {
                case ApplicationTheme.Light:
                    (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Light;
                    break;
                case ApplicationTheme.Dark:
                    (Window.Current.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark;
                    break;
                default:
                    break;
            }
        }

        public string LoginScreenWallpaper
        {
            get
            {
                if (IsProVersion)
                {
                    return _helper.ReadString(nameof(LoginScreenWallpaper), Constants.EnterpriseAppManagementContainer);
                }
                return string.Empty;
            }
            set => _helper.WriteString(nameof(LoginScreenWallpaper), value);
        }

        public LoggingPreferEnum LoggingSetting
        {
            get => _helper.SafeReadEnum(nameof(LoggingSetting), LoggingPreferEnum.Simple);
            set => _helper.WriteEnum(nameof(LoggingSetting), value);
        }

        public string PurchasedStoreId
        {
            get => _helper.ReadString(nameof(PurchasedStoreId), string.Empty);
            set => _helper.TryWrite(nameof(PurchasedStoreId), value);
        }

        public DateTime LastCheckedSystemTime
        {
            get => _helper.SafeRead(nameof(LastCheckedSystemTime), new DateTime());
            set =>_helper.TryWrite(nameof(LastCheckedSystemTime), value);
        }

        public DateTimeOffset LastCheckedInPurchaseAddon
        {
            get => _helper.SafeRead(nameof(LastCheckedInPurchaseAddon), new DateTimeOffset());
            set => _helper.TryWrite(nameof(LastCheckedInPurchaseAddon), value);
        }

        public DateTimeOffset NextCheckedInPurchaseAddon
        {
            get => _helper.SafeRead(nameof(NextCheckedInPurchaseAddon), new DateTimeOffset());
            set => _helper.TryWrite(nameof(NextCheckedInPurchaseAddon), value);
        }

        /// <summary>
        /// Indicates whether the screen capture is possible (temp)
        /// </summary>
        private bool _isScreenCaptureEnabled = false;
        public bool IsScreenCaptureEnabled
        {
            get => _isScreenCaptureEnabled;
            internal set
            {
                _isScreenCaptureEnabled = value;
#if WINDOWS_UWP || WINDOWS
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsScreenCaptureEnabled = value;
#endif
            }
        }
    }
}
