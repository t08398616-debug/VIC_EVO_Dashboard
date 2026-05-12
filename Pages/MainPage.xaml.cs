namespace VIC_EVO_Dashboard;

public partial class MainPage : ContentPage
{
    enum Languages
    {
        English,
        Spanish,
        French,
        German,
        Chinese
    }

    enum Units
    {
        CI,
        US,
    }
    public MainPage()
    {
        InitializeComponent();
        LanguagePicker.ItemsSource = Enum.GetValues(typeof(Languages));
        UnitPicker.ItemsSource = Enum.GetValues(typeof(Units));
    }
}
