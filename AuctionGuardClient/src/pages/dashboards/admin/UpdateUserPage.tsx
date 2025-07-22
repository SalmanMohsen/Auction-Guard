import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { DashboardLayout } from "../../../components/DashboardLayout";
import { Card, CardContent, CardHeader, CardTitle } from "../../../components/ui/card";
import { Input } from "../../../components/ui/input";
import { Label } from "../../../components/ui/label";
import { Button } from "../../../components/ui/button";
import { getUser, updateUser } from "../../../api/userApi";
import { useToast } from "../../../hooks/use-toast";

const UpdateUserPage = () => {
    const { userId } = useParams<{ userId: string }>();
    const navigate = useNavigate();
    const { toast } = useToast();
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        phoneNumber: '',
        address: ''
    });

    useEffect(() => {
        if (userId) {
            getUser(userId).then(response => {
                setFormData({
                    firstName: response.data.firstName,
                    lastName: response.data.lastName,
                    phoneNumber: response.data.phoneNumber,
                    address: response.data.address
                });
            });
        }
    }, [userId]);

    const handleUpdate = async (e: React.FormEvent) => {
        e.preventDefault();
        if (userId) {
            try {
                await updateUser(userId, formData);
                toast({
                    title: "Success",
                    description: "User updated successfully.",
                });
                navigate('/admin/users');
            } catch (error) {
                toast({
                    title: "Error",
                    description: "Failed to update user.",
                    variant: "destructive",
                });
            }
        }
    };

    return (
        <DashboardLayout title="Update User" description="Update user details">
            <Card>
                <CardHeader>
                    <CardTitle>Update User</CardTitle>
                </CardHeader>
                <CardContent>
                    <form onSubmit={handleUpdate} className="space-y-4">
                        <div className="space-y-2">
                            <Label htmlFor="firstName">First Name</Label>
                            <Input id="firstName" value={formData.firstName} onChange={(e) => setFormData({ ...formData, firstName: e.target.value })} />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="lastName">Last Name</Label>
                            <Input id="lastName" value={formData.lastName} onChange={(e) => setFormData({ ...formData, lastName: e.target.value })} />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="phoneNumber">Phone Number</Label>
                            <Input id="phoneNumber" value={formData.phoneNumber} onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })} />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="address">Address</Label>
                            <Input id="address" value={formData.address} onChange={(e) => setFormData({ ...formData, address: e.target.value })} />
                        </div>
                        <Button type="submit">Update User</Button>
                    </form>
                </CardContent>
            </Card>
        </DashboardLayout>
    );
};

export default UpdateUserPage;