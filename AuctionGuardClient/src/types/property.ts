

/**
 * Represents a real estate property.
 * Based on the PropertyDto from the backend.
 */
export interface Property {
  id: string;
  title: string;
  description: string;
  address: string;
  priceInitial: number;
  ownerId: string;
  propertyStatus: 'Available' | 'UnderAuction' | 'Sold';
  approvalStatus: 'Approved' | 'UnderApproval';
  propertyType: 'Apartment' | 'Villa' | 'Land' | 'Commercial' | 'Other';
  constructedAt: string; 
  lastRenew?: string | null;
  imageUrls: string[];
}

/**
 * Defines the shape of the data for creating a new property.
 * The 'images' property will be a FileList from a form input.
 * Based on the CreatePropertyDto from the backend.
 */
export interface CreatePropertyData {
  title: string;
  description: string;
  address: string;
  priceInitial: number;
  constructedOn: string; 
  propertyType: 'Apartment' | 'Villa' | 'Land' | 'Commercial' | 'Other';
  Images: FileList;
}
/**
 * Defines the shape for updating a property.
 * Based on the UpdatePropertyDto from the backend.
 */
export interface UpdatePropertyData {
    title: string;
    description: string;
    address: string;
    priceInitial?: number;
 
}
