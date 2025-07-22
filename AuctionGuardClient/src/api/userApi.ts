import api from './api';
import type { UpdateUserFormData } from '../types/auth';

export const updateUser = (userId: string, data: UpdateUserFormData) => api.put(`/user/${userId}`, data);
export const deleteUser = (userId: string) => api.delete(`/user/${userId}`);
export const getUser = (userId: string) => api.get(`/user/${userId}`);