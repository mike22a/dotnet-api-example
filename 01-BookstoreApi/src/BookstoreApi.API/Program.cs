using BookstoreApi.API.Middleware;
using BookstoreApi.Application.Interfaces;
using BookstoreApi.Application.Services;
using BookstoreApi.Domain.Interfaces.Repositories;
using BookstoreApi.Infrastructure.Data;
using BookstoreApi.Infrastructure.Repositories;
using BookstoreApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────
// 1. Database — dual provider (SQLite / SQL Server)
// ──────────────────────────────────────────────
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
var connStr = builder.Configuration.GetConnectionString(dbProvider)
    ?? throw new InvalidOperationException($"Connection string '{dbProvider}' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (dbProvider == "SqlServer")
        options.UseSqlServer(connStr);
    else
        options.UseSqlite(connStr);
});

// ──────────────────────────────────────────────
// 2. Repositories (Infrastructure → Domain interfaces)
// ──────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

// ──────────────────────────────────────────────
// 3. Application services (inject repository interfaces)
// ──────────────────────────────────────────────
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BookService>();

// ──────────────────────────────────────────────
// 4. JWT Authentication
// ──────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

// ──────────────────────────────────────────────
// 5. Exception handler
// ──────────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ──────────────────────────────────────────────
// 6. Controllers + Swagger with JWT button
// ──────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookstoreApi",
        Version = "v1",
        Description = "🟢 Level 1 — Clean Architecture API Example (Beginner)"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGci..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        },
        Array.Empty<string>()
    }});
});

// ──────────────────────────────────────────────
// 7. Build the app
// ──────────────────────────────────────────────
var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookstoreApi v1");
    c.RoutePrefix = "swagger";
});

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
