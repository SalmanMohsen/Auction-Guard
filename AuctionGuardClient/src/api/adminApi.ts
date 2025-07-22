import api from './api';

export const getAllUsers = () => api.get('/user');