"use client";

import { useQuery } from "@tanstack/react-query";
import {
  Activity,
  BarChart3,
  Database,
  ReceiptText,
  RefreshCcw,
  Users,
} from "lucide-react";
import { AdminHeader } from "@/components/admin/admin-header";
import { SimpleBarChart } from "@/components/admin/charts/simple-bar-chart";
import { getAccessToken } from "@/lib/auth/token-storage";
import { adminApi } from "@/lib/api/admin-api";

function formatDate(value: string): string {
  return new Intl.DateTimeFormat("nl-NL", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export function AdminOverview(): React.JSX.Element {
  const token = getAccessToken();

  const dashboardQuery = useQuery({
    queryKey: ["admin-dashboard"],
    queryFn: ({ signal }) => adminApi.getDashboard(token as string, signal),
    enabled: Boolean(token),
  });

  if (dashboardQuery.isLoading) {
    return (
      <div className="space-y-6">
        <AdminHeader
          eyebrow="Admin"
          title="Dashboard laden..."
          description="Bezig met het ophalen van platformdata."
        />

        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          {Array.from({ length: 4 }).map((_, index) => (
            <div key={index} className="soft-panel rounded-[1.5rem] p-5">
              <div className="h-5 w-24 animate-pulse rounded-full bg-emerald-100" />
              <div className="mt-4 h-10 w-20 animate-pulse rounded-full bg-emerald-100" />
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (dashboardQuery.isError || !dashboardQuery.data) {
    return (
      <div className="space-y-6">
        <AdminHeader
          eyebrow="Admin"
          title="Dashboard fout"
          description="Het admin dashboard kon niet worden geladen."
        />

        <div className="soft-panel rounded-[2rem] p-6">
          <p className="text-sm leading-7 text-emerald-950/65">
            Controleer of je als admin bent ingelogd en of de backend bereikbaar is.
          </p>

          <button
            type="button"
            onClick={() => void dashboardQuery.refetch()}
            className="mt-4 inline-flex items-center gap-2 rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-800"
          >
            <RefreshCcw className="h-4 w-4" />
            Opnieuw laden
          </button>
        </div>
      </div>
    );
  }

  const data = dashboardQuery.data;

  return (
    <div className="space-y-6">
      <AdminHeader
        eyebrow="Admin"
        title="Platform overview"
        description="Centraal overzicht van gebruikers, listings, predictions, training runs en auditactiviteit."
      />

      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <div className="soft-panel rounded-[1.5rem] p-5">
          <div className="flex h-11 w-11 items-center justify-center rounded-[1rem] bg-emerald-100">
            <Users className="h-5 w-5 text-emerald-900" />
          </div>
          <p className="mt-4 text-sm font-semibold text-emerald-900/55">Gebruikers</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">{data.totalUsers}</p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <div className="flex h-11 w-11 items-center justify-center rounded-[1rem] bg-lime-100">
            <Database className="h-5 w-5 text-lime-900" />
          </div>
          <p className="mt-4 text-sm font-semibold text-emerald-900/55">Listings</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">{data.totalListings}</p>
          <p className="mt-2 text-xs text-emerald-950/55">
            {data.publishedListings} published · {data.soldListings} sold
          </p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <div className="flex h-11 w-11 items-center justify-center rounded-[1rem] bg-cyan-100">
            <Activity className="h-5 w-5 text-cyan-900" />
          </div>
          <p className="mt-4 text-sm font-semibold text-emerald-900/55">Predictions</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">{data.totalPredictions}</p>
          <p className="mt-2 text-xs text-emerald-950/55">
            {data.predictionsLast24Hours} in laatste 24 uur
          </p>
        </div>

        <div className="soft-panel rounded-[1.5rem] p-5">
          <div className="flex h-11 w-11 items-center justify-center rounded-[1rem] bg-violet-100">
            <BarChart3 className="h-5 w-5 text-violet-900" />
          </div>
          <p className="mt-4 text-sm font-semibold text-emerald-900/55">Training runs</p>
          <p className="mt-2 text-3xl font-black text-emerald-950">{data.totalTrainingRuns}</p>
          <p className="mt-2 text-xs text-emerald-950/55">
            {data.listingsLast24Hours} listings in laatste 24 uur
          </p>
        </div>
      </section>

      <section className="grid gap-6 xl:grid-cols-3">
        <SimpleBarChart
          title="Listings per categorie"
          items={data.listingsByCategory.map((item) => ({
            label: item.name,
            value: item.value,
          }))}
        />

        <SimpleBarChart
          title="Predictions per categorie"
          items={data.predictionsByCategory.map((item) => ({
            label: item.name,
            value: item.value,
          }))}
        />

        <SimpleBarChart
          title="Training runs per categorie"
          items={data.trainingRunsByCategory.map((item) => ({
            label: item.name,
            value: item.value,
          }))}
        />
      </section>

      <section className="soft-panel rounded-[2rem] p-6">
        <div className="flex items-center gap-3">
          <div className="flex h-11 w-11 items-center justify-center rounded-[1rem] bg-emerald-100">
            <ReceiptText className="h-5 w-5 text-emerald-900" />
          </div>
          <div>
            <h2 className="text-2xl font-black text-emerald-950">Recente audit logs</h2>
            <p className="text-sm text-emerald-950/60">
              Laatste 20 events uit de backend.
            </p>
          </div>
        </div>

        <div className="mt-6 overflow-x-auto">
          <table className="min-w-full border-separate border-spacing-y-3">
            <thead>
              <tr className="text-left text-xs uppercase tracking-[0.18em] text-emerald-900/45">
                <th className="px-4">Actie</th>
                <th className="px-4">Entity</th>
                <th className="px-4">Details</th>
                <th className="px-4">Tijd</th>
              </tr>
            </thead>
            <tbody>
              {data.recentAuditLogs.map((log) => (
                <tr key={log.id} className="rounded-[1rem] bg-white shadow-sm">
                  <td className="rounded-l-[1rem] px-4 py-4 font-semibold text-emerald-950">
                    {log.action}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    {log.entityType}
                    {log.entityId ? ` · ${log.entityId}` : ""}
                  </td>
                  <td className="px-4 py-4 text-sm text-emerald-950/70">
                    {log.details ?? "-"}
                  </td>
                  <td className="rounded-r-[1rem] px-4 py-4 text-sm text-emerald-950/70">
                    {formatDate(log.createdAtUtc)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}