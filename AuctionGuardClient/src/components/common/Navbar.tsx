import { Link, useNavigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import type { RootState } from '../../store/store';
import { logout as logoutAction } from '../../store/slices/authSlice';
import { logout as logoutApi } from '../../api/authApi';
import { hasRole } from '../../utils/auth';

const Navbar = () => {
  const { isAuthenticated, user } = useSelector((state: RootState) => state.auth);
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
        await logoutApi();
    } catch (error) {
        console.error("Server logout failed", error);
    } finally {
        dispatch(logoutAction());
        navigate('/login');
    }
  };

  const getDashboardLink = () => {
    if (hasRole('Admin')) return '/dashboard/admin';
    if (hasRole('Seller')) return '/dashboard/seller';
    return '/dashboard/bidder';
  }

  return (
    <nav>
      <ul className="flex items-center space-x-4">
        {isAuthenticated ? (
          <>
            <li><Link to={getDashboardLink()} className="hover:text-blue-500">Dashboard</Link></li>
            <li><Link to="/profile" className="hover:text-blue-500">Profile</Link></li>
            <li>
              <button onClick={handleLogout} className="bg-red-500 text-white py-1 px-3 rounded text-sm hover:bg-red-600">
                Logout
              </button>
            </li>
          </>
        ) : (
          <>
            <li><Link to="/login" className="hover:text-blue-500">Login</Link></li>
            <li><Link to="/register" className="bg-blue-500 text-white py-1 px-3 rounded text-sm hover:bg-blue-600">Register</Link></li>
          </>
        )}
      </ul>
    </nav>
  );
};

export default Navbar;