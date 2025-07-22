import api from './api';
import type { LoginFormData, RegisterFormData ,ForgotPasswordFormData, ResetPasswordFormData} from '../types/auth';
export const loginUser = (credentials: LoginFormData) => api.post('/user/login', credentials);
export const registerUser = (data: FormData) => api.post('/user/register', data);
export const logoutUser = () => api.post('/user/logout');
export const forgotPassword = (data: ForgotPasswordFormData) => api.post('/user/forgot-password', data);
export const resetPassword = (data: ResetPasswordFormData) => api.post('/user/reset-password', data);