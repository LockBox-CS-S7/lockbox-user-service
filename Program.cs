using lockbox_user_service.Authorization;
using lockbox_user_service.Middlewares;
using lockbox_user_service.Requirement;
using lockbox_user_service.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;


namespace lockbox_user_service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Host.ConfigureAppConfiguration((configBuilder) => 
        {
            configBuilder.Sources.Clear();
            DotEnv.Load();
            configBuilder.AddEnvironmentVariables();
        });
        
        builder.WebHost.ConfigureKestrel(serverOptions => {
            serverOptions.AddServerHeader = false;
        });
        
        // Add services to the container.
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IAccountService, Auth0AccountService>();
        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(policy => {
                policy.WithOrigins(
                    builder.Configuration.GetValue<string>("CLIENT_ORIGIN_URL"))
                    .WithHeaders(new string[] {
                        HeaderNames.ContentType,
                        HeaderNames.Authorization,
                    })
                    .WithMethods("GET")
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
            });
        });
        

        builder.Services.AddControllers();
        
        builder.Host.ConfigureServices(services => {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var audience = builder.Configuration.GetValue<string>("AUTH0_AUDIENCE");

                    options.Authority = $"https://{builder.Configuration.GetValue<string>("AUTH0_DOMAIN")}/";
                    options.Audience = audience;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:admin-messages", policy =>
                {
                    policy.Requirements.Add(new RbacRequirement("read:admin-messages"));
                });
            });

            services.AddSingleton<IAuthorizationHandler, RbacHandler>();

        });
        
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        
        var requiredVars = new string[] {
            "PORT",
            "CLIENT_ORIGIN_URL",
            "AUTH0_DOMAIN",
            "AUTH0_AUDIENCE",
        };
        
        foreach (var key in requiredVars)
        {
            var value = app.Configuration.GetValue<string>(key);
            
            if (value == "" || value == null)
            {
                throw new Exception($"Config variable missing: {key}.");
            }
        }
        
        app.Urls.Add($"http://+:{app.Configuration.GetValue<string>("PORT")}");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseErrorHandler();
        app.UseSecureHeaders();
        app.MapControllers();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseHttpsRedirection();
        
        app.Run();
    }
}
