using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Reservation.Application.Abstractions;
using Reservation.Application.Abstractions.Repositories;
using Reservation.Application.Services;
using Reservation.Infrastructure.Persistence;
using Reservation.Infrastructure.Repositories;
using Reservation.Infrastructure.Security;
using Reservation.Infrastructure.Tenancy;
using Reservation_API.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ReservationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

//HttpContext에 아무 데서나 접근할 수 있게(TenantProvider에서 사용)
builder.Services.AddHttpContextAccessor();

//현재 테넌트 제공자
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecretKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            
            ValidateAudience = false,
            
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();
    dbContext.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("Bearer");
    });
}

app.MapGet("/health", () => new { status = "ok" });



app.Run();



