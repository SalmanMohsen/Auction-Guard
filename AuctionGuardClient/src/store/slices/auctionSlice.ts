import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import type { Auction, Bid } from '../../types';

interface AuctionState {
  auctions: Auction[];
  currentAuction: Auction | null;
  bids: Bid[];
  loading: boolean;
  error: string | null;
}

const initialState: AuctionState = {
  auctions: [],
  currentAuction: null,
  bids: [],
  loading: false,
  error: null,
};

const auctionSlice = createSlice({
  name: 'auction',
  initialState,
  reducers: {
    setAuctions(state, action: PayloadAction<Auction[]>) {
      state.auctions = action.payload;
    },
    setCurrentAuction(state, action: PayloadAction<Auction | null>) {
      state.currentAuction = action.payload;
    },
    addBid(state, action: PayloadAction<Bid>) {
      state.bids.unshift(action.payload); // Add new bids to the beginning
      if (state.currentAuction) {
        state.currentAuction.highestBid = action.payload.amount;
        state.currentAuction.highestBidder = action.payload.bidderName;
      }
    },
    setBids(state, action: PayloadAction<Bid[]>) {
        state.bids = action.payload;
    },
    setLoading(state, action: PayloadAction<boolean>) {
      state.loading = action.payload;
    },
    setError(state, action: PayloadAction<string | null>) {
      state.error = action.payload;
    },
  },
});

export const { setAuctions, setCurrentAuction, addBid, setBids, setLoading, setError } = auctionSlice.actions;
export default auctionSlice.reducer;
