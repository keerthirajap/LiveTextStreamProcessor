namespace LiveTextStreamProcessorWebApp.Models
{
    public class StreamDataModel
    {
        public int TotalCharacters { get; set; }
        public int TotalWords { get; set; }
        public List<string> LargestWords { get; set; }
        public Dictionary<string, int> SmallestWordsWithCounts { get; set; }
        public Dictionary<string, int> MostFrequentWords { get; set; }
        public Dictionary<char, int> CharacterFrequencies { get; set; }
        public int LiveUserCount { get; set; }
    }
}