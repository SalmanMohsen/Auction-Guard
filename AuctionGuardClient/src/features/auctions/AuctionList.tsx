import type { Auction } from '../../types';
import AuctionCard from './AuctionCard';

interface AuctionListProps {
  auctions: Auction[];
  onCancelAuction?: (auctionId: string) => void;
}

const AuctionList = ({ auctions, onCancelAuction }: AuctionListProps) => {
  if (!auctions || !auctions.length) {
    return <p>No auctions available at the moment.</p>;
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {auctions.map(auction => (
        <AuctionCard 
          key={auction.id} 
          auction={auction} 
          onCancel={onCancelAuction} 
        />
      ))}
    </div>
  );
};

export default AuctionList;