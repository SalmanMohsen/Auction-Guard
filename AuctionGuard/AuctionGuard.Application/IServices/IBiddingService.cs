using AuctionGuard.Application.DTOs.BiddingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    public interface IBiddingService
    {
        Task<(bool success, string? error)> PlaceBidAsync(Guid auctionId, decimal amount, Guid userId);
    }
}
