namespace LiveTextStreamProcessorTest.WebApp.StreamProcessingService
{
    using LiveTextStreamProcessorService.Interface;
    using LiveTextStreamProcessorTest.Inftrastructure;
    using LiveTextStreamProcessorWebApp.Hubs;
    using LiveTextStreamProcessorWebApp.Models;
    using LiveTextStreamProcessorWebApp.Services;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Newtonsoft.Json;
    using System;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class StreamProcessingServiceTests
    {
        private Mock<IHubContext<StreamHub>> _mockHubContext;
        private Mock<IHubClients> _mockClients;
        private Mock<IClientProxy> _mockClientProxy;
        private Mock<IWordStreamReaderService> _mockWordStreamReaderService;
        private TestLogger<StreamProcessingService> _testLogger;
        private StreamProcessingService _streamProcessingService;

        [TestInitialize]
        public void Setup()
        {
            _mockHubContext = new Mock<IHubContext<StreamHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockWordStreamReaderService = new Mock<IWordStreamReaderService>();
            _testLogger = new TestLogger<StreamProcessingService>();

            _mockHubContext.Setup(m => m.Clients).Returns(_mockClients.Object);
            _mockClients.Setup(m => m.All).Returns(_mockClientProxy.Object);

            _streamProcessingService = new StreamProcessingService(
                _mockHubContext.Object,
                _testLogger,
                _mockWordStreamReaderService.Object
            );
        }

        [TestMethod]
        public async Task StartProcessing_SuccessfulProcessing()
        {
            // Arrange
            var mockData = "Mock data";
            var buffer = Encoding.UTF8.GetBytes(mockData);
            var processedData = new StreamDataModel { LiveData = mockData };

            _mockWordStreamReaderService.Setup(w => w.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                                        .Returns<byte[], int, int>(async (b, offset, count) =>
                                        {
                                            Buffer.BlockCopy(buffer, 0, b, offset, buffer.Length);
                                            return buffer.Length;
                                        });

            object[] actualArgs = null;
            _mockClientProxy.Setup(c => c.SendCoreAsync(
                "ReceiveStreamData",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()))
                .Callback<string, object[], CancellationToken>((method, args, token) =>
                {
                    actualArgs = args;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _streamProcessingService.StartProcessing(false);

            // Assert
            _mockClientProxy.Verify(c => c.SendCoreAsync(
                "ReceiveStreamData",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);

            Assert.IsNotNull(actualArgs);
            Assert.IsTrue(actualArgs.Length > 0);
            Assert.IsInstanceOfType(actualArgs[0], typeof(StreamDataModel));
            var actualModel = (StreamDataModel)actualArgs[0];
            Assert.AreEqual(mockData, actualModel.LiveData);
        }

        [TestMethod]
        public async Task ReadFromStream_SuccessfulRead_ReturnsString()
        {
            // Arrange
            var mockData = "Mock data";
            var testData = Encoding.UTF8.GetBytes(mockData);

            _mockWordStreamReaderService.Setup(w => w.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                                        .ReturnsAsync(testData.Length)
                                        .Callback((byte[] buffer, int offset, int count) =>
                                        {
                                            Array.Copy(testData, 0, buffer, 0, testData.Length);
                                        });

            // Act
            var result = await _streamProcessingService.ReadFromStream();

            // Assert
            Assert.AreEqual(mockData, result);
        }

        [TestMethod]
        public async Task ReadFromStream_ExceptionThrown_LogsError()
        {
            // Arrange
            var exceptionMessage = "Simulated exception";
            _mockWordStreamReaderService.Setup(w => w.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _streamProcessingService.ReadFromStream());

            // Verify that the log contains the expected error
            var logEntry = _testLogger.LoggedEntries
                                      .Find(e => e.LogLevel == LogLevel.Error && e.Message.Contains("Error reading from stream"));

            Assert.IsNotNull(logEntry);
            Assert.AreEqual(exceptionMessage, logEntry.Exception.Message);
        }

        [TestMethod]
        public void ProcessData_ValidData_ReturnsExpectedModel()
        {
            // Arrange
            var testData = "This is a test data. Hello world!";
            var expectedTotalCharacters = testData.Length;
            var expectedTotalWords = 7; // Adjust based on your expected data
            var expectedLargestWordsCount = 5; // Number of largest words expected
            var expectedSmallestWordsCount = 5; // Number of smallest words expected
            var expectedMostFrequentWordsCount = 7; // Number of most frequent words expected
            var expectedCharacterFrequenciesCount = 15; // Number of unique characters expected

            // Act
            var result = _streamProcessingService.ProcessData(testData);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTotalCharacters, result.TotalCharacters);
            Assert.AreEqual(expectedTotalWords, result.TotalWords);
            Assert.AreEqual(expectedLargestWordsCount, result.LargestWordsWithCounts.Count);
            Assert.AreEqual(expectedSmallestWordsCount, result.SmallestWordsWithCounts.Count);
            Assert.AreEqual(expectedMostFrequentWordsCount, result.MostFrequentWords.Count);
            Assert.AreEqual(expectedCharacterFrequenciesCount, result.CharacterFrequencies.Count);
            Assert.AreEqual(testData, result.LiveData);
        }
    }
}