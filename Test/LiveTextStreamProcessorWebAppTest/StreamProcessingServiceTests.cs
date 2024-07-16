using LiveTextStreamProcessorWebApp.Hubs;
using LiveTextStreamProcessorWebApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YourUnitTestProjectNamespace
{
    //[TestClass]
    //public class StreamProcessingServiceTests
    //{
    //    [TestMethod]
    //    public async Task StartProcessing_SuccessfulProcess_CallsHubContextAndCache()
    //    {
    //        // Arrange
    //        var hubContextMock = new Mock<IHubContext<StreamHub>>();
    //        var loggerMock = new Mock<ILogger<StreamProcessingService>>();

    //        hubContextMock.Setup(h => h.Clients.All.SendCoreAsync(
    //            "ReceiveStreamData",
    //            It.IsAny<object[]>(),
    //            default)).Returns(Task.CompletedTask);

    //        var streamProcessingService = new StreamProcessingService(hubContextMock.Object, loggerMock.Object);

    //        // Act
    //        await streamProcessingService.StartProcessing(false);

    //        // Assert
    //        hubContextMock.Verify(h => h.Clients.All.SendCoreAsync(
    //            "ReceiveStreamData",
    //            It.IsAny<object[]>(),
    //            default), Times.AtLeastOnce);

    //        // Verify logging
    //        loggerMock.Verify(
    //            l => l.Log(
    //                It.IsAny<LogLevel>(),
    //                It.IsAny<EventId>(),
    //                It.IsAny<It.IsAnyType>(),
    //                It.IsAny<Exception>(),
    //                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
    //            Times.AtLeastOnce);
    //    }

    //    [TestMethod]
    //    public async Task ReadFromStream_SuccessfulRead_ReturnsString()
    //    {
    //        // Arrange
    //        var wordStreamMock = new Mock<Booster.CodingTest.Library.WordStream>();
    //        var loggerMock = new Mock<ILogger<StreamProcessingService>>();

    //        var mockData = "Mock data";
    //        var testData = Encoding.UTF8.GetBytes(mockData);

    //        wordStreamMock.Setup(w => w.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
    //                      .Returns(Task.FromResult(testData.Length));

    //        var streamProcessingService = new StreamProcessingService(null, loggerMock.Object);

    //        // Use reflection to set private field _wordStream
    //        var wordStreamField = typeof(StreamProcessingService).GetField("_wordStream", BindingFlags.NonPublic | BindingFlags.Instance);
    //        wordStreamField.SetValue(streamProcessingService, wordStreamMock.Object);

    //        // Act
    //        var result = await streamProcessingService.ReadFromStream();

    //        // Assert
    //        Assert.AreEqual(mockData, result);
    //    }

    //    [TestMethod]
    //    public async Task ReadFromStream_ExceptionThrown_LogsError()
    //    {
    //        // Arrange
    //        var wordStreamMock = new Mock<Booster.CodingTest.Library.WordStream>();
    //        var loggerMock = new Mock<ILogger<StreamProcessingService>>();

    //        var exceptionMessage = "Simulated exception";
    //        wordStreamMock.Setup(w => w.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
    //                      .Throws(new Exception(exceptionMessage));

    //        var streamProcessingService = new StreamProcessingService(null, loggerMock.Object);

    //        // Act & Assert
    //        await Assert.ThrowsExceptionAsync<Exception>(() => streamProcessingService.ReadFromStream());

    //        loggerMock.Verify(
    //            l => l.LogError(It.IsAny<Exception>(), $"Error reading from stream"),
    //            Times.Once);
    //    }

    //    [TestMethod]
    //    public void ProcessData_ValidData_ReturnsExpectedModel()
    //    {
    //        // Arrange
    //        var testData = "This is a test data. Hello world!";
    //        var expectedTotalCharacters = testData.Length;
    //        var expectedTotalWords = 7; // Adjust based on your expected data
    //        var expectedLargestWordsCount = 5; // Number of largest words expected
    //        var expectedSmallestWordsCount = 5; // Number of smallest words expected
    //        var expectedMostFrequentWordsCount = 7; // Number of most frequent words expected
    //        var expectedCharacterFrequenciesCount = 15; // Number of unique characters expected

    //        var streamProcessingService = new StreamProcessingService(null, null);

    //        // Act
    //        var result = streamProcessingService.ProcessData(testData);

    //        // Assert
    //        Assert.IsNotNull(result);
    //        Assert.AreEqual(expectedTotalCharacters, result.TotalCharacters);
    //        Assert.AreEqual(expectedTotalWords, result.TotalWords);
    //        Assert.AreEqual(expectedLargestWordsCount, result.LargestWordsWithCounts.Count);
    //        Assert.AreEqual(expectedSmallestWordsCount, result.SmallestWordsWithCounts.Count);
    //        Assert.AreEqual(expectedMostFrequentWordsCount, result.MostFrequentWords.Count);
    //        Assert.AreEqual(expectedCharacterFrequenciesCount, result.CharacterFrequencies.Count);
    //        Assert.AreEqual(testData, result.LiveData);
    //    }
    //}
}