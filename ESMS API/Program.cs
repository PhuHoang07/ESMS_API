using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Business.Services.AdminService;
using Business.Services.ExamService;
using ESMS_Data.Models;
using ESMS_Data.Repositories.ExamTimeRepository;
using ESMS_Data.Repositories.UserRepository;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Add DbContext
// Using connection string from key vault 
var keyVaultEndPoint = new Uri(builder.Configuration["VaultKey"]);
var secretClient = new SecretClient(keyVaultEndPoint, new DefaultAzureCredential());

KeyVaultSecret keyVaultSecret = secretClient.GetSecret("ESMS-AzureSQL");

builder.Services.AddDbContext<ESMSContext>(options =>
{
    //options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
    options.UseSqlServer(keyVaultSecret.Value);
});


// Add Service
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IExamService, ExamService>();


// Add Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();


builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ESMS API",
        Version = "v1",
        Description = "API for ESMS System"
    });

    // Using Time instead of Ticks
    option.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        //Example = new OpenApiString("00:00")
    });    
});




var app = builder.Build();

// Configure the HTTP request pipeline.
// Remove if allow using when deploying

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
