export interface ListingImageResponse {
  id: string;
  imageUrl: string;
  fileName?: string | null;
  contentType?: string | null;
  sizeBytes?: number | null;
  width?: number | null;
  height?: number | null;
  sortOrder: number;
  isPrimary: boolean;
  createdAtUtc?: string | null;
}

export interface ListingResponse {
  id: string;
  ownerUserId: string;
  title: string;
  description: string;
  categoryKey: string;
  askingPrice: number;
  predictedPrice: number | null;
  soldPrice: number | null;
  soldAtUtc: string | null;
  status: string;
  featuresJson: string;
  createdAtUtc: string;
  updatedAtUtc: string;

  // Image-ready, maar optioneel zolang backend dit nog niet terugstuurt.
  primaryImageUrl?: string | null;
  images?: ListingImageResponse[] | null;
}

export interface CreateListingRequest {
  title: string;
  description: string;
  categoryKey: string;
  askingPrice: number;
  featuresJson: string;
}

export interface UpdateListingRequest {
  title: string;
  description: string;
  askingPrice: number;
  featuresJson: string;
}