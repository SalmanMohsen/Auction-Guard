using AuctionGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Infrastructure.Contexts
{
    public partial class AuctionGuardDbContext :DbContext
    {
        public AuctionGuardDbContext(DbContextOptions<AuctionGuardDbContext> options)
            :base(options)
        {
            
        }
        public AuctionGuardDbContext()
        {

        }

        #region DbSets

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Auction"/> entities.
        /// </summary>
        public DbSet<Auction> Auctions { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="AuctionParticipant"/> entities.
        /// </summary>
        public DbSet<AuctionParticipant> Participants { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Bid"/> entities.
        /// </summary>
        public DbSet<Bid> Bids { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="BidderAuthorization"/> entities.
        /// </summary>
        public DbSet<BidderAuthorization> bidderAuthorizations { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="BlacklistEntry"/> entities.
        /// </summary>
        public DbSet<BlacklistEntry> blacklistEntries { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="FavoriteProperty"/> entities.
        /// </summary>
        public DbSet<FavoriteProperty> favoriteProperties { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Image"/> entities.
        /// </summary>
        public DbSet<Image> images { get; set; }

        /// <summary> 
        /// Gets or sets the DbSet for the <see cref="Invoice"/> entities.
        /// </summary>
        public DbSet<Invoice> invoices { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Notification"/> entities.
        /// </summary>
        public DbSet<Notification> notifications { get; set; }

        ///<summary> 
        /// Gets or sets the DbSet for the <see cref="Offer"/> entities.
        /// </summary>
        public DbSet<Offer> offers { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet for the <see cref="PaymentMethod"/> entites.
        /// </summary>
        public DbSet<PaymentMethod> paymentMethods { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="PaymentTransaction"/> entites.
        /// </summary>
        public DbSet<PaymentTransaction> paymentTransactions { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Property"/> entities.
        /// </summary>
        public DbSet<Property> properties { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Review"/> entities.
        /// </summary>
        public DbSet<Review> reviews { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Tag"/> entities.
        /// </summary>
        public DbSet<Tag> tags { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BidderAuthorization>().HasKey(ba => ba.BidderAuthId);
            builder.Entity<PaymentMethod>().HasKey(pm => pm.MethodId);
            builder.Entity<PaymentTransaction>().HasKey(pt => pt.TransactionId);

            #region Composite Key Configurations

            builder.Entity<FavoriteProperty>()
                .HasKey(fp => new { fp.UserId, fp.PropertyId });

            builder.Entity<BlacklistEntry>()
                .HasKey(be => new { be.UserId, be.BlacklistStartDate });

            builder.Entity<AuctionParticipant>()
                .HasKey(ap => new { ap.AuctionId, ap.ParticipantId });
            #endregion

            #region Relationship Configurations

            // --- Auction Relationships ---
            builder.Entity<Auction>()
                .HasOne(a => a.Property)
                .WithMany(p => p.Auctions)
                .HasForeignKey(a => a.PropertyId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete a property if it has auctions.

            // --- Property Relationships ---
            builder.Entity<Property>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade); // If a property is deleted, its images are also deleted.

            builder.Entity<Property>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Properties)
                .UsingEntity(j => j.ToTable("PropertyTags")); // M-N join table for Properties and Tags

            builder.Entity<Property>()
                .HasMany(p => p.FavoritedByUsers)
                .WithOne(fp => fp.Property)
                .HasForeignKey(fp => fp.PropertyId)
                .OnDelete(DeleteBehavior.Cascade); // If a property is deleted, the favorite links are deleted.

            // --- Bid Relationships ---
            builder.Entity<Bid>()
                .HasOne(b => b.Auction)
                .WithMany(a => a.Bids)
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Cascade); // Bids are deleted if the parent auction is deleted.

            // --- Offer Relationships ---
            builder.Entity<Offer>()
                .HasOne(o => o.Auction)
                .WithMany(a => a.Offers)
                .HasForeignKey(o => o.AuctionId)
                .OnDelete(DeleteBehavior.Cascade); // Offers are deleted if the parent auction is deleted.

            // --- Invoice ---
            builder.Entity<Invoice>()
                .HasOne(i => i.Auction)
                .WithMany(a => a.Invoices)
                .HasForeignKey(i => i.AuctionId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete an auction if it has invoices.

            builder.Entity<Invoice>()
                .HasMany(i => i.Transactions)
                .WithOne(t => t.Invoice)
                .HasForeignKey(t => t.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade); // Transactions are deleted if the parent invoice is deleted.

            // --- Payment Relationships ---
            builder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.PaymentMethod)
                .WithMany(pm => pm.Transactions)
                .HasForeignKey(pt => pt.MethodId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete a payment method if it has transactions.

            // --- Authorization Relationships ---
            builder.Entity<BidderAuthorization>()
                .HasOne(ba => ba.Auction)
                .WithMany(a => a.BidderAuthorizations)
                .HasForeignKey(ba => ba.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Participant Relationships ---
            builder.Entity<AuctionParticipant>()
                .HasOne(ap => ap.Auction)
                .WithMany(a => a.Participants)
                .HasForeignKey(ap => ap.AuctionId)
                .OnDelete(DeleteBehavior.Cascade); // If an auction is deleted, participant records are deleted.

            // --- Review Relationships ---
            builder.Entity<Review>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Notification Relationships ---
            builder.Entity<Notification>()
                .HasOne(n => n.Auction)
                .WithMany(a => a.Notifications)
                .HasForeignKey(n => n.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Data Type & Precision Configurations
            // Explicitly set the precision for all decimal properties to avoid database warnings.
            builder.Entity<Auction>().Property(a => a.MinBidIncrement).HasColumnType("decimal(18, 2)");
            builder.Entity<Auction>().Property(a => a.GuaranteeDeposit).HasColumnType("decimal(18, 2)");
            builder.Entity<Property>().Property(p => p.PriceInitial).HasColumnType("decimal(18, 2)");
            builder.Entity<Property>().Property(p => p.EndedPrice).HasColumnType("decimal(18, 2)");
            builder.Entity<Bid>().Property(b => b.Amount).HasColumnType("decimal(18, 2)");
            builder.Entity<Offer>().Property(o => o.TriggerPrice).HasColumnType("decimal(18, 2)");
            builder.Entity<Invoice>().Property(i => i.AmountDue).HasColumnType("decimal(18, 2)");
            builder.Entity<PaymentTransaction>().Property(pt => pt.Amount).HasColumnType("decimal(18, 2)");
            builder.Entity<PaymentTransaction>().Property(pt => pt.CryptoAmount).HasColumnType("decimal(18, 2)");
            builder.Entity<BidderAuthorization>().Property(ba => ba.AmountHeld).HasColumnType("decimal(18, 2)");
            builder.Entity<Review>().Property(r => r.Rating).HasColumnType("decimal(2, 1)");
            #endregion
        }
    }
}
