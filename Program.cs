



using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaieApi.Services;

using System.Text;




var builder = WebApplication.CreateBuilder(args);




var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var renderPort = Environment.GetEnvironmentVariable("PORT"); // Variable spécifique à Render

// Si la variable PORT existe (Render), on est forcément en Production
if (!string.IsNullOrEmpty(renderPort))
{
    builder.Environment.EnvironmentName = "Production";
    Console.WriteLine("🔵 Détection Render → Mode PRODUCTION");
}
else if (string.IsNullOrEmpty(environment))
{
    // En local sans variable définie → Development
    builder.Environment.EnvironmentName = "Development";
    Console.WriteLine("🟢 Mode local → DÉVELOPPEMENT");
}

Console.WriteLine($"🌍 Environnement final : {builder.Environment.EnvironmentName}");

if (builder.Environment.IsDevelopment())
{
    // En développement : HTTP + HTTPS
    builder.WebHost.UseUrls("http://0.0.0.0:5000", "https://0.0.0.0:5001");
}
else if
 (builder.Environment.IsProduction())
{
     var port = Environment.GetEnvironmentVariable("PORT") ?? "10001";

    //var port = 5000;//Environment.GetEnvironmentVariable("PORT") ?? "10001";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}






builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin() // Permet l'accès depuis n'importe quelle source
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});




builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Authentification OTP",
        Version = "v1",
        Description = "API d'authentification avec vérification SMS via Twilio",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "contact@monapi.com"
        }
    });

    // Configuration JWT dans Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Entrez le token JWT dans le format: Bearer {votre_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



// MongoDB Service
builder.Services.AddSingleton<MongoDbService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new MongoDbService(
        config["MongoDB:ConnectionString"],
        config["MongoDB:DatabaseName"]
    );
});

// Auth Service
builder.Services.AddScoped<AuthService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var mongoDb = sp.GetRequiredService<MongoDbService>();

    return new AuthService(
        mongoDb,
        config["Twilio:AccountSid"],
        config["Twilio:AuthToken"],
        config["Twilio:VerifyServiceSid"],
        config["Google:ClientId"],
        config["Google:ClientSecret"],
        sp.GetRequiredService<IHttpClientFactory>()
    );
});

//builder.Services.AddScoped<ShiftService>();
//builder.Services.AddScoped<WeeklyPlanService>();
//builder.Services.AddScoped<ExcelService>();
//builder.Services.AddScoped<ShiftTypeService>();
//builder.Services.AddScoped<SettingsService>();
//builder.Services.AddScoped<AdditionDeductionService>();
//builder.Services.AddScoped<SystemDataService>();
builder.Services.AddScoped<LeadsService>();
builder.Services.AddScoped<ActivitiesService>();
builder.Services.AddScoped<TenantService>();

// JWT Service
builder.Services.AddScoped<JwtService>();

// Authentication JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });



builder.Services.AddHttpClient();

// Ajouter la configuration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Auth v1");
    options.RoutePrefix = "swagger"; // Accessible sur /swagger
    options.DocumentTitle = "API Authentification OTP";
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});



app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var errorResponse = new
        {
            statusCode = 500,
            message = exception?.Message ?? "Une erreur interne s'est produite",
            stackTrace = app.Environment.IsDevelopment() ? exception?.StackTrace : null,
            path = context.Request.Path.ToString()
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});


// Headers de sécurité
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});


// 2. Utiliser le middleware CORS (avant UseAuthorization)
app.UseCors("AllowFrontend");
//app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Endpoint de test protégé
app.MapGet("/api/protected", [Authorize] () => "Accès autorisé avec JWT valide!")
    .WithName("ProtectedEndpoint");

app.MapGet("/privacy", () => Results.Content(@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Privacy Policy - Leader App</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; max-width: 800px; margin: 0 auto; padding: 20px; }
        h1 { color: #2c3e50; }
        h2 { color: #34495e; margin-top: 20px; }
        ul { padding-left: 20px; }
    </style>
</head>
<body>
    <h1>Privacy Policy</h1>
    <p>Last updated: January 28, 2026</p>
    
    <h2>Introduction</h2>
    <p>Welcome to Leader App. We respect your privacy and are committed to protecting your personal data.</p>

    <h2>Data We Collect</h2>
    <p>We may collect, use, store and transfer different kinds of personal data about you:</p>
    <ul>
        <li><strong>Identity Data:</strong> includes first name, last name.</li>
        <li><strong>Contact Data:</strong> includes email address and telephone numbers.</li>
    </ul>

    <h2>How We Use Your Data</h2>
    <p>We use your personal data to:</p>
    <ul>
        <li>Provide our services to you.</li>
        <li>Manage your account.</li>
    </ul>
    
    <h2>Contact Us</h2>
    <p>If you have any questions about this privacy policy, please contact us.</p>
</body>
</html>", "text/html"));

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
