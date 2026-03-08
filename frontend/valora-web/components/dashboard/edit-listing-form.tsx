"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  ArrowLeft,
  EyeOff,
  LoaderCircle,
  Rocket,
  Save,
  Sparkles,
  Trash2,
} from "lucide-react";
import { DashboardHeader } from "@/components/dashboard/dashboard-header";
import { ListingImageUploaderShell } from "@/components/dashboard/listing-image-uploader-shell";
import { ListingStatusBadge } from "@/components/listings/listing-status-badge";
import { listingsApi } from "@/lib/api/listings-api";
import { getAccessToken } from "@/lib/auth/token-storage";
import {
  getCategoryConfig,
  listingCategoryConfigs,
  type ListingFeatureField,
} from "@/lib/listings/category-feature-config";
import { parseFeatureObject } from "@/lib/listings/feature-parser";
import type { ListingResponse, UpdateListingRequest } from "@/types/listings";

type FeatureFormValue = string | boolean;

interface EditListingFormProps {
  listingId: string;
}

function toFeatureJsonValue(
  field: ListingFeatureField,
  value: FeatureFormValue,
): unknown {
  if (field.type === "checkbox") {
    return Boolean(value);
  }

  if (field.type === "number") {
    if (value === "") {
      return null;
    }

    const parsed = Number(value);
    return Number.isNaN(parsed) ? null : parsed;
  }

  return String(value).trim();
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

export function EditListingForm({
  listingId,
}: EditListingFormProps): React.JSX.Element {
  const router = useRouter();
  const queryClient = useQueryClient();
  const token = getAccessToken();

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [categoryKey, setCategoryKey] = useState("");
  const [askingPrice, setAskingPrice] = useState("");
  const [featureValues, setFeatureValues] = useState<
    Record<string, FeatureFormValue>
  >({});
  const [serverError, setServerError] = useState<string | null>(null);

  const listingsQuery = useQuery({
    queryKey: ["my-listings"],
    queryFn: ({ signal }) => listingsApi.getMine(token as string, signal),
    enabled: Boolean(token),
  });

  const listing = useMemo<ListingResponse | null>(() => {
    return listingsQuery.data?.find((item) => item.id === listingId) ?? null;
  }, [listingsQuery.data, listingId]);

  const categoryConfig = useMemo(
    () => getCategoryConfig(categoryKey),
    [categoryKey],
  );

  useEffect(() => {
    if (!listing) {
      return;
    }

    setTitle(listing.title);
    setDescription(listing.description);
    setCategoryKey(listing.categoryKey);
    setAskingPrice(String(listing.askingPrice));

    const parsedFeatures = parseFeatureObject(listing.featuresJson);

    const normalizedEntries = Object.entries(parsedFeatures).map(([key, value]) => {
      if (typeof value === "boolean") {
        return [key, value] as const;
      }

      return [key, String(value)] as const;
    });

    setFeatureValues(Object.fromEntries(normalizedEntries));
  }, [listing]);

  const updateMutation = useMutation({
    mutationFn: (request: UpdateListingRequest) =>
      listingsApi.update(listingId, request, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["published-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["listing-detail", listingId] });
    },
    onError: () => {
      setServerError("Opslaan is mislukt. Controleer je invoer en probeer opnieuw.");
    },
  });

  const predictMutation = useMutation({
    mutationFn: () => listingsApi.predictPrice(listingId, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["published-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["listing-detail", listingId] });
    },
    onError: () => {
      setServerError("Prijs voorspellen is mislukt.");
    },
  });

  const publishMutation = useMutation({
    mutationFn: () => listingsApi.publish(listingId, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["published-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["listing-detail", listingId] });
    },
    onError: () => {
      setServerError("Publiceren is mislukt.");
    },
  });

  const unpublishMutation = useMutation({
    mutationFn: () => listingsApi.unpublish(listingId, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["published-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["listing-detail", listingId] });
    },
    onError: () => {
      setServerError("Unpublish is mislukt.");
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () => listingsApi.delete(listingId, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      await queryClient.invalidateQueries({ queryKey: ["published-listings"] });
      router.push("/dashboard/listings");
    },
    onError: () => {
      setServerError("Verwijderen is mislukt.");
    },
  });

  function updateFeatureValue(key: string, value: FeatureFormValue): void {
    setFeatureValues((current) => ({
      ...current,
      [key]: value,
    }));
  }

  function buildFeaturesJson(): string {
    const fields = categoryConfig?.fields ?? [];

    const features = Object.fromEntries(
      fields
        .map((field) => {
          const rawValue = featureValues[field.key];

          if (field.type === "checkbox") {
            return [field.key, Boolean(rawValue)];
          }

          if (rawValue === undefined || rawValue === null || rawValue === "") {
            return [field.key, null];
          }

          return [field.key, toFeatureJsonValue(field, rawValue)];
        })
        .filter(([, value]) => value !== null),
    );

    return JSON.stringify(features);
  }

  function handleSave(event: React.FormEvent<HTMLFormElement>): void {
    event.preventDefault();
    setServerError(null);

    if (!token) {
      setServerError("Je bent niet ingelogd.");
      return;
    }

    if (!title.trim() || !description.trim() || !askingPrice.trim()) {
      setServerError("Vul titel, beschrijving en vraagprijs in.");
      return;
    }

    const price = Number(askingPrice);

    if (Number.isNaN(price) || price <= 0) {
      setServerError("Vul een geldige vraagprijs in.");
      return;
    }

    updateMutation.mutate({
      title: title.trim(),
      description: description.trim(),
      askingPrice: price,
      featuresJson: buildFeaturesJson(),
    });
  }

  if (listingsQuery.isLoading) {
    return (
      <div className="space-y-6">
        <DashboardHeader
          eyebrow="Seller"
          title="Listing laden..."
          description="Bezig met het ophalen van je listing."
        />

        <div className="soft-panel rounded-[2rem] p-6">
          <div className="h-8 w-56 animate-pulse rounded-full bg-emerald-100" />
          <div className="mt-4 h-5 w-80 animate-pulse rounded-full bg-emerald-100" />
          <div className="mt-8 h-48 animate-pulse rounded-[1.5rem] bg-emerald-50" />
        </div>
      </div>
    );
  }

  if (listingsQuery.isError || !listing) {
    return (
      <div className="space-y-6">
        <DashboardHeader
          eyebrow="Seller"
          title="Listing niet gevonden"
          description="De listing kon niet worden geladen."
        />

        <div className="soft-panel rounded-[2rem] p-6">
          <p className="text-sm leading-7 text-emerald-950/65">
            Deze listing bestaat niet, of hoort niet bij jouw account.
          </p>
          <Link
            href="/dashboard/listings"
            className="mt-4 inline-flex items-center gap-2 rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-800"
          >
            <ArrowLeft className="h-4 w-4" />
            Terug naar mijn listings
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <DashboardHeader
        eyebrow="Seller"
        title="Listing bewerken"
        description="Werk hier je listing bij, vraag een AI-prijsvoorspelling aan en publiceer wanneer je tevreden bent."
      />

      <div className="flex flex-wrap items-center gap-3">
        <Link
          href="/dashboard/listings"
          className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 transition hover:bg-emerald-50"
        >
          <ArrowLeft className="h-4 w-4" />
          Terug
        </Link>

        <ListingStatusBadge status={listing.status} />

        <div className="rounded-full bg-white px-4 py-2 text-sm font-semibold text-emerald-950 shadow-sm">
          Listing ID: {listing.id.slice(0, 8)}...
        </div>
      </div>

      <form onSubmit={handleSave} className="space-y-6">
        <section className="soft-panel rounded-[2rem] p-6">
          <div className="grid gap-6 lg:grid-cols-2">
            <div>
              <label className="mb-2 block text-sm font-bold text-emerald-950">
                Titel
              </label>
              <input
                value={title}
                onChange={(event) => setTitle(event.target.value)}
                className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
              />
            </div>

            <div>
              <label className="mb-2 block text-sm font-bold text-emerald-950">
                Categorie
              </label>
              <select
                value={categoryKey}
                onChange={(event) => {
                  setCategoryKey(event.target.value);
                  setFeatureValues({});
                }}
                className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
              >
                {listingCategoryConfigs.map((category) => (
                  <option key={category.key} value={category.key}>
                    {category.label}
                  </option>
                ))}
              </select>
              <p className="mt-2 text-xs text-emerald-900/55">
                {categoryConfig?.description}
              </p>
            </div>

            <div className="lg:col-span-2">
              <label className="mb-2 block text-sm font-bold text-emerald-950">
                Beschrijving
              </label>
              <textarea
                value={description}
                onChange={(event) => setDescription(event.target.value)}
                rows={5}
                className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
              />
            </div>

            <div>
              <label className="mb-2 block text-sm font-bold text-emerald-950">
                Vraagprijs
              </label>
              <input
                type="number"
                value={askingPrice}
                onChange={(event) => setAskingPrice(event.target.value)}
                className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
                min={1}
                step="0.01"
              />
            </div>

            <div>
              <label className="mb-2 block text-sm font-bold text-emerald-950">
                Huidige AI prijs
              </label>
              <div className="rounded-[1.2rem] bg-lime-50 px-4 py-3 text-sm font-semibold text-emerald-950">
                {formatCurrency(listing.predictedPrice)}
              </div>
            </div>
          </div>
        </section>

        <section className="soft-panel rounded-[2rem] p-6">
          <h2 className="text-2xl font-black text-emerald-950">Features</h2>
          <p className="mt-2 text-sm leading-7 text-emerald-950/62">
            Pas de listing-features aan die door je ML-flow gebruikt kunnen worden.
          </p>

          <div className="mt-6 grid gap-5 md:grid-cols-2">
            {categoryConfig?.fields.map((field) => (
              <div key={field.key}>
                <label className="mb-2 block text-sm font-bold text-emerald-950">
                  {field.label}
                </label>

                {field.type === "text" ? (
                  <input
                    value={String(featureValues[field.key] ?? "")}
                    onChange={(event) =>
                      updateFeatureValue(field.key, event.target.value)
                    }
                    className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
                    placeholder={field.placeholder}
                  />
                ) : null}

                {field.type === "number" ? (
                  <input
                    type="number"
                    value={String(featureValues[field.key] ?? "")}
                    onChange={(event) =>
                      updateFeatureValue(field.key, event.target.value)
                    }
                    className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
                    min={field.min}
                    step={field.step}
                    placeholder={field.placeholder}
                  />
                ) : null}

                {field.type === "select" ? (
                  <select
                    value={String(featureValues[field.key] ?? "")}
                    onChange={(event) =>
                      updateFeatureValue(field.key, event.target.value)
                    }
                    className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
                  >
                    <option value="">Maak een keuze</option>
                    {field.options?.map((option) => (
                      <option key={option.value} value={option.value}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                ) : null}

                {field.type === "checkbox" ? (
                  <label className="flex items-center gap-3 rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 shadow-sm">
                    <input
                      type="checkbox"
                      checked={Boolean(featureValues[field.key] ?? false)}
                      onChange={(event) =>
                        updateFeatureValue(field.key, event.target.checked)
                      }
                    />
                    <span className="text-sm text-emerald-950">
                      {field.label} inschakelen
                    </span>
                  </label>
                ) : null}
              </div>
            ))}
          </div>
        </section>

        <ListingImageUploaderShell />

        {serverError ? (
          <div className="rounded-[1.2rem] border border-red-200 bg-red-50 p-4 text-sm text-red-700">
            {serverError}
          </div>
        ) : null}

        <section className="soft-panel rounded-[2rem] p-6">
          <div className="flex flex-wrap gap-3">
            <button
              type="submit"
              disabled={updateMutation.isPending}
              className="inline-flex items-center gap-2 rounded-full bg-emerald-900 px-5 py-3 text-sm font-bold text-white transition hover:bg-emerald-800 disabled:opacity-70"
            >
              {updateMutation.isPending ? (
                <>
                  <LoaderCircle className="h-4 w-4 animate-spin" />
                  Opslaan...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4" />
                  Wijzigingen opslaan
                </>
              )}
            </button>

            <button
              type="button"
              onClick={() => predictMutation.mutate()}
              disabled={predictMutation.isPending}
              className="inline-flex items-center gap-2 rounded-full border border-lime-300 bg-lime-50 px-5 py-3 text-sm font-bold text-emerald-950 transition hover:bg-lime-100 disabled:opacity-70"
            >
              {predictMutation.isPending ? (
                <>
                  <LoaderCircle className="h-4 w-4 animate-spin" />
                  Voorspellen...
                </>
              ) : (
                <>
                  <Sparkles className="h-4 w-4" />
                  AI prijs voorspellen
                </>
              )}
            </button>

            {listing.status.toLowerCase() !== "published" ? (
              <button
                type="button"
                onClick={() => publishMutation.mutate()}
                disabled={publishMutation.isPending}
                className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-5 py-3 text-sm font-bold text-emerald-950 transition hover:bg-emerald-50 disabled:opacity-70"
              >
                {publishMutation.isPending ? (
                  <>
                    <LoaderCircle className="h-4 w-4 animate-spin" />
                    Publiceren...
                  </>
                ) : (
                  <>
                    <Rocket className="h-4 w-4" />
                    Publiceren
                  </>
                )}
              </button>
            ) : (
              <button
                type="button"
                onClick={() => unpublishMutation.mutate()}
                disabled={unpublishMutation.isPending}
                className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-5 py-3 text-sm font-bold text-emerald-950 transition hover:bg-emerald-50 disabled:opacity-70"
              >
                {unpublishMutation.isPending ? (
                  <>
                    <LoaderCircle className="h-4 w-4 animate-spin" />
                    Unpublish...
                  </>
                ) : (
                  <>
                    <EyeOff className="h-4 w-4" />
                    Unpublish
                  </>
                )}
              </button>
            )}

            <button
              type="button"
              onClick={() => {
                const confirmed = window.confirm(
                  "Weet je zeker dat je deze listing wilt verwijderen?",
                );

                if (confirmed) {
                  deleteMutation.mutate();
                }
              }}
              disabled={deleteMutation.isPending}
              className="inline-flex items-center gap-2 rounded-full border border-red-200 bg-white px-5 py-3 text-sm font-bold text-red-700 transition hover:bg-red-50 disabled:opacity-70"
            >
              {deleteMutation.isPending ? (
                <>
                  <LoaderCircle className="h-4 w-4 animate-spin" />
                  Verwijderen...
                </>
              ) : (
                <>
                  <Trash2 className="h-4 w-4" />
                  Listing verwijderen
                </>
              )}
            </button>
          </div>
        </section>
      </form>
    </div>
  );
}