import { Routes, Route, Navigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import type { RootState } from '../store/store';
import { hasRole } from '../utils/auth';

// Import Page Components
import LandingPage from '../pages/LandingPage';
import LoginPage from '../pages/LoginPage';
import RegisterPage from '../pages/RegisterPage';
import ProfilePage from '../pages/ProfilePage';
import BidderDashboardPage from '../pages/BidderDashboardPage';
import SellerDashboardPage from '../pages/SellerDashboardPage';
import AdminDashboardPage from '../pages/AdminDashbardPage';
import AuctionDetailsPage from '../pages/AuctionDetailsPage';
import type { JSX } from 'react';

const PrivateRoute = ({ children }: { children: JSX.Element }) => {
  const { isAuthenticated } = useSelector((state: RootState) => state.auth);
  return isAuthenticated ? children : <Navigate to="/login" />;
};

const RoleRoute = ({ children, role }: { children: JSX.Element, role: string }) => {
    if (!hasRole(role)) {
        return <Navigate to="/" />;
    }
    return children;
}

const AppRouter = () => {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path="/" element={<LandingPage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/auctions/:id" element={<AuctionDetailsPage />} />

      {/* Private Routes */}
      <Route path="/profile" element={<PrivateRoute><ProfilePage /></PrivateRoute>} />

      {/* Role-Based Routes */}
      <Route
        path="/dashboard/bidder"
        element={<PrivateRoute><RoleRoute role="Bidder"><BidderDashboardPage /></RoleRoute></PrivateRoute>}
      />
      <Route
        path="/dashboard/seller"
        element={<PrivateRoute><RoleRoute role="Seller"><SellerDashboardPage /></RoleRoute></PrivateRoute>}
      />
      <Route
        path="/dashboard/admin"
        element={<PrivateRoute><RoleRoute role="Admin"><AdminDashboardPage /></RoleRoute></PrivateRoute>}
      />
    </Routes>
  );
};

export default AppRouter;