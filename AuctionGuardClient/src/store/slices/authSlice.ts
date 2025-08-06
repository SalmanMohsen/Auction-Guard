import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import type { UserProfile } from '../../types';

interface AuthState {
  isAuthenticated: boolean;
  user: UserProfile | null;
  token: string | null;
}

const initialState: AuthState = {
  isAuthenticated: false,
  user: null,
  token: localStorage.getItem('authToken'),
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    /**
     * Sets the user's authentication credentials upon successful login.
     *
     * @param state - The current authentication state.
     * @param action - The action containing the user profile and token.
     */
    setCredentials(state, action: PayloadAction<{ user: UserProfile; token: string }>) {
      state.user = action.payload.user;
      state.token = action.payload.token;
      state.isAuthenticated = true;
      localStorage.setItem('authToken', action.payload.token);
    },
    /**
     * Clears the user's authentication credentials upon logout.
     *
     * @param state - The current authentication state.
     */
    logout(state) {
      state.user = null;
      state.token = null;
      state.isAuthenticated = false;
      localStorage.removeItem('authToken');
    },
  },
});

export const { setCredentials, logout } = authSlice.actions;
export default authSlice.reducer;
