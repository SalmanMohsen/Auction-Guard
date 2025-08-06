using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuctionGuard.API.Hubs
{

    
    public sealed class TestingHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Received Message", $"{Context.ConnectionId} has joined");
        }
    }
}
