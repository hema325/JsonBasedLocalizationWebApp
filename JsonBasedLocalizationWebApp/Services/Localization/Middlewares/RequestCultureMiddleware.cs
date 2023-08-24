using JsonBasedLocalizationWebApp.Services.Localization.Settings;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Globalization;

namespace JsonBasedLocalizationWebApp.Services.Localization.Middlewares
{
    public class RequestCultureMiddleware : IMiddleware
    {
        private readonly LocalizationSettings _localizationSettings;

        public RequestCultureMiddleware(IOptions<LocalizationSettings> localizationSettings)
        {
            _localizationSettings = localizationSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName] == null)
            {
                var header = context.Request.Headers[HeaderNames.AcceptLanguage].ToString();
                var langLength = header.IndexOfAny(new[] { ',', ';' });
                var lang = header.Substring(0, langLength != -1 ? langLength : 2);

                var culture = _localizationSettings.SupportedCultures?.FirstOrDefault(c => c.StartsWith(lang));

                //context.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(new RequestCulture(culture,culture), null));

                CultureInfo.CurrentCulture = new CultureInfo(culture ?? _localizationSettings?.SupportedCultures?.FirstOrDefault()!);
                CultureInfo.CurrentUICulture = new CultureInfo(culture ?? _localizationSettings?.SupportedCultures?.FirstOrDefault()!);
            }

            await next(context);
        }
    }
}
