import axios from 'axios';

// The base URL for your backend API
export const API_BASE_URL = 'http://localhost:7044/api'; 

// Create an axios instance with a base URL
const api = axios.create({
  baseURL: API_BASE_URL,
});

// Add a request interceptor to include the token in headers
api.interceptors.request.use(
  (config) => {
    // Get the token from local storage or your state management
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    
    return Promise.reject(error);
  }
);


api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      console.log(error.response);
      console.error('Unauthorized access - redirecting to login.');
      localStorage.removeItem('authToken');
    }
    return Promise.reject(error);
  }
);

export default api;
