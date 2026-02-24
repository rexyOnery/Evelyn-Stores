using System.Text;
using EvelynStores.Core.Models;
using EvelynStores.Infrastructure.Extension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

// Configure CORS to allow browser clients to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var slidingClaim = context.Principal?.FindFirst("sliding_expiration")?.Value;
            if (int.TryParse(slidingClaim, out var slidingMinutes))
            {
                var expClaim = context.Principal?.FindFirst("exp")?.Value;
                if (long.TryParse(expClaim, out var expUnix))
                {
                    var expiration = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                    var remaining = expiration - DateTime.UtcNow;

                    if (remaining.TotalMinutes <= slidingMinutes && remaining.TotalMinutes > 0)
                    {
                        context.HttpContext.Response.Headers["X-Token-Expiring"] = "true";
                        context.HttpContext.Response.Headers["X-Token-Remaining-Minutes"] = ((int)remaining.TotalMinutes).ToString();
                    }
                }
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSingleton(sp =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001";
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Serve static files (for uploaded images under wwwroot/uploads)
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Enable CORS
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
