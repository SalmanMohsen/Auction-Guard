import { useEffect, useState } from 'react';
import { getAuctions } from '../../api/auctionApi';
import type { Auction } from '../../types';
import AuctionList from '../auctions/AuctionList';
import WonAuctions from './WonAuctions';

type FilterType = 'active' | 'scheduled' | 'won';

const BidderDashboard = () => {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [filter, setFilter] = useState<FilterType>('active');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (filter === 'won') return;

    const fetchFilteredAuctions = async () => {
      setLoading(true);
      setError(null);
      try {
        const data = await getAuctions({ status: filter });
        setAuctions(data);
      } catch (err) {
        setError('Failed to load auctions.');
      } finally {
        setLoading(false);
      }
    };

    fetchFilteredAuctions();
  }, [filter]);

  const renderContent = () => {
    if (loading) return <div>Loading...</div>;
    if (error) return <div className="text-red-500">{error}</div>;
    if (filter === 'won') return <WonAuctions />;
    return <AuctionList auctions={auctions} />;
  };

  return (
    <div>
      <div className="flex space-x-4 mb-4">
        <button onClick={() => setFilter('active')} className={`py-2 px-4 rounded ${filter === 'active' ? 'bg-blue-500 text-white' : 'bg-gray-200'}`}>Active Auctions</button>
        <button onClick={() => setFilter('scheduled')} className={`py-2 px-4 rounded ${filter === 'scheduled' ? 'bg-blue-500 text-white' : 'bg-gray-200'}`}>Scheduled Auctions</button>
        <button onClick={() => setFilter('won')} className={`py-2 px-4 rounded ${filter === 'won' ? 'bg-blue-500 text-white' : 'bg-gray-200'}`}>My Won Auctions</button>
      </div>
      {renderContent()}
    </div>
  );
};

export default BidderDashboard;
