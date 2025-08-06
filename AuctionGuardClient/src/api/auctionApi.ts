import api from './api';

/**
 * Defines the shape of the filters that can be applied when fetching auctions.
 */
interface AuctionFilters {
  status?: 'active' | 'scheduled';
  ownerId?: string;
}

/**
 * Fetches all auctions based on the provided filters.
 * @param filters - Optional filters for status or ownerId.
 * @returns A promise that resolves with a list of auctions.
 */
export const getAuctions = async (filters: AuctionFilters = {}) => {
  try {
    const response = await api.get(`Auctions/${filters.status}`);
    return response.data;
  } catch (error) {
    console.error('Failed to fetch auctions:', error);
    throw error;
  }
};

/**
 * Fetches a single auction by its ID.
 * @param auctionId - The ID of the auction to fetch.
 * @returns A promise that resolves with the auction details.
 */
export const getAuctionById = async (auctionId: string) => {
  try {
    const response = await api.get(`Auctions/${auctionId}`);
    return response.data;
  } catch (error) {
    console.error(`Failed to fetch auction ${auctionId}:`, error);
    throw error;
  }
};

/**
 * Joins an auction, returning a PayPal approval URL if a deposit is required.
 * @param auctionId - The ID of the auction to join.
 * @returns A promise that resolves with the response containing the approval URL.
 */
export const joinAuction = async (auctionId: string) => {
  try {
    const response = await api.post(`Auctions/${auctionId}/join`);
    return response.data; 
  } catch (error) {
    console.error(`Failed to join auction ${auctionId}:`, error);
    throw error;
  }
};

/**
 * Checks if the current user is a participant in a specific auction.
 * @param auctionId - The ID of the auction to check.
 * @returns A promise that resolves with the participation status.
 */
export const getParticipationStatus = async (auctionId: string) => {
    try {
        const response = await api.get(`Auctions/${auctionId}/participation-status`);
        return response.data; // Expected to be { isParticipant: boolean, statusMessage: string }
    } catch (error) {
        console.error(`Failed to get participation status for auction ${auctionId}:`, error);
        // Assume not a participant if the check fails
        return { isParticipant: false, statusMessage: 'Could not verify participation.' };
    }
}

/**
 * Places a bid on an auction.
 * @param auctionId - The ID of the auction.
 * @param amount - The bid amount.
 * @returns A promise that resolves with the new bid information.
 */
export const placeBid = async (auctionId: string, amount: number) => {
  try {
    const response = await api.post(`Auctions/${auctionId}/bids`, { amount });
    return response.data;
  } catch (error) {
    console.error(`Failed to place bid on auction ${auctionId}:`, error);
    throw error;
  }
};

/**
 * Fetches auctions won by the current user.
 * @returns A promise that resolves with a list of won auctions.
 */
export const getWonAuctions = async () => {
    try {
        const response = await api.get('Auctions/The-Auctions-I-Won');
        return response.data;
    } catch (error) {
        console.error('Failed to fetch won auctions:', error);
        throw error;
    }
}

/**
 * Adds a new offer to an existing auction.
 * @param auctionId - The ID of the auction to add the offer to.
 * @param offerData - The offer details (description, triggerPrice).
 * @returns A promise that resolves with the created offer data.
 */
export const addOfferToAuction = async (auctionId: string, offerData: { description: string; triggerPrice: number; }) => {
    try {
        const response = await api.post(`/offers/auctions/${auctionId}`, offerData);
        return response.data;
    } catch (error) {
        console.error(`Failed to add offer to auction ${auctionId}:`, error);
        throw error;
    }
};