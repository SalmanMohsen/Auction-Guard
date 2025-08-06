import type { UserProfile } from './user';

/**
 * Defines the shape of the credentials for a login request.
 * Based on the LoginDto from the backend.
 */
export interface LoginCredentials {
  email: string;
  rememberMe: boolean;
  password: string;
}

/**
 * Defines the shape of the data for a new user registration.
 * Based on the RegisterDto from the backend.
 */
export interface RegisterData {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  phoneNumber: string;
  role: 'Bidder' | 'Seller';
  identificationImageFile: File;
}

/**
 * Represents the successful response from an authentication request.
 * Based on the AuthenticationResultDto from the backend.
 */
export interface AuthResponse {
  token: string;
}
