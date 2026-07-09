using HotelApi.API.Middleware;
using HotelApi.Application.Interfaces;
using HotelApi.Application.Services;
using HotelApi.Domain.Interfaces.Repositories;
using HotelApi.Infrastructure.Data;
using HotelApi.Infrastructure.Repositories;
using HotelApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Database — switchable SQLite / SQL Server
var provider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        opts.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    else
        opts.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

// 2. Repositories
builder.Services.AddScoped<IUserRepository,        UserRepository>();
builder.Services.AddScoped<IRoomTypeRepository,    RoomTypeRepository>();
builder.Services.AddScoped<IRoomRepository,        RoomRepository>();
builder.Services.AddScoped<IGuestRepository,       GuestRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IPaymentRepository,     PaymentRepository>();

// 3. Services
builder.Services.AddScoped<IJwtService,       JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RoomTypeService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<GuestService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ReportService>();

// 4. JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts => opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = false,
        ValidateAudience         = false,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    });

builder.Services.AddControllers();

// 5. Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 6. Swagger with Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Reservation System API (Clean Architecture)", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Example: \"Bearer {token}\"",
        Name = "Authorization", In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey, Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
        Array.Empty<string>()
    }});
});

var app = builder.Build();

// 7. Auto-migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try { db.Database.Migrate(); }
    catch (Exception ex) { Console.WriteLine($"Migration error: {ex.Message}"); }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
