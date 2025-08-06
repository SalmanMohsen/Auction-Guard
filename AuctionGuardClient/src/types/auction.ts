

export interface Offer{
  description: string;
  triggerPrice: number;
}
/**
 * Represents a bid placed on an auction.
 * Based on the BidDto from the backend.
 */
export interface Bid {
  bidId: string;
  auctionId: string;
  bidderId: string;
  amount: number;
  timestamp: string; 
  bidderName: string;
}

/**
 * Represents an auction.
 * Based on the AuctionDto from the backend.
 */
export interface Auction {
  auctionId: string;
  propertyId: string;

  startTime: string; 
  endTime: string; 
  
  minBidIncrement: number;
  guaranteeDeposit: number;
  currentHighestBid?: number;
  propertyDescription: string;
  status: 'Scheduled' | 'Active' | 'Ended' | 'Cancelled';
  propertyTitle: string;
  creatorId: string;
  winnerId?: string;
  cancellationReason?: string;
  
  offers: Offer[]; 
  propertyImageUrls: string[];
}

/**
 * Defines the shape of the data for creating a new auction.
 * Based on the CreateAuctionDto from the backend.
 */
export interface CreateAuctionData {
    propertyId: string;
    startTime: string; 
    endTime: string; 
    minBidIncrement: number;
    guaranteeDeposit: number;
    offers: Offer[];
}
