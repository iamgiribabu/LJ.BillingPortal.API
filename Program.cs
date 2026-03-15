using FluentValidation;
using LJ.BillingPortal.API.Data;
using LJ.BillingPortal.API.Data.Repositories;
using LJ.BillingPortal.API.Data.Repositories.Interfaces;
using LJ.BillingPortal.API.Middleware;
using LJ.BillingPortal.API.Services;
using LJ.BillingPortal.API.Services.Interfaces;
using LJ.BillingPortal.API.Validators;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting LJ.BillingPortal.API Web API Core");

    // Add services to the container
    builder.Services.AddControllers();

    // Add Database Context
    var connectionString = builder.Configuration.GetConnectionString("BillingPortalDBConnection")
        ?? throw new InvalidOperationException("Connection string 'BillingPortalDBConnection' not found.");

    builder.Services.AddDbContext<BillingPortalDbContext>(options => options.UseSqlServer(connectionString));

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Add Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "LJ.BillingPortal.API",
            Version = "1.0.0",
            Description = "Billing Portal REST API for Invoice Management",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "LJ Lifters"
            }
        });

        var xmlFile = "LJ.BillingPortal.API.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // Add Validators
    builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateClientDetailsDtoValidator));

    // Add Repositories
    builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

    // Add Services
    builder.Services.AddScoped<IInvoiceService, InvoiceService>();
    builder.Services.AddScoped<IPdfGenerationService, PdfGenerationService>();

    // Add Health Check
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<BillingPortalDbContext>();

    //builder.WebHost.UseWebRoot("wwwroot");

    var app = builder.Build();

    // Configure middleware
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "LJ.BillingPortal.API v1.0");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseCors();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    // Database Migration on startup
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BillingPortalDbContext>();
        try
        {
            Log.Information("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            Log.Information("Database migrations completed successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during database migration");
        }
    }

    var port = builder.Configuration.GetValue<int>("Port", 5001);
    app.Urls.Add($"http://0.0.0.0:{port}");
    app.Urls.Add($"https://0.0.0.0:{port + 1}");

    Log.Information($"Application starting on port {port}");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
