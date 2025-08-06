import { useState } from 'react';
import type {FormEvent} from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { register as registerApi } from '../../api/authApi';
import { setCredentials } from '../../store/slices/authSlice';
import type{ RegisterData } from '../../types';

const Register = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [formData, setFormData] = useState<Omit<RegisterData, 'identificationImageFile' | 'confirmPassword'>>({
    email: '',
    password: '',
    firstName: '',
    middleName: '',
    lastName: '',
    phoneNumber: '',
    role: 'Bidder',
  });
  const [confirmPassword, setConfirmPassword] = useState('');
  const [identificationImageFile, setIdentificationImageFile] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setIdentificationImageFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (formData.password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }
    if (!identificationImageFile) {
        setError("Please upload an identification image.");
        return;
    }
    setError(null);
    setLoading(true);

    try {
      const registrationData: RegisterData = {
        ...formData,
        confirmPassword,
        identificationImageFile,
      };
      const { user, token } = await registerApi(registrationData);
      dispatch(setCredentials({ user, token }));
      navigate('/dashboard/bidder'); // Redirect to bidder dashboard by default
    } catch (err) {
      setError('Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <div className="p-3 bg-red-100 text-red-700 rounded">{error}</div>}
       <input name="email" type="email" placeholder="Email" onChange={handleChange} required className="w-full p-2 border rounded" />
       <input name="firstName" placeholder="First Name" onChange={handleChange} required className="w-full p-2 border rounded" />
       <input name="middleName" placeholder="Middle Name (Optional)" onChange={handleChange} className="w-full p-2 border rounded" />
       <input name="lastName" placeholder="Last Name" onChange={handleChange} required className="w-full p-2 border rounded" />
       <input name="phoneNumber" placeholder="Phone Number" onChange={handleChange} required className="w-full p-2 border rounded" />
       <input name="password" type="password" placeholder="Password" value={formData.password} onChange={handleChange} required className="w-full p-2 border rounded" />
       <input name="confirmPassword" type="password" placeholder="Confirm Password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} required className="w-full p-2 border rounded" />
       
       <div>
        <label htmlFor="role" className="block text-sm font-medium text-gray-700">I want to register as a</label>
        <select
          name="role"
          id="role"
          value={formData.role}
          onChange={handleChange}
          className="w-full p-2 border rounded"
        >
          <option value="Bidder">Bidder</option>
          <option value="Seller">Seller</option>
        </select>
      </div>

       <div>
        <label htmlFor="identificationImageFile" className="block text-sm font-medium text-gray-700">Identification Document</label>
        <input
          id="identificationImageFile"
          name="identificationImageFile"
          type="file"
          onChange={handleImageChange}
          required
          className="w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-blue-50 file:text-blue-700 hover:file:bg-blue-100"
        />
      </div>

      <button type="submit" disabled={loading} className="w-full bg-blue-500 text-white p-2 rounded disabled:bg-blue-300">
        {loading ? 'Registering...' : 'Register'}
      </button>
    </form>
  );
};

export default Register;