using System.Globalization;

namespace VIC_EVO_Dashboard;

public partial class MainPage : ContentPage
{
    MainPageViewModel _viewModel;
    bool isInitializing = true;

    public MainPage()
    {
        InitializeComponent();

        _viewModel = new MainPageViewModel();
        BindingContext = _viewModel;

        LanguagePicker.ItemsSource = new List<string> { "English", "Español" };
        UnitPicker.ItemsSource = new List<string> { "SI (BAR)", "US (PSI)" };

        LanguagePicker.SelectedIndex = Preferences.Get("SelectedLanguageIndex", 0);
        UnitPicker.SelectedIndex = Preferences.Get("SelectedUnitIndex", 0);

        isInitializing = false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.StartModbusPolling();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.StopModbusPolling();
    }

    private void LanguagePicker_SelectedLanguage(object sender, EventArgs e)
    {
        if (isInitializing || LanguagePicker.SelectedIndex == -1) return;

        string cultureCode = LanguagePicker.SelectedIndex == 0 ? "en" : "es";
        var culture = new CultureInfo(cultureCode);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        Preferences.Set("SelectedLanguageIndex", LanguagePicker.SelectedIndex);

        _viewModel.RefreshUI();
    }

    private void UnitPicker_SelectedUnit(object sender, EventArgs e)
    {
        if (isInitializing || UnitPicker.SelectedIndex == -1) return;

        bool isSi = UnitPicker.SelectedIndex == 0;
        _viewModel.PressureUnit = isSi ? "BAR" : "PSI";
        _viewModel.TempUnit = isSi ? "°C" : "°F";

        Preferences.Set("SelectedUnitIndex", UnitPicker.SelectedIndex);
        Preferences.Set("SelectedPressureUnit", _viewModel.PressureUnit);
    }
}