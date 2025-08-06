import api from './api';
import type { CreatePropertyData, Property } from '../types/property'; // Make sure to import both types
import type { Auction, CreateAuctionData } from '../types';

/**
 * Fetches properties owned by the current user (seller).
 * @returns A promise that resolves with a list of properties.
 */
export const getMyProperties = async (): Promise<Property[]> => {
  try {
    const response = await api.get('/Properties/GetMyProperties');
    return response.data;
  } catch (error) {
    console.error('Failed to fetch properties:', error);
    throw error;
  }
};

/**
 * Adds a new property by sending form data to the backend.
 * @param propertyData - The data for the new property, conforming to the CreatePropertyData type.
 * @returns A promise that resolves with the created property's data.
 */
export const addProperty = async (propertyData: CreatePropertyData): Promise<Property> => {
  try {
    const formData = new FormData();

    // Append all text fields to match the C# CreatePropertyDto
    formData.append('Title', propertyData.title);
    formData.append('Description', propertyData.description);
    formData.append('Address', propertyData.address);
    formData.append('PriceInitial', propertyData.priceInitial.toString());
    formData.append('ConstructedOn', propertyData.constructedOn); // e.g., '2025-08-05'
    formData.append('PropertyType', propertyData.propertyType); // e.g., 'Villa'

    // Append each file from the FileList. The field name 'Images' must match the backend.
    if (propertyData.Images) {
      for (let i = 0; i < propertyData.Images.length; i++) {
        formData.append('Images', propertyData.Images[i]);
      }
    }

    const response = await api.post('/Properties/Create-Property', formData, {
      headers: {
        // Axios and modern browsers will automatically set the correct
        // 'multipart/form-data' header with the boundary when you send a FormData object.
        // Explicitly setting it can sometimes cause issues, so it's often best to omit it.
      },
    });
    return response.data;
  } catch (error) {
    console.error('Failed to add property:', error);
    throw error;
  }
};

/**
 * Starts an auction for a given property.
 * @param propertyId - The ID of the property to auction.
 * @param auctionDetails - The details for the auction (define a type for this).
 * @returns A promise that resolves with the new auction data.
 */
export const startAuction = async (auctionData: CreateAuctionData): Promise<Auction> => {
  try {
    // The backend expects a POST request to the base /api/Auctions endpoint
    const response = await api.post('/Auctions', auctionData);
    return response.data;
  } catch (error) {
    console.error('Failed to start auction:', error);
    throw error;
  }
};