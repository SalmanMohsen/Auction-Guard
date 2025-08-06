import { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { useLocation, useNavigate } from 'react-router-dom';
import { getAuctionById, joinAuction, getParticipationStatus } from '../../api/auctionApi';
import type { Auction } from '../../types';
import type { RootState } from '../../store/store';
import { formatCurrency, formatDate, formatCountdown } from '../../utils/formatters';
import useCountdown from '../../hooks/useCountdown';
import BiddingPage from './BiddingPage';
import Modal from '../../components/common/Modal';
import { API_BASE_URL } from '../../api/api';
import { openPopup } from '../../utils/popup'; // Import the new utility

interface AuctionDetailsProps {
  auctionId: string;
}

const AuctionDetails = ({ auctionId }: AuctionDetailsProps) => {
  const [auction, setAuction] = useState<Auction | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isJoinModalOpen, setJoinModalOpen] = useState(false);
  const [hasJoined, setHasJoined] = useState(false);
  const [selectedImageUrl, setSelectedImageUrl] = useState<string | null>(null);
  const [joinStatusMessage, setJoinStatusMessage] = useState<string | null>(null);

  const { isAuthenticated } = useSelector((state: RootState) => state.auth);
  const timeRemaining = useCountdown(auction?.endTime || new Date());
  const isAuctionActive = auction?.status === 'Active';
  
  const location = useLocation();
  const navigate = useNavigate();

  const checkParticipation = async () => {
    if (isAuthenticated) {
        const status = await getParticipationStatus(auctionId);
        if (status.isParticipant) {
            setHasJoined(true);
        }
    }
  };

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const status = queryParams.get('status');
    if (status === 'joined_successfully') {
      setJoinStatusMessage("You have successfully joined the auction!");
      setHasJoined(true);
      navigate(location.pathname, { replace: true });
    } else if (status === 'join_cancelled') {
      setError("You cancelled the process of joining the auction.");
      navigate(location.pathname, { replace: true });
    }

    const fetchAuction = async () => {
      try {
        setLoading(true);
        const data: Auction = await getAuctionById(auctionId);
        setAuction(data);
        await checkParticipation();
        if (data.propertyImageUrls && data.propertyImageUrls.length > 0) {
          const fullUrl = `${API_BASE_URL.replace('/api', '')}${data.propertyImageUrls[0]}`;
          setSelectedImageUrl(fullUrl);
        }
      } catch (err) {
        setError('Failed to fetch auction details.');
      } finally {
        setLoading(false);
      }
    };

    fetchAuction();
  }, [auctionId, isAuthenticated, location.search, navigate]);

  const handleJoinAuction = async () => {
    if (!auction) return;
    try {
      setError(null);
      setJoinModalOpen(false);
      const response = await joinAuction(auction.auctionId);

      if (response.approvalUrl === "JOINED_NO_DEPOSIT") {
          setHasJoined(true);
          setJoinStatusMessage("Successfully joined the auction. No deposit was required.");
      } else if (response.approvalUrl) {
          // *** USE THE POPUP INSTEAD OF REDIRECTING ***
          await openPopup(response.approvalUrl, 'PayPal');
          // After the popup is closed, re-check the participation status.
          await checkParticipation();
      } else {
          setError("Could not get a payment approval link. Please try again.");
      }
    } catch (err: any) {
      console.error("Failed to join auction:", err);
      setError(err.response?.data?.message || "An unknown error occurred while trying to join.");
    }
  };

  if (loading) return <div className="text-center p-8">Loading auction...</div>;
  if (error) return <div className="bg-red-100 text-red-700 p-4 rounded-md text-center my-4">{error}</div>;
  if (!auction) return <div className="text-center p-8">Auction not found.</div>;

  if (isAuctionActive && hasJoined) {
    return <BiddingPage auction={auction} />;
  }

  return (
    <div className="container mx-auto p-4">
      {joinStatusMessage && <div className="bg-green-100 text-green-700 p-4 rounded-md text-center my-4">{joinStatusMessage}</div>}
      {/* ... rest of the component JSX remains the same */}
      <h1 className="text-4xl font-bold mb-2">{auction.propertyTitle}</h1>
      <p className="text-lg text-gray-600 mb-6">{auction.propertyDescription}</p>
      
      <div className="grid md:grid-cols-2 gap-8">
        {/* Image Gallery Section */}
        <div>
          <div className="mb-4">
            <img 
              src={selectedImageUrl || 'https://placehold.co/800x600/eee/ccc?text=No+Image'} 
              alt={auction.propertyTitle} 
              className="w-full h-auto max-h-[500px] object-cover rounded-lg shadow-md bg-gray-200"
            />
          </div>
          <div className="flex space-x-2 overflow-x-auto p-2">
            {auction.propertyImageUrls.map((url, index) => {
              const fullUrl = `${API_BASE_URL.replace('/api', '')}${url}`;
              return (
                <img
                  key={index}
                  src={fullUrl}
                  alt={`Thumbnail ${index + 1}`}
                  onClick={() => setSelectedImageUrl(fullUrl)}
                  className={`w-24 h-24 object-cover rounded-md cursor-pointer border-2 transition-all ${selectedImageUrl === fullUrl ? 'border-blue-500 scale-105' : 'border-transparent'} hover:border-blue-400`}
                />
              );
            })}
          </div>
        </div>

        {/* Bidding and Info Section */}
        <div className="bg-gray-50 p-6 rounded-lg shadow-inner">
          <h2 className="text-2xl font-semibold mb-4">Auction Details</h2>
          <div className="space-y-3">
            <p><strong>Highest Bid:</strong> <span className="text-green-600 font-bold text-xl">{formatCurrency(auction.currentHighestBid || 0)}</span></p>
            <p><strong>Ends On:</strong> {formatDate(auction.endTime)}</p>
            <div className="text-2xl font-bold my-4 text-red-600">
              Time Left: {formatCountdown(timeRemaining)}
            </div>
          </div>
          {isAuthenticated && auction.status !== 'Ended' && !hasJoined && (
            <button 
              onClick={() => setJoinModalOpen(true)}
              className="mt-6 w-full bg-green-500 text-white py-3 px-6 rounded-lg text-xl hover:bg-green-600 transition-colors"
            >
              Join Auction
            </button>
          )}
           {!isAuthenticated && (
            <p className="mt-6 text-center text-gray-600">Please log in to join the auction.</p>
           )}
           {hasJoined && (
            <p className="mt-6 text-center text-green-700 font-semibold">You have joined this auction.</p>
           )}
        </div>
      </div>

      <Modal isOpen={isJoinModalOpen} onClose={() => setJoinModalOpen(false)}>
        <h2 className="text-2xl font-bold mb-4">Join Auction</h2>
        <p>To participate, a temporary hold for the guarantee deposit of <strong>{formatCurrency(auction.guaranteeDeposit)}</strong> will be placed on your payment method. Do you agree?</p>
        <div className="mt-6 flex justify-end space-x-4">
          <button onClick={() => setJoinModalOpen(false)} className="bg-gray-300 py-2 px-4 rounded">Cancel</button>
          <button onClick={handleJoinAuction} className="bg-blue-500 text-white py-2 px-4 rounded">Agree & Join</button>
        </div>
      </Modal>
    </div>
  );
};

export default AuctionDetails;
