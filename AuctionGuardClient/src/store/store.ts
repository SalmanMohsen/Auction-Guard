import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import userReducer from './slices/userSlice';
import propertyReducer from './slices/propertySlice';
import auctionReducer from './slices/auctionSlice';

/**
 * The main Redux store for the application.
 *
 * This store is configured with reducers for each feature of the application,
 * such as authentication, user management, properties, and auctions.
 *
 * The `configureStore` function from Redux Toolkit automatically sets up
 * the Redux DevTools Extension for easier debugging.
 */
export const store = configureStore({
  reducer: {
    auth: authReducer,
    user: userReducer,
    property: propertyReducer,
    auction: auctionReducer,
  },
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
