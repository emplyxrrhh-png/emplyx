using Emplyx.ApiAdapter.DependencyInjection;
using Emplyx.Blazor.Services;
using Emplyx.Blazor.Services.Employees;
using Emplyx.Blazor.Services.Access;
using Emplyx.Blazor.Services.Scheduler;
using Emplyx.Shared.UI;
using Emplyx.Shared.UI.Telemetry; // added
using Emplyx.Blazor.Services.Features; // added
using Emplyx.Blazor.Services.Security; // added
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// Localization configuration
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddApplicationInsightsTelemetry(); // instrumentation key via configuración
var supportedCultures = new[] { new CultureInfo("es-ES"), new CultureInfo("en-US") };
builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    opts.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-ES");
    opts.SupportedCultures = supportedCultures;
    opts.SupportedUICultures = supportedCultures;
});

builder.Services.AddScoped<TenantContextState>();
builder.Services.AddScoped<ITenantContextAccessor>(sp => sp.GetRequiredService<TenantContextState>());
builder.Services.AddScoped<AppUiMessagingService>();
builder.Services.AddScoped<UiTelemetry>();
// feature flags
builder.Services.AddSingleton<IFeatureFlags, FeatureFlags>();
// permissions mock
builder.Services.AddScoped<IPermissionService>(sp => new PermissionService(sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal()));
builder.Services.AddHttpContextAccessor();
// download client
builder.Services.AddHttpClient<DownloadClient>();

builder.Services.AddSingleton<IEmployeesDataSource, EmployeesMockDataSource>();
builder.Services.AddSingleton<IAccessGroupsDataSource, AccessGroupsMockDataSource>();
builder.Services.AddSingleton<IAccessPeriodsDataSource, AccessPeriodsMockDataSource>();
builder.Services.AddSingleton<IAccessStatusDataSource, AccessStatusMockDataSource>();
builder.Services.AddSingleton<IAccessZonesDataSource, AccessZonesMockDataSource>();
builder.Services.AddSingleton<IAccessEventsDataSource, AccessEventsMockDataSource>();
builder.Services.AddSingleton<IProductiveUnitsDataSource, ProductiveUnitsMockDataSource>();
builder.Services.AddSingleton<IBudgetsDataSource, BudgetsMockDataSource>();
builder.Services.AddSingleton<IMovesDataSource, MovesMockDataSource>();
builder.Services.AddSingleton<ICoverageSummaryDataSource, CoverageSummaryMockDataSource>();
builder.Services.AddSingleton<SampleTasksQueueService>();
builder.Services.AddEmplyxApiAdapter(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
// Localization middleware
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

