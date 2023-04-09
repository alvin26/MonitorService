using IndoCash.MonitorService.Host.Models;
using System;
using System.Collections.Generic;

namespace IndoCash.MonitorService.Host.Interface
{
    public interface IRecorder
    {
        void SetRecord(NoticeMessage msg); 
    }
}
