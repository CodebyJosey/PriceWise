"use client";

import { useMemo, useState } from "react";
import Image from "next/image";
import { getListingImages, getPrimaryImageUrl } from "@/lib/listings/listing-images";
import { ListingImagePlaceholder } from "@/components/listings/listing-image-placeholder";
import type { ListingResponse } from "@/types/listings";

interface ListingImageGalleryProps {
  listing: ListingResponse;
}

export function ListingImageGallery({
  listing,
}: ListingImageGalleryProps): React.JSX.Element {
  const images = useMemo(() => getListingImages(listing), [listing]);
  const fallbackPrimary = getPrimaryImageUrl(listing);
  const [selectedUrl, setSelectedUrl] = useState<string | null>(fallbackPrimary);

  if (!fallbackPrimary) {
    return (
      <div className="overflow-hidden rounded-[2.2rem] border border-emerald-950/8 bg-white/88 shadow-[0_20px_70px_rgba(16,24,40,0.08)]">
        <ListingImagePlaceholder
          categoryKey={listing.categoryKey}
          title={listing.title}
          heightClassName="h-72 lg:h-[420px]"
        />
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-[2.2rem] border border-emerald-950/8 bg-white/88 shadow-[0_20px_70px_rgba(16,24,40,0.08)]">
      <div className="relative h-72 lg:h-[420px]">
        <Image
          src={selectedUrl ?? fallbackPrimary}
          alt={listing.title}
          fill
          priority
          className="object-cover"
          sizes="(max-width: 1024px) 100vw, 70vw"
        />
      </div>

      {images.length > 1 ? (
        <div className="flex gap-3 overflow-x-auto p-4">
          {images.map((image) => {
            const isActive = (selectedUrl ?? fallbackPrimary) === image.imageUrl;

            return (
              <button
                key={image.id}
                type="button"
                onClick={() => setSelectedUrl(image.imageUrl)}
                className={`relative h-20 w-20 shrink-0 overflow-hidden rounded-[1rem] border-2 transition ${
                  isActive ? "border-emerald-500" : "border-transparent"
                }`}
              >
                <Image
                  src={image.imageUrl}
                  alt={image.fileName ?? listing.title}
                  fill
                  className="object-cover"
                  sizes="80px"
                />
              </button>
            );
          })}
        </div>
      ) : null}
    </div>
  );
}