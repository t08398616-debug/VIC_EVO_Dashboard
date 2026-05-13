using System.Globalization;
using VIC_EVO_Dashboard.Resources.Strings;

namespace VIC_EVO_Dashboard.Resources.Strings
{
    // Wrapper that exposes the static resource properties as instance properties
    // so XAML can bind to Localization.SomeKey
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
            // Ensure the designer/runtime uses the current UI culture
            AppResources.Culture = CultureInfo.CurrentUICulture;
        }

        public string ErrorCode1 => AppResources.ErrorCode1;
        public string ErrorCode2 => AppResources.ErrorCode2;
        public string ErrorCode3 => AppResources.ErrorCode3;
        public string ErrorCode4 => AppResources.ErrorCode4;
        public string ErrorCode5 => AppResources.ErrorCode5;
        public string ErrorCode6 => AppResources.ErrorCode6;
        public string ErrorCode7 => AppResources.ErrorCode7;
        public string ErrorCode8 => AppResources.ErrorCode8;
        public string ErrorCode9 => AppResources.ErrorCode9;
        public string ErrorCode10 => AppResources.ErrorCode10;
        public string ErrorCode11 => AppResources.ErrorCode11;
        public string ErrorCodeTitle => AppResources.ErrorCodeTitle;
        public string FlowAlgorithm => AppResources.FlowAlgorithm;
        public string FlowTitle => AppResources.FlowTitle;
        public string P1Title => AppResources.P1Title;
        public string P2Title => AppResources.P2Title;
        public string P3Title => AppResources.P3Title;
        public string P4Title => AppResources.P4Title;
        public string PositionTitle => AppResources.PositionTitle;
        public string FuelTempTitle => AppResources.FuelTempTitle;
        public string Picker => AppResources.Picker;
        public string UnitTitle => AppResources.UnitTitle;
        public string LanguageTitle => AppResources.LanguageTitle;
    }
}