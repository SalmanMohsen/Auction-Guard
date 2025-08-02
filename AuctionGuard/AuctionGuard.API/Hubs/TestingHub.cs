using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuctionGuard.API.Hubs
{
    //[Authorize]
    public class TestingHub: Hub
    {
        private readonly ILogger<TestingHub> _logger;

        public TestingHub(ILogger<TestingHub> logger)
        {
            _logger = logger;
        }

        public async Task Ping(string message)
        {
            
            // This is the log we are trying to see
            _logger.LogInformation("-------------------------------------------- TEST HUB PING RECEIVED: {Message} -------------------------------------", message);

            // Send a message back to the caller to confirm
            await Clients.All.SendAsync("Pong", $"Server received your message: {message}");
        }
    }
}
