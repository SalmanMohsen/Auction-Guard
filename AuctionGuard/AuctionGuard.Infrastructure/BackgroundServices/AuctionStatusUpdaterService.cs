using AuctionGuard.Domain.Entities;
using AuctionGuard.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionGuard.Application.IServices;

namespace AuctionGuard.Infrastructure.BackgroundServices
{
    public class AuctionStatusUpdaterService : BackgroundService
    {
        private readonly ILogger<AuctionStatusUpdaterService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public AuctionStatusUpdaterService(ILogger<AuctionStatusUpdaterService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Auction Status Updater Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateAuctionStatuses();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating auction statuses.");
                }

                // Check every 30 seconds for higher precision
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            _logger.LogInformation("Auction Status Updater Service is stopping.");
        }

        private async Task UpdateAuctionStatuses()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var notifier = scope.ServiceProvider.GetRequiredService<IAuctionNotifier>();
                var auctionRepo = unitOfWork.GetRepository<Auction>();
                var participantRepo = unitOfWork.GetRepository<AuctionParticipant>();
                var propertyRepo = unitOfWork.GetRepository<Property>();
                var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time"));

                // 1. Find auctions that should become ACTIVE
                var auctionsToStart = await auctionRepo.FindAllByPredicateAsync(
                    predicate: a => a.Status == AuctionStatus.Scheduled && a.StartTime <= now
                );

                foreach (var auction in auctionsToStart)
                {
                    var property = await propertyRepo.GetByIdAsync(auction.PropertyId);
                    property.PropertyStatus = PropertyStatus.UnderAuction;
                    auction.Status = AuctionStatus.Active;
                    _logger.LogInformation("------------------------------------------------------Before update the property's status to under auction.......................................................................");
                    propertyRepo.Update(property);
                    _logger.LogInformation("------------------------------------------------------After update the property's status to under auction.......................................................................");
                    auctionRepo.Update(auction);
                    _logger.LogInformation($"Auction '{auction.AuctionId}' is starting.");

                    var participantIds = (await participantRepo.FindAllByPredicateAsync(p => p.AuctionId == auction.AuctionId))
                                             .Select(p => p.ParticipantId.ToString())
                                             .ToList();
                    if (participantIds.Any())
                    {
                        await notifier.NotifyAuctionStarted(participantIds, auction.EndTime);
                    }
                }

                // 2. Find auctions that should be FINISHED
                var auctionsToEnd = await auctionRepo.FindAllByPredicateAsync(
                    predicate: a => a.Status == AuctionStatus.Active && a.EndTime <= now
                );

                foreach (var auction in auctionsToEnd)
                {
                    var property = await propertyRepo.GetByIdAsync(auction.PropertyId);
                    if (auction.WinnerId == null)
                    {
                        property.PropertyStatus = PropertyStatus.Available;
                    }
                    else
                    {
                        property.PropertyStatus = PropertyStatus.Sold; 
                    }
                    propertyRepo.Update(property);
                    auction.Status = AuctionStatus.Ended;
                    auctionRepo.Update(auction);
                    _logger.LogInformation($"Auction '{auction.AuctionId}' has finished.");

                    var participantIds = (await participantRepo.FindAllByPredicateAsync(p => p.AuctionId == auction.AuctionId))
                                             .Select(p => p.ParticipantId.ToString())
                                             .ToList();
                    if (participantIds.Any())
                    {
                        await notifier.NotifyAuctionFinished(participantIds);
                    }
                }

                if (auctionsToStart.Any() || auctionsToEnd.Any())
                {
                    await unitOfWork.CommitAsync();
                    _logger.LogInformation("Successfully updated auction statuses.");
                }
            }
        }
    }
}
