namespace LiveTextStreamProcessorWebApp.Services
{
    using LiveTextStreamProcessorWebApp.Cache;
    using LiveTextStreamProcessorWebApp.Hubs;
    using LiveTextStreamProcessorWebApp.Models;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using System.Text;
    using System.Text.RegularExpressions;

    public class StreamProcessingService
    {
        private readonly IHubContext<StreamHub> _hubContext;
        private readonly Booster.CodingTest.Library.WordStream _wordStream;
        private readonly ILogger<StreamProcessingService> _logger;

        private const int MaxRetryAttempts = 5;
        private const int DelayBetweenRetriesInSeconds = 5;

        public StreamProcessingService(IHubContext<StreamHub> hubContext, ILogger<StreamProcessingService> logger)
        {
            _wordStream = new Booster.CodingTest.Library.WordStream(); // Initialize WordStream
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task StartProcessing()
        {
            int retryAttempts = 0;

            while (true)
            {
                try
                {
                    // Read data from stream
                    var data = await ReadFromStream();

                    // Process data
                    var processedData = ProcessData(data);

                    // Use _hubContext to push results
                    await _hubContext.Clients.All.SendAsync("ReceiveStreamData", processedData);

                    _logger.LogInformation($"Send Data to client : {JsonConvert.SerializeObject(processedData)}");

                    // Access the singleton instance and Set cached data
                    InMemoryCacheService.Instance.SetCachedData(processedData);

                    // Reset retry attempts on successful read
                    retryAttempts = 0;

                    // Delay for 5 seconds
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing the stream.");

                    // Increment retry attempts and check if max attempts are reached
                    retryAttempts++;
                    if (retryAttempts >= MaxRetryAttempts)
                    {
                        _logger.LogError("Max retry attempts reached. Stopping processing.");
                        break;
                    }

                    // Delay before retrying
                    await Task.Delay(TimeSpan.FromSeconds(DelayBetweenRetriesInSeconds));
                }
            }
        }

        private async Task<string> ReadFromStream()
        {
            try
            {
                var buffer = new byte[1024];
                int bytesRead = await _wordStream.ReadAsync(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading from stream");
                throw;
            }
        }

        private StreamDataModel ProcessData(string data)
        {
            var wordList = Regex.Split(data, @"\W+")
                                .Where(w => !string.IsNullOrWhiteSpace(w))
                                .ToList();

            var totalCharacters = data.Length;
            var totalWords = wordList.Count;

            var wordGroups = wordList.GroupBy(w => w.Length)
                                     .ToDictionary(g => g.Key, g => g.ToList());

            var largestWords = wordGroups.OrderByDescending(g => g.Key)
                                         .SelectMany(g => g.Value.Distinct())
                                         .Take(5)
                                         .ToList();

            var smallestWords = wordGroups.OrderBy(g => g.Key)
                                          .SelectMany(g => g.Value.Distinct())
                                          .Take(5)
                                          .ToList();

            var mostFrequentWords = wordList.GroupBy(w => w)
                                            .OrderByDescending(g => g.Count())
                                            .Take(10)
                                            .ToDictionary(g => g.Key, g => g.Count());

            var charFrequencies = data.Where(c => !char.IsWhiteSpace(c))
                           .GroupBy(c => c)
                           .ToDictionary(g => g.Key, g => g.Count());

            return new StreamDataModel
            {
                TotalCharacters = totalCharacters,
                TotalWords = totalWords,
                LargestWordsWithCounts = largestWords.ToDictionary(w => w, w => w.Length),
                SmallestWordsWithCounts = smallestWords.ToDictionary(w => w, w => w.Length),
                MostFrequentWords = mostFrequentWords,
                CharacterFrequencies = charFrequencies,
                LiveData = data
            };
        }

        private StreamDataModel ProcessDatav1(string data)
        {
            var model = new StreamDataModel
            {
                TotalCharacters = data.Length,
                TotalWords = data.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length,
                LargestWordsWithCounts = GetLargestWords(data, 5),
                SmallestWordsWithCounts = GetSmallestWordsWithCounts(data, 5),
                MostFrequentWords = GetMostFrequentWords(data, 10),
                CharacterFrequencies = GetCharacterFrequencies(data),
                LiveData = data,
            };

            return model;
        }

        private int CountWords(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return 0;

            var words = Regex.Split(data, @"\W+").Where(word => !string.IsNullOrEmpty(word)); // Exclude empty entries
            return words.Count();
        }

        private Dictionary<string, int> GetLargestWords(string data, int count)
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
            var smallestWords = wordCounts.OrderByDescending(pair => pair.Key.Length)
                                          .ThenBy(pair => pair.Key)
                                          .Take(count)
                                          .ToDictionary(pair => $"{pair.Key}", pair => pair.Key.Length);

            return smallestWords;
        }

        private Dictionary<string, int> GetSmallestWordsWithCounts(string data, int count)
        {
            var words = Regex.Split(data, @"\W+");
            var wordCounts = new Dictionary<string, int>();

            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    var trimmedWord = word.Trim();
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

            var smallestWords = wordCounts.OrderBy(pair => pair.Key.Length)
                                          .ThenBy(pair => pair.Key)
                                          .Take(count)
                                          .ToDictionary(pair => $"{pair.Key}", pair => pair.Key.Length);

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

            var mostFrequent = wordCounts.OrderByDescending(pair => pair.Value)
                                         .Take(count)
                                         .ToDictionary(pair => pair.Key, pair => pair.Value);

            return mostFrequent;
        }

        private Dictionary<char, int> GetCharacterFrequencies(string data)
        {
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