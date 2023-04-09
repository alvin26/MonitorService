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
            var ActionKey = $"{msg.IP},{msg.MachineName},{msg.ServiceName},{msg.JobName},{msg.ActionName},{msg.Guid}";
            var ServiceKey = $"{msg.IP},{msg.MachineName},{msg.ServiceName},{msg.JobName}";
            _myService.SetDicServiceRecKeyValue(ServiceKey, DateTime.Now);


            //成功執行就不做紀錄
            if (msg.IsSuccess.Equals("Y"))
            {
                _logger.LogTrace($"執行成功:{ActionKey}");

                _myService.RemoveDicActionRecByKey(ActionKey);
            }
            else if (msg.IsSuccess.Equals("N"))
            {
                //失敗要 Log 下來
                _logger.LogError($"執行失敗:{ActionKey}");

                _myService.RemoveDicActionRecByKey(ActionKey);

            }
            else
            {
                _logger.LogTrace($"執行開始:{ActionKey}");

                //開始執行要做紀錄
                _myService.SetDicActionRecKeyValue(ActionKey, DateTime.Now);
            }
        }


    }
}
