namespace LiveTextStreamProcessorWebApp.Cache
{
    using LiveTextStreamProcessorWebApp.Models;

    // Sealed class to ensure no inheritance
    public sealed class InMemoryCacheService
    {
        // Private static instance of the cache service
        private static readonly InMemoryCacheService instance = new InMemoryCacheService();

        // Private constructor to prevent instantiation outside the class
        private InMemoryCacheService()
        {
        }

        // Public static method to access the singleton instance
        public static InMemoryCacheService Instance
        {
            get
            {
                return instance;
            }
        }

        // Private cached data field
        private StreamDataModel _cachedData;

        // Public method to get cached data
        public StreamDataModel GetCachedData()
        {
            return _cachedData;
        }

        // Public method to set cached data
        public void SetCachedData(StreamDataModel data)
        {
            _cachedData = data;
        }
    }
}