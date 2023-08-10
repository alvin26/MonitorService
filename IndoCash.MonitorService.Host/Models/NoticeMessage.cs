using System;

namespace IndoCash.MonitorService.Host.Models
{
    public class NoticeMessage
    {
        public NoticeMessage() { }
        public string Guid { get; set; }
        public string IP { get; set; }
        public string MachineName { get; set; }
        public string ServiceName { get; set; }
        public string JobName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string IsSuccess { get; set; }
    }
}
