import { useState, type FormEvent, useEffect } from 'react';
import type { Auction, Bid } from '../../types';
import { useRealtimeBids } from '../../hooks/useRealtimeBids'; // Corrected import
import useCountdown from '../../hooks/useCountdown';
import { formatCurrency } from '../../utils/formatters';

interface BiddingPageProps {
  auction: Auction;
}

const BiddingPage = ({ auction }: BiddingPageProps) => {
  const [bidAmount, setBidAmount] = useState<number | ''>('');
  
  // Use the custom hook to manage bids and the connection
  const { bids, placeBid, error: biddingError, connectionStatus } = useRealtimeBids(auction.auctionId);

  const [localBids, setLocalBids] = useState<Bid[]>([]);

  const timeRemaining = useCountdown(auction.endTime);

  useEffect(() => {
    setLocalBids(bids);
  }, [bids]);

  const highestBid = localBids.length > 0 ? localBids[0].amount : auction.currentHighestBid || 0;
  const highestBidder = localBids.length > 0 ? localBids[0].bidderName : 'No bids yet';
  const minNextBid = highestBid + auction.minBidIncrement;

  const handlePlaceBid = async (e: FormEvent) => {
    e.preventDefault();
    if (typeof bidAmount !== 'number' || bidAmount < minNextBid) {
      alert(`Your bid must be at least ${formatCurrency(minNextBid)}.`);
      return;
    }
    
    // Use the function exposed by the hook
    await placeBid(bidAmount);
    setBidAmount(''); // Clear input after placing bid
  };

  const calculateTimeParts = (ms: number) => {
    if (ms <= 0) {
      return { days: '00', hours: '00', minutes: '00', seconds: '00' };
    }
    const totalSeconds = Math.floor(ms / 1000);
    const days = String(Math.floor(totalSeconds / (3600 * 24))).padStart(2, '0');
    const hours = String(Math.floor((totalSeconds % (3600 * 24)) / 3600)).padStart(2, '0');
    const minutes = String(Math.floor((totalSeconds % 3600) / 60)).padStart(2, '0');
    const seconds = String(Math.floor(totalSeconds % 60)).padStart(2, '0');
    
    return { days, hours, minutes, seconds };
  };

  const timeParts = calculateTimeParts(timeRemaining);

  return (
    <div className="flex flex-col md:flex-row gap-8 p-4">
      <div className="w-full md:w-2/3">
        <h2 className="text-2xl font-semibold mb-4">Recent Bids</h2>
        <div className="space-y-2 max-h-96 overflow-y-auto pr-2">
            {localBids.map((bid) => (
                <div key={bid.bidId} className="bg-gray-100 p-3 rounded-md flex justify-between items-center">
                    <div>
                        <p className="font-semibold">{bid.bidderName}</p>
                        <p className="text-sm text-gray-500">{new Date(bid.timestamp).toLocaleTimeString()}</p>
                    </div>
                    <p className="text-lg font-bold text-green-600">{formatCurrency(bid.amount)}</p>
                </div>
            ))}
            {localBids.length === 0 && <p>No bids have been placed yet.</p>}
        </div>
      </div>
      <div className="w-full md:w-1/3">
        <div className="bg-white p-6 rounded-lg shadow-md sticky top-4">
            <h2 className="text-2xl font-bold">Time Remaining</h2>
            <div className="flex justify-start space-x-2 text-center my-4">
                <div><p className="text-3xl font-bold text-red-600">{timeParts.days}</p><p className="text-xs">DAYS</p></div>
                <div><p className="text-3xl font-bold text-red-600">{timeParts.hours}</p><p className="text-xs">HOURS</p></div>
                <div><p className="text-3xl font-bold text-red-600">{timeParts.minutes}</p><p className="text-xs">MINS</p></div>
                <div><p className="text-3xl font-bold text-red-600">{timeParts.seconds}</p><p className="text-xs">SECS</p></div>
            </div>
            <div className="mb-4">
                <p className="text-lg">Highest Bid:</p>
                <p className="text-4xl font-bold text-green-600">{formatCurrency(highestBid)}</p>
                <p className="text-md text-gray-600">by {highestBidder}</p>
            </div>
            <form onSubmit={handlePlaceBid}>
              <input
                type="number"
                value={bidAmount}
                onChange={(e) => setBidAmount(e.target.value === '' ? '' : Number(e.target.value))}
                className="w-full p-3 border rounded text-xl"
                placeholder={`Min bid: ${formatCurrency(minNextBid)}`}
                step="0.01"
              />
              {biddingError && <p className="text-red-500 mt-2">{biddingError}</p>}
              <button type="submit" disabled={connectionStatus !== 'Connected'} className="w-full mt-4 bg-blue-600 text-white py-3 rounded-lg text-xl font-bold hover:bg-blue-700 transition-colors disabled:bg-gray-400">
                Place Bid
              </button>
            </form>
        </div>
      </div>
    </div>
  );
};

export default BiddingPage;
