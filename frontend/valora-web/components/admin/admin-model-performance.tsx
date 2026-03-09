"use client";

import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { RefreshCcw } from "lucide-react";
import { AdminHeader } from "@/components/admin/admin-header";
import { SimpleBarChart } from "@/components/admin/charts/simple-bar-chart";
import { adminApi } from "@/lib/api/admin-api";
import { getAccessToken } from "@/lib/auth/token-storage";

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("nl-NL", {
    style: "currency",
    currency: "EUR",
    maximumFractionDigits: 0,
  }).format(value);
}

function formatPercentage(value: number): string {
  return `${value.toFixed(2)}%`;
}

export function AdminModelPerformance(): React.JSX.Element {
  const token = getAccessToken();
  const [selectedCategory, setSelectedCategory] = useState("");

  const categoriesQuery = useQuery({
    queryKey: ["admin-model-categories"],
    queryFn: ({ signal }) => adminApi.getModelCategories(token as string, signal),
    enabled: Boolean(token),
  });

  const overviewQuery = useQuery({
    queryKey: ["admin-model-performance-overview"],
    queryFn: ({ signal }) => adminApi.getModelPerformanceOverview(token as string, signal),
    enabled: Boolean(token),
  });

  const listingsQuery = useQuery({
    queryKey: ["admin-model-performance-listings"],
    queryFn: ({ signal }) => adminApi.getModelPerformanceListings(token as string, signal),
    enabled: Boolean(token),
  });

  const categoryQuery = useQuery({
    queryKey: ["admin-model-performance-category", selectedCategory],
    queryFn: ({ signal }) =>
      adminApi.getModelPerformanceByCategory(selectedCategory, token as string, signal),
    enabled: Boolean(token) && selectedCategory.length > 0,
    retry: false,
  });

  const categories = useMemo(() => {
    return categoriesQuery.data?.categories.map((item) => item.key) ?? [];
  }, [categoriesQuery.data]);

  return (
    <div className="space-y-6">
      <AdminHeader
        eyebrow="Admin"
        title="Model performance"
        description="Analyseer modelnauwkeurigheid per categorie en bekijk recente listing-level performance data."
      />

      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Categorieën</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">
            {overviewQuery.data?.summary.totalCategories ?? 0}
          </p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Listings</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">
            {overviewQuery.data?.summary.totalListings ?? 0}
          </p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Weighted MAE</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">
            {overviewQuery.data
              ? formatCurrency(overviewQuery.data.summary.weightedMeanAbsoluteError)
              : "-"}
          </p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Weighted MAPE</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">
            {overviewQuery.data
              ? formatPercentage(
                  overviewQuery.data.summary.weightedMeanAbsolutePercentageError,
                )
              : "-"}
          </p>
        </div>
      </section>

      <SimpleBarChart
        title="MAE per categorie"
        items={
          overviewQuery.data?.categories.map((item) => ({
            label: item.category,
            value: item.meanAbsoluteError,
          })) ?? []
        }
        valueFormatter={(value) => formatCurrency(value)}
      />

      <section className="soft-panel rounded-[2rem] p-6">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <h2 className="text-2xl font-black text-emerald-950">Categorie-analyse</h2>
            <p className="mt-2 text-sm leading-7 text-emerald-950/62">
              Kies een categorie om detailperformance te bekijken.
            </p>
          </div>

          <div className="w-full max-w-xs">
            <select
              value={selectedCategory}
              onChange={(event) => setSelectedCategory(event.target.value)}
              className="w-full rounded-full border border-emerald-200 bg-white px-4 py-3 text-sm font-semibold text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
            >
              <option value="">Kies een categorie</option>
              {categories.map((category) => (
                <option key={category} value={category}>
                  {category}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="mt-6 overflow-x-auto">
          <table className="min-w-full border-separate border-spacing-y-3">
            <thead>
              <tr className="text-left text-xs uppercase tracking-[0.18em] text-emerald-900/45">
                <th className="px-4">Categorie</th>
                <th className="px-4">Listings</th>
                <th className="px-4">MAE</th>
                <th className="px-4">MAPE</th>
                <th className="px-4">Median AE</th>
              </tr>
            </thead>
            <tbody>
              {overviewQuery.data?.categories.map((item) => (
                <tr key={item.category} className="bg-white shadow-sm">
                  <td className="rounded-l-[1rem] px-4 py-4 font-semibold text-emerald-950">
                    {item.category}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    {item.totalListings}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    {formatCurrency(item.meanAbsoluteError)}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    {formatPercentage(item.meanAbsolutePercentageError)}
                  </td>
                  <td className="rounded-r-[1rem] px-4 py-4 text-sm text-emerald-950/70">
                    {formatCurrency(item.medianAbsoluteError)}
                  </td>
                </tr>
              )) ?? null}
            </tbody>
          </table>
        </div>
      </section>

      {selectedCategory ? (
        <section className="soft-panel rounded-[2rem] p-6">
          <div className="flex items-center justify-between gap-4">
            <div>
              <h2 className="text-2xl font-black text-emerald-950">
                Detailperformance · {selectedCategory}
              </h2>
              <p className="mt-2 text-sm leading-7 text-emerald-950/62">
                Listing-level prestaties voor de gekozen categorie.
              </p>
            </div>

            <button
              type="button"
              onClick={() => void categoryQuery.refetch()}
              className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 transition hover:bg-emerald-50"
            >
              <RefreshCcw className="h-4 w-4" />
              Vernieuwen
            </button>
          </div>

          {categoryQuery.isLoading ? (
            <p className="mt-6 text-sm text-emerald-950/60">Laden...</p>
          ) : null}

          {categoryQuery.isError ? (
            <p className="mt-6 text-sm text-emerald-950/60">
              Voor deze categorie is nog geen sold listing performance data beschikbaar.
            </p>
          ) : null}

          {categoryQuery.data ? (
            <>
              <div className="mt-6 grid gap-4 md:grid-cols-3">
                <div className="rounded-[1.5rem] bg-white p-5 shadow-sm">
                  <p className="text-sm font-semibold text-emerald-900/55">Listings</p>
                  <p className="mt-2 text-3xl font-black text-emerald-950">
                    {categoryQuery.data.totalListings}
                  </p>
                </div>

                <div className="rounded-[1.5rem] bg-white p-5 shadow-sm">
                  <p className="text-sm font-semibold text-emerald-900/55">MAE</p>
                  <p className="mt-2 text-3xl font-black text-emerald-950">
                    {formatCurrency(categoryQuery.data.meanAbsoluteError)}
                  </p>
                </div>

                <div className="rounded-[1.5rem] bg-white p-5 shadow-sm">
                  <p className="text-sm font-semibold text-emerald-900/55">MAPE</p>
                  <p className="mt-2 text-3xl font-black text-emerald-950">
                    {formatPercentage(categoryQuery.data.meanAbsolutePercentageError)}
                  </p>
                </div>
              </div>

              <SimpleBarChart
                title={`Absolute error per listing · ${selectedCategory}`}
                items={categoryQuery.data.listings.map((item) => ({
                  label: item.title,
                  value: item.absoluteError,
                }))}
                valueFormatter={(value) => formatCurrency(value)}
              />

              <div className="mt-6 overflow-x-auto">
                <table className="min-w-full border-separate border-spacing-y-3">
                  <thead>
                    <tr className="text-left text-xs uppercase tracking-[0.18em] text-emerald-900/45">
                      <th className="px-4">Titel</th>
                      <th className="px-4">Actueel</th>
                      <th className="px-4">Voorspeld</th>
                      <th className="px-4">Absolute error</th>
                      <th className="px-4">APE</th>
                    </tr>
                  </thead>
                  <tbody>
                    {categoryQuery.data.listings.map((item) => (
                      <tr key={item.listingId} className="bg-white shadow-sm">
                        <td className="rounded-l-[1rem] px-4 py-4 font-semibold text-emerald-950">
                          {item.title}
                        </td>
                        <td className="px-4 py-4 text-sm text-emerald-950/70">
                          {formatCurrency(item.actualPrice)}
                        </td>
                        <td className="px-4 py-4 text-sm text-emerald-950/70">
                          {formatCurrency(item.predictedPrice)}
                        </td>
                        <td className="px-4 py-4 text-sm text-emerald-950/70">
                          {formatCurrency(item.absoluteError)}
                        </td>
                        <td className="rounded-r-[1rem] px-4 py-4 text-sm text-emerald-950/70">
                          {formatPercentage(item.absolutePercentageError)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          ) : null}
        </section>
      ) : null}

      <SimpleBarChart
        title="Recente absolute error rows"
        items={
          listingsQuery.data?.map((item) => ({
            label: `${item.category} · ${item.title}`,
            value: item.absoluteError,
          })) ?? []
        }
        valueFormatter={(value) => formatCurrency(value)}
      />
    </div>
  );
}