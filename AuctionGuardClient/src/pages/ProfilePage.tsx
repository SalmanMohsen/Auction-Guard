import { useState, useEffect } from 'react';
import type { FormEvent } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import type { RootState } from '../store/store';
import { getMyProfile, updateMyProfile } from '../api/userApi';
import { setCredentials } from '../store/slices/authSlice';
import type { UserProfile, UpdateUserProfile } from '../types';

const ProfilePage = () => {
  const dispatch = useDispatch();
  const { user, token } = useSelector((state: RootState) => state.auth);
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState<UpdateUserProfile>({
    firstName: '',
    lastName: '',
    phoneNumber: '',
  });

  useEffect(() => {
    const fetchProfile = async () => {
      if (user) {
        try {
          const profileData = await getMyProfile();
          setProfile(profileData);
          setFormData({
            firstName: profileData.firstName,
            lastName: profileData.lastName,
            phoneNumber: profileData.phoneNumber || '',
          });
        } catch (error) {
          console.error("Failed to fetch profile", error);
        }
      }
    };
    fetchProfile();
  }, [user]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleUpdate = async (e: FormEvent) => {
    e.preventDefault();
    if (!token) return;
    try {
        const updatedUser = await updateMyProfile(formData);
        dispatch(setCredentials({ user: updatedUser, token }));
        setProfile(updatedUser);
        setIsEditing(false);
    } catch (error) {
        console.error("Failed to update profile", error);
    }
  }

  if (!profile) return <div>Loading profile...</div>;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="max-w-2xl mx-auto bg-white p-8 rounded-lg shadow-md">
        <h1 className="text-3xl font-bold mb-6 text-gray-800">My Profile</h1>
        
        {!isEditing ? (
          <div className="space-y-4">
            <div>
              <label className="text-sm font-semibold text-gray-500">Username</label>
              <p className="text-lg text-gray-900">{profile.userName}</p>
            </div>
            <div>
              <label className="text-sm font-semibold text-gray-500">Email</label>
              <p className="text-lg text-gray-900">{profile.email}</p>
            </div>
            <div>
              <label className="text-sm font-semibold text-gray-500">First Name</label>
              <p className="text-lg text-gray-900">{profile.firstName}</p>
            </div>
            <div>
              <label className="text-sm font-semibold text-gray-500">Last Name</label>
              <p className="text-lg text-gray-900">{profile.lastName}</p>
            </div>
            <div>
              <label className="text-sm font-semibold text-gray-500">Phone Number</label>
              <p className="text-lg text-gray-900">{profile.phoneNumber || 'Not provided'}</p>
            </div>
            <div className="pt-4">
              <button 
                onClick={() => setIsEditing(true)}
                className="w-full bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded-md transition duration-300"
              >
                Edit Profile
              </button>
            </div>
          </div>
        ) : (
          <form onSubmit={handleUpdate} className="space-y-4">
            <div>
              <label htmlFor="firstName" className="block text-sm font-medium text-gray-700">First Name</label>
              <input
                type="text"
                id="firstName"
                name="firstName"
                value={formData.firstName}
                onChange={handleInputChange}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div>
              <label htmlFor="lastName" className="block text-sm font-medium text-gray-700">Last Name</label>
              <input
                type="text"
                id="lastName"
                name="lastName"
                value={formData.lastName}
                onChange={handleInputChange}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div>
              <label htmlFor="phoneNumber" className="block text-sm font-medium text-gray-700">Phone Number</label>
              <input
                type="tel"
                id="phoneNumber"
                name="phoneNumber"
                value={formData.phoneNumber || ''}
                onChange={handleInputChange}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div className="flex space-x-4 pt-4">
              <button 
                type="submit"
                className="w-full bg-green-500 hover:bg-green-600 text-white font-bold py-2 px-4 rounded-md transition duration-300"
              >
                Save Changes
              </button>
              <button 
                type="button"
                onClick={() => setIsEditing(false)}
                className="w-full bg-gray-500 hover:bg-gray-600 text-white font-bold py-2 px-4 rounded-md transition duration-300"
              >
                Cancel
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
};

export default ProfilePage;