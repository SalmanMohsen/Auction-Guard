import { useEffect, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { getMyProperties } from '../../api/propertyApi';
import { setMyProperties } from '../../store/slices/propertySlice';
import type { RootState } from '../../store/store';
import PropertyList from '../properties/PropertyList';
import AddProperty from '../properties/AddProperty';
import StartAuctionModal from '../properties/StartAuctionModal';

const MyProperties = () => {
  const dispatch = useDispatch();
  const { myProperties } = useSelector((state: RootState) => state.property);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isAddModalOpen, setAddModalOpen] = useState(false);
  const [isAuctionModalOpen, setAuctionModalOpen] = useState(false);
  const [selectedPropertyId, setSelectedPropertyId] = useState<string | null>(null);

  useEffect(() => {
    const fetchProperties = async () => {
      try {
        const data = await getMyProperties();
        dispatch(setMyProperties(data));
      } catch (err) {
        setError('Failed to load your properties.');
      } finally {
        setLoading(false);
      }
    };
    fetchProperties();
  }, [dispatch]);

  const handleOpenAuctionModal = (propertyId: string) => {
    setSelectedPropertyId(propertyId);
    setAuctionModalOpen(true);
  };

  if (loading) return <div>Loading your properties...</div>;
  if (error) return <div className="text-red-500">{error}</div>;

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-2xl font-bold">My Properties</h2>
        <button onClick={() => setAddModalOpen(!isAddModalOpen)} className="bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600">
          {isAddModalOpen ? 'Close Form' : '+ Add New Property'}
        </button>
      </div>

      {isAddModalOpen && <AddProperty />}

      <div className="mt-6">
        <PropertyList properties={myProperties} onStartAuction={handleOpenAuctionModal} />
      </div>

      {selectedPropertyId && (
        <StartAuctionModal
          isOpen={isAuctionModalOpen}
          onClose={() => setAuctionModalOpen(false)}
          propertyId={selectedPropertyId}
        />
      )}
    </div>
  );
};

export default MyProperties;