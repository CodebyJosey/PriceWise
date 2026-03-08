"use client";

import { useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { LoaderCircle, Save } from "lucide-react";
import { DashboardHeader } from "@/components/dashboard/dashboard-header";
import { listingsApi } from "@/lib/api/listings-api";
import { getAccessToken } from "@/lib/auth/token-storage";
import {
  getCategoryConfig,
  listingCategoryConfigs,
  type ListingFeatureField,
} from "@/lib/listings/category-feature-config";
import type { CreateListingRequest } from "@/types/listings";

type FeatureFormValue = string | boolean;

function toFeatureJsonValue(field: ListingFeatureField, value: FeatureFormValue): unknown {
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

export function CreateListingForm(): React.JSX.Element {
  const router = useRouter();
  const queryClient = useQueryClient();
  const token = getAccessToken();

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [categoryKey, setCategoryKey] = useState(listingCategoryConfigs[0]?.key ?? "laptops");
  const [askingPrice, setAskingPrice] = useState("");
  const [featureValues, setFeatureValues] = useState<Record<string, FeatureFormValue>>({});
  const [serverError, setServerError] = useState<string | null>(null);

  const categoryConfig = useMemo(() => getCategoryConfig(categoryKey), [categoryKey]);

  const createMutation = useMutation({
    mutationFn: (request: CreateListingRequest) =>
      listingsApi.create(request, token as string),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["my-listings"] });
      router.push("/dashboard/listings");
    },
    onError: () => {
      setServerError(
        "De listing kon niet worden aangemaakt. Controleer je invoer en probeer opnieuw.",
      );
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

  function handleSubmit(event: React.FormEvent<HTMLFormElement>): void {
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

    createMutation.mutate({
      title: title.trim(),
      description: description.trim(),
      categoryKey,
      askingPrice: price,
      featuresJson: buildFeaturesJson(),
    });
  }

  return (
    <div className="space-y-6">
      <DashboardHeader
        eyebrow="Seller"
        title="Nieuwe listing"
        description="Maak hier een nieuwe draft-listing aan. In de volgende stap koppelen we prediction en bewerken nog verder door."
      />

      <form onSubmit={handleSubmit} className="soft-panel rounded-[2rem] p-6">
        <div className="grid gap-6 lg:grid-cols-2">
          <div>
            <label className="mb-2 block text-sm font-bold text-emerald-950">
              Titel
            </label>
            <input
              value={title}
              onChange={(event) => setTitle(event.target.value)}
              className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
              placeholder="Bijv. Lenovo Legion 5"
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
              placeholder="Beschrijf de staat, gebruikssporen en extra details."
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
              placeholder="950"
              min={1}
              step="0.01"
            />
          </div>
        </div>

        <div className="mt-8">
          <h2 className="text-2xl font-black text-emerald-950">Features</h2>
          <p className="mt-2 text-sm leading-7 text-emerald-950/62">
            Deze velden worden straks gebruikt voor prediction en nette listing-details.
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
        </div>

        {serverError ? (
          <div className="mt-6 rounded-[1.2rem] border border-red-200 bg-red-50 p-4 text-sm text-red-700">
            {serverError}
          </div>
        ) : null}

        <div className="mt-8 flex flex-wrap gap-3">
          <button
            type="submit"
            disabled={createMutation.isPending}
            className="inline-flex items-center gap-2 rounded-full bg-emerald-900 px-5 py-3 text-sm font-bold text-white transition hover:bg-emerald-800 disabled:opacity-70"
          >
            {createMutation.isPending ? (
              <>
                <LoaderCircle className="h-4 w-4 animate-spin" />
                Listing opslaan...
              </>
            ) : (
              <>
                <Save className="h-4 w-4" />
                Opslaan als draft
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
}