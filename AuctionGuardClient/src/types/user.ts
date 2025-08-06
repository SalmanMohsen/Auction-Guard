/**
 * Represents the public profile of a user.
 * Based on the UserDto from the backend.
 */
export interface UserProfile {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string | null;
  roles: string[];
}

/**
 * Represents the data needed to update a user's profile.
 * Based on the UpdateUserDto from the backend.
 */
export interface UpdateUserProfile {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}
