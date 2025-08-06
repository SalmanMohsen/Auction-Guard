import type { Property } from '../../types';
import PropertyCard from './PropertyCard';

interface PropertyListProps {
  properties: Property[];
  onStartAuction: (propertyId: string) => void;
}

const PropertyList = ({ properties, onStartAuction }: PropertyListProps) => {
  if (!properties.length) {
    return <p>You have not added any properties yet.</p>;
  }

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      {properties.map(property => (
        <PropertyCard key={property.id} property={property} onStartAuction={onStartAuction} />
      ))}
    </div>
  );
};

export default PropertyList;