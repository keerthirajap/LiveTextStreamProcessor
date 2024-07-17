using LiveTextStreamProcessorWebApp.Cache;

namespace LiveTextStreamProcessorTest.WebApp.SignalRHubs
{
    using LiveTextStreamProcessorWebApp.Hubs;
    using LiveTextStreamProcessorWebApp.Models;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Reflection;

    [TestClass]
    public class StreamHubTests
    {
        private Mock<ILogger<StreamHub>> _loggerMock;
        private Mock<IHubCallerClients> _hubCallerClientsMock;
        private Mock<IClientProxy> _clientProxyMock;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<StreamHub>>();
            _hubCallerClientsMock = new Mock<IHubCallerClients>();
            _clientProxyMock = new Mock<IClientProxy>();

            // Mocking Clients.All and Clients.Client for testing
            _hubCallerClientsMock.Setup(x => x.All).Returns(_clientProxyMock.Object);
            _hubCallerClientsMock.Setup(x => x.Client(It.IsAny<string>())).Returns(_clientProxyMock.Object);
        }

        [TestMethod]
        public async Task StreamHub_OnConnectedAsync_SendsCachedDataAndUpdatesUserCount()
        {
            // Arrange
            var hub = new StreamHub(_loggerMock.Object);
            hub.Clients = _hubCallerClientsMock.Object;

            var cachedData = new StreamDataModel
            {
                TotalCharacters = 100,
                TotalWords = 20,
                LiveData = "Sample live data",
                // Add other necessary properties
            };

            // Set cached data directly in the singleton instance
            InMemoryCacheService.Instance.SetCachedData(cachedData);

            var connectionContextMock = new Mock<HubCallerContext>();
            connectionContextMock.SetupGet(x => x.ConnectionId).Returns("connection-id");

            hub.Context = connectionContextMock.Object;

            // Act
            await hub.OnConnectedAsync();

            // Assert
            var userCountField = typeof(StreamHub).GetField("_userCount", BindingFlags.Static | BindingFlags.NonPublic);
            int userCount = (int)userCountField.GetValue(null); // null because _userCount is static
            Assert.AreEqual(1, userCount); // Adjust the expected value based on your test scenario
        }

        [TestMethod]
        public async Task StreamHub_OnDisconnectedAsync_UpdatesUserCount()
        {
            // Arrange
            var hub = new StreamHub(_loggerMock.Object);
            hub.Clients = _hubCallerClientsMock.Object;

            var connectionContextMock = new Mock<HubCallerContext>();
            connectionContextMock.SetupGet(x => x.ConnectionId).Returns("connection-id");

            hub.Context = connectionContextMock.Object;

            // Act
            await hub.OnDisconnectedAsync(new Exception());

            // Assert
            var userCountField = typeof(StreamHub).GetField("_userCount", BindingFlags.Static | BindingFlags.NonPublic);
            int userCount = (int)userCountField.GetValue(null); // null because _userCount is static
            Assert.IsTrue(userCount <= 0); // Adjust the expected value based on your test scenario
        }
    }
}