import api from './api';
import type { LoginCredentials, RegisterData } from '../types/auth'; 
/**
 * Logs in a user.
 * @param credentials - The user's email and password.
 * @returns A promise that resolves with the user data and token.
 */
export const login = async (credentials: LoginCredentials) => {
  try {
    const response = await api.post('/User/login', credentials);
    // Save the token, e.g., in local storage
    if (response.data.token) {
      localStorage.setItem('authToken', response.data.token);
    }
    return response.data;
  } catch (error) {
    console.error('Login failed:', error);
    throw error;
  }
};

/**
 * Registers a new user.
 * @param userData - The data for the new user.
 * @returns A promise that resolves with the newly created user's data.
 */
export const register = async (userData: RegisterData) => {
  const formData = new FormData();
  
  // Append all fields from userData to formData
  (Object.keys(userData) as Array<keyof RegisterData>).forEach((key) => {
    if (key === 'identificationImageFile') {
      formData.append('IdentificationImageFile', userData[key]);
    } else if (key !== 'confirmPassword' && userData[key] !== undefined) {
      formData.append(key, String(userData[key]));
    }
  });

  try {
    const response = await api.post('/User/register', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
     if (response.data.token) {
      localStorage.setItem('authToken', response.data.token);
    }
    return response.data;
  } catch (error) {
    console.error('Registration failed:', error);
    throw error;
  }
};

/**
 * Logs out the current user.
 * This function sends a request to the server to invalidate the session/token
 * and then removes the token from local storage.
 */
export const logout = async () => {
  try {
    // Make an API call to the server's logout endpoint.
    // This allows the server to invalidate the token (e.g., by adding it to a blacklist).
    await api.post('/User/logout');
  } catch (error) {
    
  } finally {
    // Always remove the token from local storage to complete the logout process on the client.
    localStorage.removeItem('authToken');
  }
};