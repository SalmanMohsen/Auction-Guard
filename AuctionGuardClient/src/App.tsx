import { Toaster } from "./components/ui/toaster";
import { Toaster as Sonner } from "./components/ui/sonner";
import { TooltipProvider } from "./components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route, Navigate, Outlet } from "react-router-dom";
import { AuthProvider, useAuth } from "./contexts/AuthContext";

// Import all your page components
import LandingPage from "./pages/LandingPage";
import RegisterPage from "./pages/RegisterPage";
import LoginPage from "./pages/LoginPage";
import BidderDashboard from "./pages/dashboards/BidderDashboard";
import SellerDashboard from "./pages/dashboards/SellerDashboard";
import AdminDashboard from "./pages/dashboards/AdminDashboard";
import NotFound from "./pages/NotFound";
import ForgotPasswordPage from "./pages/ForgotPasswordPage";
import ResetPasswordPage from "./pages/ResetPasswordPage";
import GetAllUsersPage from "./pages/dashboards/admin/GetAllUsersPage";
import UpdateUserPage from "./pages/dashboards/admin/UpdateUserPage";

// --- Layout Components for Routing ---
const ProtectedRoute = () => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
};

const GuestRoute = () => {
  const { isAuthenticated } = useAuth();
  return !isAuthenticated ? <Outlet /> : <Navigate to="/dashboard" replace />;
};

const AdminRoute = () => {
    const { user } = useAuth();
    return user?.roles.includes('Admin') ? <Outlet /> : <Navigate to="/dashboard" replace />;
}

const DashboardRouter = () => {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;
  const userRole = user.roles[0];
  switch (userRole) {
    case 'Bidder': return <BidderDashboard />;
    case 'Seller': return <SellerDashboard />;
    case 'Admin': return <AdminDashboard />;
    default: return <Navigate to="/" replace />;
  }
};

// --- Route Definitions ---
const AppRoutes = () => (
  <Routes>
    <Route path="/" element={<LandingPage />} />
    <Route element={<GuestRoute />}>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPasswordPage />} />
    </Route>
    <Route element={<ProtectedRoute />}>
      <Route path="/dashboard" element={<DashboardRouter />} />
      <Route element={<AdminRoute />}>
        <Route path="/admin/users" element={<GetAllUsersPage />} />
        <Route path="/admin/user/update/:userId" element={<UpdateUserPage />} />
      </Route>
    </Route>
    <Route path="*" element={<NotFound />} />
  </Routes>
);

const queryClient = new QueryClient();

// --- Main App Component ---

// This component wrapper prevents routes from rendering until the auth state is known.
const AppContent = () => {
    const { isLoading } = useAuth();
    if (isLoading) {
        return <div className="min-h-screen w-full flex items-center justify-center"><p>Loading...</p></div>;
    }
    return <AppRoutes />;
}

const App = () => (
  <QueryClientProvider client={queryClient}>
    <AuthProvider>
      <TooltipProvider>
        <Toaster />
        <Sonner />
        <BrowserRouter>
          <AppContent />
        </BrowserRouter>
      </TooltipProvider>
    </AuthProvider>
  </QueryClientProvider>
);

export default App;