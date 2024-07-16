namespace LiveTextStreamProcessorWebApp.Models
{
    public class StreamDataModel
    {
        public int TotalCharacters { get; set; }
        public int TotalWords { get; set; }
        public Dictionary<string, int> LargestWordsWithCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SmallestWordsWithCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MostFrequentWords { get; set; } = new Dictionary<string, int>();
        public Dictionary<char, int> CharacterFrequencies { get; set; } = new Dictionary<char, int>();
        public string LiveData { get; set; }

        public string CacheOn { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}