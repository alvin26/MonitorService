using Autofac.Core;
using IndoCash.MonitorService.Host.Interface;
using IndoCash.MonitorService.Host.Models;
using IndoCash.MonitorService.Utils;
using IndoCash.MonitorService.Utils.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz.Util;
using System;
using System.Configuration;
using Xunit;

namespace IndoCash.MonitorService.Host.UnitTest
{
    public class RecoderTest
    {
        private readonly Recorder _Recorder;
        private readonly Mock<ILogger<Recorder>> _loggerMock = new Mock<ILogger<Recorder>>();
        private readonly Mock<MyService> _myServiceMock = new Mock<MyService>();
        private readonly MyService _myService;
        public RecoderTest()
        {
            _Recorder = new Recorder(_loggerMock.Object, _myServiceMock.Object);
        }

        [Fact]
        public void SetRecord_Service¬ö¿ý_Test()
        {
            // Arrange
            var recorder = new Recorder(_loggerMock.Object, _myService);
            var msg = new NoticeMessage
            {
                IP = "127.0.0.1",
                MachineName = "TestMachine",
                ServiceName = "TestService",
                JobName = "TestJob",
                ActionName = "Action",
                StartTime = "2023/04/07 10:00:00.000",
                EndTime = "2023/04/07 10:00:00.999",
                Guid = "xxxxxx-xxxx-xxxx-xxxx",
                IsSuccess = ""
            };
            var ServiceKey = $"{msg.IP},{msg.MachineName},{msg.ServiceName},{msg.JobName}";
            //_myServiceMock.Setup(x=>x.)

            // Act
            recorder.SetRecord(msg);

            // Assert
            _myServiceMock.Verify(mock => mock.SetDicServiceRecKeyValue(ServiceKey, It.IsAny<DateTime>()), Times.Once);
        }



    }

}
