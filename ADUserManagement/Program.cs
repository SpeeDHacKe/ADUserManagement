using ADUserManagement.Common;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System;
using static System.Collections.Specialized.BitVector32;

string? aspnetcoreENV = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (string.IsNullOrEmpty(aspnetcoreENV)) throw new ArgumentException("Not found ASPNETCORE_ENVIRONMENT Variable");
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5001");
AppSetting.Configuration = builder.Configuration;


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoTransient();
builder.Services.AddMvc(o => o.Conventions.Add(new JwtAuthorizationConvention("JwtPolicy", Convert.ToBoolean(builder.Configuration["JwtAuthen:Authen"]), builder?.Configuration["JwtAuthen:ActionIgnore"]?.Split(',')))).AddNewtonsoftJson();

//Authen 1
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .Build());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtAuthen:Issuer"],
        ValidAudience = builder.Configuration["JwtAuthen:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder?.Configuration["JwtAuthen:Key"] ?? ""))
    };
});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("JwtPolicy", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{AppSetting.AssemblyName} ({DateTime.Now.ToString("dd/MM/yyyy-HH:mm")}) - {aspnetcoreENV}", Version = "v1" });
});

var app = builder.Build();
//AppSetting.Configuration = app.Configuration;

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", $"{AppSetting.AssemblyName} v1"));
}

app.UseAuthorization();

//app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


public static class AppSetting
{
    public static IConfiguration? Configuration { get; set; }
    public static string? AssemblyName => Assembly.GetExecutingAssembly().GetName().Name;
    public static string? SystemMessage(string code) => Configuration?[$"SystemMessage:{code.ToUpper()}"];
}

public static class SetupServiceLifeTime
{
    public static void AddAutoTransient(this IServiceCollection services)
    {
        List<Type> allType = new List<Type>();
        List<string> nsRange = new List<string> { $"{AppSetting.AssemblyName}.Services" };
        nsRange.ForEach(n =>
        {
            List<Type> srvTyp = Assembly.GetExecutingAssembly().GetTypes()
                                    .Where(t => t.Namespace != null && t.Namespace.StartsWith(n))
                                    .Where(t => t.Namespace != null && !t.Namespace.EndsWith(".Common") && !t.Namespace.EndsWith(".MicroServices") && !t.Namespace.EndsWith(".Entities"))
                                    .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic).ToList();

            allType.AddRange(srvTyp);
        });

        if (allType != null & allType?.Count > 0)
        {
            allType?.ForEach(clss =>
            {
                var intrf = clss.GetInterfaces().FirstOrDefault();
                if (intrf != null)
                {
                    services.AddTransient(intrf, clss);
                }
                else
                {
                    services.AddTransient(clss);
                }
            });
        }
    }
}

public class JwtAuthorizationConvention : IApplicationModelConvention
{
    private readonly string? _policy;
    private readonly bool _auth;
    private readonly string[]? _actionIgnore;

    public JwtAuthorizationConvention(string? policy, bool auth, string[]? actionIgnore)
    {
        _policy = policy;
        _auth = auth;
        _actionIgnore = actionIgnore;
    }

    public void Apply(ApplicationModel application)
    {
        if (_auth)
        {
            application.Controllers.ToList().ForEach(controller =>
            {
                var isController = controller.Selectors.Any(x => x.AttributeRouteModel != null && x.AttributeRouteModel.Template.ToLower().StartsWith("api"));
                if (isController)
                {
                    foreach (var item in controller.Actions.ToList())
                    {
                        var myList = _actionIgnore?.ConvertToModel<List<string>>().ConvertAll(x => x.ToLower());
                        var isActionAuthen = _actionIgnore == null || myList?.Contains(item.ActionName.ToLower()) == false;
                        if (isActionAuthen)
                        {
                            item.Filters.Add(new AuthorizeFilter(_policy));
                        }
                    }
                    //controller.Actions.ToList().ForEach(action => 
                    //{
                    //    var myList = _actionIgnore?.ConvertToModel<List<string>>().ConvertAll(x => x.ToLower());
                    //    var isActionAuthen = _actionIgnore == null || myList?.Contains(action.ActionName.ToLower()) == false;
                    //    if (isActionAuthen)
                    //    {
                    //        action.Filters.Add(new AuthorizeFilter(_policy));
                    //    }
                    //});
                }
            });
        }
    }
}