using IndoCash.MonitorService.AlertSender.Interfaces;
using NLog;
using System;

namespace IndoCash.MonitorService.SkypeBotSender
{
    public class SkypeMessageSender : ISender
    {
        private readonly ILogger _logger = LogManager.GetLogger("SkypeBotSender");
        public void SendAlert(string Message)
        {
            //to do
            // send message via skype or some where else
            _logger.Trace(Message);
        }
    }
}
