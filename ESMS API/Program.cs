using Business.Interfaces;
using Business.Services;
using ESMS_Data.Interfaces;
using ESMS_Data.Models;
using ESMS_Data.Repositories;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add DbContext
builder.Services.AddDbContext<ESMSContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

// Add Service
builder.Services.AddScoped<IAdminService, AdminService>();

// Add Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddGoogle(googleOptions =>
//    {
//        googleOptions.ClientId = builder.Configuration["Google:ClientId"];
//        googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
//    })
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = "https://localhost:7212",
//            ValidAudience = "https://localhost:7212",
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretKey"))
//        };
//    });



//builder.Services
//       .AddAuthentication(o =>
//       {
//           // This forces challenge results to be handled by Google OpenID Handler, so there's no
//           // need to add an AccountController that emits challenges for Login.
//           o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
//           // This forces forbid results to be handled by Google OpenID Handler, which checks if
//           // extra scopes are required and does automatic incremental auth.
//           o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
//           // Default scheme that will handle everything else.
//           // Once a user is authenticated, the OAuth2 token info is stored in cookies.
//           o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//       })
//       .AddCookie()
//       .AddGoogleOpenIdConnect(options =>
//       {
//           options.ClientId = builder.Configuration["Google:ClientId"];
//           options.ClientSecret = builder.Configuration["Google:ClientSecret"];
//       });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"];
        options.ClientSecret = builder.Configuration["Google:ClientSecret"];
        options.SaveTokens = true;
    });

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ESMS API",
        Version = "v1",
        Description = "API for ESMS System"
    });
    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        //In = ParameterLocation.Header,
        //Description = "Please enter a valid token",
        //Name = "Authorization",
        //Type = SecuritySchemeType.Http,
        //BearerFormat = "JWT",
        //Scheme = "Bearer"
        Description = "oauth",
        Name = "oauth2.0",
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows

        {
            AuthorizationCode = new OpenApiOAuthFlow
            {

                AuthorizationUrl = new Uri($"https://accounts.google.com/o/oauth2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                
                Scopes = new Dictionary<string, string>
                    {
                        {
                            $"https://www.googleapis.com/auth/userinfo.email",
                            "Get email"
                        },
                        {
                            $"https://www.googleapis.com/auth/userinfo.profile",
                            "Get profile"
                        },
                    }
            }
        }

    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="oauth2"
                }
            },
            new List<string> ()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ESMS API v1");
        c.RoutePrefix = "swagger";
        c.OAuthClientId(builder.Configuration["Google:ClientId"]);
        c.OAuthClientSecret(builder.Configuration["Google:ClientSecret"]);
        c.OAuthUsePkce();
        //c.OAuthUseBasicAuthenticationWithAccessCodeGrant();

    });
}

app.UseHttpsRedirection();

app.UseCors("EnableCORS");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
