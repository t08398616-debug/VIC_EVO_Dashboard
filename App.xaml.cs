using System.Globalization;
namespace VIC_EVO_Dashboard;

public partial class App : Application
{
    public App()
    {
        MainPage = new AppShell();
        InitializeComponent();

        string savedCulture = Preferences.Get("SelectedCulture", "en");
        var culture = new CultureInfo(savedCulture);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.Created += (s, e) =>
        {
#if WINDOWS
            var nativeWindow = window.Handler.PlatformView as Microsoft.UI.Xaml.Window;
            if (nativeWindow != null)
            {
                var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
                if (presenter != null)
                {
                    presenter.Maximize();
                    presenter.IsResizable = false;

                    presenter.IsMinimizable = true;
                    presenter.IsMaximizable = false;
                }
            }
#endif
        };

        return window;
    }
}