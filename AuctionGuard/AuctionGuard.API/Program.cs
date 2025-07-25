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


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AuctionGuardDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

builder.Services.AddDbContext<AuctionGuardIdentityDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Identity")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(IdentityGenericRepository<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(AppGenericRepository<>));


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        RequireExpirationTime = true, 
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"message\": \"Unauthorized. Token is missing or invalid.\"}");
        }
    };
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
    // Only include API controllers (those with [ApiController]) 
    c.DocInclusionPredicate((docName, description) =>
    {
        if (description.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor controllerActionDescriptor)
        {
            return controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(true)
                .Any(attr => attr is ApiControllerAttribute);
        }
        return false;
    });

    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; 
    //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename)); 

});


// --- START: Authorization Configuration ---

// Register the custom Authorization Policy Provider as a Singleton.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, HasPermissionPolicyProvider>();

// Register the Authorization Handler as Scoped.
builder.Services.AddScoped<IAuthorizationHandler, HasPermissionHandler>();

// Add base authorization services.
builder.Services.AddAuthorization();

// --- END: Authorization Configuration ---

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


builder.Services.ConfigureApplicationCookie(options =>
{
    // These settings are crucial for cross-site cookie authentication
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None; // Allow the cookie to be set from a different origin (your React app)
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // This is REQUIRED for SameSite.None to work

    // This part prevents the API from redirecting to a login page on 401 errors
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }
        return Task.CompletedTask;
    };
});


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
                          // This must match the URL of your running React app.
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
        // Call the seeder to create roles and assign permissions
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
