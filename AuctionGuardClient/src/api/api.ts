import axios from 'axios';
import { API_BASE_URL } from './config';

// Create a new Axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true, // This is crucial for sending cookies
});

export default api;