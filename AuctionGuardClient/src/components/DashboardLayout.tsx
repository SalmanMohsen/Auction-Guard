import type { ReactNode } from "react";
import { Button } from "../components/ui/button";
import { Card } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import { 
  Gavel, 
  LogOut, 
  User, 
  Settings, 
  Bell,
  Home,
  Users,
  TrendingUp,
  Shield,
  Search,
  Plus
} from "lucide-react";
import { useAuth } from "../contexts/AuthContext";
import { useNavigate } from "react-router-dom";

interface DashboardLayoutProps {
  children: ReactNode;
  title: string;
  description?: string;
}

export const DashboardLayout = ({ children, title, description }: DashboardLayoutProps) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const getRoleIcon = () => {
    switch (user?.role) {
      case 'admin': return Shield;
      case 'seller': return TrendingUp;
      case 'bidder': return Users;
      default: return User;
    }
  };

  const getRoleColor = () => {
    switch (user?.role) {
      case 'admin': return 'bg-destructive';
      case 'seller': return 'bg-accent';
      case 'bidder': return 'bg-primary';
      default: return 'bg-muted';
    }
  };

  const RoleIcon = getRoleIcon();

  return (
    <div className="min-h-screen bg-gradient-hero">
      {/* Header */}
      <header className="border-b border-border/50 backdrop-blur-sm bg-background/80 sticky top-0 z-50">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            {/* Logo */}
            <div className="flex items-center gap-3">
              <div className="p-2 rounded-lg bg-gradient-primary">
                <Gavel className="h-6 w-6 text-white" />
              </div>
              <div>
                <h1 className="text-xl font-bold text-foreground">Auction Guard</h1>
                <p className="text-xs text-muted-foreground">Professional Dashboard</p>
              </div>
            </div>

            {/* User Info & Actions */}
            <div className="flex items-center gap-4">
              {/* Search */}
              <Button variant="ghost" size="sm" className="gap-2 hidden sm:flex">
                <Search className="h-4 w-4" />
                Search
              </Button>

              {/* Notifications */}
              <Button variant="ghost" size="sm" className="relative">
                <Bell className="h-4 w-4" />
                <span className="absolute -top-1 -right-1 h-3 w-3 bg-destructive rounded-full text-[10px] flex items-center justify-center text-white">
                  3
                </span>
              </Button>

              {/* User Profile */}
              <div className="flex items-center gap-3">
                <div className="text-right hidden sm:block">
                  <p className="text-sm font-medium text-foreground">
                    {user?.firstName} {user?.lastName}
                  </p>
                  <div className="flex items-center gap-2">
                    <Badge variant="outline" className="text-xs">
                      <RoleIcon className="h-3 w-3 mr-1" />
                      {user?.role?.charAt(0).toUpperCase()}{user?.role?.slice(1)}
                    </Badge>
                  </div>
                </div>
                
                <div className={`p-2 rounded-lg ${getRoleColor()}`}>
                  <RoleIcon className="h-5 w-5 text-white" />
                </div>
              </div>

              {/* Settings */}
              <Button variant="ghost" size="sm">
                <Settings className="h-4 w-4" />
              </Button>

              {/* Logout */}
              <Button variant="outline" size="sm" onClick={handleLogout} className="gap-2">
                <LogOut className="h-4 w-4" />
                <span className="hidden sm:inline">Logout</span>
              </Button>
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="container mx-auto px-4 py-8">
        {/* Page Header */}
        <div className="mb-8">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h1 className="text-3xl font-bold text-foreground">{title}</h1>
              {description && (
                <p className="text-lg text-muted-foreground mt-1">{description}</p>
              )}
            </div>
            
            {/* Quick Actions */}
            <div className="flex items-center gap-3">
              {user?.role === 'seller' && (
                <Button variant="neon" className="gap-2">
                  <Plus className="h-4 w-4" />
                  Add Property
                </Button>
              )}
              {user?.role === 'bidder' && (
                <Button variant="accent" className="gap-2">
                  <Search className="h-4 w-4" />
                  Browse Auctions
                </Button>
              )}
              {user?.role === 'admin' && (
                <Button variant="success" className="gap-2">
                  <Shield className="h-4 w-4" />
                  Admin Panel
                </Button>
              )}
            </div>
          </div>

          {/* Quick Stats */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <Card className="p-4 bg-card/50 backdrop-blur-sm border-0 shadow-card">
              <div className="flex items-center gap-3">
                <div className="p-2 rounded-lg bg-primary/20">
                  <Home className="h-5 w-5 text-primary" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Active Properties</p>
                  <p className="text-2xl font-bold text-foreground">127</p>
                </div>
              </div>
            </Card>
            
            <Card className="p-4 bg-card/50 backdrop-blur-sm border-0 shadow-card">
              <div className="flex items-center gap-3">
                <div className="p-2 rounded-lg bg-accent/20">
                  <Users className="h-5 w-5 text-accent" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Total Users</p>
                  <p className="text-2xl font-bold text-foreground">2,847</p>
                </div>
              </div>
            </Card>
            
            <Card className="p-4 bg-card/50 backdrop-blur-sm border-0 shadow-card">
              <div className="flex items-center gap-3">
                <div className="p-2 rounded-lg bg-success/20">
                  <TrendingUp className="h-5 w-5 text-success" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Revenue</p>
                  <p className="text-2xl font-bold text-foreground">$1.2M</p>
                </div>
              </div>
            </Card>
            
            <Card className="p-4 bg-card/50 backdrop-blur-sm border-0 shadow-card">
              <div className="flex items-center gap-3">
                <div className="p-2 rounded-lg bg-warning/20">
                  <Gavel className="h-5 w-5 text-warning" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Live Auctions</p>
                  <p className="text-2xl font-bold text-foreground">18</p>
                </div>
              </div>
            </Card>
          </div>
        </div>

        {/* Dashboard Content */}
        {children}
      </main>
    </div>
  );
};