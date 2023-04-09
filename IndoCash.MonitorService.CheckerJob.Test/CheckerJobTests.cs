using IndoCash.MonitorService.AlertSender.Interfaces;
using IndoCash.MonitorService.CheckerJob;
using IndoCash.MonitorService.Utils.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace IndoCash.MonitorService.CheckerJob.Test
{
    public class CheckerJobTests
    {
        private readonly CheckerJob _checkerJob;
        private readonly Mock<MyService> _mockMyService = new Mock<MyService>();
        private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>();
        private readonly Mock<ISender> _mockAlertSender = new Mock<ISender>();
        private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();
        private readonly Mock<CheckHelper> _mockCheckHelper = new Mock<CheckHelper>();
        public CheckerJobTests()
        {
            _checkerJob = new CheckerJob("TestJobType")
            {
                myService = _mockMyService.Object,
                //_logger = _mockLogger.Object,
                alertSender = _mockAlertSender.Object,
                Configuration = _mockConfiguration.Object,
                helper= _mockCheckHelper.Object,
                _logger= _mockLogger.Object
            };
        }

        class FakeConfigurationSection : IConfigurationSection
        {

            public FakeConfigurationSection(string key, string value)
            {
                this._key = key;
                this._val = value;
            }
            public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            string _key;
            public string Key { get { return _key; } set { value = _key; } }

            public string Path => throw new NotImplementedException();
            string _val;
            public string Value { get { return this._val; } set { _val = value; } }

            public IEnumerable<IConfigurationSection> GetChildren()
            {
                List<FakeConfigurationSection> rtn = new List<FakeConfigurationSection>();
                int idx = 0;
                foreach (var item in _val.Split(','))
                {
                    rtn.Add(new FakeConfigurationSection($"{idx++}", item));
                }
                return rtn;
            }

            public IChangeToken GetReloadToken()
            {
                throw new NotImplementedException();
            }

            public IConfigurationSection GetSection(string key)
            {
                throw new NotImplementedException();
            }
        }

      

        [Fact]
        public void Execute_ServiceLastUpdateDateTimeIsNull_SendsError()
        {
            // Arrange
            var watchList = new List<FakeConfigurationSection>()
            {
                new FakeConfigurationSection("0","Account,Job1"),
                new FakeConfigurationSection("1","Account,Job2")
            };
            _mockConfiguration.Setup(x => x.GetSection("WatchingList").GetChildren()).Returns(watchList);
            string WarningMins = "5";
            string ErrorMins = "10";
            _mockConfiguration.Setup(x => x["WarningMins"]).Returns(WarningMins);
            _mockConfiguration.Setup(x => x["ErrorMins"]).Returns(ErrorMins);
            _mockMyService.Setup(x => x.GetDicServiceRecValueByKey("Account.Job1")).Returns((DateTime?)null);
            var localIp = IPAddress.Parse("127.0.0.1"); 
            var machineName = "TestMachine";
            var watchName = "Account,Job1";
            _mockCheckHelper.Setup(x => x.GetLocalIP()).Returns(localIp.ToString());
            _mockCheckHelper.Setup(x => x.GetMachineName()).Returns(machineName);
            _mockCheckHelper.Setup(x => x.GetNow()).Returns(DateTime.Now.AddMinutes(-(int.Parse(WarningMins)+1)));
            //_mockAlertSender.Setup(x => x.SendAlert(It.IsAny<string>()));
            // Act
            _checkerJob.Execute();


            // Assert
            _mockAlertSender.Verify(x => x.SendAlert($"[Service Error]: {localIp},{machineName},{watchName} 異常! 沒有活動紀錄!"), Times.AtLeastOnce);
            //_mockAlertSender.Verify(x => x.SendAlert($"[Service Error]: {localIp},{machineName},{watchName} 異常! 沒有活動紀錄!"));
            _mockLogger.Verify(x => x.Error($"{localIp},{machineName},{watchName} 異常! 沒有活動紀錄!"), Times.AtLeastOnce);
        }

        [Fact]
        public void Execute_NoConfiguration_ThrowsException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x.GetSection("WatchingList")).Returns((IConfigurationSection)null);

            // Act + Assert
            Assert.Throws<Exception>(() => _checkerJob.Execute());
        }
    }
}
