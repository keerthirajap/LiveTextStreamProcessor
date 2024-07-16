namespace LiveTextStreamProcessorWebApp.Hubs
{
    using LiveTextStreamProcessorWebApp.Cache;
    using LiveTextStreamProcessorWebApp.Models;
    using LiveTextStreamProcessorWebApp.Services;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;

    public class StreamHub : Hub
    {
        private static int _userCount = 0;

        private readonly ILogger<StreamHub> _logger;

        public StreamHub(ILogger<StreamHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _userCount++;

            _logger.LogInformation($"OnConnectedAsync User Count : {_userCount}");

            // Send cached data to the new user immediately upon connection
            var cachedData = InMemoryCacheService.Instance.GetCachedData();

            if (cachedData != null)
            {
                Clients.Client(Context.ConnectionId).SendAsync("ReceiveStreamData", cachedData);
                _logger.LogInformation($"Sending cached data to new connection: {JsonConvert.SerializeObject(cachedData)}");
            }

            Clients.All.SendAsync("UpdateUserCount", _userCount);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _userCount--;
            _logger.LogInformation($"OnDisconnectedAsync User Count : {_userCount}");

            Clients.All.SendAsync("UpdateUserCount", _userCount);
            return base.OnDisconnectedAsync(exception);
        }
    }
}