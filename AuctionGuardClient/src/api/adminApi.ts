import api from './api';

/**
 * Fetches all users (Admin only).
 * @returns A promise that resolves with a list of all users.
 */
export const getAllUsers = async () => {
  try {
    const response = await api.get('/User');
    console.log(response, "\n", response.data);
    return response.data;
  } catch (error) {
    console.error('Failed to fetch users:', error);
    throw error;
  }
};

/**
 * Fetches all properties, including those pending approval (Admin only).
 * @returns A promise that resolves with a list of properties.
 */
export const getAllProperties = async () => {
  try {
    // Use Promise.all to fetch both sets of data concurrently
    const [underApprovalResponse, approvedResponse] = await Promise.all([
      api.get('/Properties/admin/GetUnderApprovalProperties'), 
      api.get('/Properties/GetApprovedProperties')             
    ]);

    // Correctly combine the two arrays into one
    return [...underApprovalResponse.data, ...approvedResponse.data];

  } catch (error) {
    console.error('Failed to fetch properties:', error);
    throw error;
  }
};

/**
 * Approves a property (Admin only).
 * @param propertyId - The ID of the property to approve.
 * @returns A promise that resolves when the operation is complete.
 */
export const approveProperty = async (propertyId: string) => {
  try {
    // The backend expects a PUT request to the '/status' endpoint
    const response = await api.put(
      `/Properties/admin/${propertyId}/status`, 
      // It also requires a body specifying the new status
      { approvalStatus: 'Approved' } 
    );
    return response.data;
  } catch (error) {
    console.error(`Failed to approve property ${propertyId}:`, error);
    throw error;
  }
};

/**
 * Fetches all auctions with any status (Admin only).
 * @returns A promise that resolves with a list of all auctions.
 */
export const getAllAuctions = async () => {
  try {
    const response = await api.get('/admin/auctions');
    return response.data;
  } catch (error) {
    console.error('Failed to fetch all auctions:', error);
    throw error;
  }
};

/**
 * Cancels a scheduled auction (Admin or Seller).
 * @param auctionId - The ID of the auction to cancel.
 * @returns A promise that resolves when the operation is complete.
 */
export const cancelAuction = async (auctionId: string) => {
    try {
        const response = await api.patch(`/auctions/${auctionId}/cancel`);
        return response.data;
    } catch (error) {
        console.error(`Failed to cancel auction ${auctionId}:`, error);
        throw error;
    }
}