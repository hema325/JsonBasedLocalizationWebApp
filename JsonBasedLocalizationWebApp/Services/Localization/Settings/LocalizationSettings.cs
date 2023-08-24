namespace JsonBasedLocalizationWebApp.Services.Localization.Settings
{
    public class LocalizationSettings
    {
        public const string SectionName = "Localization";
        public string RootPath { get; init; }
        public string[] SupportedCultures { get; init; }
    }
}
