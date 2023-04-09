using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IndoCash.MonitorService.CheckerJob
{
    public class CheckHelper
    {
        public CheckHelper() { }
        public virtual string GetLocalIP()
        {
            return (from ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    where ip.AddressFamily == AddressFamily.InterNetwork
                    select $"{ip}").FirstOrDefault();
        }
        public virtual string GetMachineName()
        {
            return Environment.MachineName;
        }

        public virtual DateTime GetNow() { return DateTime.Now; }
    }
}
