using IndoCash.MonitorService.AlertSender.Interfaces;
using IndoCash.MonitorService.Utils;
using IndoCash.MonitorService.Utils.Models;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace IndoCash.MonitorService.CheckerJob
{
    public class CheckerJob : JobExecute
    {
        public ILogger _logger = LogManager.GetLogger($"MonitorService.{nameof(CheckerJob)}");
        public MyService myService { get; set; }
        private NamedConcurrentDictionary<string, DateTime> dicActionRec { get; set; }
        private NamedConcurrentDictionary<string, DateTime> dicServiceRec { get; set; }
        public IConfiguration Configuration { get; set; }
        public ISender alertSender { get; set; }
        public CheckHelper helper = new CheckHelper();
        private static string _jobType;
        public CheckerJob(string jobType)
        {
            _jobType = jobType;
        }




        public virtual string GetWarningMins()
        {
            return Configuration["WarningMins"];
        }
        public virtual string GetErrorMins()
        {
            return Configuration["ErrorMins"];
        }
        public override void Execute()
        {

            _logger.Trace("CheckerJob Execute");
            if (Configuration == null)
            {
                var errmsg = "Configuration inject fail!";
                _logger.Error(errmsg);
                throw new Exception(errmsg);
            }
            var root = Configuration.GetSection("WatchingList");
            if (root == null)
            {
                var errmsg = "Configuration WatchingList Section is null!";
                throw new Exception(errmsg);
            }
            var WatchList = root.GetChildren();
            var WarningMins = double.Parse($"{Configuration["WarningMins"]}");
            var ErrorMins = double.Parse($"{Configuration["ErrorMins"]}");

            DateTime now = helper.GetNow();

            foreach (var watchItem in WatchList)
            {
                var WatchName = watchItem.Value;
                var fullWatchName = $"{helper.GetLocalIP()},{helper.GetMachineName()},{WatchName}";
                _logger.Trace($"正在檢查 service:{fullWatchName}");
                var serviceLastUpdateDateTime = myService.GetDicServiceRecValueByKey(WatchName);
                if (serviceLastUpdateDateTime != null)
                {
                    var serviceLastDateTime = serviceLastUpdateDateTime.Value;
                    var diff = Math.Round(now.Subtract(serviceLastDateTime).TotalMinutes, 2);
                    if (diff > ErrorMins)
                    {
                        var msg = $"{fullWatchName} 已經 {diff} 分鐘以上沒有回應!";
                        SendError(msg);
                    }
                    else if (diff > WarningMins)
                    {
                        var msg = $"{fullWatchName} 已經 {diff} 分鐘以上沒有回應!";
                        SendWarning(msg);
                    }
                    else
                    {
                        _logger.Info($"{fullWatchName} 正常工作中");
                    }
                }
                else
                {
                    //沒有紀錄 有異常
                    var msg = $"{fullWatchName} 異常! 沒有活動紀錄!";
                    SendError(msg);
                }
            }

        }

        private void SendWarning(string warningMessage)
        {
            if (alertSender == null) return;
            if (string.IsNullOrWhiteSpace(warningMessage)) return;
            var msg = $"[Service Warning]: {warningMessage}";
            alertSender.SendAlert(msg);
            _logger.Warn($"{warningMessage}");
        }

        private void SendError(string errorMessage)
        {
            if (alertSender == null) return;
            if (string.IsNullOrWhiteSpace(errorMessage)) return;
            var msg = $"[Service Error]: {errorMessage}";
            alertSender.SendAlert(msg);
            _logger.Error($"{errorMessage}");
        }
    }
}
