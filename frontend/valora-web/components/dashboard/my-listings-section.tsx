"use client";

import Link from "next/link";
import { useMemo } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  Pencil,
  PlusCircle,
  RefreshCcw,
  Rocket,
  Trash2,
  EyeOff,
} from "lucide-react";
import { DashboardHeader } from "@/components/dashboard/dashboard-header";
import { FeatureBadges } from "@/components/listings/feature-badges";
import { ListingStatusBadge } from "@/components/listings/listing-status-badge";
import { getAccessToken } from "@/lib/auth/token-storage";
import { listingsApi } from "@/lib/api/listings-api";
import { formatCategoryLabel } from "@/lib/listings/category-feature-config";

export function MyListingsSection(): React.JSX.Element {
  const token = getAccessToken();
  const queryClient = useQueryClient();

  const listingsQuery = useQuery({
    queryKey: ["my-listings"],
    queryFn: ({ signal }) => listingsApi.getMine(token as string, signal),
    enabled: Boolean(token),
  });

  const publishMutation = useMutation({
    mutationFn: (id: string) => listingsApi.publish(id, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["published-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["listing-detail"] });
    },
  });

  const unpublishMutation = useMutation({
    mutationFn: (id: string) => listingsApi.unpublish(id, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["published-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["listing-detail"] });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => listingsApi.delete(id, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["published-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["listing-detail"] });
    },
  });

  const counts = useMemo(() => {
    const listings = listingsQuery.data ?? [];

    return {
      total: listings.length,
      draft: listings.filter((listing) => listing.status.toLowerCase() === "draft").length,
      published: listings.filter((listing) => listing.status.toLowerCase() === "published")
        .length,
    };
  }, [listingsQuery.data]);

  return (
    <div className="space-y-6">
      <DashboardHeader
        eyebrow="Seller"
        title="Mijn listings"
        description="Beheer hier je eigen listings. Je kunt drafts publiceren, gepubliceerde listings terugtrekken en listings verwijderen."
      />

      <section className="grid gap-4 md:grid-cols-3">
        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Totaal</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">{counts.total}</p>
        </div>
        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Drafts</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">{counts.draft}</p>
        </div>
        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Published</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">{counts.published}</p>
        </div>
      </section>

      <section className="soft-panel rounded-[2rem] p-6">
        <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
          <div>
            <h2 className="text-2xl font-black text-emerald-950">Overzicht</h2>
            <p className="mt-2 text-sm leading-7 text-emerald-950/62">
              Live gekoppeld aan `GET /api/listings/mine`.
            </p>
          </div>

          <Link
            href="/dashboard/listings/new"
            className="inline-flex items-center gap-2 rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-800"
          >
            <PlusCircle className="h-4 w-4" />
            Nieuwe listing
          </Link>
        </div>

        {listingsQuery.isLoading ? (
          <div className="mt-6 space-y-4">
            {Array.from({ length: 3 }).map((_, index) => (
              <div
                key={index}
                className="rounded-[1.5rem] bg-white p-5 shadow-sm"
              >
                <div className="h-6 w-48 animate-pulse rounded-full bg-emerald-100" />
                <div className="mt-4 h-4 w-64 animate-pulse rounded-full bg-emerald-100" />
                <div className="mt-6 h-20 animate-pulse rounded-[1.25rem] bg-emerald-50" />
              </div>
            ))}
          </div>
        ) : null}

        {listingsQuery.isError ? (
          <div className="mt-6 rounded-[1.5rem] bg-white p-6 shadow-sm">
            <h3 className="text-lg font-black text-emerald-950">
              Jouw listings konden niet worden geladen
            </h3>
            <p className="mt-2 text-sm leading-7 text-emerald-950/62">
              Controleer of je bent ingelogd en of de backend bereikbaar is.
            </p>
            <button
              type="button"
              onClick={() => void listingsQuery.refetch()}
              className="mt-4 inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 transition hover:bg-emerald-50"
            >
              <RefreshCcw className="h-4 w-4" />
              Opnieuw laden
            </button>
          </div>
        ) : null}

        {!listingsQuery.isLoading &&
        !listingsQuery.isError &&
        (listingsQuery.data?.length ?? 0) === 0 ? (
          <div className="mt-6 rounded-[1.5rem] bg-white p-6 shadow-sm">
            <h3 className="text-lg font-black text-emerald-950">
              Je hebt nog geen listings
            </h3>
            <p className="mt-2 text-sm leading-7 text-emerald-950/62">
              Maak je eerste listing aan om je seller flow te starten.
            </p>
            <Link
              href="/dashboard/listings/new"
              className="mt-4 inline-flex items-center gap-2 rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-800"
            >
              <PlusCircle className="h-4 w-4" />
              Eerste listing maken
            </Link>
          </div>
        ) : null}

        {!listingsQuery.isLoading &&
        !listingsQuery.isError &&
        (listingsQuery.data?.length ?? 0) > 0 ? (
          <div className="mt-6 space-y-4">
            {listingsQuery.data?.map((listing) => (
              <article
                key={listing.id}
                className="rounded-[1.6rem] bg-white p-6 shadow-sm"
              >
                <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
                  <div className="min-w-0">
                    <div className="flex flex-wrap items-center gap-3">
                      <p className="text-sm font-semibold text-emerald-900/50">
                        {formatCategoryLabel(listing.categoryKey)}
                      </p>
                      <ListingStatusBadge status={listing.status} />
                    </div>

                    <h3 className="mt-2 text-2xl font-black tracking-tight text-emerald-950">
                      {listing.title}
                    </h3>

                    <p className="mt-3 max-w-3xl text-sm leading-7 text-emerald-950/62">
                      {listing.description}
                    </p>

                    <div className="mt-4">
                      <FeatureBadges featuresJson={listing.featuresJson} maxItems={4} />
                    </div>
                  </div>

                  <div className="grid min-w-[240px] gap-3 sm:grid-cols-2 lg:grid-cols-1">
                    <div className="rounded-[1.25rem] bg-emerald-50 p-4">
                      <p className="text-xs font-bold uppercase tracking-[0.18em] text-emerald-900/45">
                        Vraagprijs
                      </p>
                      <p className="mt-2 text-xl font-black text-emerald-950">
                        {new Intl.NumberFormat("nl-NL", {
                          style: "currency",
                          currency: "EUR",
                          maximumFractionDigits: 0,
                        }).format(listing.askingPrice)}
                      </p>
                    </div>

                    <div className="rounded-[1.25rem] bg-lime-50 p-4">
                      <p className="text-xs font-bold uppercase tracking-[0.18em] text-lime-900/45">
                        AI prijs
                      </p>
                      <p className="mt-2 text-xl font-black text-emerald-950">
                        {listing.predictedPrice === null
                          ? "Nog niet voorspeld"
                          : new Intl.NumberFormat("nl-NL", {
                              style: "currency",
                              currency: "EUR",
                              maximumFractionDigits: 0,
                            }).format(listing.predictedPrice)}
                      </p>
                    </div>
                  </div>
                </div>

                <div className="mt-6 flex flex-wrap gap-3">
                  <Link
                    href={`/dashboard/listings/${listing.id}/edit`}
                    className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 transition hover:bg-emerald-50"
                  >
                    <Pencil className="h-4 w-4" />
                    Bewerken
                  </Link>

                  {listing.status.toLowerCase() !== "published" ? (
                    <button
                      type="button"
                      onClick={() => publishMutation.mutate(listing.id)}
                      disabled={publishMutation.isPending}
                      className="inline-flex items-center gap-2 rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-800 disabled:opacity-70"
                    >
                      <Rocket className="h-4 w-4" />
                      Publiceren
                    </button>
                  ) : (
                    <button
                      type="button"
                      onClick={() => unpublishMutation.mutate(listing.id)}
                      disabled={unpublishMutation.isPending}
                      className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 transition hover:bg-emerald-50 disabled:opacity-70"
                    >
                      <EyeOff className="h-4 w-4" />
                      Unpublish
                    </button>
                  )}

                  <button
                    type="button"
                    onClick={() => {
                      const confirmed = window.confirm(
                        "Weet je zeker dat je deze listing wilt verwijderen?",
                      );

                      if (confirmed) {
                        deleteMutation.mutate(listing.id);
                      }
                    }}
                    disabled={deleteMutation.isPending}
                    className="inline-flex items-center gap-2 rounded-full border border-red-200 bg-white px-4 py-2 text-sm font-semibold text-red-700 transition hover:bg-red-50 disabled:opacity-70"
                  >
                    <Trash2 className="h-4 w-4" />
                    Verwijderen
                  </button>
                </div>
              </article>
            ))}
          </div>
        ) : null}
      </section>
    </div>
  );
}