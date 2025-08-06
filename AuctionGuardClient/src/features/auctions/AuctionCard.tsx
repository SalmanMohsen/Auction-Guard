import { Link } from 'react-router-dom';
import type { Auction } from '../../types';
import { formatCurrency, formatDate, formatCountdown } from '../../utils/formatters';
import useCountdown from '../../hooks/useCountdown';
import { API_BASE_URL } from '../../api/api';

interface AuctionCardProps {
  auction: Auction;
  onCancel?: (auctionId: string) => void;
}

const AuctionCard = ({ auction, onCancel }: AuctionCardProps) => {
  const timeRemaining = useCountdown(auction.endTime);
  const isAuctionActive = timeRemaining > 0 && new Date(auction.startTime) < new Date();

  // Use the first image from the property's image list as the cover.
  const coverImageUrl = auction.propertyImageUrls && auction.propertyImageUrls.length > 0
    ? `${API_BASE_URL.replace('/api', '')}${auction.propertyImageUrls[0]}`
    : 'https://placehold.co/600x400/eee/ccc?text=No+Image';

  return (
    <div className="border rounded-lg p-4 shadow-lg hover:shadow-xl transition-shadow duration-300 flex flex-col justify-between bg-white">
      <div>
        <img 
          src={coverImageUrl} 
          alt={auction.propertyTitle} 
          className="w-full h-48 object-cover rounded-md mb-4 bg-gray-200"
        />
        <h3 className="text-xl font-bold">{auction.propertyTitle}</h3>
        <p className="text-gray-600 mt-2 text-sm">
          {auction.propertyDescription?.substring(0, 100) || 'No description available.'}...
        </p>
        <div className="mt-4">
          <p><strong>Current Bid:</strong> {formatCurrency(auction.currentHighestBid || 0)}</p>
          <p><strong>Status:</strong> {auction.status}</p>
          {isAuctionActive && (
              <p><strong>Time Left:</strong> {formatCountdown(timeRemaining)}</p>
          )}
           <p className="text-sm text-gray-500">Ends: {formatDate(auction.endTime)}</p>
        </div>
      </div>
      
      <div className="mt-4 flex flex-col space-y-2">
        <Link to={`/auctions/${auction.auctionId}`} className="text-center bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600 transition-colors">
          View Auction
        </Link>
        {onCancel && auction.status === 'Scheduled' && (
          <button
            onClick={() => onCancel(auction.auctionId)}
            className="bg-red-500 text-white py-2 px-4 rounded hover:bg-red-600 transition-colors"
          >
            Cancel Auction
          </button>
        )}
      </div>
    </div>
  );
};

export default AuctionCard;
