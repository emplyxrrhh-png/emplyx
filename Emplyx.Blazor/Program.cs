using Emplyx.ApiAdapter.DependencyInjection;
using Emplyx.Blazor.Services;
using Emplyx.Blazor.Services.Employees;
using Emplyx.Blazor.Services.Access;
using Emplyx.Blazor.Services.Scheduler;
using Emplyx.Shared.UI;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddLocalization();

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddScoped<TenantContextState>();
builder.Services.AddScoped<ITenantContextAccessor>(sp => sp.GetRequiredService<TenantContextState>());
builder.Services.AddScoped<AppUiMessagingService>();
builder.Services.AddScoped<UiTelemetry>();
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

