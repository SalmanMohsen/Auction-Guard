import { DashboardLayout } from "../../components/DashboardLayout";
import { Card, CardContent, CardHeader, CardTitle } from "../../components/ui/card";
import { Button } from "../../components/ui/button";
import { Badge } from "../../components/ui/badge";
import { Shield, Users, AlertTriangle, CheckCircle } from "lucide-react";
import { useNavigate } from "react-router-dom";

const AdminDashboard = () => {
  const navigate = useNavigate();

  const pendingApprovals = [
    { id: 1, type: "Property", title: "Modern Condo", submitter: "John Doe" },
    { id: 2, type: "User", title: "Seller Account", submitter: "Jane Smith" }
  ];

  return (
    <DashboardLayout title="Admin Dashboard" description="System administration and oversight">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <Card className="border-0 shadow-card bg-card/50">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-warning" />
              Pending Approvals
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            {pendingApprovals.map((item) => (
              <div key={item.id} className="flex items-center justify-between p-3 rounded-lg bg-muted/30">
                <div>
                  <h3 className="font-medium">{item.title}</h3>
                  <p className="text-sm text-muted-foreground">by {item.submitter}</p>
                  <Badge variant="outline">{item.type}</Badge>
                </div>
                <div className="flex gap-2">
                  <Button variant="success" size="sm"><CheckCircle className="h-4 w-4" /></Button>
                  <Button variant="destructive" size="sm">Reject</Button>
                </div>
              </div>
            ))}
          </CardContent>
        </Card>

        <Card className="border-0 shadow-card bg-card/50">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Users className="h-5 w-5" />
              User Management
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="flex justify-between">
                <span>Total Users</span>
                <span className="font-bold">2,847</span>
              </div>
              <div className="flex justify-between">
                <span>Active Bidders</span>
                <span className="font-bold text-primary">1,923</span>
              </div>
              <div className="flex justify-between">
                <span>Verified Sellers</span>
                <span className="font-bold text-accent">892</span>
              </div>
              <Button variant="neon" className="w-full gap-2" onClick={() => navigate('/admin/users')}>
                <Shield className="h-4 w-4" />
                Manage Users
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </DashboardLayout>
  );
};

export default AdminDashboard;
