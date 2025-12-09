using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MudBlazor.Services;
using NSwag;
using NSwag.Generation.Processors.Security;
using ProLab.Api.Infrastructure.Configurations;
using ProLab.Api.Infrastructure.Interfaces;
using ProLab.Api.Infrastructure.Services;
using ProLab.Api.Logging;
using ProLab.Data;
using ProLab.Data.Entities;
using ProLab.Data.Entities.Users;
using System.Collections.Concurrent;
using System.Text;

ILogger<Program>? logger = null;

var builder = WebApplication.CreateBuilder(args);

AppSettings appSettings = ConfigureAppSettings(builder.Services);

try
{
    //ConfigureCookiePolicy(builder);

    ConfigureLogging(builder, appSettings);
    ConfigureCors(builder.Services, appSettings);
    ConfigureDbContext(builder.Services);
    ConfigureIdentity(builder.Services);
    ConfigureJwtAuth(builder.Services, appSettings);
    ConfigureEndpoints(builder);
    ConfigureOpenApi(builder);

    builder.Services.AddHttpClient();
    builder.Services.AddMudServices();

    builder.Services.AddSpaStaticFiles(o => o.RootPath = "nuxtApp");

    #region services
    builder.Services.AddSingleton<RouteSelectionService>();
    builder.Services.AddScoped<RouteOptimizationService>();
    builder.Services.AddHttpClient<MapboxService>((serviceProvider, client) =>
    {
        var mapboxOptions = serviceProvider
            .GetRequiredService<IOptions<MapboxOptions>>()
            .Value;

        client.BaseAddress = new Uri(mapboxOptions.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("User-Agent", "RouteOptimization/1.0");
    })
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

    builder.Services.Configure<MapboxOptions>(builder.Configuration.GetSection(MapboxOptions.SectionName));
    #endregion

    ConfigureSingleton(builder);

    var app = builder.Build();

    logger = app.Services.GetRequiredService<ILogger<Program>>();

    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        app.UseDeveloperExceptionPage();
        app.UseOpenApi();
        app.UseSwaggerUi();
    }

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseDefaultFiles(new DefaultFilesOptions()
    {
        FileProvider = new PhysicalFileProvider(
          Path.Combine(builder.Environment.ContentRootPath, "nuxtApp")),
        RequestPath = ""
    });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
          Path.Combine(builder.Environment.ContentRootPath, "nuxtApp")),
        RequestPath = ""
    });

    app.UseSpaStaticFiles(new StaticFileOptions() { });
    //app.UseStaticFiles(new StaticFileOptions
    //{
    //  OnPrepareResponse = ctx => ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={60 * 60 * 12}")
    //});


    app.Map("/api", subapp =>
    {
        subapp.UsePathBase("/api/");
        subapp.UseEndpoints(endpoints => endpoints.MapControllers());
    });

    app.UseSpa(o =>
    {
        o.Options.DefaultPage = "/200.html";
    });

    app.Run();
}
catch (Exception ex)
{
    if (logger == null)
    {
        // if the logger wasn't assigned by app build, we go yolo
        logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
    }

    logger.LogCritical(ex, "Error while starting the application");
    throw ex;
}

void ConfigureDbContext(IServiceCollection services)
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("ProLab.Data")),
      contextLifetime: ServiceLifetime.Scoped,
      optionsLifetime: ServiceLifetime.Singleton
    );
    services.AddDbContextFactory<ApplicationDbContext>(options =>
      options.UseSqlServer(connectionString),
      ServiceLifetime.Singleton
    );
}

void ConfigureIdentity(IServiceCollection services)
{
    services.AddDefaultIdentity<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;
        options.User.RequireUniqueEmail = true;
        //options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
}


AppSettings ConfigureAppSettings(IServiceCollection services)
{
    var appSettingsSection = builder.Configuration.GetSection("AppSettings");
    var appSettings = appSettingsSection.Get<AppSettings>();
    services.AddSingleton(appSettings);
    return appSettings;
}

void ConfigureCors(IServiceCollection services, AppSettings appSettings)
{
    var allCors = appSettings.CorsWhitelist?.ToList() ?? new();
    allCors.Add(appSettings.FrontendBaseUrl);

    services.AddCors(options =>
    {
        options.AddDefaultPolicy(
        builder =>
        {
            builder
          .WithOrigins(allCors.ToArray())
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
        });
    });
}

static void ConfigureJwtAuth(IServiceCollection services, AppSettings appSettings)
{

    var key = Encoding.ASCII.GetBytes(appSettings.Secret);

    services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "JWT_OR_COOKIE";
        options.DefaultAuthenticateScheme = "JWT_OR_COOKIE";
        options.DefaultSignInScheme = "JWT_OR_COOKIE";
        options.DefaultSignOutScheme = "JWT_OR_COOKIE";
        options.DefaultChallengeScheme = "JWT_OR_COOKIE";
        options.DefaultForbidScheme = "JWT_OR_COOKIE";
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", o =>
    {
        o.ForwardDefaultSelector = context =>
        {
            // filter by auth type
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return JwtBearerDefaults.AuthenticationScheme;

            // otherwise always check for cookie auth
            return IdentityConstants.ApplicationScheme;
        };
    })
    ;
}

static void ConfigureLogging(WebApplicationBuilder builder, AppSettings appSettings)
{

    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    builder.Services.AddHostedService<DatabaseLoggingBackgroundService>();
    builder.Services.AddSingleton(new ConcurrentQueue<LogEntry>());
    builder.Logging.AddDatabase();
}


static void ConfigureOpenApi(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddOpenApiDocument(document =>
    {
        document.PostProcess = o =>
      {
          o.Schemes.Clear();
          o.Schemes.Add(OpenApiSchema.Https);
      };
        document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Type into the textbox: Bearer {your JWT token}."
        });

        document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
    });
}

static void ConfigureSingleton(WebApplicationBuilder builder)
{
    var types = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => typeof(ISingletonProvider).IsAssignableFrom(p) && !p.IsInterface)
        .Select(x => builder.Services.AddSingleton(x))
        .ToList();
}

static void ConfigureEndpoints(WebApplicationBuilder builder)
{
    builder.Services.AddControllers().AddNewtonsoftJson();
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
}

