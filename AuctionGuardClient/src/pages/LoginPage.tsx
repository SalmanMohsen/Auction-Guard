import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "../components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "../components/ui/card";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Gavel, ArrowLeft, Eye, EyeOff } from "lucide-react";
import { useAuth } from "../contexts/AuthContext";
import type { LoginFormData } from "../types/auth";
import { useToast } from "../hooks/use-toast";
import { Checkbox } from "../components/ui/checkbox";

const LoginPage = () => {
  const navigate = useNavigate();
  const { login, isLoading } = useAuth();
  const { toast } = useToast();

  const [formData, setFormData] = useState<LoginFormData>({
    login: '',
    password: '',
    rememberMe: false
  });

  const [showPassword, setShowPassword] = useState(false);
  const [errors, setErrors] = useState<Partial<LoginFormData>>({});


  const handleInputChange = (field: keyof LoginFormData, value: string | boolean) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field as keyof LoginFormData]) {
      setErrors(prev => ({ ...prev, [field as keyof LoginFormData]: undefined }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await login(formData);
      navigate('/dashboard');
      toast({
        title: "Welcome back!",
        description: "You have been successfully logged in.",
      });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : "An unknown error occurred.";
      toast({
        title: "Login Failed",
        description: errorMessage,
        variant: "destructive",
      });
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 to-blue-900 flex flex-col items-center justify-center p-4">
      <header className="fixed top-0 left-0 w-full z-50 border-b border-gray-700/50 backdrop-blur-sm bg-gray-900/70">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <Button
              variant="ghost"
              onClick={() => navigate('/')}
              className="gap-2 text-gray-300 hover:bg-gray-800 hover:text-gray-100"
            >
              <ArrowLeft className="h-4 w-4" />
              Back to Home
            </Button>
            
            <div className="flex items-center gap-3">
              <div className="p-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600">
                <Gavel className="h-5 w-5 text-white" />
              </div>
              <span className="font-semibold text-white">Auction Guard</span>
            </div>
          </div>
        </div>
      </header>

      <Card className="w-full max-w-md bg-gray-800/50 backdrop-blur-sm border-blue-500/30 shadow-lg shadow-blue-500/20">
        <CardHeader className="text-center">
          <div className="flex justify-center items-center mb-4">
              <Gavel className="h-8 w-8 text-blue-400" />
          </div>
          <CardTitle className="text-2xl text-white">Welcome Back</CardTitle>
          <CardDescription className="text-gray-400">
            Sign in to access your Auction Guard account.
          </CardDescription>
        </CardHeader>
        
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="login" className="text-gray-300">Email Address</Label>
              <Input
                id="login"
                type="email"
                value={formData.login}
                onChange={(e) => handleInputChange('login', e.target.value)}
                className={`bg-gray-900/50 border-gray-700 text-white focus:ring-blue-500 focus:border-blue-500 ${errors.login ? 'border-red-500' : ''}`}
                placeholder="you@example.com"
                required
              />
              {errors.login && (
                <p className="text-xs text-red-400">{errors.login}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="password" className="text-gray-300">Password</Label>
              <div className="relative">
                <Input
                  id="password"
                  type={showPassword ? 'text' : 'password'}
                  value={formData.password}
                  onChange={(e) => handleInputChange('password', e.target.value)}
                  className={`bg-gray-900/50 border-gray-700 text-white focus:ring-blue-500 focus:border-blue-500 pr-10 ${errors.password ? 'border-red-500' : ''}`}
                  placeholder="••••••••"
                  required
                />
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  className="absolute right-0 top-0 h-full px-3 py-2 hover:bg-transparent"
                  onClick={() => setShowPassword(!showPassword)}
                  aria-label={showPassword ? "Hide password" : "Show password"}
                >
                  {showPassword ? (
                    <EyeOff className="h-4 w-4 text-gray-400" />
                  ) : (
                    <Eye className="h-4 w-4 text-gray-400" />
                  )}
                </Button>
              </div>
              {errors.password && (
                <p className="text-xs text-red-400">{errors.password}</p>
              )}
            </div>

            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-2">
                <Checkbox id="remember-me" checked={formData.rememberMe} onCheckedChange={(checked) => handleInputChange('rememberMe', checked as boolean)} />
                <Label htmlFor="remember-me" className="text-sm text-gray-400">Remember me</Label>
              </div>
              <Button variant="link" onClick={() => navigate('/forgot-password')} className="text-blue-400 hover:text-blue-300 p-0 h-auto">
                Forgot your password?
              </Button>
            </div>

            <Button 
              type="submit" 
              className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold"
              disabled={isLoading}
            >
              {isLoading ? 'Signing In...' : 'Sign In'}
            </Button>

            <div className="text-center">
              <p className="text-sm text-gray-400">
                Don't have an account?{' '}
                <Button variant="link" onClick={() => navigate('/register')} className="text-blue-400 hover:text-blue-300 p-0 h-auto">
                  Create one
                </Button>
              </p>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
};

export default LoginPage;