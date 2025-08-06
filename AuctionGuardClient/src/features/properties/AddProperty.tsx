import { useState } from 'react';
import type { FormEvent } from 'react';
import { useDispatch } from 'react-redux';
import { addProperty as addPropertyApi } from '../../api/propertyApi';
import { addProperty as addPropertyAction } from '../../store/slices/propertySlice';
import type { CreatePropertyData } from '../../types';

const AddProperty = () => {
  const dispatch = useDispatch();
  // Update state to match the CreatePropertyData type from property.ts
  const [formData, setFormData] = useState<Omit<CreatePropertyData, 'Images'>>({
    title: '',
    description: '',
    address: '',
    priceInitial: 0,
    constructedOn: '',
    propertyType: 'Apartment', 
  });
  const [images, setImages] = useState<FileList | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;
    // Handle number inputs correctly
    const finalValue = type === 'number' ? parseFloat(value) : value;
    setFormData({ ...formData, [name]: finalValue });
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setImages(e.target.files);
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!images || images.length === 0) {
      setError('Please upload at least one image.');
      return;
    }
    setError(null);
    setSuccess(null);
    setLoading(true);

    const propertyData: CreatePropertyData = { ...formData, Images: images };

    try {
      const newProperty = await addPropertyApi(propertyData);
      dispatch(addPropertyAction(newProperty));
      setSuccess('Property added successfully!');
      // Reset form to initial state
      setFormData({ title: '', description: '', address: '', priceInitial: 0, constructedOn: '', propertyType: 'Apartment' });
      setImages(null);
      // Also reset the file input visually
      const fileInput = e.target as HTMLFormElement;
      fileInput.reset();
    } catch (err) {
      setError('Failed to add property. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 p-4 border rounded-lg bg-white shadow">
      <h2 className="text-2xl font-bold">Add a New Property</h2>
      {error && <div className="p-3 bg-red-100 text-red-700 rounded">{error}</div>}
      {success && <div className="p-3 bg-green-100 text-green-700 rounded">{success}</div>}
      
      {/* These inputs are correct */}
      <input name="title" placeholder="Property Title" value={formData.title} onChange={handleChange} required className="w-full p-2 border rounded" />
      <textarea name="description" placeholder="Description" value={formData.description} onChange={handleChange} required className="w-full p-2 border rounded" />
      <input name="address" placeholder="Address" value={formData.address} onChange={handleChange} required className="w-full p-2 border rounded" />
      
      {/* These are the new, required inputs */}
      <input name="priceInitial" type="number" placeholder="Initial Price" value={formData.priceInitial} onChange={handleChange} required min="1" className="w-full p-2 border rounded" />
      <input name="constructedOn" type="date" placeholder="Construction Date" value={formData.constructedOn} onChange={handleChange} required className="w-full p-2 border rounded" />
      
      <select name="propertyType" value={formData.propertyType} onChange={handleChange} required className="w-full p-2 border rounded bg-white">
        <option value="Apartment">Apartment</option>
        <option value="Villa">Villa</option>
        <option value="Land">Land</option>
        <option value="Commercial">Commercial</option>
        <option value="Other">Other</option>
      </select>
      
      <div>
        <label className="block text-sm font-medium text-gray-700">Property Images</label>
        <input type="file" multiple onChange={handleImageChange} required className="w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-blue-50 file:text-blue-700 hover:file:bg-blue-100"/>
      </div>

      <button type="submit" disabled={loading} className="w-full bg-blue-600 text-white p-3 rounded-lg font-semibold hover:bg-blue-700 disabled:bg-blue-400">
        {loading ? 'Adding Property...' : 'Add Property'}
      </button>
    </form>
  );
};

export default AddProperty;