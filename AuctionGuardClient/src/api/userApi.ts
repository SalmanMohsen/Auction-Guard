import api from './api';
import type { UserProfile } from '../types/user'; 

/**
 * Fetches the profile of the currently logged-in user.
 * @returns A promise that resolves with the user's profile data.
 */
export const getMyProfile = async () => {
  try {
    const response = await api.get('/user/profile');
    return response.data;
  } catch (error) {
    console.error('Failed to fetch user profile:', error);
    throw error;
  }
};

/**
 * Updates the profile of the currently logged-in user.
 * @param profileData - The updated profile data.
 * @returns A promise that resolves with the updated user profile.
 */
export const updateMyProfile = async (profileData: Partial<UserProfile>) => {
  try {
    const response = await api.put('/user/profile', profileData);
    return response.data;
  } catch (error) {
    console.error('Failed to update user profile:', error);
    throw error;
  }
};