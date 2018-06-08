using System;
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
using BookingServer.Web.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace BookingServer
{
    public class Startup
    {
        private static IConfiguration Configuration { get; set; }

        public Startup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsetings.json");
            Configuration = config.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtSettings>(Configuration.GetSection("JWTSettings"))
                .Configure<Redis>(Configuration.GetSection("redis"));

            services.AddAuthentication(options=> 
            {
                try
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Authentication failed: "+ex);
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
                        ValidIssuer = Configuration["JWTSettings:Issuer"],
                        ValidAudience = Configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSettings:SecretKey"]))
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

            services.AddLogging();

            services.AddTransient<TokenManagerMiddleware>()
                //.AddTransient<PropDetailMiddleware>()
                .AddTransient<ITokenManager, TokenManager>()
                //.AddTransient<IPropDetail, PropDetail>();
                ;

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDistributedRedisCache(r => { r.Configuration = Configuration["redis:connectionString"]; });

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
                            builder.WithOrigins(Configuration["Client:AngularSite"])
                            .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                        });
                }
            );

            services.AddDbContext<UserDBContext>(
                options =>
                options.UseSqlServer(Configuration.GetConnectionString("UserDatabase"))
            )
            .AddDbContext<AccommodationDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AccommodationDatabase"))

            )
            .AddDbContext<FlightDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FlightDatabase"))
            )
            .AddDbContext<CarRentalDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CarRentalDatabase"))
            )
            .AddDbContext<AirTaxiDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AirTaxiDatabase"))
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env
            , ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug().AddConsole();

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

            app.UseMiddleware<TokenManagerMiddleware>()
                //.UseMiddleware<PropDetailMiddleware>()
                ;

            app.UseCors("ClientDomain");

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
