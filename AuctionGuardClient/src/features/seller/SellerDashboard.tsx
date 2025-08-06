import { useState, useEffect } from 'react';
import MyAuctions from './MyAuctions';
import MyProperties from './MyProperties';
import AuctionList from '../auctions/AuctionList';
import { getAuctions } from '../../api/auctionApi';
import type { Auction } from '../../types';

type Tab = 'all-auctions' | 'my-auctions' | 'my-properties';

const SellerDashboard = () => {
  const [activeTab, setActiveTab] = useState<Tab>('all-auctions');
  const [allAuctions, setAllAuctions] = useState<Auction[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // Fetch all auctions only when the 'all-auctions' tab is active
    if (activeTab === 'all-auctions') {
      const fetchAllAuctions = async () => {
        setLoading(true);
        setError(null);
        try {
          // Fetch both active and scheduled auctions
          const active = await getAuctions({ status: 'active' });
          const scheduled = await getAuctions({ status: 'scheduled' });
          setAllAuctions([...active, ...scheduled]);
        } catch (err) {
          setError('Failed to load auctions.');
        } finally {
          setLoading(false);
        }
      };
      fetchAllAuctions();
    }
  }, [activeTab]);

  const renderContent = () => {
    switch (activeTab) {
      case 'all-auctions':
        if (loading) return <div>Loading auctions...</div>;
        if (error) return <div className="text-red-500">{error}</div>;
        return <AuctionList auctions={allAuctions} />;
      case 'my-auctions':
        return <MyAuctions />;
      case 'my-properties':
        return <MyProperties />;
      default:
        return null;
    }
  };

  return (
    <div>
      <div className="flex space-x-4 mb-4 border-b">
        <button onClick={() => setActiveTab('all-auctions')} className={`py-2 px-4 ${activeTab === 'all-auctions' ? 'border-b-2 border-blue-500 font-semibold' : ''}`}>Browse Auctions</button>
        <button onClick={() => setActiveTab('my-auctions')} className={`py-2 px-4 ${activeTab === 'my-auctions' ? 'border-b-2 border-blue-500 font-semibold' : ''}`}>My Auctions</button>
        <button onClick={() => setActiveTab('my-properties')} className={`py-2 px-4 ${activeTab === 'my-properties' ? 'border-b-2 border-blue-500 font-semibold' : ''}`}>My Properties</button>
      </div>
      <div>
        {renderContent()}
      </div>
    </div>
  );
};

export default SellerDashboard;
