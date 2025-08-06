import { useEffect, useState } from 'react';
import { getAllAuctions, cancelAuction } from '../../api/adminApi';
import type { Auction } from '../../types';
import { formatDate } from '../../utils/formatters';

const ManageAuctions = () => {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchAuctions = async () => {
    try {
      setLoading(true);
      const auctionData = await getAllAuctions();
      setAuctions(auctionData);
    } catch (err) {
      setError('Failed to load auctions.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAuctions();
  }, []);

  const handleCancel = async (auctionId: string) => {
    if (window.confirm('Are you sure you want to cancel this auction?')) {
        try {
            await cancelAuction(auctionId);
            // Refresh the list
            fetchAuctions();
        } catch (error) {
            console.error(`Failed to cancel auction ${auctionId}`, error);
            setError(`Failed to cancel auction ${auctionId}`);
        }
    }
  };

  if (loading) return <div>Loading auctions...</div>;
  if (error) return <div className="text-red-500">{error}</div>;

  return (
    <div className="overflow-x-auto">
      <table className="min-w-full bg-white">
        <thead>
          <tr>
            <th className="py-2 px-4 border-b">Title</th>
            <th className="py-2 px-4 border-b">Status</th>
            <th className="py-2 px-4 border-b">Start Time</th>
            <th className="py-2 px-4 border-b">End Time</th>
            <th className="py-2 px-4 border-b">Actions</th>
          </tr>
        </thead>
        <tbody>
          {auctions.map(auction => (
            <tr key={auction.id}>
              <td className="py-2 px-4 border-b">{auction.title}</td>
              <td className="py-2 px-4 border-b">{auction.status}</td>
              <td className="py-2 px-4 border-b">{formatDate(auction.startTime)}</td>
              <td className="py-2 px-4 border-b">{formatDate(auction.endTime)}</td>
              <td className="py-2 px-4 border-b">
                {auction.status === 'Scheduled' && (
                  <button
                    onClick={() => handleCancel(auction.id)}
                    className="bg-red-500 hover:bg-red-700 text-white font-bold py-1 px-2 rounded"
                  >
                    Cancel
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default ManageAuctions;