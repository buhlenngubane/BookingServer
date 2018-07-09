using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BookingServer.Models;
using BookingServer.Models.Accommodations;
using BookingServer.Models.AirTaxis;
using BookingServer.Models.CarRentals;
using BookingServer.Models.Flights;
using BookingServer.Models.Users;
using BookingServer.Services;
using BookingServer.Services.Email;
using BookingServer.Web.Filters;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace BookingServer
{
    public class Startup
    {
         private static IConfiguration _Configuration { get; set; }
        

        public Startup(IConfiguration configuration)
        {
            _Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.Configure<JwtSettings>(_Configuration.GetSection("JWTSettings"))
                    .Configure<Redis>(_Configuration.GetSection("redis"));

                services.AddAuthentication(options =>
                {
                    try
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }
                    catch (Exception ex)
                    {
                        /// Debug.WriteLine(ex);
                        Console.WriteLine("Authentication failed: " + ex);
                        Log.Fatal("Add Authentication failed: " + ex);
                        throw ex;
                    }

                })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = _Configuration["JWTSettings:Issuer"],
                            ValidAudience = _Configuration["JWTSettings:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["JWTSettings:SecretKey"]))
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {

                                Console.WriteLine("OnAuthenticationFailed: " +
                                    context.Exception.Message);
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                Console.WriteLine("OnTokenValidated: " +
                                    context.SecurityToken);
                                return Task.CompletedTask;
                            }
                        };
                    }
                 );



                // services.AddLogging();

                services.AddTransient<TokenManagerMiddleware>();

                services.AddTransient<ITokenManager, TokenManager>();

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                services.AddDistributedRedisCache(r =>
                {

                    r.Configuration = _Configuration["redis:connectionString"] + ",abortConnect = " + _Configuration["redis:AbortOnConnectFail"];
                });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Administrator",
                        policy => policy.RequireRole("Admin")
                        );
                });

                services.AddCors(options =>
                    {
                        options.AddPolicy("ClientDomain",
                            builder =>
                            {
                                builder.WithOrigins(_Configuration["Client:AngularSite"])
                                .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                            });
                    }
                );

                services.AddDbContext<UserDBContext>(
                    options =>
                    options.UseSqlServer(_Configuration.GetConnectionString("UserDatabase"))
                );

                services.AddDbContext<AccommodationDBContext>(options =>
                    options.UseSqlServer(_Configuration.GetConnectionString("AccommodationDatabase"))

                );

                services.AddDbContext<FlightDBContext>(options =>
                    options.UseSqlServer(_Configuration.GetConnectionString("FlightDatabase"))
                );

                services.AddDbContext<CarRentalDBContext>(options =>
                    options.UseSqlServer(_Configuration.GetConnectionString("CarRentalDatabase"))
                );

                services.AddDbContext<AirTaxiDBContext>(options =>
                    options.UseSqlServer(_Configuration.GetConnectionString("AirTaxiDatabase"))
                );

                services.AddMvc()
                    .AddJsonOptions(
                        options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Booking Server", Version = "v1" });
                    c.OperationFilter<AddAuthTokenHeaderParam>();
                });

                services.AddSignalR();

                services.AddSingleton<IEmailConfiguration>(_Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());

                services.AddTransient<IEmailService, EmailService>();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, " Host services not loaded!");
                Console.WriteLine(ex);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env
            , ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddDebug().AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking Server V1");
            });

            app.UseAuthentication();

            app.UseMiddleware<TokenManagerMiddleware>();

            app.UseCors("ClientDomain");

            app.UseStaticFiles();

            app.UseSignalR(route =>
            {
                route.MapHub<Booking_Notify>("/async");
            });

            app.UseMvc(
                    routes=>
                    routes.MapRoute(
                        name:"default",
                        template:"api/[controller=User]/[action=GetUser]"
                        )
                );
        }
    }
}
