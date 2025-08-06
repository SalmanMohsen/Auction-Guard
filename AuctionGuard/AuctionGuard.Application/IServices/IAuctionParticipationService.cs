using AuctionGuard.Application.DTOs.AuctionParticipationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    /// <summary>
    /// Defines the contract for handling auction participation, including the guarantee deposit hold.
    /// </summary>
    public interface IAuctionParticipationService
    {
        /// <summary>
        /// Initiates the process for a user to join an auction. It creates a PayPal order
        /// for the guarantee deposit and returns an approval link.
        /// </summary>
        /// <param name="auctionId">The ID of the auction to join.</param>
        /// <param name="userId">The ID of the user joining.</param>
        /// <returns>A response DTO with the PayPal approval URL or an error.</returns>
        Task<(JoinAuctionResponseDto? response, string? error)> InitiateJoinProcessAsync(Guid auctionId, Guid userId);

        /// <summary>
        /// Confirms and finalizes the auction participation after the user approves the hold on PayPal.
        /// </summary>
        /// <param name="payPalOrderId">The order ID returned from PayPal.</param>
        /// <param name="userId">The ID of the user who is joining.</param>
        /// <param name="auctionId">The ID of the auction being joined.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<(bool success, string? error)> ConfirmJoinProcessAsync(string payPalOrderId, Guid userId, Guid auctionId);

        /// <summary>
        /// Allows a user to leave a scheduled auction, releasing any held guarantee deposit.
        /// </summary>
        /// <param name="auctionId">The ID of the auction to leave.</param>
        /// <param name="userId">The ID of the user leaving.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<(bool success, string? error)> LeaveAuctionAsync(Guid auctionId, Guid userId);

        /// <summary>
        /// Checks if a user is currently a participant in a specific auction.
        /// </summary>
        /// <param name="auctionId">The ID of the auction.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A DTO indicating the participation status.</returns>
        Task<ParticipationStatusDto> CheckParticipationStatusAsync(Guid auctionId, Guid userId);
    }
}
