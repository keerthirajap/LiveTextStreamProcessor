namespace LiveTextStreamProcessorWebApp.Services
{
    using Hangfire;
    using LiveTextStreamProcessorWebApp.Hubs;
    using LiveTextStreamProcessorWebApp.Models;
    using Microsoft.AspNetCore.SignalR;
    using System.Text.RegularExpressions;

    public class StreamProcessingService
    {
        private readonly IHubContext<StreamHub> _hubContext;
        private readonly Booster.CodingTest.Library.WordStream _wordStream;

        public StreamProcessingService(IHubContext<StreamHub> hubContext)
        {
            _wordStream = new Booster.CodingTest.Library.WordStream(); // Initialize WordStream
            _hubContext = hubContext;
        }

        public async Task StartProcessing()
        {
            while (true)
            {
                // Read data from stream
                var data = await ReadFromStream();

                // Process data
                var processedData = ProcessData(data);

                await _hubContext.Clients.All.SendAsync("ReceiveStreamData", processedData); // Use _hubContext instead of Clients directly

                // Delay for 5 seconds
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        private async Task<string> ReadFromStream()
        {
            // Example of how to read asynchronously from the stream
            var buffer = new byte[1024]; // Example buffer size
            var bytesRead = await _wordStream.ReadAsync(buffer, 0, buffer.Length);
            var data = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
            return data;
        }

        private StreamDataModel ProcessData(string data)
        {
            // Example logic to process stream data
            var model = new StreamDataModel
            {
                TotalCharacters = data.Length,
                TotalWords = data.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length,
                LargestWords = GetLargestWords(data, 5),
                SmallestWordsWithCounts = GetSmallestWordsWithCounts(data, 5),
                MostFrequentWords = GetMostFrequentWords(data, 10),
                CharacterFrequencies = GetCharacterFrequencies(data),
                //LiveUserCount = new Random().Next(1, 100) // Example random live user count
            };

            return model;
        }

        private int CountWords(string data)
        {
            // Example method to count words in the data
            if (string.IsNullOrWhiteSpace(data))
                return 0;

            var words = Regex.Split(data, @"\W+").Where(word => !string.IsNullOrEmpty(word)); // Exclude empty entries
            return words.Count();
        }

        private List<string> GetLargestWords(string data, int count)
        {
            // Example method to get largest words
            if (string.IsNullOrWhiteSpace(data))
                return new List<string>();

            var words = Regex.Split(data, @"\W+")
                            .Where(word => !string.IsNullOrEmpty(word)) // Exclude empty entries
                            .OrderByDescending(w => w.Length)
                            .Take(count)
                            .ToList();

            return words;
        }

        private Dictionary<string, int> GetSmallestWordsWithCounts(string data, int count)
        {
            var words = Regex.Split(data, @"\W+");
            var wordCounts = new Dictionary<string, int>();

            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    var trimmedWord = word.Trim(); // Trim any leading or trailing whitespace
                    if (wordCounts.ContainsKey(trimmedWord))
                    {
                        wordCounts[trimmedWord]++;
                    }
                    else
                    {
                        wordCounts[trimmedWord] = 1;
                    }
                }
            }

            // Order by ascending length and then alphabetically by word
            var smallestWords = wordCounts.OrderBy(pair => pair.Key.Length)
                                          .ThenBy(pair => pair.Key)
                                          .Take(count)
                                          .ToDictionary(pair => $"{pair.Key} ({pair.Key.Length})", pair => pair.Value);

            return smallestWords;
        }

        private Dictionary<string, int> GetMostFrequentWords(string data, int count)
        {
            // Example logic to get most frequent words
            var words = Regex.Split(data, @"\W+");
            var wordCounts = new Dictionary<string, int>();

            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    if (wordCounts.ContainsKey(word))
                    {
                        wordCounts[word]++;
                    }
                    else
                    {
                        wordCounts[word] = 1;
                    }
                }
            }

            // Order by descending frequency and take the top 'count'
            var mostFrequent = wordCounts.OrderByDescending(pair => pair.Value)
                                         .Take(count)
                                         .ToDictionary(pair => pair.Key, pair => pair.Value);

            return mostFrequent;
        }

        private Dictionary<char, int> GetCharacterFrequencies(string data)
        {
            // Example method to get character frequencies
            var charFreq = new Dictionary<char, int>();

            foreach (char c in data)
            {
                if (char.IsLetterOrDigit(c)) // Count letters and digits only
                {
                    if (charFreq.ContainsKey(c))
                        charFreq[c]++;
                    else
                        charFreq[c] = 1;
                }
            }

            // Sort dictionary by frequency descending
            charFreq = charFreq.OrderByDescending(x => x.Value)
                               .ToDictionary(pair => pair.Key, pair => pair.Value);

            return charFreq;
        }
    }
}