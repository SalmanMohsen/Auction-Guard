import React, { createContext, useContext, useState, useEffect } from 'react';
import type { AuthContextType, User, LoginFormData, RegisterFormData } from '../types/auth';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  // Mock authentication functions - replace with actual API calls
  const login = async (credentials: LoginFormData): Promise<void> => {
    setIsLoading(true);
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 1000)); // Simulate API call
      
      // Mock user data based on email
      const mockUser: User = {
        id: '1',
        firstName: 'John',
        middleName: 'M',
        lastName: 'Doe',
        email: credentials.email,
        phoneNumber: '+1234567890',
        address: '123 Main St, City, State',
        role: credentials.email.includes('admin') ? 'admin' : 
              credentials.email.includes('seller') ? 'seller' : 'bidder',
        createdAt: new Date().toISOString(),
        isActive: true
      };
      
      setUser(mockUser);
      localStorage.setItem('auth-user', JSON.stringify(mockUser));
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (data: RegisterFormData): Promise<void> => {
    setIsLoading(true);
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 1000)); // Simulate API call
      
      const newUser: User = {
        id: Date.now().toString(),
        firstName: data.firstName,
        middleName: data.middleName,
        lastName: data.lastName,
        email: data.email,
        phoneNumber: data.phoneNumber,
        address: data.address,
        role: data.role,
        createdAt: new Date().toISOString(),
        isActive: true
      };
      
      setUser(newUser);
      localStorage.setItem('auth-user', JSON.stringify(newUser));
    } catch (error) {
      console.error('Registration failed:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('auth-user');
  };

  // Check for existing session on mount
  useEffect(() => {
    const savedUser = localStorage.getItem('auth-user');
    if (savedUser) {
      try {
        setUser(JSON.parse(savedUser));
      } catch (error) {
        console.error('Failed to parse saved user:', error);
        localStorage.removeItem('auth-user');
      }
    }
  }, []);

  const value: AuthContextType = {
    user,
    login,
    register,
    logout,
    isLoading
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};