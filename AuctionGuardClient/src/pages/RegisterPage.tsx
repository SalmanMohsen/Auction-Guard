import { useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import axios from "axios";
import { Button } from "../components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "../components/ui/card";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Badge } from "../components/ui/badge";
// import { UserRole } from "../types/auth";
// import { FileUpload } from "../components/ui/file-upload"; // Import the FileUpload component
import { Gavel, ArrowLeft, Users, TrendingUp, Check, Shield } from "lucide-react";
import { useAuth } from "../contexts/AuthContext";
import type { RegisterFormData } from "../types/auth";
import { useToast } from "../hooks/use-toast";

const RegisterPage = () => {
  const navigate = useNavigate();
  // const [searchParams] = useSearchParams();
  // const roleParam = searchParams.get('role') as 'Bidder' | 'Seller';
  const { register, isLoading } = useAuth();
  const { toast } = useToast();

  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    middleName:'',
    email: '',
    phoneNumber: '',
    password: '',
    confirmPassword: '',
    role: ['']
  });

  // State specifically for the image file
  // const [idImage, setIdImage] = useState<File | null>(null);
  // const [idImage, setIdImage] = useState("null");
  const [errors, setErrors] = useState<Partial<RegisterFormData>>({});

  const validateForm = (): boolean => {
    const newErrors: Partial<RegisterFormData> = {};
    if (!formData.firstName.trim()) newErrors.firstName = 'First name is required';
    if (!formData.lastName.trim()) newErrors.lastName = 'Last name is required';
    if (!formData.email.trim()) {
        newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
        newErrors.email = 'Email is invalid';
    }
    if (!formData.password) {
        newErrors.password = 'Password is required';
    } else if (formData.password.length < 8) {
        newErrors.password = 'Password must be at least 8 characters';
    }
    // if (formData.password !== formData.confirmPassword) {
    //   newErrors.confirmPassword = 'Passwords do not match';
    // }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleInputChange = (field: keyof Omit<RegisterFormData, 'role' | 'idImage'>, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };
  // --- MODIFIED: handleSubmit now sends JSON ---
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      // Pass the formData state directly as a JSON object.
      await register(formData);
      toast({
        title: "Registration Successful!",
        description: "Your account has been created. Please sign in.",
      });
      navigate('/login');
    } catch (error) {
        let errorMessage = "An unknown error occurred.";

        if (axios.isAxiosError(error)) {
            if (error.code === 'ECONNABORTED') {
                errorMessage = "The server took too long to respond.";
            } else if (error.response) {
                const serverMessage = error.response.data?.message || JSON.stringify(error.response.data);
                errorMessage = `Registration failed: ${serverMessage || error.response.statusText}`;
            } else {
                errorMessage = "A network error occurred.";
            }
        } else if (error instanceof Error) {
            errorMessage = error.message;
        }

        toast({
            title: "Registration Failed",
            description: errorMessage,
            variant: "destructive",
        });
    }
  };


  const roleInfo = {
    Bidder: { icon: Users, title: "Join as Bidder", description: "Start bidding on amazing properties", color: "bg-primary", benefits: ["Access to all property auctions", "Real-time bidding system", "Secure payment processing"] },
    Seller: { icon: TrendingUp, title: "Join as Seller", description: "List and sell your properties", color: "bg-accent", benefits: ["List unlimited properties", "Advanced marketing tools", "Professional support"] }
  };
  
  const RoleIcon = roleInfo.Bidder.icon;

  return (
    <div className="min-h-screen bg-gradient-hero flex flex-col">
      <header className="border-b border-border/50 backdrop-blur-sm bg-background/80">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <Button variant="ghost" onClick={() => navigate('/')} className="gap-2">
              <ArrowLeft className="h-4 w-4" />
              Back to Home
            </Button>
            <div className="flex items-center gap-3">
              <div className="p-2 rounded-lg bg-gradient-primary">
                <Gavel className="h-5 w-5 text-white" />
              </div>
              <span className="font-semibold text-foreground">Auction Guard</span>
            </div>
          </div>
        </div>
      </header>
      <div className="flex-1 container mx-auto px-4 py-8">
        <div className="max-w-4xl mx-auto">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <Card className="border-0 shadow-glow bg-card/50 backdrop-blur-sm">
              <CardHeader className="text-center">
                <div className={`mx-auto p-4 rounded-lg ${roleInfo.Bidder.color} w-fit mb-4`}>
                  <RoleIcon className="h-8 w-8 text-white" />
                </div>
                <CardTitle className="text-2xl">{roleInfo.Bidder.title}</CardTitle>
                <CardDescription className="text-lg">{roleInfo.Bidder.description}</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="space-y-3">
                  {roleInfo.Bidder.benefits.map((benefit, index) => (
                    <div key={index} className="flex items-center gap-3">
                      <div className="p-1 rounded-full bg-success/20">
                        <Check className="h-3 w-3 text-success" />
                      </div>
                      <span className="text-sm text-muted-foreground">{benefit}</span>
                    </div>
                  ))}
                </div>
                <div className="pt-4">
                  <Label className="text-sm font-medium">Choose your role:</Label>
                  <div className="flex gap-2 mt-2">
                    <Badge variant={formData.role![0] === 'Bidder' ? 'default' : 'outline'} className="cursor-pointer" onClick={() => setFormData(prev => ({ ...prev, role: ['Bidder'] }))}>Bidder</Badge>
                    <Badge variant={formData.role![0] === 'Seller' ? 'default' : 'outline'} className="cursor-pointer" onClick={() => setFormData(prev => ({ ...prev, role: ['Seller'] }))}>Seller</Badge>
                  </div>
                </div>
              </CardContent>
            </Card>
            <Card className="border-0 shadow-card bg-card/50 backdrop-blur-sm">
              <CardHeader>
                <CardTitle className="text-xl">Create Your Account</CardTitle>
                <CardDescription>Fill in your details to get started</CardDescription>
              </CardHeader>
              <CardContent>
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-2"><Label htmlFor="firstName">First Name *</Label><Input id="firstName" value={formData.firstName} onChange={(e) => handleInputChange('firstName', e.target.value)} className={errors.firstName ? 'border-destructive' : ''} required />{errors.firstName && <p className="text-xs text-destructive">{errors.firstName}</p>}</div>
                    <div className="space-y-2"><Label htmlFor="lastName">Last Name *</Label><Input id="lastName" value={formData.lastName} onChange={(e) => handleInputChange('lastName', e.target.value)} className={errors.lastName ? 'border-destructive' : ''} required />{errors.lastName && <p className="text-xs text-destructive">{errors.lastName}</p>}</div>
                    <div className="space-y-2"><Label htmlFor="middleName">Middle Name *</Label><Input id="lastName" value={formData.middleName} onChange={(e) => handleInputChange('middleName', e.target.value)} className={errors.middleName ? 'border-destructive' : ''} required />{errors.middleName && <p className="text-xs text-destructive">{errors.middleName}</p>}</div>
                  </div>
                  <div className="space-y-2"><Label htmlFor="email">Email *</Label><Input id="email" type="email" value={formData.email} onChange={(e) => handleInputChange('email', e.target.value)} className={errors.email ? 'border-destructive' : ''} required />{errors.email && <p className="text-xs text-destructive">{errors.email}</p>}</div>
                  <div className="space-y-2"><Label htmlFor="phoneNumber">Phone Number *</Label><Input id="phoneNumber" value={formData.phoneNumber} onChange={(e) => handleInputChange('phoneNumber', e.target.value)} className={errors.phoneNumber ? 'border-destructive' : ''} required />{errors.phoneNumber && <p className="text-xs text-destructive">{errors.phoneNumber}</p>}</div>
                  
                  
                  {/* ID Upload Component */}
                   {/* <div className="space-y-2">
                    <Label>ID Document *</Label>
                    <FileUpload
                      onFileSelect={setIdImage}
                      selectedFile={idImage}
                      placeholder="Upload ID Image"
                      required
                    /> 
                  </div> */}

                  <div className="space-y-2"><Label htmlFor="password">Password *</Label><Input id="password" type="password" value={formData.password} onChange={(e) => handleInputChange('password', e.target.value)} className={errors.password ? 'border-destructive' : ''} required />{errors.password && <p className="text-xs text-destructive">{errors.password}</p>}</div>
                  <div className="space-y-2"><Label htmlFor="confirmPassword">Confirm Password *</Label><Input id="confirmPassword" type="password" value={formData.confirmPassword} onChange={(e) => handleInputChange('confirmPassword', e.target.value)} className={errors.confirmPassword ? 'border-destructive' : ''} required />{errors.confirmPassword && <p className="text-xs text-destructive">{errors.confirmPassword}</p>}</div>
                  <Button type="submit" className="w-full" variant="neon" disabled={isLoading}>
                    {isLoading ? 'Creating Account...' : (<><Shield className="h-4 w-4" />Create Account</>)}
                  </Button>
                  <p className="text-center text-sm text-muted-foreground">Already have an account?{' '}<Button variant="link" onClick={() => navigate('/email')} className="p-0 h-auto">Sign in here</Button></p>
                </form>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;