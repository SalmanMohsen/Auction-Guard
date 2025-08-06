import { useEffect, useState } from 'react';
import { getAuctions } from '../api/auctionApi';
import AuctionList from '../features/auctions/AuctionList';
import type { Auction } from '../types';

const LandingPage = () => {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchAuctions = async () => {
      try {
        const auctionData = await getAuctions({ status: 'active' });
        setAuctions(auctionData);
      } catch (err) {
        setError('Failed to load auctions.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchAuctions();
  }, []);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>{error}</div>;

  return (
    <div className="container mx-auto px-4">
      <h1 className="text-3xl font-bold my-6">Welcome to AuctionGuard</h1>
      <h2 className="text-2xl font-semibold mb-4">Live Auctions</h2>
      <AuctionList auctions={auctions} />
    </div>
  );
};

export default LandingPage;