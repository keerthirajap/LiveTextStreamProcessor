namespace LiveTextStreamProcessorTest.WebApp.InMemoryCache
{
    using LiveTextStreamProcessorWebApp.Cache;
    using LiveTextStreamProcessorWebApp.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InMemoryCacheServiceTests
    {
        [TestMethod]
        public void GetCachedData_ReturnsNullInitially()
        {
            // Arrange

            // Act
            var cachedData = InMemoryCacheService.Instance.GetCachedData();

            // Assert
            Assert.IsNull(cachedData, "Cached data should be null initially");
        }

        [TestMethod]
        public void SetCachedData_SetsDataCorrectly()
        {
            // Arrange
            var testData = new StreamDataModel { TotalCharacters = 10, TotalWords = 2 };

            // Act
            InMemoryCacheService.Instance.SetCachedData(testData);
            var cachedData = InMemoryCacheService.Instance.GetCachedData();

            // Assert
            Assert.AreEqual(testData, cachedData, "Cached data should be set correctly");
        }

        [TestMethod]
        public void SetCachedData_OverwritesExistingData()
        {
            // Arrange
            var initialData = new StreamDataModel { TotalCharacters = 10, TotalWords = 2 };
            var updatedData = new StreamDataModel { TotalCharacters = 15, TotalWords = 3 };

            // Act
            InMemoryCacheService.Instance.SetCachedData(initialData);
            InMemoryCacheService.Instance.SetCachedData(updatedData);
            var cachedData = InMemoryCacheService.Instance.GetCachedData();

            // Assert
            Assert.AreEqual(updatedData, cachedData, "Cached data should be updated correctly");
        }
    }
}