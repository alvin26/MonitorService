using IndoCash.MonitorService.Host.Interface;
using IndoCash.MonitorService.Utils.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IndoCash.MonitorService.Host.Models
{
    public class Recorder : IRecorder
    {
        //private readonly NamedConcurrentDictionary<string, DateTime> _dicActionRec;
        //private readonly NamedConcurrentDictionary<string, DateTime> _dicServiceRec;
        private readonly ILogger<Recorder> _logger;
        private readonly MyService _myService;

        public Recorder(ILogger<Recorder> logger, MyService myService)
        {
            _myService = myService;
            _logger = logger;
        }



        /// <summary>
        /// 最後一次有活動的紀錄
        /// </summary>
        /// <param name="msg"></param>
        public void SetRecord(NoticeMessage msg)
        {
            var ServiceKey = $"{msg.IP},{msg.MachineName},{msg.ServiceName},{msg.JobName}";
            try
            {
                //yyyy-MM-dd HH:mm:ss.fff
                var arrDttm = msg.EndTime.Split(' ');
                var arrDate = arrDttm[0].Split('-');
                var arrTime = arrDttm[1].Split(":");
                var arrMs = arrTime[2].Split('.');
                int year = int.Parse(arrDate[0]);
                int month = int.Parse(arrDate[1]);
                int day = int.Parse(arrDate[2]);
                int hour = int.Parse(arrTime[0]);
                int min = int.Parse(arrTime[1]);
                int sec = int.Parse(arrMs[0]);
                DateTime dttm = new DateTime(year, month, day, hour, min, sec);
                _myService.SetDicServiceRecKeyValue(ServiceKey, dttm);
                _logger.LogTrace($"收到 {ServiceKey} 回報 {msg.EndTime}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Parse msg fail:{ex.Message}");
            }
        }


    }
}
