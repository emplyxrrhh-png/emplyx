using Emplyx.Application.Extensions;
using Emplyx.Infrastructure.Extensions;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
});

var app = builder.Build();

Console.WriteLine("Applying migrations...");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EmplyxDbContext>();
    try 
    {
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Enabling Swagger");
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
