"use client";

import { useQuery } from "@tanstack/react-query";
import { AdminHeader } from "@/components/admin/admin-header";
import { SimpleBarChart } from "@/components/admin/charts/simple-bar-chart";
import { SimpleDonutChart } from "@/components/admin/charts/simple-donut-chart";
import { adminApi } from "@/lib/api/admin-api";
import { getAccessToken } from "@/lib/auth/token-storage";

function formatDate(value: string): string {
  return new Intl.DateTimeFormat("nl-NL", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export function AdminModelStats(): React.JSX.Element {
  const token = getAccessToken();

  const modelStatsQuery = useQuery({
    queryKey: ["admin-model-stats"],
    queryFn: ({ signal }) => adminApi.getModelStats(token as string, signal),
    enabled: Boolean(token),
  });

  const userStatsQuery = useQuery({
    queryKey: ["admin-user-stats"],
    queryFn: ({ signal }) => adminApi.getUserStats(token as string, signal),
    enabled: Boolean(token),
  });

  const listingStatsQuery = useQuery({
    queryKey: ["admin-listing-stats"],
    queryFn: ({ signal }) => adminApi.getListingStats(token as string, signal),
    enabled: Boolean(token),
  });

  const predictionStatsQuery = useQuery({
    queryKey: ["admin-prediction-stats"],
    queryFn: ({ signal }) => adminApi.getPredictionStats(token as string, signal),
    enabled: Boolean(token),
  });

  return (
    <div className="space-y-6">
      <AdminHeader
        eyebrow="Admin"
        title="Model & platform stats"
        description="Verdiepende statistieken voor users, listings, predictions en model training."
      />

      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Training runs</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">
            {modelStatsQuery.data?.totalTrainingRuns ?? 0}
          </p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Users</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">
            {userStatsQuery.data?.totalUsers ?? 0}
          </p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Listings</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">
            {listingStatsQuery.data?.total ?? 0}
          </p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <p className="text-sm font-semibold text-emerald-900/55">Predictions</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">
            {predictionStatsQuery.data?.total ?? 0}
          </p>
        </div>
      </section>

      <section className="grid gap-6 xl:grid-cols-2">
        <SimpleDonutChart
          title="User verdeling"
          totalLabel="users"
          segments={[
            {
              label: "Admins",
              value: userStatsQuery.data?.admins ?? 0,
              toneClassName: "bg-emerald-400",
            },
            {
              label: "Sellers",
              value: userStatsQuery.data?.sellers ?? 0,
              toneClassName: "bg-lime-400",
            },
            {
              label: "Buyers",
              value: userStatsQuery.data?.buyers ?? 0,
              toneClassName: "bg-cyan-400",
            },
          ]}
        />

        <SimpleDonutChart
          title="Listing status verdeling"
          totalLabel="listings"
          segments={[
            {
              label: "Draft",
              value: listingStatsQuery.data?.draft ?? 0,
              toneClassName: "bg-amber-400",
            },
            {
              label: "Predicted",
              value: listingStatsQuery.data?.predicted ?? 0,
              toneClassName: "bg-cyan-400",
            },
            {
              label: "Published",
              value: listingStatsQuery.data?.published ?? 0,
              toneClassName: "bg-emerald-400",
            },
            {
              label: "Sold",
              value: listingStatsQuery.data?.sold ?? 0,
              toneClassName: "bg-violet-400",
            },
          ]}
        />
      </section>

      <section className="grid gap-6 xl:grid-cols-2">
        <SimpleBarChart
          title="Listings per categorie"
          items={
            listingStatsQuery.data?.byCategory.map((item) => ({
              label: item.category,
              value: item.count,
            })) ?? []
          }
        />

        <SimpleBarChart
          title="Training runs per categorie"
          items={
            modelStatsQuery.data?.byCategory.map((item) => ({
              label: `${item.category} (v${item.latestVersion})`,
              value: item.count,
            })) ?? []
          }
        />
      </section>

      <SimpleBarChart
        title="Prediction counts per modelversie"
        items={
          predictionStatsQuery.data?.byModelVersion.map((item) => ({
            label: `${item.category} · v${item.modelVersion}`,
            value: item.count,
          })) ?? []
        }
      />

      <section className="soft-panel rounded-[2rem] p-6">
        <h2 className="text-2xl font-black text-emerald-950">Laatste training runs</h2>
        <div className="mt-6 overflow-x-auto">
          <table className="min-w-full border-separate border-spacing-y-3">
            <thead>
              <tr className="text-left text-xs uppercase tracking-[0.18em] text-emerald-900/45">
                <th className="px-4">Categorie</th>
                <th className="px-4">Versie</th>
                <th className="px-4">R²</th>
                <th className="px-4">RMSE</th>
                <th className="px-4">MAE</th>
                <th className="px-4">Tijd</th>
              </tr>
            </thead>
            <tbody>
              {modelStatsQuery.data?.latestRuns.map((run) => (
                <tr key={run.id} className="bg-white shadow-sm">
                  <td className="rounded-l-[1rem] px-4 py-4 font-semibold text-emerald-950">
                    {run.categoryKey}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    v{run.version}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    {run.rSquared.toFixed(4)}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    {run.rootMeanSquaredError.toFixed(2)}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    {run.meanAbsoluteError.toFixed(2)}
                  </td>
                  <td className="rounded-r-[1rem] px-4 py-4 text-sm text-emerald-950/70">
                    {formatDate(run.createdAtUtc)}
                  </td>
                </tr>
              )) ?? null}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}