using JsonBasedLocalizationWebApp.Services.Localization;
using JsonBasedLocalizationWebApp.Services.Localization.Middlewares;
using JsonBasedLocalizationWebApp.Services.Localization.Settings;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<LocalizationSettings>(builder.Configuration.GetSection(LocalizationSettings.SectionName));
builder.Services.AddLocalization();
//builder.Services.AddSingleton<IStringLocalizer, JsonStringLocalizerService>(); to use non generic IStringLocalizer
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (Type, factory) =>
    factory.Create(typeof(JsonStringLocalizerService)));

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var localizationSettings = builder.Configuration.GetSection(LocalizationSettings.SectionName).Get<LocalizationSettings>();
    var supportedCultures = localizationSettings.SupportedCultures.Select(c => new CultureInfo(c)).ToList();
    
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<RequestCultureMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseRequestLocalization();
app.UseMiddleware<RequestCultureMiddleware>(); // must come after UseRequestLocalization

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
