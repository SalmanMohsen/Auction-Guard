using AuctionGuard.Application.DTOs.AuctionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    public interface IAuctionService
    {
        Task<(AuctionDto? auction, string? error)> CreateAuctionAsync(CreateAuctionDto dto, Guid sellerId);
        Task<IEnumerable<AuctionDto?>> GetActiveAuctionsAsync();
        Task<IEnumerable<AuctionDto?>> GetScheduledAuctionsAsync();
        Task<IEnumerable<AuctionDto?>> GetCancelledAuctionsAsync();
        Task<IEnumerable<AuctionDto?>> GetEndedAuctionsAsync();
        Task<IEnumerable<AuctionDto>> GetSellerAuctionsAsync(Guid id);
        Task<IEnumerable<AuctionDto>> GetMyWonAuctionsAsync(Guid id);
        Task<AuctionDto> GetAuctionByIdAsync(string id);
        Task<(AuctionDto? auction, string? error)> UpdateAuctionAsync(Guid auctionId, UpdateAuctionDto dto, Guid userId);
        Task<(bool success, string? error)> AttemptRemakeAuctionAsync(Guid auctionId, Guid sellerId);
        Task<(bool success, string? error)> DeleteAuctionAsync(Guid auctionId, Guid userId,bool isAdmin);
        Task<(bool success, string? error)> CancelAuctionAsync(Guid auctionId, Guid userId, bool isAdmin, string reason);

    }
}
