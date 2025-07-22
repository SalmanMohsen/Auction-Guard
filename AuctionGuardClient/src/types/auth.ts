/**
 * Defines the roles a user can have within the application.
 */
export type UserRole = 'Bidder' | 'Seller' | 'Admin';
// export type RegUserRole = 
/**
 * Interface for the data required to register a new user.
 */
export interface RegisterFormData {
  firstName: string;
  middleName?: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  password: string;
  confirmPassword: string;
  //idImage?: string;
  role: string[];
}

/**
 * Interface for the data required for a user to log in.
 */
export interface LoginFormData {
  login: string;
  password: string;
  rememberMe: boolean;
}

/**
 * Interface for the data required for the forgot password form.
 */
export interface ForgotPasswordFormData {
  email: string;
}

/**
 * Interface for the data required for the reset password form.
 */
export interface ResetPasswordFormData {
  email: string;
  token: string;
  newPassword: string;
}

/**
 * Interface for updating user data.
 */
export interface UpdateUserFormData {
    firstName: string;
    lastName: string;
    phoneNumber: string;
    address: string;
}

/**
 * Represents the structure of a user object in the application.
 */
export interface User {
  id: string;
  firstName: string;
  lastName: string;
  userName: string;
  login: string;
  roles: UserRole[];
  // Add any other user properties you expect from the backend
}

/**
 * Defines the structure of the successful authentication response from the backend API.
 * This matches the backend's `AuthenticationResultDto`.
 */
export interface AuthResponse {
  succeeded: boolean;
  token: string;
  expiration: string;
  user: User;
  errors?: string[];
}


/**
 * Defines the shape of the authentication context provided to the app.
 */
export interface AuthContextType {
  user: User | null;
  //token: string | null;
  isAuthenticated: boolean;
  login: (credentials: LoginFormData) => Promise<void>;
 // register: (data: RegisterFormData) => Promise<void>;
  register: (data: RegisterFormData) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}