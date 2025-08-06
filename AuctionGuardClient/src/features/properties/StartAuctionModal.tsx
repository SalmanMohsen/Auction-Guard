import { useState } from 'react';
import type { FormEvent } from 'react';
import { startAuction as startAuctionApi } from '../../api/propertyApi'; 
import type { CreateAuctionData, Offer } from '../../types';
import Modal from '../../components/common/Modal';

interface StartAuctionModalProps {
  isOpen: boolean;
  onClose: () => void;
  propertyId: string;
}

const StartAuctionModal = ({ isOpen, onClose, propertyId }: StartAuctionModalProps) => {
  const [formData, setFormData] = useState<Omit<CreateAuctionData, 'propertyId' | 'offers'>>({
    startTime: '',
    endTime: '',
    minBidIncrement: 10,
    guaranteeDeposit: 100,
  });
  
  // State to manage the list of offers
  const [offers, setOffers] = useState<Offer[]>([{ description: '', triggerPrice: 0 }]);
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type } = e.target;
    setFormData({ ...formData, [name]: type === 'number' ? parseFloat(value) : value });
  };

  // --- Functions to manage the offers list ---
  const handleOfferChange = (index: number, e: React.ChangeEvent<HTMLInputElement>) => {
    const newOffers = [...offers];
    const { name, value, type } = e.target;
    newOffers[index] = {
      ...newOffers[index],
      [name]: type === 'number' ? parseFloat(value) : value,
    };
    setOffers(newOffers);
  };

  const addOffer = () => {
    setOffers([...offers, { description: '', triggerPrice: 0 }]);
  };

  const removeOffer = (index: number) => {
    const newOffers = offers.filter((_, i) => i !== index);
    setOffers(newOffers);
  };
  // --- End of offer management functions ---

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      // Construct the full payload including the propertyId and the offers list
      const auctionData: CreateAuctionData = { ...formData, propertyId, offers };
      await startAuctionApi(auctionData);
      onClose(); // Close modal on success
    } catch (err) {
      setError('Failed to start auction. Please check the details and try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <form onSubmit={handleSubmit} className="space-y-4">
        <h2 className="text-2xl font-bold">Start an Auction</h2>
        {error && <div className="p-3 bg-red-100 text-red-700 rounded">{error}</div>}
        
        {/* Auction Details */}
        <div>
            <label>Start Time</label>
            <input type="datetime-local" name="startTime" value={formData.startTime} onChange={handleChange} required className="w-full p-2 border rounded" />
        </div>
        <div>
            <label>End Time</label>
            <input type="datetime-local" name="endTime" value={formData.endTime} onChange={handleChange} required className="w-full p-2 border rounded" />
        </div>
        <div>
            <label>Minimum Bid Increment</label>
            <input type="number" name="minBidIncrement" value={formData.minBidIncrement} onChange={handleChange} required min="1" className="w-full p-2 border rounded" />
        </div>
         <div>
            <label>Guarantee Deposit</label>
            <input type="number" name="guaranteeDeposit" value={formData.guaranteeDeposit} onChange={handleChange} required min="0" className="w-full p-2 border rounded" />
        </div>

        {/* Dynamic Offers Section */}
        <hr/>
        <h3 className="text-xl font-semibold">Special Offers</h3>
        {offers.map((offer, index) => (
          <div key={index} className="p-2 border rounded space-y-2 relative">
            <input
              type="text"
              name="description"
              placeholder="Offer Description (e.g., 'Free Shipping')"
              value={offer.description}
              onChange={(e) => handleOfferChange(index, e)}
              className="w-full p-2 border rounded"
            />
            <input
              type="number"
              name="triggerPrice"
              placeholder="Trigger Price (at which offer becomes active)"
              value={offer.triggerPrice}
              onChange={(e) => handleOfferChange(index, e)}
              className="w-full p-2 border rounded"
            />
            {offers.length > 1 && (
              <button type="button" onClick={() => removeOffer(index)} className="absolute top-1 right-1 text-red-500 hover:text-red-700 font-bold">
                &times;
              </button>
            )}
          </div>
        ))}
        <button type="button" onClick={addOffer} className="w-full text-blue-600 hover:text-blue-800 text-sm font-semibold">
          + Add Another Offer
        </button>
        <hr/>

        <div className="flex justify-end space-x-3 pt-4">
          <button type="button" onClick={onClose} className="bg-gray-200 py-2 px-4 rounded">Cancel</button>
          <button type="submit" disabled={loading} className="bg-green-500 text-white py-2 px-4 rounded disabled:bg-green-300">
            {loading ? 'Starting...' : 'Start Auction'}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default StartAuctionModal;