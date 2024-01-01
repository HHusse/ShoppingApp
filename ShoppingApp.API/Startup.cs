using System;
using System.Text;
using Data;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Events.ServiceBus;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoppingApp.Events;
using Microsoft.Extensions.Azure;
using ShoppingApp.Data;

namespace ShoppingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSingleton<IDbContextFactory, DbContextFactory>();
            services.AddDbContext<ShoppingAppDbContext>(opt =>
            {
                opt.UseSqlServer(Environment.GetEnvironmentVariable("DBCONNECTIONSTRING"));
            });
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
            services.AddAuthentication().AddJwtBearer(optional =>
                optional.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "ShoppingApp",
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRETKEY")!))
                }
            );
            services.AddSingleton<IEventSender, ServiceBusTopicEventSender>();
            services.AddAzureClients(builder =>
            {
                builder.AddServiceBusClient(Environment.GetEnvironmentVariable("SERVICEBUS"));
            });
            using (var dbContext = new ShoppingAppDbContext())
            {
                dbContext.Database.Migrate();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            app.UseCors();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

