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
using System.Diagnostics;

namespace IndoCash.MonitorService.CheckerJob
{
    public class CheckerJob : JobExecute
    {
        public ILogger _logger = LogManager.GetLogger($"MonitorService.{nameof(CheckerJob)}");
        public MyService myService { get; set; }
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
            var mb = Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024;
            _logger.Trace($"CheckerJob Execute 目前程式記憶體使用量: {mb} mb");


            if (myService == null)
            {
                var errmsg = "MyService inject fail!";
                _logger.Error(errmsg);
                throw new Exception(errmsg);
            }

            _logger.Trace($"DicService({myService.DicServiceRec.Count})");
            if (Configuration == null)
            {
                var errmsg = "Configuration inject fail!";
                _logger.Error(errmsg);
                throw new Exception(errmsg);
            }

            if (Configuration["WarningMins"] == null)
            {
                var errmsg = "Configuration WarningMins is null!";
                throw new Exception(errmsg);
            }
            if (Configuration["ErrorMins"] == null)
            {
                var errmsg = "Configuration ErrorMins is null!";
                throw new Exception(errmsg);
            }

            var root = Configuration.GetSection("WatchingList");
            if (root == null)
            {
                var errmsg = "Configuration WatchingList Section is null!";
                throw new Exception(errmsg);
            }

            var WarningMins = double.Parse($"{Configuration["WarningMins"]}");
            var ErrorMins = double.Parse($"{Configuration["ErrorMins"]}");

            DateTime now = helper.GetNow();

            var WatchList = root.GetChildren();
            foreach (var watchItem in WatchList)
            {
                var WatchName = $"{watchItem.Value}";
                _logger.Trace($"正在檢查 service:{WatchName}");
                var serviceLastUpdateDateTime = myService.GetDicServiceRecValueByKey(WatchName);
                if (serviceLastUpdateDateTime != null)
                {
                    var serviceLastDateTime = serviceLastUpdateDateTime.Value;
                    var diff = Math.Round(now.Subtract(serviceLastDateTime).TotalMinutes, 2);
                    if (IsMoreThanBufferMins(serviceLastDateTime, now, ErrorMins))
                    {
                        var msg = $"{WatchName} 已經 {diff} 分鐘以上沒有回應!";
                        SendError(msg);
                    }
                    else if (IsMoreThanBufferMins(serviceLastDateTime, now, WarningMins))
                    {
                        var msg = $"{WatchName} 已經 {diff} 分鐘以上沒有回應!";
                        SendWarning(msg);
                    }
                    else
                    {
                        _logger.Info($"{WatchName} 正常工作中");
                    }
                }
                else
                {
                    //沒有紀錄 有異常
                    var msg = $"{WatchName} 異常! 沒有活動紀錄!";
                    SendError(msg);
                }
            }
            myService.RemoveInvalidKey();
        }

        public bool IsMoreThanBufferMins(DateTime lastExecTime, DateTime now, double bufferMins)
        {
            var diff = now.Subtract(lastExecTime).TotalMinutes;
            return diff >= bufferMins;
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
