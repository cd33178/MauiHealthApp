using CommunityToolkit.Maui;
using MauiHealthApp.Core.Handlers;
using MauiHealthApp.Core.Services;
using MauiHealthApp.Core.Validators;
using MauiHealthApp.DataAccess.Contexts;
using MauiHealthApp.DataAccess.Repositories;
using MauiHealthApp.DataAccess.Sqlite;
using MauiHealthApp.Handlers;
using MauiHealthApp.Services;
using MauiHealthApp.ViewModels;
using MauiHealthApp.Views;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Reflection;

namespace MauiHealthApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .WriteTo.Debug()
            .CreateLogger();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configuration from embedded appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("MauiHealthApp.appsettings.json");
        if (stream != null)
        {
            var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
            builder.Configuration.AddConfiguration(config);
        }

        // Serilog integration
        builder.Logging.AddSerilog(dispose: true);

        // MSAL – configure ClientId, Authority, and RedirectUri via appsettings.json or User Secrets
        var msalSettings = builder.Configuration.GetSection("MsalSettings");
        var msalClientId = msalSettings["ClientId"]
            ?? throw new InvalidOperationException("MsalSettings:ClientId is required. Configure it in appsettings.json or User Secrets.");
        var msalAuthority = msalSettings["Authority"]
            ?? throw new InvalidOperationException("MsalSettings:Authority is required. Configure it in appsettings.json or User Secrets.");
        var msalRedirectUri = msalSettings["RedirectUri"]
            ?? throw new InvalidOperationException("MsalSettings:RedirectUri is required. Configure it in appsettings.json or User Secrets.");
        var msalClient = PublicClientApplicationBuilder
            .Create(msalClientId)
            .WithAuthority(msalAuthority)
            .WithRedirectUri(msalRedirectUri)
            .Build();
        builder.Services.AddSingleton<IPublicClientApplication>(msalClient);

        // Auth & Navigation Services
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddTransient<AuthTokenHandler>();

        // HTTP Client with Polly
        var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/";
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        builder.Services.AddHttpClient("HealthApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthTokenHandler>()
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(circuitBreakerPolicy);

        // Local SQLite database
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "healthapp.db");
        builder.Services.AddSingleton(new LocalDbContext(dbPath));
        builder.Services.AddSingleton<DatabaseInitializer>();

        // Repositories
        builder.Services.AddScoped<IApiProfileRepository, ApiProfileRepository>();
        builder.Services.AddScoped<IApiQuestionRepository, ApiQuestionRepository>();
        builder.Services.AddScoped<ILocalProfileRepository, LocalProfileRepository>();

        // Core Services
        builder.Services.AddScoped<IProfileService>(sp =>
        {
            var apiRepo = sp.GetRequiredService<IApiProfileRepository>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ProfileService>>();
            return new ProfileService(logger,
                id => apiRepo.GetByIdAsync(id),
                userId => apiRepo.GetByUserIdAsync(userId),
                req => apiRepo.CreateAsync(req),
                (id, req) => apiRepo.UpdateAsync(id, req),
                id => apiRepo.DeleteAsync(id));
        });

        builder.Services.AddScoped<IQuestionService>(sp =>
        {
            var apiRepo = sp.GetRequiredService<IApiQuestionRepository>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<QuestionService>>();
            return new QuestionService(logger,
                (userId, page, pageSize, search) => apiRepo.GetPagedAsync(userId, page, pageSize, search),
                id => apiRepo.GetByIdAsync(id),
                req => apiRepo.CreateAsync(req),
                (id, req) => apiRepo.UpdateAsync(id, req),
                id => apiRepo.DeleteAsync(id));
        });

        // MediatR
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProfileCommandHandler).Assembly));

        // Validators
        builder.Services.AddTransient<CreateProfileCommandValidator>();
        builder.Services.AddTransient<CreateQuestionCommandValidator>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<QuestionsViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<QuestionsPage>();
        builder.Services.AddTransient<AppShell>();

        return builder.Build();
    }
}
