using Autofac;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IndoCash.MonitorService.Host.Interface;
using IndoCash.MonitorService.CheckerJob.Interfaces;

namespace IndoCash.MonitorService.Host.Models
{
    [DisallowConcurrentExecution]
    public class JobConfigure : IJobConfigure, IJob
    {
        public static IContainer Container { get; set; }
        public static IScheduler Scheduler { get; set; }
        private static List<JobSetting> _JobSettings { get; set; }

        private Logger _logger = LogManager.GetLogger("MonitorService.JobConfigure");

        public JobConfigure(IScheduler scheduler, List<JobSetting> jobSettings)
        {
            if (Scheduler == null)
            {
                Scheduler = scheduler;
            }
            if (_JobSettings == null)
            {
                _JobSettings = jobSettings;
            }
        }

        public void Start()
        {
            Scheduler.Start();
            _logger.Info("Scheduler Start...");
            foreach (var jobSetting in _JobSettings)
            {
                if (jobSetting.Enable)
                {
                    var job = JobBuilder.Create<JobConfigure>()
                        .WithIdentity(jobSetting.Name)
                        .Build();

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(jobSetting.Name + "Trigger")
                        .WithSimpleSchedule(x => x
                            .RepeatForever()
                            .WithIntervalInSeconds(jobSetting.RepeatIntervalInSeconds))
                        .ForJob(job)
                        .UsingJobData("JobName", jobSetting.Name)
                        .StartAt(DateBuilder.FutureDate(jobSetting.StartDelayInSeconds, IntervalUnit.Second))
                        .Build();

                    Scheduler.ScheduleJob(job, trigger);
                }
            }
        }

        public virtual async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var jobName = context.Trigger.JobDataMap["JobName"];
                using (var scope = Container.BeginLifetimeScope())
                {
                    var ex = scope.ResolveKeyed<IJobExecute>(jobName);
                    ex.DoExecute();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
    }
}
