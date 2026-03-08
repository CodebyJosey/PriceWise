import type { ListingImageResponse, ListingResponse } from "@/types/listings";

export function getListingImages(listing: ListingResponse): ListingImageResponse[] {
  const images = listing.images ?? [];
  return [...images].sort((left, right) => left.sortOrder - right.sortOrder);
}

export function getPrimaryImageUrl(listing: ListingResponse): string | null {
  if (listing.primaryImageUrl && listing.primaryImageUrl.trim().length > 0) {
    return listing.primaryImageUrl;
  }

  const images = getListingImages(listing);
  const primary = images.find((image) => image.isPrimary);

  if (primary?.imageUrl) {
    return primary.imageUrl;
  }

  if (images[0]?.imageUrl) {
    return images[0].imageUrl;
  }

  return null;
}