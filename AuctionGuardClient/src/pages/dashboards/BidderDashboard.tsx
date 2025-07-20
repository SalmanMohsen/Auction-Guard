import { DashboardLayout } from "../../components/DashboardLayout";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "../../components/ui/card";
import { Button } from "../../components/ui/button";
import { Badge } from "../../components/ui/badge";
import { 
  Heart, 
  Clock, 
  DollarSign, 
  MapPin, 
  Eye, 
  Gavel,
  TrendingUp,
  Calendar,
  Star
} from "lucide-react";

const BidderDashboard = () => {
  const activeAuctions = [
    {
      id: 1,
      title: "Modern Downtown Condo",
      location: "Manhattan, NY",
      currentBid: 850000,
      timeLeft: "2h 15m",
      image: "/api/placeholder/300/200",
      bedrooms: 2,
      bathrooms: 2,
      sqft: 1200,
      isFavorite: true
    },
    {
      id: 2,
      title: "Luxury Waterfront Villa",
      location: "Miami, FL", 
      currentBid: 2400000,
      timeLeft: "1d 8h",
      image: "/api/placeholder/300/200",
      bedrooms: 4,
      bathrooms: 3,
      sqft: 3500,
      isFavorite: false
    },
    {
      id: 3,
      title: "Historic Brownstone",
      location: "Boston, MA",
      currentBid: 675000,
      timeLeft: "3h 42m",
      image: "/api/placeholder/300/200", 
      bedrooms: 3,
      bathrooms: 2,
      sqft: 2100,
      isFavorite: true
    }
  ];

  const myBids = [
    {
      id: 1,
      property: "Modern Downtown Condo",
      myBid: 825000,
      currentBid: 850000,
      status: "outbid",
      timeLeft: "2h 15m"
    },
    {
      id: 2,
      property: "Garden View Apartment",
      myBid: 545000,
      currentBid: 545000,
      status: "winning",
      timeLeft: "5h 22m"
    },
    {
      id: 3,
      property: "Suburban Family Home",
      myBid: 425000,
      currentBid: 440000,
      status: "outbid",
      timeLeft: "1d 3h"
    }
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'winning': return 'bg-success text-success-foreground';
      case 'outbid': return 'bg-destructive text-destructive-foreground';
      default: return 'bg-muted text-muted-foreground';
    }
  };

  return (
    <DashboardLayout 
      title="Bidder Dashboard" 
      description="Track your bids and discover new properties"
    >
      <div className="space-y-8">
        {/* Active Bids Section */}
        <section>
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-semibold text-foreground">My Active Bids</h2>
            <Button variant="outline" className="gap-2">
              <TrendingUp className="h-4 w-4" />
              View All Bids
            </Button>
          </div>
          
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
            {myBids.map((bid) => (
              <Card key={bid.id} className="border-0 shadow-card bg-card/50 backdrop-blur-sm">
                <CardHeader className="pb-3">
                  <div className="flex items-center justify-between">
                    <CardTitle className="text-lg">{bid.property}</CardTitle>
                    <Badge className={getStatusColor(bid.status)}>
                      {bid.status === 'winning' ? 'Winning' : 'Outbid'}
                    </Badge>
                  </div>
                </CardHeader>
                <CardContent className="space-y-3">
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">My Bid:</span>
                      <span className="font-semibold text-foreground">
                        ${bid.myBid.toLocaleString()}
                      </span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">Current:</span>
                      <span className="font-semibold text-primary">
                        ${bid.currentBid.toLocaleString()}
                      </span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">Time Left:</span>
                      <span className="font-semibold text-warning">
                        {bid.timeLeft}
                      </span>
                    </div>
                  </div>
                  
                  <Button 
                    className="w-full" 
                    variant={bid.status === 'winning' ? 'success' : 'neon'}
                  >
                    {bid.status === 'winning' ? 'Monitor Bid' : 'Place New Bid'}
                  </Button>
                </CardContent>
              </Card>
            ))}
          </div>
        </section>

        {/* Featured Auctions */}
        <section>
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-semibold text-foreground">Featured Auctions</h2>
            <Button variant="accent" className="gap-2">
              <Eye className="h-4 w-4" />
              Browse All
            </Button>
          </div>
          
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {activeAuctions.map((auction) => (
              <Card key={auction.id} className="border-0 shadow-card bg-card/50 backdrop-blur-sm overflow-hidden group hover:shadow-glow transition-all duration-300">
                <div className="relative">
                  <div className="h-48 bg-gradient-primary/20 flex items-center justify-center">
                    <div className="text-center text-muted-foreground">
                      <Eye className="h-8 w-8 mx-auto mb-2" />
                      <p className="text-sm">Property Image</p>
                    </div>
                  </div>
                  
                  {/* Overlay Actions */}
                  <div className="absolute top-3 right-3 flex gap-2">
                    <Button 
                      size="sm" 
                      variant="secondary" 
                      className="h-8 w-8 p-0 bg-white/90 hover:bg-white"
                    >
                      <Heart className={`h-4 w-4 ${auction.isFavorite ? 'fill-red-500 text-red-500' : ''}`} />
                    </Button>
                  </div>
                  
                  {/* Time Left Badge */}
                  <div className="absolute bottom-3 left-3">
                    <Badge variant="destructive" className="gap-1">
                      <Clock className="h-3 w-3" />
                      {auction.timeLeft}
                    </Badge>
                  </div>
                </div>
                
                <CardContent className="p-4">
                  <div className="space-y-3">
                    <div>
                      <h3 className="font-semibold text-foreground mb-1">{auction.title}</h3>
                      <div className="flex items-center gap-1 text-sm text-muted-foreground">
                        <MapPin className="h-3 w-3" />
                        {auction.location}
                      </div>
                    </div>
                    
                    <div className="flex justify-between text-sm">
                      <span>{auction.bedrooms} bed</span>
                      <span>{auction.bathrooms} bath</span>
                      <span>{auction.sqft.toLocaleString()} sqft</span>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-xs text-muted-foreground">Current Bid</p>
                        <p className="text-lg font-bold text-primary">
                          ${auction.currentBid.toLocaleString()}
                        </p>
                      </div>
                      <div className="flex items-center gap-1 text-warning">
                        <Star className="h-4 w-4 fill-current" />
                        <span className="text-sm font-medium">4.8</span>
                      </div>
                    </div>
                    
                    <div className="flex gap-2">
                      <Button className="flex-1" variant="neon">
                        <Gavel className="h-4 w-4 mr-2" />
                        Bid Now
                      </Button>
                      <Button variant="outline" size="sm" className="px-3">
                        <Eye className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </section>

        {/* Quick Actions */}
        <section>
          <h2 className="text-2xl font-semibold text-foreground mb-6">Quick Actions</h2>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <Card className="p-4 bg-card/50 backdrop-blur-sm border-0 shadow-card hover:shadow-glow transition-all duration-300 cursor-pointer">
              <div className="text-center space-y-3">
                <div className="mx-auto p-3 rounded-lg bg-primary/20 w-fit">
                  <Heart className="h-6 w-6 text-primary" />
                </div>
                <div>
                  <h3 className="font-semibold text-foreground">Favorites</h3>
                  <p className="text-sm text-muted-foreground">12 saved properties</p>
                </div>
              </div>
            </Card>
            
            <Card className="p-4 bg-card/50 backdrop-blur-sm border-0 shadow-card hover:shadow-glow transition-all duration-300 cursor-pointer">
              <div className="text-center space-y-3">
                <div className="mx-auto p-3 rounded-lg bg-accent/20 w-fit">
                  <Calendar className="h-6 w-6 text-accent" />
                </div>
                <div>
                  <h3 className="font-semibold text-foreground">Schedule</h3>
                  <p className="text-sm text-muted-foreground">3 upcoming auctions</p>
                </div>
              </div>
            </Card>
            
            <Card className="p-4 bg-card/50 backdrop-blur-sm border-0 shadow-card hover:shadow-glow transition-all duration-300 cursor-pointer">
              <div className="text-center space-y-3">
                <div className="mx-auto p-3 rounded-lg bg-success/20 w-fit">
                  <DollarSign className="h-6 w-6 text-success" />
                </div>
                <div>
                  <h3 className="font-semibold text-foreground">Payments</h3>
                  <p className="text-sm text-muted-foreground">Manage methods</p>
                </div>
              </div>
            </Card>
            
            <Card className="p-4 bg-card/50 backdrop-blur-sm border-0 shadow-card hover:shadow-glow transition-all duration-300 cursor-pointer">
              <div className="text-center space-y-3">
                <div className="mx-auto p-3 rounded-lg bg-warning/20 w-fit">
                  <TrendingUp className="h-6 w-6 text-warning" />
                </div>
                <div>
                  <h3 className="font-semibold text-foreground">Analytics</h3>
                  <p className="text-sm text-muted-foreground">Bidding insights</p>
                </div>
              </div>
            </Card>
          </div>
        </section>
      </div>
    </DashboardLayout>
  );
};

export default BidderDashboard;