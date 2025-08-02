using AuctionGuard.Application.IServices;
using AuctionGuard.Application.Services;
using AuctionGuard.Domain.Entities;
using AuctionGuard.Domain.Interfaces;
using AuctionGuard.Infrastructure.Contexts;
using AuctionGuard.Infrastructure.Repositories;
using AuctionGuard.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuctionGuard.Infrastructure.Services;
using IEmailSender = AuctionGuard.Application.IServices.IEmailSender;
using Microsoft.AspNetCore.Authorization;
using AuctionGuard.Application.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using AuctionGuard.API.Authorization;
using AuctionGuard.API.Hubs;
using AuctionGuard.API.Services;
using AuctionGuard.Infrastructure.BackgroundServices;


var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddLogging();

// Add services to the container.

builder.Services.AddDbContext<AuctionGuardDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

builder.Services.AddDbContext<AuctionGuardIdentityDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Identity")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(AppGenericRepository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IBiddingService, BiddingService>();

// Add SignalR
builder.Services.AddSignalR();

// Bind PayPal settings from appsettings.json
builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PayPal"));

builder.Services.AddScoped<IAuctionParticipationService, AuctionParticipationService>();

builder.Services.AddHostedService<AuctionStatusUpdaterService>();
builder.Services.AddSingleton<IAuctionNotifier, SignalRNotifier>();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IPayPalOnboardingService, PayPalOnboardingService>();
builder.Services.AddScoped<IPaymentGatewayService, PayPalService>();

builder.Services.AddHttpClient<IPayPalClientService, PayPalClientService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var payPalMode = configuration["PayPal:Platform:Mode"]; 
    var baseUrl = payPalMode == "Sandbox"
        ? "https://api-m.sandbox.paypal.com"
        : "https://api-m.paypal.com";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient();
builder.Services.AddTransient<IEmailSender, EmailSender>();
var configuration = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
            )
        };

    //    options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        var accessToken = context.Request.Query["access_token"];

       
    //        var path = context.HttpContext.Request.Path;
    //        if (!string.IsNullOrEmpty(accessToken) &&
    //            (path.StartsWithSegments("/testing-hub")))
    //        {
                
    //            context.Token = accessToken;
    //        }
    //        return Task.CompletedTask;
    //    },
    //    OnAuthenticationFailed = context =>
    //    {
           
    //        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
    //        logger.LogError(context.Exception, "@@@@@ JWT Authentication failed.");
    //        return Task.CompletedTask;
    //    }
    //};
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auction-Guard API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your token: Bearer {your token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    
    c.DocInclusionPredicate((docName, description) =>
    {
        if (description.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor controllerActionDescriptor)
        {
            return controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(true)
                .Any(attr => attr is ApiControllerAttribute);
        }
        return false;
    });


});

//var jwtKey = builder.Configuration["Jwt:Key"];
//var jwtIssuer = builder.Configuration["Jwt:Issuer"];
//var jwtAudience = builder.Configuration["Jwt:Audience"];

// Register the custom Authorization Policy Provider as a Singleton.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, HasPermissionPolicyProvider>();

// Register the Authorization Handler as Scoped.
builder.Services.AddScoped<IAuthorizationHandler, HasPermissionHandler>();

// Add base authorization services.
builder.Services.AddAuthorization();



builder.Services.AddIdentity<User, Role>(options =>
{
    // Configure Identity options here (e.g., password complexity, lockout settings)
    options.SignIn.RequireConfirmedAccount = false; 
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddEntityFrameworkStores<AuctionGuardIdentityDbContext>() 
    .AddDefaultTokenProviders();



//Settings.SECRET_KEY = jwtKey;
//Settings.ISSUER = jwtIssuer;
//Settings.AUDIENCE = jwtAudience;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



const string aunctionGuardClientOrigin = "AuctionGuardClientOrigin";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: aunctionGuardClientOrigin,
                      policy =>
                      {
                          policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});


//builder.Services.AddTransient<IEmailSender, MailgunEmailSender>();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        
        await IdentityDataSeeder.SeedRolesAndPermissionsAsync(services);        
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auction-Guard API V1");
        c.RoutePrefix = "swagger"; // Makes it available at /swagger 
    });
    
}

app.UseHttpsRedirection();



app.UseCors(aunctionGuardClientOrigin);

app.UseRouting();   

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<TestingHub>("/testing-hub");

app.Run();
