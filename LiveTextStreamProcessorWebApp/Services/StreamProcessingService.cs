namespace LiveTextStreamProcessorWebApp.Services
{
    using LiveTextStreamProcessorService.Interface;
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
        private readonly IWordStreamReaderService _wordStreamReaderService;

        private const int MaxRetryAttempts = 5;
        private const int DelayBetweenRetriesInSeconds = 5;

        public StreamProcessingService(IHubContext<StreamHub> hubContext, ILogger<StreamProcessingService> logger, IWordStreamReaderService wordStreamReaderService)
        {
            _wordStream = new Booster.CodingTest.Library.WordStream(); // Initialize WordStream
            _hubContext = hubContext;
            _logger = logger;
            _wordStreamReaderService = wordStreamReaderService;
        }

        public async Task StartProcessing(bool calContinueLoop = true)
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

                    if (!calContinueLoop)
                    {
                        break;
                    }
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

        public async Task<string> ReadFromStream()
        {
            try
            {
                var buffer = new byte[1024];
                int bytesRead = await _wordStreamReaderService.ReadAsync(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading from stream");
                throw;
            }
        }

        public StreamDataModel ProcessData(string data)
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
    }
}