import React, { createContext, useState, useContext, useEffect } from "react";
import type { ReactNode } from "react";
import api from "../api/api";
import axios from "axios";
import { registerUser } from "../api/authApi";
import type { AuthContextType, LoginFormData, User, AuthResponse, RegisterFormData } from "../types/auth";

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // On initial app load, check localStorage for a persisted user session.
  useEffect(() => {
    try {
      const storedUser = localStorage.getItem("user");
      if (storedUser) {
        // If a user object exists in storage, we set it as the current user.
        setUser(JSON.parse(storedUser));
      }
    } catch (error) {
      console.error("Failed to parse user data from localStorage. Clearing it.", error);
      localStorage.removeItem("user");
    } finally {
      setIsLoading(false);
    }
  }, []);

  const login = async (loginData: LoginFormData) => {
    const response = await api.post<AuthResponse>("/User/login", loginData);
    const { user: loggedInUser } = response.data;
    if (loggedInUser) {
      // Store the user object for session persistence.
      localStorage.setItem("user", JSON.stringify(loggedInUser));
      setUser(loggedInUser);
    } else {
      throw new Error(response.data.errors?.join(", ") || "Login failed.");
    }
  };

  const register = async (registerData: RegisterFormData) => {
    await api.post('/user/register', registerData);
  };

  const logout = async () => {
    try {
      await api.post('/user/logout');
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        console.log("Backend session invalidated.");
      } else {
        console.error("An unexpected error occurred during server logout:", error);
      }
    } finally {
      // Clear the user from the app state and from localStorage.
      setUser(null);
      localStorage.removeItem("user");
    }
  };

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    register,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};