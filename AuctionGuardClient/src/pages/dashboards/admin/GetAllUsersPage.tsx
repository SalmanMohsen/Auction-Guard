import { useEffect, useState, useMemo } from "react";
import { DashboardLayout } from "../../../components/DashboardLayout";
import { Card, CardContent, CardHeader, CardTitle } from "../../../components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "../../../components/ui/table";
import { Button } from "../../../components/ui/button";
import { getAllUsers } from "../../../api/adminApi";
import type { User } from "../../../types/auth";
import { useNavigate } from "react-router-dom";
import {
    Pagination,
    PaginationContent,
    PaginationItem,
    PaginationLink,
    PaginationNext,
    PaginationPrevious,
} from "../../../components/ui/pagination";
import { Edit } from "lucide-react";

const USERS_PER_PAGE = 5;

const GetAllUsersPage = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const navigate = useNavigate();

    useEffect(() => {
        getAllUsers().then(response => {
            setUsers(response.data);
        }).catch(error => {
            console.error("Failed to fetch users:", error);
        });
    }, []);

    // Memoize the paginated users to avoid recalculating on every render
    const currentUsers = useMemo(() => {
        const indexOfLastUser = currentPage * USERS_PER_PAGE;
        const indexOfFirstUser = indexOfLastUser - USERS_PER_PAGE;
        return users.slice(indexOfFirstUser, indexOfLastUser);
    }, [users, currentPage]);

    const totalPages = Math.ceil(users.length / USERS_PER_PAGE);

    const handlePageChange = (page: number) => {
        // Ensure the page number is within the valid range
        if (page > 0 && page <= totalPages) {
            setCurrentPage(page);
        }
    };

    return (
        <DashboardLayout title="All Users" description="Manage all users in the system">
            <Card>
                <CardHeader>
                    <CardTitle>Users</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="rounded-md border">
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Name</TableHead>
                                    <TableHead>Email</TableHead>
                                    <TableHead>Roles</TableHead>
                                    <TableHead className="text-right">Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {currentUsers.length > 0 ? (
                                    currentUsers.map(user => (
                                        <TableRow key={user.id}>
                                            <TableCell className="font-medium">{user.firstName} {user.lastName}</TableCell>
                                            <TableCell>{user.login}</TableCell>
                                            <TableCell>{user.roles.join(', ')}</TableCell>
                                            <TableCell className="text-right">
                                                <Button variant="outline" size="sm" onClick={() => navigate(`/admin/user/update/${user.id}`)}>
                                                    <Edit className="h-4 w-4 mr-2" />
                                                    Update
                                                </Button>
                                            </TableCell>
                                        </TableRow>
                                    ))
                                ) : (
                                    <TableRow>
                                        <TableCell colSpan={4} className="text-center">
                                            No users found.
                                        </TableCell>
                                    </TableRow>
                                )}
                            </TableBody>
                        </Table>
                    </div>
                    {totalPages > 1 && (
                        <Pagination className="mt-4">
                            <PaginationContent>
                                <PaginationItem>
                                    <PaginationPrevious
                                        href="#"
                                        onClick={(e) => {
                                            e.preventDefault();
                                            handlePageChange(currentPage - 1);
                                        }}
                                        className={currentPage === 1 ? "pointer-events-none opacity-50" : ""}
                                    />
                                </PaginationItem>
                                {[...Array(totalPages).keys()].map(pageNumber => (
                                    <PaginationItem key={pageNumber + 1}>
                                        <PaginationLink
                                            href="#"
                                            onClick={(e) => {
                                                e.preventDefault();
                                                handlePageChange(pageNumber + 1);
                                            }}
                                            isActive={currentPage === pageNumber + 1}
                                        >
                                            {pageNumber + 1}
                                        </PaginationLink>
                                    </PaginationItem>
                                ))}
                                <PaginationItem>
                                    <PaginationNext
                                        href="#"
                                        onClick={(e) => {
                                            e.preventDefault();
                                            handlePageChange(currentPage + 1);
                                        }}
                                        className={currentPage === totalPages ? "pointer-events-none opacity-50" : ""}
                                    />
                                </PaginationItem>
                            </PaginationContent>
                        </Pagination>
                    )}
                </CardContent>
            </Card>
        </DashboardLayout>
    );
};

export default GetAllUsersPage;
