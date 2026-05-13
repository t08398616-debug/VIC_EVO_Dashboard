namespace VIC_EVO_Dashboard.Pages.Components;

public partial class ErrorBox : ContentView
{
    public static readonly BindableProperty ErrorTextProperty =
        BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(ErrorBox), string.Empty);

    public static readonly BindableProperty LedColorProperty =
        BindableProperty.Create(nameof(LedColor), typeof(Color), typeof(ErrorBox), Colors.Green);

    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    public Color LedColor
    {
        get => (Color)GetValue(LedColorProperty);
        set => SetValue(LedColorProperty, value);
    }

    public ErrorBox()
    {
        InitializeComponent();
    }
}