import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import type { RootState } from '../../store/store';
import { setCredentials, logout as logoutAction } from '../../store/slices/authSlice';
import { login as loginApi, logout as logoutApi } from '../../api/authApi';
import type { LoginCredentials } from '../../types';

/**
 * A custom hook to manage authentication logic.
 *
 * This hook provides a simplified interface for components to interact with
 * the authentication state and perform login/logout actions.
 *
 * @returns An object containing the authentication state and action handlers.
 */
export const useAuth = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { isAuthenticated, user } = useSelector((state: RootState) => state.auth);

  const login = async (credentials: LoginCredentials) => {
    try {
      const data = await loginApi(credentials);
      dispatch(setCredentials(data));
      // Redirect logic can be handled in the component or here
      navigate('/profile');
    } catch (error) {
      console.error('Login failed in useAuth:', error);
      throw error; // Re-throw to be caught by the component
    }
  };

  const logout = async () => {
    try {
        await logoutApi();
    } catch (error) {
        console.error("Server logout failed, proceeding with client-side logout.", error)
    } finally {
        dispatch(logoutAction());
        navigate('/login');
    }
  };

  return {
    isAuthenticated,
    user,
    login,
    logout,
  };
};