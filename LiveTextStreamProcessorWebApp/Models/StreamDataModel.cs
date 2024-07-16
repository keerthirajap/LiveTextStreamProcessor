namespace LiveTextStreamProcessorWebApp.Models
{
    public class StreamDataModel
    {
        public int TotalCharacters { get; set; }
        public int TotalWords { get; set; }
        public Dictionary<string, int> LargestWords { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SmallestWordsWithCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MostFrequentWords { get; set; } = new Dictionary<string, int>();
        public Dictionary<char, int> CharacterFrequencies { get; set; } = new Dictionary<char, int>();
        public int LiveUserCount { get; set; }
        public string LiveData { get; set; }
    }
}
