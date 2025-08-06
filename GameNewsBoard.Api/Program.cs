using GameNewsBoard.Api.Configurations;
using GameNewsBoard.Application.IServices.Auth;
using GameNewsBoard.Application.Mapping;
using GameNewsBoard.Application.Services.Auth;
using GameNewsBoard.Application.Settings;
using GameNewsBoard.Infrastructure.Configurations;
using GameNewsBoard.Infrastructure.Configurations.Settings;
using GameNewsBoard.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========== LOG de conexão ==========
Console.WriteLine(">>> Connection String: " + builder.Configuration.GetConnectionString("DefaultConnection"));

// ========== CONFIGURAÇÕES ==========
builder.Services.Configure<NewsDataSettings>(builder.Configuration.GetSection("NewsData"));
builder.Services.Configure<BackendSettings>(builder.Configuration.GetSection("Backend"));
builder.Services.Configure<IgdbSettings>(builder.Configuration.GetSection("Igdb"));
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("FrontendSettings"));
builder.Services.Configure<SteamSettings>(builder.Configuration.GetSection("Steam"));
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(ExternalMappingProfile));
builder.Services.AddHttpClient();
builder.Services.AddControllers();

// ========== DB CONTEXT ==========
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== SERVIÇOS ==========
builder.Services.AddInfrastructureServices();
builder.Services.AddScoped<ICookieService, CookieService>();

// ========== CONFIGURAÇÃO AUTENTICAÇÃO ==========
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
        ClockSkew = TimeSpan.Zero
    };

    // Suporte ao cookie jwtToken
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwtToken"))
            {
                context.Token = context.Request.Cookies["jwtToken"];
            }
            return Task.CompletedTask;
        }
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) // usado apenas pela Steam
.AddSteam();

// ========== AUTORIZAÇÃO ==========
builder.Services.AddAuthorization();

// ========== SWAGGER ==========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT no cabeçalho: 'Bearer {seu token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ========== CORS ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://projeto-game-news-board-front-woad.vercel.app",
                "https://projeto-game-news-board-git-cf6e93-leonardos-projects-706f1137.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ========== PIPELINE ==========
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

Console.WriteLine($"🌎 Ambiente atual: {app.Environment.EnvironmentName}");

app.Run();