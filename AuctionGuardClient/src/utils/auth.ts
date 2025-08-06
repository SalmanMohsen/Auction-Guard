import { jwtDecode } from 'jwt-decode';

/**
 * Interface for the decoded JWT payload.
 * Extend this with any other data you store in the token.
 */
interface DecodedToken {
  sub: string; // Subject (user ID)
  role: string[];
  exp: number; // Expiration time
}

/**
 * Retrieves the authentication token from local storage.
 * @returns The token string, or null if it doesn't exist.
 */
export const getAuthToken = (): string | null => {
  return localStorage.getItem('authToken');
};



/**
 * Checks if the current user has a specific role based on the JWT token.
 * @param role The role to check for (e.g., 'Admin').
 * @returns `true` if the user has the role, otherwise `false`.
 */
export const hasRole = (requiredRole: string): boolean => {
  const token = localStorage.getItem('authToken');

  // --- FIX ---
  // If no token exists, the user is not authenticated and has no roles.
  // Return false immediately to prevent decoding a null value.
  if (!token || token === 'null' || token.split('.').length !== 3) {
    return false;
  }

  try {
    const decodedToken = jwtDecode<DecodedToken>(token);
    const userRoles = decodedToken.role;
    return userRoles.includes(requiredRole);
  } catch (error) {
    // This will catch any errors from a malformed token.
    console.error('Failed to decode JWT token:', error);
    return false;
  }
};

/**
 * Checks if the user is authenticated by verifying the token's existence and expiration.
 * @returns True if the user is authenticated, false otherwise.
 */
export const isAuthenticated = (): boolean => {
    const token = getAuthToken();
    if (!token) {
        return false;
    }

    try {
        const decoded: DecodedToken = jwtDecode(token);
        // Check if the token's expiration time is in the future
        return decoded.exp * 1000 > Date.now();
    } catch (error) {
        console.error("Invalid token:", error);
        return false;
    }
};
