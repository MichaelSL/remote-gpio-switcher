using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPIO.Service.Cmd
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string defaultLogFileLocation = "/var/log/switcher/switcher_.log";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(defaultLogFileLocation, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting host");
                var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<SwitchConfig>(hostContext.Configuration.GetSection("Daemon"));

                    services.AddSingleton<IHostedService, AppService>();
                })
                .UseSerilog();

                await builder.RunConsoleAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw ex;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}