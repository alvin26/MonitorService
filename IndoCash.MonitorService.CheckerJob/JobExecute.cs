using IndoCash.MonitorService.CheckerJob.Interfaces;
using NLog;
using System;
using System.Data.Common;
using System.Net;

namespace IndoCash.MonitorService.CheckerJob
{
    public abstract class JobExecute : IJobExecute
    {
        private readonly ILogger WebErrorLogger = LogManager.GetLogger("MonitorService.WebError");
        private readonly ILogger DbErrorLogger = LogManager.GetLogger("MonitorService.DbError");
        private readonly ILogger SysErrorLogger = LogManager.GetLogger("MonitorService.SysError");

        public void DoExecute()
        {
            try
            {
                BeforeExecute();
                Execute();
                AfterExecute();
            }
            catch (WebException wex)
            {
                WebErrorLogger.Error(wex);
            }
            catch (DbException dex)
            {
                DbErrorLogger.Error(dex);
            }
            catch (Exception ex)
            {
                SysErrorLogger.Error(ex);
            }
        }

        protected virtual void BeforeExecute()
        {

        }

        public abstract void Execute();


        protected virtual void AfterExecute()
        {

        }

    }
}
