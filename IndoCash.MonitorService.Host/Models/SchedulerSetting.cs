using System.Collections.Generic;

namespace IndoCash.MonitorService.Host.Models
{
    public class SchedulerSetting
    {
        public Scheduler Scheduler { get; set; }
    }
    public class Scheduler
    {
        public string ThreadCount { get; set; }
        public List<JobSetting> JobSettings { get; set; }
    }

    public class JobSetting
    {
        public string Name { get; set; }
        public int StartDelayInSeconds { get; set; }
        public int RepeatIntervalInSeconds { get; set; }
        public bool Enable { get; set; }
    }
}
