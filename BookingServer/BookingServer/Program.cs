using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.IO;

namespace BookingServer
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsetings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            /*ColumnOptions columnOptions = new ColumnOptions();
            columnOptions.Store.Add(StandardColumn.LogEvent);

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.MSSqlServer(Configuration.GetConnectionString("UserDatabase"), "Logs", columnOptions: columnOptions)
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Getting the motors running...");*/
                BuildWebHost(args).Run();
            /*}
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }*/
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5000")
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // delete all default configuration providers
                    config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsetings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
                })
                // .UseSerilog()
                .Build();
    }
}
