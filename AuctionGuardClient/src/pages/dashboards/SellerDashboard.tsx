import { DashboardLayout } from "../../components/DashboardLayout";
import { Card, CardContent, CardHeader, CardTitle } from "../../components/ui/card";
import { Button } from "../../components/ui/button";
import { Badge } from "../../components/ui/badge";
import { Plus, Eye, Edit, BarChart3, DollarSign } from "lucide-react";

const SellerDashboard = () => {
  const properties = [
    { id: 1, title: "Downtown Office", status: "active", bids: 12, currentBid: 850000 },
    { id: 2, title: "Retail Space", status: "pending", bids: 0, currentBid: 0 },
    { id: 3, title: "Warehouse", status: "sold", bids: 18, currentBid: 1200000 }
  ];

  return (
    <DashboardLayout title="Seller Dashboard" description="Manage your property listings">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <Card className="border-0 shadow-card bg-card/50">
          <CardHeader>
            <CardTitle className="flex items-center justify-between">
              My Properties
              <Button variant="neon" size="sm" className="gap-2">
                <Plus className="h-4 w-4" />
                Add Property
              </Button>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            {properties.map((property) => (
              <div key={property.id} className="flex items-center justify-between p-3 rounded-lg bg-muted/30">
                <div>
                  <h3 className="font-medium">{property.title}</h3>
                  <Badge variant={property.status === 'active' ? 'default' : 'outline'}>
                    {property.status}
                  </Badge>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-sm text-muted-foreground">{property.bids} bids</span>
                  <Button variant="ghost" size="sm"><Eye className="h-4 w-4" /></Button>
                  <Button variant="ghost" size="sm"><Edit className="h-4 w-4" /></Button>
                </div>
              </div>
            ))}
          </CardContent>
        </Card>

        <Card className="border-0 shadow-card bg-card/50">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <BarChart3 className="h-5 w-5" />
              Sales Analytics
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="flex justify-between">
                <span>Total Revenue</span>
                <span className="font-bold text-success">$2,050,000</span>
              </div>
              <div className="flex justify-between">
                <span>Properties Sold</span>
                <span className="font-bold">8</span>
              </div>
              <div className="flex justify-between">
                <span>Average Sale Price</span>
                <span className="font-bold">$256,250</span>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </DashboardLayout>
  );
};

export default SellerDashboard;