import { useParams } from 'react-router-dom';
import AuctionDetails from '../features/auctions/AuctionDetails';

const AuctionDetailsPage = () => {
  const { id } = useParams<{ id: string }>();

  if (!id) return <div>Auction not found.</div>;

  return (
    <div className="container mx-auto px-4">
      <AuctionDetails auctionId={id} />
    </div>
  );
};

export default AuctionDetailsPage;
