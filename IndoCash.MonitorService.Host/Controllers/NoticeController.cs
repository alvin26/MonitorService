using IndoCash.MonitorService.Host.Interface;
using IndoCash.MonitorService.Host.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IndoCash.MonitorService.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly ILogger<NoticeController> _logger;
        private readonly IRecorder _record;
        public NoticeController(ILogger<NoticeController> logger, IRecorder record)
        {
            _logger = logger;
            _record = record;
        }

        [HttpPost("ReportToMonitor")]
        public Task<IActionResult> ReportToMonitor(NoticeMessage msg)
        {
            _record.SetRecord(msg);
            return Task.FromResult<IActionResult>(Ok("OK"));
        }

    }
}
