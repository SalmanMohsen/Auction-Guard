using AuctionGuard.Domain.Entities;
using AuctionGuard.Domain.Interfaces;
using AuctionGuard.Infrastructure.Contexts;
using AuctionGuard.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AuctionGuardDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

builder.Services.AddDbContext<AuctionGuardIdentityDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Identity")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(AppGenericRepository<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(IdentityGenericRepository<>));

builder.Services.AddIdentity<User, Role>(options =>
{
    // Configure Identity options here (e.g., password complexity, lockout settings)
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddEntityFrameworkStores<AuctionGuardDbContext>() 
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
