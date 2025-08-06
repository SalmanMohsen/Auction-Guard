import { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { getAuctions } from '../../api/auctionApi';
import { cancelAuction } from '../../api/adminApi';
import type { Auction } from '../../types';
import type { RootState } from '../../store/store';
import AuctionList from '../auctions/AuctionList';

const MyAuctions = () => {
  const [myAuctions, setMyAuctions] = useState<Auction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { user } = useSelector((state: RootState) => state.auth);

  const fetchMyAuctions = async () => {
    if (!user) return;
    setLoading(true);
    setError(null);
    try {
      const allUserAuctions = await getAuctions({ ownerId: user.id }); 
      setMyAuctions(allUserAuctions);
    } catch (err) {
      setError("Failed to load your auctions.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMyAuctions();
  }, [user]);

  const handleCancelAuction = async (auctionId: string) => {
    if (window.confirm("Are you sure you want to cancel this scheduled auction?")) {
      try {
        await cancelAuction(auctionId);
        fetchMyAuctions();
      } catch (error) {
        console.error("Failed to cancel auction", error);
        setError("Could not cancel the auction. Please try again.");
      }
    }
  };

  if (loading) return <div>Loading your auctions...</div>;
  if (error) return <div className="text-red-500">{error}</div>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">My Created Auctions</h2>
      <AuctionList auctions={myAuctions} onCancelAuction={handleCancelAuction} />
    </div>
  );
};

export default MyAuctions;