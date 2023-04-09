using Autofac;
using Autofac.Configuration;
using Autofac.Extras.Quartz;
using IndoCash.MonitorService.AlertSender.Interfaces;
using IndoCash.MonitorService.Host.Interface;
using IndoCash.MonitorService.Host.Models;
using IndoCash.MonitorService.SkypeBotSender;
using IndoCash.MonitorService.Utils;
using IndoCash.MonitorService.Utils.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IndoCash.MonitorService.Host
{
    public class Startup
    {
        private readonly Logger _logger = LogManager.GetLogger("MonitorService");
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            ContainerBuilder builder = new ContainerBuilder();
            //builder.RegisterInstance(dicActionRec).Named<ConcurrentDictionary<string, DateTime>>("dicActionRec");
            //builder.RegisterInstance(dicServiceRec).Named<ConcurrentDictionary<string, DateTime>>("dicServiceRec");            
            builder.RegisterInstance(new MyService(dicActionRec, dicServiceRec, new UnitHelper()));
            builder.RegisterInstance<IConfiguration>(configuration);

            AppRootPath = env.ContentRootPath;
            _logger.Info("_appRootPath: " + AppRootPath);
            string filePath = Path.Combine(AppRootPath, "App_Data/Scheduler.json");
            _logger.Info("filePath: " + filePath);
            string schedulerJson = File.ReadAllText(filePath);
            var schedulerSetting = JsonConvert.DeserializeObject<SchedulerSetting>(schedulerJson);
            var schedulerConfig = new NameValueCollection
            {
                {"quartz.threadPool.threadCount", schedulerSetting.Scheduler.ThreadCount}
            };
            builder.RegisterModule(new QuartzAutofacFactoryModule
            {
                ConfigurationProvider = c => schedulerConfig
            });



            var assembly = typeof(JobConfigure).Assembly;
            builder.RegisterModule(new QuartzAutofacJobsModule(assembly));
            builder.RegisterType<JobConfigure>().As<IJobConfigure>()
                .WithParameter("jobSettings", schedulerSetting.Scheduler.JobSettings)
                .AsSelf();


            string jobFilePath = Path.Combine(AppRootPath, "App_Data/Autofac/Job.json");
            _logger.Info($"jobFilePath: {jobFilePath}");
            builder.RegisterModule(new ConfigurationModule(new ConfigurationBuilder().AddJsonFile(jobFilePath).Build()));

            builder.RegisterType<SkypeMessageSender>().As<ISender>();

            var container = builder.Build();


            JobConfigure.Container = container;
            using (var scope = container.BeginLifetimeScope())
            {
                var scheduler = scope.Resolve<IJobConfigure>();
                scheduler.Start();
            }


        }
        public static string AppRootPath { get; set; }
        public IConfiguration Configuration { get; }

        /// <summary>
        /// ¬¡°Ê¬ö¿ý
        /// </summary>
        public static NamedConcurrentDictionary<string, DateTime> dicActionRec = new NamedConcurrentDictionary<string, DateTime>("dicActionRec");
        public static NamedConcurrentDictionary<string, DateTime> dicServiceRec = new NamedConcurrentDictionary<string, DateTime>("dicServiceRec");


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new MyService(dicActionRec, dicServiceRec, new UnitHelper()));
            services.AddSingleton(new UnitHelper());
            services.AddSingleton(Configuration);
            services.AddScoped<IRecorder, Recorder>();
            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }


}
