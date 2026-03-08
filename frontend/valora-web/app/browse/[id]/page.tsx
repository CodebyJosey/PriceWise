"use client";

import Link from "next/link";
import { use } from "react";
import { useQuery } from "@tanstack/react-query";
import { ArrowLeft, CalendarDays, Sparkles, Tag } from "lucide-react";
import { FeatureBadges } from "@/components/listings/feature-badges";
import { ListingImageGallery } from "@/components/listings/listing-image-gallery";
import { Footer } from "@/components/layout/footer";
import { Navbar } from "@/components/layout/navbar";
import { parseFeaturesJson } from "@/lib/listings/feature-parser";
import { listingsApi } from "@/lib/api/listings-api";

interface ListingDetailPageProps {
  params: Promise<{
    id: string;
  }>;
}

function formatCurrency(value: number | null): string {
  if (value === null) {
    return "Niet beschikbaar";
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

export default function ListingDetailPage({
  params,
}: ListingDetailPageProps): React.JSX.Element {
  const resolvedParams = use(params);

  const query = useQuery({
    queryKey: ["listing-detail", resolvedParams.id],
    queryFn: ({ signal }) => listingsApi.getById(resolvedParams.id, signal),
  });

  const features = query.data ? parseFeaturesJson(query.data.featuresJson) : [];

  return (
    <>
      <Navbar />

      <main className="mx-auto w-full max-w-6xl px-6 py-10 lg:px-8">
        <Link
          href="/browse"
          className="inline-flex items-center gap-2 text-sm font-semibold text-emerald-950/65 transition hover:text-emerald-950"
        >
          <ArrowLeft className="h-4 w-4" />
          Terug naar browse
        </Link>

        {query.isLoading ? (
          <div className="soft-panel mt-6 rounded-[2rem] p-8">
            <div className="h-8 w-48 animate-pulse rounded-full bg-emerald-100" />
            <div className="mt-4 h-5 w-80 animate-pulse rounded-full bg-emerald-100" />
            <div className="mt-8 h-48 animate-pulse rounded-[1.6rem] bg-emerald-100" />
          </div>
        ) : null}

        {query.isError ? (
          <div className="soft-panel mt-6 rounded-[2rem] p-8">
            <h1 className="text-2xl font-black text-emerald-950">
              Listing kon niet worden geladen
            </h1>
            <p className="mt-3 text-sm leading-7 text-emerald-950/65">
              De listing bestaat mogelijk niet, of je backend is niet bereikbaar.
            </p>
          </div>
        ) : null}

        {query.data ? (
          <section className="mt-6 grid gap-6 lg:grid-cols-[1.25fr_0.75fr]">
            <div className="space-y-6">
              <ListingImageGallery listing={query.data} />

              <div className="overflow-hidden rounded-[2.2rem] border border-emerald-950/8 bg-white/88 shadow-[0_20px_70px_rgba(16,24,40,0.08)]">
                <div className="p-8">
                  <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
                    <div>
                      <p className="text-sm font-medium text-emerald-900/50">
                        {formatCategory(query.data.categoryKey)}
                      </p>
                      <h1 className="mt-2 text-4xl font-black tracking-tight text-emerald-950">
                        {query.data.title}
                      </h1>
                    </div>

                    <span className="rounded-full bg-emerald-100 px-4 py-2 text-sm font-bold text-emerald-900">
                      {query.data.status}
                    </span>
                  </div>

                  <p className="mt-6 max-w-3xl text-base leading-8 text-emerald-950/68">
                    {query.data.description}
                  </p>

                  <div className="mt-6">
                    <FeatureBadges featuresJson={query.data.featuresJson} maxItems={6} />
                  </div>

                  <div className="mt-8 grid gap-4 md:grid-cols-2">
                    <div className="rounded-[1.6rem] bg-emerald-50 p-5">
                      <div className="flex items-center gap-2 text-emerald-900/55">
                        <Tag className="h-4 w-4" />
                        <p className="text-xs font-bold uppercase tracking-[0.18em]">
                          Vraagprijs
                        </p>
                      </div>
                      <p className="mt-2 text-3xl font-black text-emerald-950">
                        {formatCurrency(query.data.askingPrice)}
                      </p>
                    </div>

                    <div className="rounded-[1.6rem] bg-lime-50 p-5">
                      <div className="flex items-center gap-2 text-lime-900/60">
                        <Sparkles className="h-4 w-4" />
                        <p className="text-xs font-bold uppercase tracking-[0.18em]">
                          Voorspelde prijs
                        </p>
                      </div>
                      <p className="mt-2 text-3xl font-black text-emerald-950">
                        {formatCurrency(query.data.predictedPrice)}
                      </p>
                    </div>
                  </div>

                  <div className="mt-8 rounded-[1.6rem] bg-emerald-50/70 p-5">
                    <div className="flex items-center gap-2 text-emerald-900/60">
                      <CalendarDays className="h-4 w-4" />
                      <p className="text-sm font-semibold">
                        Geplaatst op{" "}
                        {new Intl.DateTimeFormat("nl-NL", {
                          dateStyle: "full",
                        }).format(new Date(query.data.createdAtUtc))}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <aside className="space-y-6">
              <div className="soft-panel rounded-[2rem] p-6">
                <h2 className="text-xl font-black text-emerald-950">
                  Specificaties
                </h2>
                <p className="mt-2 text-sm leading-7 text-emerald-950/60">
                  Hier zie je de belangrijkste listing-features netjes uitgewerkt.
                </p>

                <div className="mt-6 space-y-3">
                  {features.length > 0 ? (
                    features.map((feature) => (
                      <div
                        key={feature.key}
                        className="flex items-start justify-between gap-4 rounded-[1.25rem] bg-white p-4 shadow-sm"
                      >
                        <span className="text-sm font-bold text-emerald-950">
                          {feature.label}
                        </span>
                        <span className="max-w-[60%] text-right text-sm text-emerald-950/65">
                          {feature.value}
                        </span>
                      </div>
                    ))
                  ) : (
                    <div className="rounded-[1.25rem] bg-white p-4 text-sm text-emerald-950/60 shadow-sm">
                      Geen leesbare features gevonden voor deze listing.
                    </div>
                  )}
                </div>
              </div>

              <div className="soft-panel rounded-[2rem] p-6">
                <h2 className="text-xl font-black text-emerald-950">
                  Afbeelding-architectuur klaar
                </h2>
                <p className="mt-3 text-sm leading-7 text-emerald-950/65">
                  Deze detailpagina ondersteunt nu al een primary image en thumbnail
                  gallery zodra je backend listing images teruggeeft.
                </p>
              </div>
            </aside>
          </section>
        ) : null}
      </main>

      <Footer />
    </>
  );
}