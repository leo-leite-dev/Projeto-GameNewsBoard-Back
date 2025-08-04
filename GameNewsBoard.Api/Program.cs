using GameNewsBoard.Api.Configurations;
using GameNewsBoard.Application.Mapping;
using GameNewsBoard.Application.Settings;
using GameNewsBoard.Infrastructure;
using GameNewsBoard.Infrastructure.Configurations;
using GameNewsBoard.Infrastructure.Data;
using GameNewsBoard.Infrastructure.Services;
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
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(ExternalMappingProfile));
builder.Services.AddHttpClient();
builder.Services.AddControllers();

// ========== DB CONTEXT ==========
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== REGISTRO DE SERVIÇOS ==========
builder.Services.AddInfrastructureServices(); // Inclui UserService, TokenService, etc.

// ========== CONFIGURAÇÃO JWT ==========
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };

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
    });

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

// ========== BUILD E PIPELINE ==========
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

// ========== VERIFICAÇÃO SUPABASE ENV VAR ==========
var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY");

if (string.IsNullOrWhiteSpace(supabaseUrl) || string.IsNullOrWhiteSpace(supabaseKey))
    Console.WriteLine("⚠️ SUPABASE_URL ou SUPABASE_KEY não estão definidas!");

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

Console.WriteLine($"🌎 Ambiente atual: {app.Environment.EnvironmentName}");

app.Run();