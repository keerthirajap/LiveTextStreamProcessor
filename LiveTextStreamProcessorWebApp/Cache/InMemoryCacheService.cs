namespace LiveTextStreamProcessorWebApp.Cache
{
    using LiveTextStreamProcessorWebApp.Models;

    // Sealed class 
    public sealed class InMemoryCacheService
    {
        // Private static instance of the cache service
        private static readonly InMemoryCacheService instance = new InMemoryCacheService();

       
        private InMemoryCacheService()
        {
        }

        public static InMemoryCacheService Instance
        {
            get
            {
                return instance;
            }
        }

        private StreamDataModel _cachedData;

        public StreamDataModel GetCachedData()
        {
            return _cachedData;
        }

        public void SetCachedData(StreamDataModel data)
        {
            _cachedData = data;
        }
    }
}