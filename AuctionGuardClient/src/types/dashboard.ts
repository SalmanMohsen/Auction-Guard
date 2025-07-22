// Shared types for properties and auctions
export interface PropertySummary {
  id: string;
  title: string;
  location: string;
  currentBid: number;
  imageUrl: string;
  bedrooms: number;
  bathrooms: number;
  sqft: number;
}

// For the Bidder Dashboard's featured auctions
export interface FeaturedAuction extends PropertySummary {
  timeLeft: string;
  isFavorite: boolean;
}

// For the Bidder Dashboard's active bids list
export interface ActiveBid {
  id: string;
  propertyTitle: string;
  myBid: number;
  currentBid: number;
  status: 'winning' | 'outbid' | 'ended';
  timeLeft: string;
}

// For the Seller Dashboard's property list
export interface SellerProperty {
  id: string;
  title: string;
  status: 'active' | 'pending' | 'sold' | 'rejected';
  bids: number;
  currentBid: number;
}

// For the Seller Dashboard's analytics
export interface SalesAnalytics {
  totalRevenue: number;
  propertiesSold: number;
  averageSalePrice: number;
}

// For the Admin Dashboard
export interface PendingApproval {
  id: string;
  type: 'Property' | 'User';
  title: string;
  submitter: string;
}

export interface UserStats {
  totalUsers: number;
  activeBidders: number;
  verifiedSellers: number;
}

// File: AuctionGuardClient/src/api/api.ts
// This new file centralizes all your API calls.

