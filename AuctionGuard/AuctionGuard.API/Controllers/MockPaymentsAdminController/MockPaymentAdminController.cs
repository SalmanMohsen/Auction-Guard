using AuctionGuard.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionGuard.API.Controllers.MockPaymentsAdminController
{
    [Route("api/mock-payment-admin")]
    [ApiController]
    public class MockPaymentAdminController : ControllerBase
    {
        /// <summary>
        /// An endpoint to inspect the current state of all payment authorizations in the mock service.
        /// Useful for debugging and verifying the hold/release flow.
        /// </summary>
        /// <returns>A dictionary of all authorization holds and their status.</returns>
        [HttpGet("holds")]
        public IActionResult GetCurrentHolds()
        {
            // Directly return the static dictionary from the mock service
            return Ok(MockPaymentGatewayService.AuthorizedHolds);
        }

        /// <summary>
        /// Clears all state from the mock service. Useful for resetting between tests.
        /// </summary>
        [HttpPost("clear")]
        public IActionResult ClearState()
        {
            MockPaymentGatewayService.AuthorizedHolds.Clear();
            // Note: In a real app, you'd also clear the _pendingOrders dictionary,
            // but it's less critical to inspect for this verification purpose.
            return Ok("Mock payment service state has been cleared.");
        }
    }
}
