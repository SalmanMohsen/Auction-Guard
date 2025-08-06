import { useEffect, useState } from 'react';
import { getWonAuctions } from '../../api/auctionApi';
import type { Auction } from '../../types';
import AuctionList from '../auctions/AuctionList';

const WonAuctions = () => {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchWonAuctions = async () => {
      try {
        const data = await getWonAuctions();
        setAuctions(data);
      } catch (err) {
        setError('Failed to load your won auctions.');
      } finally {
        setLoading(false);
      }
    };

    fetchWonAuctions();
  }, []);

  if (loading) return <div>Loading won auctions...</div>;
  if (error) return <div className="text-red-500">{error}</div>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Auctions You've Won</h2>
      <AuctionList auctions={auctions} />
    </div>
  );
};

export default WonAuctions;