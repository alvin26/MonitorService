
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Web;
using NLog;
using Autofac.Extensions.DependencyInjection;

namespace IndoCash.MonitorService.Host
{
    public class Program
    {
        private static Logger _logger = LogManager.GetLogger("MonitorService");
        public static void Main(string[] args)
        {
            try
            {
                _logger.Info("go into Main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    _logger.Info("Start UseStartup...");
                    webBuilder.UseStartup<Startup>();
                    _logger.Info("Finish UseStartup...");
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .UseNLog()
                .UseWindowsService();
    }
}
