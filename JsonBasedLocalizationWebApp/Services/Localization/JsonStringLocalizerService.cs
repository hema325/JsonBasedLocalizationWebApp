using JsonBasedLocalizationWebApp.Services.Localization.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;

namespace JsonBasedLocalizationWebApp.Services.Localization
{
    public class JsonStringLocalizerService : IStringLocalizer
    {
        private readonly JsonSerializer _jsonSerializer = new();
        private readonly LocalizationSettings _localizationSettings;
        private readonly IDistributedCache _distributedCache;

        public JsonStringLocalizerService(IOptions<LocalizationSettings> localizationSettings, IDistributedCache distributedCache)
        {
            _localizationSettings = localizationSettings.Value;
            _distributedCache = distributedCache;
        }
        public LocalizedString this[string name] 
            => new(name,GetValueFromJson(name));

        public LocalizedString this[string name, params object[] arguments] 
            => new(name,string.Format(GetValueFromJson(name),arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = Path.Combine(_localizationSettings.RootPath, CultureInfo.CurrentCulture.Name + ".json");
            var fullFilePath = Path.GetFullPath(filePath);

            if (File.Exists(fullFilePath)) 
            {
                using var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var streamReader = new StreamReader(fileStream);
                using var jsonTextReader = new JsonTextReader(streamReader);

                while (jsonTextReader.Read())
                {
                    if(jsonTextReader.TokenType == JsonToken.PropertyName)
                    {
                        var key = jsonTextReader.Value as string;
                        jsonTextReader.Read();
                        var value = _jsonSerializer.Deserialize<string>(jsonTextReader);

                        yield return new LocalizedString(key!,value!);
                    }
                }
            }
        }

        private string GetValueFromJson(string propertyName)
        {
            var filePath = Path.Combine(_localizationSettings.RootPath, CultureInfo.CurrentCulture.Name + ".json");
            var fullFilePath = Path.GetFullPath(filePath);

            if (File.Exists(fullFilePath))
            {
                var key = $"{propertyName}-{CultureInfo.CurrentCulture.Name}";
                var value = _distributedCache.GetString(key);

                if (value != null)
                    return value;

                value = GetValueFromJson(propertyName, fullFilePath);

                _distributedCache.SetString(key, value);

                return value;
            }

            return string.Empty;
        }

        private string GetValueFromJson(string propertyName, string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var streamReader = new StreamReader(fileStream);
            using var jsonTextReader = new JsonTextReader(streamReader);

            while (jsonTextReader.Read())
            {
                if(jsonTextReader.TokenType == JsonToken.PropertyName && (jsonTextReader.Value as string).ToLower() == propertyName.ToLower())
                {
                    jsonTextReader.Read();
                    return _jsonSerializer.Deserialize<string>(jsonTextReader)!;
                }
            }

            return string.Empty;
        }
    }
}
