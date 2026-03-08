import Link from "next/link";
import { ArrowUpRight, Sparkles } from "lucide-react";
import { FeatureBadges } from "@/components/listings/feature-badges";
import { ListingImage } from "@/components/listings/listing-image";
import type { ListingResponse } from "@/types/listings";

interface ListingCardProps {
  listing: ListingResponse;
}

function formatCurrency(value: number | null): string {
  if (value === null) {
    return "Nog niet voorspeld";
  }

  return new Intl.NumberFormat("nl-NL", {
    style: "currency",
    currency: "EUR",
    maximumFractionDigits: 0,
  }).format(value);
}

function formatCategory(categoryKey: string): string {
  return categoryKey
    .replaceAll("-", " ")
    .replaceAll("_", " ")
    .replace(/\b\w/g, (char) => char.toUpperCase());
}

export function ListingCard({
  listing,
}: ListingCardProps): React.JSX.Element {
  return (
    <article className="group overflow-hidden rounded-[2rem] border border-emerald-950/8 bg-white/90 shadow-[0_18px_60px_rgba(16,24,40,0.08)] transition hover:-translate-y-1 hover:shadow-[0_24px_80px_rgba(16,24,40,0.12)]">
      <ListingImage listing={listing} />

      <div className="p-6">
        <div className="flex items-start justify-between gap-4">
          <div>
            <p className="text-sm font-medium text-emerald-900/50">
              {formatCategory(listing.categoryKey)}
            </p>
            <h2 className="mt-2 line-clamp-2 text-xl font-black tracking-tight text-emerald-950">
              {listing.title}
            </h2>
          </div>

          <span className="rounded-full bg-emerald-100 px-3 py-1 text-xs font-bold text-emerald-900">
            {listing.status}
          </span>
        </div>

        <p className="mt-4 line-clamp-3 text-sm leading-7 text-emerald-950/65">
          {listing.description}
        </p>

        <div className="mt-4">
          <FeatureBadges featuresJson={listing.featuresJson} maxItems={3} />
        </div>

        <div className="mt-6 grid grid-cols-2 gap-3">
          <div className="rounded-[1.4rem] bg-emerald-50 p-4">
            <p className="text-xs font-bold uppercase tracking-[0.18em] text-emerald-900/45">
              Vraagprijs
            </p>
            <p className="mt-2 text-xl font-black text-emerald-950">
              {formatCurrency(listing.askingPrice)}
            </p>
          </div>

          <div className="rounded-[1.4rem] bg-lime-50 p-4">
            <div className="flex items-center gap-2 text-lime-900/60">
              <Sparkles className="h-4 w-4" />
              <p className="text-xs font-bold uppercase tracking-[0.18em]">
                AI prijs
              </p>
            </div>
            <p className="mt-2 text-xl font-black text-emerald-950">
              {formatCurrency(listing.predictedPrice)}
            </p>
          </div>
        </div>

        <div className="mt-6 flex items-center justify-between">
          <p className="text-xs text-emerald-950/45">
            Geplaatst op{" "}
            {new Intl.DateTimeFormat("nl-NL", {
              dateStyle: "medium",
            }).format(new Date(listing.createdAtUtc))}
          </p>

          <Link
            href={`/browse/${listing.id}`}
            className="inline-flex items-center gap-2 rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-800"
          >
            Bekijken
            <ArrowUpRight className="h-4 w-4" />
          </Link>
        </div>
      </div>
    </article>
  );
}