export type UserRole = 'bidder' | 'seller' | 'admin';

export interface RegisterFormData {
  firstName: string;
  middleName?: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  address: string;
  password: string;
  confirmPassword: string;
  idImage?: File;
  role: 'bidder' | 'seller';
}

export interface LoginFormData {
  email: string;
  password: string;
}

export interface User {
  id: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  address: string;
  role: UserRole;
  idImageUrl?: string;
  createdAt: string;
  isActive: boolean;
}

export interface AuthContextType {
  user: User | null;
  login: (credentials: LoginFormData) => Promise<void>;
  register: (data: RegisterFormData) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}