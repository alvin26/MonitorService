using IndoCash.MonitorService.AlertSender.Interfaces;
using IndoCash.MonitorService.CheckerJob;
using IndoCash.MonitorService.Utils;
using IndoCash.MonitorService.Utils.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace IndoCash.MonitorService.CheckerJob.Test
{
    public class CheckerJobTests
    {
        private readonly Mock<MyService> _myServiceMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ISender> _alertSenderMock;
        public CheckerJobTests()
        {

            _myServiceMock = new Mock<MyService>();
            _loggerMock = new Mock<ILogger>();
            _configurationMock = new Mock<IConfiguration>();
            _alertSenderMock = new Mock<ISender>();
        }

        [Fact]
        public void 已經超過緩衝時間_分鐘_Test()
        {

            // Arrange
            var checkerJob = new CheckerJob("test");
            checkerJob = new CheckerJob("test_job")
            {
                myService = _myServiceMock.Object,
                Configuration = _configurationMock.Object,
                alertSender = _alertSenderMock.Object,
                _logger = _loggerMock.Object
            };

            // Act
            var dttm = new DateTime(2023, 04, 28, 0, 0, 0);
            var now = new DateTime(2023, 04, 28, 0, 2, 0);
            var rst = checkerJob.IsMoreThanBufferMins(dttm, now, 2);

            // Assert
            Assert.Equal<bool>(true, rst);
        }

        [Fact]
        public void Execute_NoWatchList_ThrowsException()
        {

            // Arrange
            var checkerJob = new CheckerJob("test");
            checkerJob = new CheckerJob("test_job")
            {
                myService = _myServiceMock.Object,
                Configuration = _configurationMock.Object,
                alertSender = _alertSenderMock.Object,
                _logger = _loggerMock.Object
            };

            var dicActionRec = new NamedConcurrentDictionary<string, DateTime>("dicActionRec");
            dicActionRec.TryAdd("action", DateTime.Now);
            _myServiceMock.SetupGet(myService => myService.DicActionRec).Returns(dicActionRec);

            var dicServiceRec = new NamedConcurrentDictionary<string, DateTime>("dicServiceRec");
            dicServiceRec.TryAdd("service", DateTime.Now);
            _myServiceMock.SetupGet(myService => myService.DicServiceRec).Returns(dicServiceRec);


            _configurationMock.SetupGet(c => c["WarningMins"]).Returns("1");
            _configurationMock.SetupGet(c => c["ErrorMins"]).Returns("2");
            _configurationMock.Setup(c => c.GetSection("WatchingList")).Returns((IConfigurationSection)null);


            // Act & Assert
            var exception = Assert.Throws<Exception>(() => checkerJob.Execute());
            Assert.Equal("Configuration WatchingList Section is null!", exception.Message);

        }

        [Fact]
        public void Execute_NoWarningMinsConfiguration_ThrowsException()
        {
            // Arrange 
            var checkerJob = new CheckerJob("test_job")
            {
                myService = _myServiceMock.Object,
                Configuration = _configurationMock.Object,
                alertSender = _alertSenderMock.Object,
                _logger = _loggerMock.Object
            };

            var dicActionRec = new NamedConcurrentDictionary<string, DateTime>("dicActionRec");
            dicActionRec.TryAdd("action", DateTime.Now);
            _myServiceMock.SetupGet(myService => myService.DicActionRec).Returns(dicActionRec);

            var dicServiceRec = new NamedConcurrentDictionary<string, DateTime>("dicServiceRec");
            dicServiceRec.TryAdd("service", DateTime.Now);
            _myServiceMock.SetupGet(myService => myService.DicServiceRec).Returns(dicServiceRec);

            //string jsonString = @"{
            //  ""urls"": ""http://localhost:17100"",
            //  ""Logging"": {
            //    ""LogLevel"": {
            //      ""Default"": ""Information"",
            //      ""Microsoft"": ""Warning"",
            //      ""Microsoft.Hosting.Lifetime"": ""Information""
            //    }
            //  },
            //  ""AllowedHosts"": ""*"",
            //  ""WarningMins"": 1,
            //  ""ErrorMins"": 2,
            //  ""WatchingList"": [
            //    ""AccountService,""
            //  ]
            //}"
            //;


            //var config = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(jsonString))).Build();
            //_configurationMock.Setup(c => c.GetSection("WatchingList")).Returns(config.GetSection("WatchingList"));
            //_configurationMock.Setup(c => c.GetSection("WatchingList")).Returns((IConfigurationSection)null);


            _configurationMock.SetupGet(c => c["WarningMins"]).Returns((string)null);
            _configurationMock.SetupGet(c => c["ErrorMins"]).Returns("2");



            // Act & Assert 
            var exception = Assert.Throws<Exception>(() => checkerJob.Execute());
            Assert.Equal("Configuration WarningMins is null!", exception.Message);
        }

        [Fact]
        public void Execute_NoErrorMinsConfiguration_ThrowsException()
        {
            // Arrange 
            var checkerJob = new CheckerJob("test_job")
            {
                myService = _myServiceMock.Object,
                Configuration = _configurationMock.Object,
                alertSender = _alertSenderMock.Object,
                _logger = _loggerMock.Object
            };

            var dicActionRec = new NamedConcurrentDictionary<string, DateTime>("dicActionRec");
            dicActionRec.TryAdd("action", DateTime.Now);
            _myServiceMock.SetupGet(myService => myService.DicActionRec).Returns(dicActionRec);

            var dicServiceRec = new NamedConcurrentDictionary<string, DateTime>("dicServiceRec");
            dicServiceRec.TryAdd("service", DateTime.Now);
            _myServiceMock.SetupGet(myService => myService.DicServiceRec).Returns(dicServiceRec);

            //string jsonString = @"{
            //  ""urls"": ""http://localhost:17100"",
            //  ""Logging"": {
            //    ""LogLevel"": {
            //      ""Default"": ""Information"",
            //      ""Microsoft"": ""Warning"",
            //      ""Microsoft.Hosting.Lifetime"": ""Information""
            //    }
            //  },
            //  ""AllowedHosts"": ""*"",
            //  ""WarningMins"": 1,
            //  ""ErrorMins"": 2,
            //  ""WatchingList"": [
            //    ""AccountService,""
            //  ]
            //}"
            //;


            //var config = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(jsonString))).Build();

            //_configurationMock.Setup(c => c.GetSection("WatchingList")).Returns(config.GetSection("WatchingList"));



            _configurationMock.SetupGet(c => c["WarningMins"]).Returns("1");
            _configurationMock.SetupGet(c => c["ErrorMins"]).Returns((string)null);



            // Act & Assert 
            var exception = Assert.Throws<Exception>(() => checkerJob.Execute());
            Assert.Equal("Configuration ErrorMins is null!", exception.Message);
        }

        [Fact]
        public void Execute_WithConfigurationIsNull_ShouldThrowException()
        {
            // Arrange
            var checkerJob = new CheckerJob("test");
            checkerJob.myService = _myServiceMock.Object;
            checkerJob.Configuration = null;
            checkerJob.alertSender = _alertSenderMock.Object;
            checkerJob._logger = _loggerMock.Object;

            var dicActionRec = new NamedConcurrentDictionary<string, DateTime>("dicActionRec");
            dicActionRec.TryAdd("action", DateTime.Now);
            _myServiceMock.SetupGet(myService => myService.DicActionRec).Returns(dicActionRec);

            var dicServiceRec = new NamedConcurrentDictionary<string, DateTime>("dicServiceRec");
            dicServiceRec.TryAdd("service", DateTime.Now);
            _myServiceMock.SetupGet(myService => myService.DicServiceRec).Returns(dicServiceRec);

            // Act + Assert
            var exception = Assert.Throws<Exception>(() => checkerJob.Execute());
            Assert.Equal("Configuration inject fail!", exception.Message);
        }

        [Fact]
        public void Execute_WithMyServiceIsNull_ShouldThrowException()
        {
            // Arrange
            var checkerJob = new CheckerJob("test");
            //checkerJob.Configuration = new ConfigurationBuilder()
            //    .AddInMemoryCollection(new Dictionary<string, string>()
            //    {
            //        { "WarningMins", "1" },
            //        { "ErrorMins", "2" }
            //    })
            //    .Build();
            //checkerJob.alertSender = _alertSenderMock.Object;
            //checkerJob._logger = _loggerMock.Object;
            checkerJob.myService = null;

            // Act + Assert 
            var exception = Assert.Throws<Exception>(() => checkerJob.Execute());
            Assert.Equal("MyService inject fail!", exception.Message);
        }
    }
}
