import { useState, useEffect } from "react";
import { Button } from "../components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "../components/ui/card";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { useAuth } from "../contexts/AuthContext";
import { getUser, updateUser, deleteUser } from "../api/userApi";
import { useToast } from "../hooks/use-toast";
import { useNavigate } from "react-router-dom";

const ProfilePage = () => {
    const { user, logout } = useAuth();
    const { toast } = useToast();
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        phoneNumber: '',
        address: ''
    });

    useEffect(() => {
        if (user) {
            getUser(user.id).then(response => {
                setFormData({
                    firstName: response.data.firstName,
                    lastName: response.data.lastName,
                    phoneNumber: response.data.phoneNumber,
                    address: response.data.address
                });
            });
        }
    }, [user]);

    const handleUpdate = async (e: React.FormEvent) => {
        e.preventDefault();
        if (user) {
            try {
                await updateUser(user.id, formData);
                toast({
                    title: "Success",
                    description: "Profile updated successfully.",
                });
            } catch (error) {
                toast({
                    title: "Error",
                    description: "Failed to update profile.",
                    variant: "destructive",
                });
            }
        }
    };

    const handleDelete = async () => {
        if (user && window.confirm("Are you sure you want to delete your account? This action cannot be undone.")) {
            try {
                await deleteUser(user.id);
                logout();
                toast({
                    title: "Account Deleted",
                    description: "Your account has been successfully deleted.",
                });
                navigate('/');
            } catch (error) {
                toast({
                    title: "Error",
                    description: "Failed to delete account.",
                    variant: "destructive",
                });
            }
        }
    };

    return (
        <div className="min-h-screen p-8">
            <Card>
                <CardHeader>
                    <CardTitle>Your Profile</CardTitle>
                </CardHeader>
                <CardContent>
                    <form onSubmit={handleUpdate} className="space-y-4">
                        {/* Form fields for firstName, lastName, phoneNumber, address */}
                        <Button type="submit">Update Profile</Button>
                    </form>
                    <Button variant="destructive" onClick={handleDelete} className="mt-4">Delete Account</Button>
                </CardContent>
            </Card>
        </div>
    );
};

export default ProfilePage;