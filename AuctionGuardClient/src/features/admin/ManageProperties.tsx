import { useEffect, useState } from 'react';
import { getAllProperties, approveProperty } from '../../api/adminApi';
import type { Property } from '../../types';

const ManageProperties = () => {
  const [properties, setProperties] = useState<Property[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchProperties = async () => {
    try {
      setLoading(true);
      const propertiesData = await getAllProperties();
      // This debug line is helpful!
      console.log('Properties received from API:', propertiesData);
      setProperties(propertiesData);
    } catch (err) {
      setError('Failed to load properties.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProperties();
  }, []);

  const handleApprove = async (propertyId: string) => {
    try {
      await approveProperty(propertyId);
      setProperties(prevProperties =>
        prevProperties.map(p =>
          p.id === propertyId ? { ...p, approvalStatus: 'Approved' } : p
        )
      );
    } catch (error) {
      console.error(`Failed to approve property ${propertyId}`, error);
      setError(`Failed to approve property ${propertyId}`);
    }
  };

  if (loading) return <div>Loading properties...</div>;
  if (error) return <div className="text-red-500">{error}</div>;

  return (
    <div className="overflow-x-auto p-4">
      <h2 className="text-2xl font-bold mb-4">Manage Properties</h2>
      <table className="min-w-full bg-white shadow-md rounded-lg">
        <thead>
          <tr className="bg-gray-200 text-gray-600 uppercase text-sm leading-normal">
            <th className="py-3 px-6 text-left">Title</th>
            <th className="py-3 px-6 text-left">Status</th>
            <th className="py-3 px-6 text-center">Owner ID</th>
            <th className="py-3 px-6 text-center">Actions</th>
          </tr>
        </thead>
        <tbody className="text-gray-600 text-sm font-light">
          {properties.map(property => (
            <tr key={property.id} className="border-b border-gray-200 hover:bg-gray-100">
              <td className="py-3 px-6 text-left whitespace-nowrap">{property.title}</td>
              <td className="py-3 px-6 text-left">
                <span className={`px-2 py-1 text-xs font-semibold rounded-full ${
                  property.approvalStatus === "Approved" ? 'bg-green-100 text-green-800' :
                  property.approvalStatus === "UnderApproval" ? 'bg-yellow-100 text-yellow-800' :
                  'bg-red-100 text-red-800'
                }`}>
                  {property.approvalStatus}
                </span>
              </td>
              <td className="py-3 px-6 text-center font-mono text-xs">{property.ownerId}</td>
              <td className="py-3 px-6 text-center">
                {/* This condition MUST exactly match the data from your console.log */}
                {property.approvalStatus === 'UnderApproval' && (
                  <button
                    onClick={() => handleApprove(property.id)}
                    className="bg-green-500 hover:bg-green-600 text-white font-bold py-1 px-3 rounded text-xs"
                  >
                    Approve
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default ManageProperties;