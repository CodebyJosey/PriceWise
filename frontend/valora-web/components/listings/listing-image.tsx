import Image from "next/image";
import { getPrimaryImageUrl } from "@/lib/listings/listing-images";
import { ListingImagePlaceholder } from "@/components/listings/listing-image-placeholder";
import type { ListingResponse } from "@/types/listings";

interface ListingImageProps {
  listing: ListingResponse;
  alt?: string;
  heightClassName?: string;
  priority?: boolean;
}

export function ListingImage({
  listing,
  alt,
  heightClassName = "h-44",
  priority = false,
}: ListingImageProps): React.JSX.Element {
  const imageUrl = getPrimaryImageUrl(listing);

  if (!imageUrl) {
    return (
      <ListingImagePlaceholder
        categoryKey={listing.categoryKey}
        title={listing.title}
        heightClassName={heightClassName}
      />
    );
  }

  return (
    <div className={`relative overflow-hidden ${heightClassName}`}>
      <Image
        src={imageUrl}
        alt={alt ?? listing.title}
        fill
        priority={priority}
        className="object-cover"
        sizes="(max-width: 768px) 100vw, (max-width: 1280px) 50vw, 33vw"
      />
    </div>
  );
}