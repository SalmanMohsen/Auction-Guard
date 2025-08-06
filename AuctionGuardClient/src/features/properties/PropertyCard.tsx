import type { Property } from '../../types';
import { API_BASE_URL } from '../../api/api';

interface PropertyCardProps {
  property: Property;
  onStartAuction: (propertyId: string) => void;
}

const PropertyCard = ({ property, onStartAuction }: PropertyCardProps) => {
  const canStartAuction = property.approvalStatus === 'Approved' && property.propertyStatus === 'Available';

  // Use the first URL from the imageUrls array.
  // The API_BASE_URL replacement is to construct an absolute URL if the backend returns a relative path.
  const imageUrl = property.imageUrls && property.imageUrls.length > 0
    ? `${API_BASE_URL.replace('/api', '')}${property.imageUrls[0]}`
    : 'https://placehold.co/600x400/eee/ccc?text=No+Image';

  return (
    <div className="border rounded-lg p-4 shadow-md bg-white">
      <img
        src={imageUrl}
        alt={property.title}
        className="w-full h-40 object-cover rounded-md mb-3 bg-gray-200"
        // Add an error handler for broken image links
        onError={(e) => {
            const target = e.target as HTMLImageElement;
            target.onerror = null; // Prevent infinite loop
            target.src = 'https://placehold.co/600x400/eee/ccc?text=Image+Error';
        }}
      />

      <h3 className="text-lg font-bold">{property.title}</h3>
      <p className="text-sm text-gray-500">{property.address}</p>
      <p className="mt-2 text-sm"><strong>Type:</strong> {property.propertyType}</p>

      {/* --- Status Display Logic --- */}
      <div className="mt-1">
        {property.approvalStatus === 'Approved' ? (
          <p className="text-sm">
            <strong>Status:</strong>
            <span className={`ml-2 px-2 py-1 text-xs font-semibold rounded-full ${
              property.propertyStatus === 'Available' ? 'bg-blue-100 text-blue-800' :
              property.propertyStatus === 'Sold' ? 'bg-gray-200 text-gray-800' :
              'bg-purple-100 text-purple-800'
            }`}>
              {property.propertyStatus}
            </span>
          </p>
        ) : (
          <p className="text-sm">
            <strong>Status:</strong>
            <span className={`ml-2 px-2 py-1 text-xs font-semibold rounded-full ${
              property.approvalStatus === 'UnderApproval' ? 'bg-yellow-100 text-yellow-800' :
              'bg-red-100 text-red-800'
            }`}>
              {property.approvalStatus}
            </span>
          </p>
        )}
      </div>
      {/* --- End of Status Display Logic --- */}

      {canStartAuction && (
        <button
          onClick={() => onStartAuction(property.id)}
          className="mt-4 w-full bg-green-500 text-white py-2 px-4 rounded hover:bg-green-600 transition-colors"
        >
          Start Auction
        </button>
      )}
    </div>
  );
};

export default PropertyCard;
