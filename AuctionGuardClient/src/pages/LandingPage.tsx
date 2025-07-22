import { Button } from "../components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import { Gavel, Users, Shield, TrendingUp, Eye, UserPlus, LogIn, User, LogOut } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

const LandingPage = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user, logout } = useAuth();

  const handleLogout = async () => {
    await logout();
    // After logout, the user will be redirected to the landing page automatically
    // by the router's logic, so no navigate() call is needed here.
  };

  const features = [
    {
      icon: Gavel,
      title: "Real-Time Auctions",
      description: "Participate in live property auctions with instant bidding"
    },
    {
      icon: Shield,
      title: "Secure Transactions",
      description: "Cryptocurrency and traditional payment methods supported"
    },
    {
      icon: Users,
      title: "Professional Platform",
      description: "Connect with verified buyers, sellers, and administrators"
    },
    {
      icon: TrendingUp,
      title: "Market Analytics",
      description: "Access comprehensive property insights and market trends"
    }
  ];

  return (
    <div className="min-h-screen bg-gradient-hero">
      {/* Header */}
      <header className="border-b border-border/50 backdrop-blur-sm bg-background/80 sticky top-0 z-50">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3 cursor-pointer" onClick={() => navigate('/')}>
              <div className="p-2 rounded-lg bg-gradient-primary">
                <Gavel className="h-6 w-6 text-white" />
              </div>
              <div>
                <h1 className="text-xl font-bold text-foreground">Auction Guard</h1>
                <p className="text-xs text-muted-foreground">Real Estate Auctions</p>
              </div>
            </div>
            
            <div className="flex items-center gap-3">
              {isAuthenticated && user ? (
                // --- Authenticated User View ---
                <>
                  <Button 
                    variant="ghost" 
                    onClick={() => navigate('/dashboard')} // This can be changed to a /profile route later
                    className="gap-2"
                  >
                    <User className="h-4 w-4" />
                    {user.firstName}
                  </Button>
                  <Button
                    variant="outline"
                    onClick={handleLogout}
                    className="gap-2"
                  >
                    <LogOut className="h-4 w-4" />
                    Logout
                  </Button>
                </>
              ) : (
                // --- Guest View ---
                <>
                  <Button 
                    variant="ghost" 
                    onClick={() => navigate('/login')}
                    className="gap-2"
                  >
                    <LogIn className="h-4 w-4" />
                    Login
                  </Button>
                  <Button 
                    variant="neon" 
                    onClick={() => navigate('/register')}
                    className="gap-2"
                  >
                    <UserPlus className="h-4 w-4" />
                    Get Started
                  </Button>
                </>
              )}
            </div>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <section className="container mx-auto px-4 py-20">
        <div className="text-center max-w-4xl mx-auto">
          <Badge variant="secondary" className="mb-6 text-sm font-medium">
            <Eye className="h-3 w-3 mr-1" />
            Trusted by 10,000+ Users
          </Badge>
          
          <h1 className="text-4xl md:text-6xl font-bold text-foreground mb-6 leading-tight">
            The Future of{" "}
            <span className="text-transparent bg-gradient-primary bg-clip-text">
              Real Estate Auctions
            </span>
          </h1>
          
          <p className="text-xl text-muted-foreground mb-12 max-w-2xl mx-auto leading-relaxed">
            Join the most secure and innovative real estate auction platform. 
            Whether you're buying, selling, or managing properties, we provide 
            the tools you need to succeed.
          </p>

          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button 
              size="lg" 
              variant="neon" 
              onClick={() => navigate('/register?role=bidder')}
              className="text-lg px-8 py-4 h-auto gap-3"
            >
              <Users className="h-5 w-5" />
              Join as Bidder
            </Button>
            <Button 
              size="lg" 
              variant="accent" 
              onClick={() => navigate('/register?role=seller')}
              className="text-lg px-8 py-4 h-auto gap-3"
            >
              <TrendingUp className="h-5 w-5" />
              Sell Properties
            </Button>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="container mx-auto px-4 py-20">
        <div className="text-center mb-16">
          <h2 className="text-3xl md:text-4xl font-bold text-foreground mb-4">
            Why Choose Auction Guard?
          </h2>
          <p className="text-lg text-muted-foreground max-w-2xl mx-auto">
            Experience the next generation of real estate auctions with our 
            cutting-edge platform designed for professionals.
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {features.map((feature, index) => (
            <Card key={index} className="border-0 shadow-card hover:shadow-glow transition-all duration-300 bg-card/50 backdrop-blur-sm">
              <CardHeader className="text-center pb-4">
                <div className="mx-auto p-3 rounded-lg bg-gradient-primary w-fit mb-4">
                  <feature.icon className="h-6 w-6 text-white" />
                </div>
                <CardTitle className="text-lg">{feature.title}</CardTitle>
              </CardHeader>
              <CardContent className="pt-0">
                <CardDescription className="text-center">
                  {feature.description}
                </CardDescription>
              </CardContent>
            </Card>
          ))}
        </div>
      </section>

      {/* CTA Section */}
      <section className="container mx-auto px-4 py-20">
        <Card className="border-0 shadow-glow bg-gradient-primary text-white">
          <CardContent className="text-center py-16">
            <h2 className="text-3xl md:text-4xl font-bold mb-4">
              Ready to Get Started?
            </h2>
            <p className="text-lg mb-8 text-white/90 max-w-2xl mx-auto">
              Join thousands of professionals who trust Auction Guard for their 
              real estate auction needs.
            </p>
            
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button 
                size="lg" 
                variant="secondary"
                onClick={() => navigate('/register?role=bidder')}
                className="text-lg px-8 py-4 h-auto"
              >
                Start Bidding Today
              </Button>
              <Button 
                size="lg" 
                variant="outline"
                onClick={() => navigate('/register?role=seller')}
                className="text-lg px-8 py-4 h-auto bg-white/10 border-white/20 text-white hover:bg-white/20"
              >
                List Your Property
              </Button>
            </div>
          </CardContent>
        </Card>
      </section>

      {/* Footer */}
      <footer className="border-t border-border/50 bg-muted/30 backdrop-blur-sm">
        <div className="container mx-auto px-4 py-8">
          <div className="text-center text-muted-foreground">
            <p>&copy; {new Date().getFullYear()} Auction Guard. All rights reserved.</p>
            <p className="text-sm mt-2">Secure • Professional • Trusted</p>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default LandingPage;