using JsonBasedLocalizationWebApp.Services.Localization.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace JsonBasedLocalizationWebApp.Services.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IOptions<LocalizationSettings> _localizationSettings;
        private readonly IDistributedCache _distributedCache;

        public JsonStringLocalizerFactory(IOptions<LocalizationSettings> localizationSettings, IDistributedCache distributedCache)
        {
            _localizationSettings = localizationSettings;
            _distributedCache = distributedCache;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizerService(_localizationSettings, _distributedCache);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizerService(_localizationSettings, _distributedCache);
        }
    }
}
