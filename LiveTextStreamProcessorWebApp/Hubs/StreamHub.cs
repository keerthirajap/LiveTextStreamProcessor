namespace LiveTextStreamProcessorWebApp.Hubs
{
    using LiveTextStreamProcessorWebApp.Models;
    using Microsoft.AspNetCore.SignalR;

    public class StreamHub : Hub
    {
        private static int _userCount = 0;

        public override Task OnConnectedAsync()
        {
            _userCount++;
            Clients.All.SendAsync("UpdateUserCount", _userCount);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _userCount--;
            Clients.All.SendAsync("UpdateUserCount", _userCount);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendStreamData(StreamDataModel data)
        {
            await Clients.All.SendAsync("ReceiveStreamData", data);
        }
    }
}