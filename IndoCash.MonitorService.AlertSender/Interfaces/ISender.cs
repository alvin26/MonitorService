using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoCash.MonitorService.AlertSender.Interfaces
{
    public interface ISender
    {
        void SendAlert(string Message);
    }
}
