using BookstoreApi.API.Middleware;
using BookstoreApi.Application.Data;
using BookstoreApi.Application.Services;
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
    ?? throw new InvalidOperationException($"Connection string for '{dbProvider}' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (dbProvider == "SqlServer")
        options.UseSqlServer(connStr, b => b.MigrationsAssembly("BookstoreApi.Infrastructure"));
    else
        options.UseSqlite(connStr, b => b.MigrationsAssembly("BookstoreApi.Infrastructure"));
});

// ──────────────────────────────────────────────
// 2. Application services (Application layer)
// ──────────────────────────────────────────────
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BookService>();

// ──────────────────────────────────────────────
// 3. JWT Authentication
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
// 4. Exception handler
// ──────────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ──────────────────────────────────────────────
// 5. Controllers + Swagger with JWT button
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
// 6. Build the app
// ──────────────────────────────────────────────
var app = builder.Build();

// Auto-migrate and seed on startup
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
